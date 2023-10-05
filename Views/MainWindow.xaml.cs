using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using StoreExam.CheckData;
using StoreExam.Data.DAL;
using StoreExam.Enums;
using StoreExam.ModelViews;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class MainWindow : Window
    {
        public Data.Entity.User User { get; set; }
        public ObservableCollection<Data.Entity.Category>? Categories { get; set; }
        public Data.Entity.Category? ChoiceCategory { get; set; }
        public ObservableCollection<Data.Entity.Product> Products { get; set; }
        ICollectionView productsListView;
        public ObservableCollection<Data.Entity.BasketProduct> BasketProducts { get; set; } = null!;
        public MainWindowModel ViewModel { get; set; } = null!;  // ViewModel окна
        public StateData stateUserData;  // состояние данных пользователя

        public MainWindow(Data.Entity.User user)
        {
            InitializeComponent();
            DataContext = this;

            User = user;

            // запускаем асинхронно методы по работе с БД для инициализации коллекций
            Task? pbPriceTask = null;
            Task.Run(async () =>
                {
                    Categories = await CategoriesDal.GetCategories();

                    var bs = await BasketProductsDal.GetBasketProductsByUser(User);
                    BasketProducts = bs is null ? new() : bs;

                    ViewModel = new MainWindowModel();
                    pbPriceTask = UpdateTotalBasketProductsPrice();  // запускаем обновление суммы в корзине, через ViewModel окна
                }
            ).Wait();  // ждём выполнение

            Products = new();
            productsListView = CollectionViewSource.GetDefaultView(Products);

            pbPriceTask?.Wait();  // получаем результат
        }


        private Data.Entity.Product? GetProductFromButton(object sender)
        {
            if (sender is Button btn)  // если это кнопка
            {
                if (btn.DataContext is Data.Entity.Product product)  // получаем объект Entity.Product
                {
                    return product;
                }
            }
            return null;
        }

        private Data.Entity.Product? GetProductFromBorder(object sender)
        {
            if (sender is Border border)  // если это border
            {
                if (border.DataContext is Data.Entity.Product product)  // получаем объект Entity.Product
                {
                    return product;
                }
            }
            return null;
        }


        private void UpdateUIUserData()
        {
            // обновляем значения интерфейса, где привязано свойство User
            if (btnUserName.Template.FindName("textBlockUserName", btnUserName) is TextBlock textBlock)  // находим TextBlock в элементе Button
            {
                textBlock.Text = User.Name;
            }
        }

        private void UpdateUIChoiceProductData()
        {
            // обновляем значения интерфейса, где привязано свойство ChoiceCategory
            textBlockChoiceCategory.Text = ChoiceCategory?.Name;
            textBlockChoiceCategory.Tag = ChoiceCategory?.Id.ToString();
        }

        private void UpdateProducts(List<Data.Entity.Product> newListProducts)
        {
            // обновляем коллекцию продуктов
            Products.Clear();
            foreach (var product in newListProducts)
            {
                Products.Add(product);
            }
        }

        private void UpdateBasketProducts(List<Data.Entity.BasketProduct> newListBasketProducts)
        {
            // обновляем коллекцию корзины продуктов
            BasketProducts.Clear();
            foreach (var product in newListBasketProducts)
            {
                BasketProducts.Add(product);
            }
        }

        private async Task UpdateTotalBasketProductsPrice()
        {
            await Task.Run(() => ViewModel.TotalBasketProductsPrice = BasketProducts.Sum(bp => bp.Product.Price * bp.Amounts));
        }


        private async void AddProductInBasket(Data.Entity.Product product, int amount)
        {
            Data.Entity.BasketProduct? basketProduct = await BasketProductsDal.GetBasketProduct(User.Id, product.Id);  // находим объект
            if (basketProduct is not null)  // если товар уже в корзине, то просто добавляем кол-во
            {
                basketProduct.Amounts += amount;  // прибавляем кол-во
                await BasketProductsDal.Update(basketProduct);  // обновляем данные в БД
            }
            else
            {
                basketProduct = new()  // создаём новый объект для добавления продукта в корзину
                {
                    Id = Guid.NewGuid(),
                    UserId = User.Id,
                    ProductId = product.Id,
                    Amounts = amount
                };
                BasketProducts.Add(basketProduct);  // добавляем в коллекцию
                await BasketProductsDal.Add(basketProduct);  // добавляем в БД
            }
            Task pbPriceTask = UpdateTotalBasketProductsPrice();  // запускаем обновление суммы в корзине, через ViewModel окна
            MessageBox.Show("Товар успешно добавлен в корзину!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            await pbPriceTask;  // получаем результат
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stateUserData != StateData.Delete)  // если удаление аккаунта не было, то выводим пользователю вопрос
            {
                if (MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;  // отменяем событие закрытия
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new UserAccountWindow(User);  // в конструктор передаём ссылку User
                dialog.ShowDialog();  // отображаем окно аккаунта пользователя
                stateUserData = dialog.stateUserData;  // сохраняем состояние работы окна

                if (stateUserData == StateData.Save)
                {
                    User = dialog.User;  // сохраняем объект User из окна аккаунта пользователя
                    UpdateUIUserData();  // обновляем значения интерфейса
                    if (await UserDal.Update(User))  // обновляем данные в таблице
                    {
                        MessageBox.Show("Изменения сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("При обновлении аккаунта, что-то пошло не так!\nПопробуйте чуть похже.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (stateUserData == StateData.Delete)
                {
                    if (await UserDal.Delete(User))  // если User-а успешно удалили
                    {
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("При удалении аккаунта, что-то пошло не так!\nПопробуйте чуть похже.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (stateUserData == StateData.Exit)
                {
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnUserBasketProduct_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(BasketProducts.Count + "");

            Hide();
            var dialog = new BasketProductWindow(BasketProducts, ViewModel);
            dialog.ShowDialog();
            Show();

            // обновляем коллекцию
            //UpdateBasketProducts(dialog.BasketProductsView.ToList());
            BasketProducts.Clear();
            foreach (var bp in dialog.BasketProductsView)
            {
                BasketProducts.Add(bp.BasketProduct);
            }

            //MessageBox.Show(BasketProducts.Count + "");
        }


        private async void ListBoxItemCategories_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.Content is Data.Entity.Category category)  // получаем ссылку на объект Entity.Category
                {
                    var listProducts = await ProductsDal.GetByCategory(category.Id);
                    if (listProducts is not null)
                    {
                        ChoiceCategory = category;  // присваиваем новую выбранную категорию
                        UpdateUIChoiceProductData();  // обновляем интерфейс
                        UpdateProducts(listProducts);  // обновляем коллекцию продуктов
                    }
                    else
                    {
                        MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BorderProduct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Data.Entity.Product? product = GetProductFromBorder(sender);  // получаем объект Entity.Product
                if (product is not null)
                {
                    var dialog = new ProductInfoWindow(product);  // создаём информационное окно для продукта
                    if (dialog.ShowDialog() == true)  // если true, значит успешно
                    {
                        AddProductInBasket(product, dialog.AddAmount);  // добавляем продукт в корзину
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
        {
            bool isBadMessage = true;
            try
            {
                Data.Entity.Product? product = GetProductFromButton(sender);  // получаем объект Entity.Product
                if (product is not null)
                {
                    TextBlock? textBlockAmount = GuiBaseManipulation.FindTextBlockAmountsProductBtnBasket(sender);  // получаем TextBlock
                    int amount;
                    if (textBlockAmount is not null && int.TryParse(textBlockAmount.Text, out amount))  // преобразовываем в int
                    {
                        AddProductInBasket(product, amount);  // добавляем продукт в корзину
                        isBadMessage = false;
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                if (isBadMessage)
                {
                    MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void BtnAddAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            Data.Entity.Product? product = GetProductFromButton(sender);  // получаем объект Entity.Product
            if (product is not null)
            {
                GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, product, true);  // увеличиваем значение, передаём Product для дальнейшей проверки
            }
        }

        private void BtnReduceAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            Data.Entity.Product? product = GetProductFromButton(sender);  // получаем объект Entity.Product
            if (product is not null)
            {
                GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, product, false);  // уменьшаем значение, передаём Product для дальнейшей проверки
            }
        }


        private async void BtnSearch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Guid.TryParse(textBlockChoiceCategory.Tag.ToString(), out Guid idCat))  // преобразовываем string в Id(категории)
                {
                    List<Data.Entity.Product>? listProducts;
                    if (String.IsNullOrEmpty(textBoxSearch.Text))  // если поисковая строка пустая, то получаем все продукты категории
                    {
                        listProducts = await ProductsDal.GetByCategory(idCat);
                    }
                    else
                    {
                        listProducts = await ProductsDal.FindByName(textBoxSearch.Text, idCat);  // получаем продукты с учётом выбранной категории
                    }
                    if (listProducts is not null)  // если продукты нашлись, то выводим
                    {
                        UpdateProducts(listProducts);
                    }
                    else MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

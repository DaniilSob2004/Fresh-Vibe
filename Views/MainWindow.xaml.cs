using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using StoreExam.Data.DAL;
using StoreExam.Enums;
using StoreExam.ModelViews;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class MainWindow : Window
    {
        private Window mainLoginWindow;  // ссылка на главное окно входа (MainLoginWindow)
        public MainWindowModel ViewModel { get; set; } = null!;  // ViewModel окна
        public StateData stateUserData;  // состояние данных пользователя

        public MainWindow(Data.Entity.User user, Window mainLoginWindow)
        {
            InitializeComponent();

            this.mainLoginWindow = mainLoginWindow;

            // запускаем асинхронно методы по работе с БД и коллекций
            Task.Run(async () =>
                {
                    ViewModel = new MainWindowModel(user);  // создаём ViewModel окна и передаём user
                    await ViewModel.LoadBasketProduct();  // загружаем корзину товаров
                    await ViewModel.UpdateTotalBasketProductsPrice();  // обновление суммы в корзине
                }
            ).Wait();  // ждём выполнение

            DataContext = ViewModel;
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


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stateUserData != StateData.Delete)  // если удаление аккаунта не было, то выводим пользователю вопрос
            {
                if (MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;  // отменяем событие закрытия
                    return;
                }
            }
            mainLoginWindow.Show();  // снова показываем главное окно входа
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new UserAccountWindow(ViewModel.User);  // в конструктор передаём ссылку User
                dialog.ShowDialog();  // отображаем окно аккаунта пользователя
                stateUserData = dialog.stateUserData;  // сохраняем состояние работы окна

                if (stateUserData == StateData.Save)
                {
                    ViewModel.User = dialog.User;  // сохраняем объект User из окна аккаунта пользователя
                    if (await UserDal.Update(ViewModel.User))  // обновляем данные в таблице
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
                    if (await UserDal.Delete(ViewModel.User))  // если User-а успешно удалили
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
            Hide();
            var dialog = new BasketProductWindow(ViewModel);  // запускаем окно корзины
            dialog.ShowDialog();
            Show();
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
                        ViewModel.ChoiceCategory = category;  // присваиваем новую выбранную категорию через ViewModel окна
                        ViewModel.UpdateProducts(listProducts);  // обновляем коллекцию продуктов
                    }
                    else
                    {
                        MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private async void BorderProduct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Data.Entity.Product? product = GetProductFromBorder(sender);  // получаем объект Entity.Product
                if (product is not null)
                {
                    var dialog = new ProductInfoWindow(product);  // создаём информационное окно для продукта
                    if (dialog.ShowDialog() != true) { return; }  // если не true, значит не добавили в корзину
                    if (await ViewModel.AddProductInBasket(product, dialog.AddAmount))  // добавляем продукт в корзину
                    {
                        MessageBox.Show("Товар успешно добавлен в корзину!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
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
                        if (await ViewModel.AddProductInBasket(product, amount))  // добавляем продукт в корзину
                        {
                            isBadMessage = false;
                        }
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
                else
                {
                    MessageBox.Show("Товар успешно добавлен в корзину!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
            bool isBadMessage = true;
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
                        ViewModel.UpdateProducts(listProducts);  // обновляем коллекцию продуктов
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
    }
}

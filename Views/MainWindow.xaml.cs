using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.Data.DAL;
using StoreExam.Enums;
using StoreExam.ViewModels;
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

            // запускаем задачу, и ждём, т.к. внутри конструктора выполняются ассинхронные методы и др. задачи
            Task.Run(() => ViewModel = new MainWindowModel(user)).Wait();  // создаём ViewModel окна и передаём user

            DataContext = ViewModel;
        }


        private ProductViewModel? GetProductFromBorder(object sender)
        {
            if (sender is Border border)  // если это border
            {
                if (border.DataContext is ProductViewModel productVM)  // получаем объект ProductViewModel
                {
                    return productVM;
                }
            }
            return null;
        }


        private void OpenConfirmEmailWindow()
        {
            if (ViewModel.User.Email is not null)  // если почта указана
            {
                Hide();
                var dialog = new ConfirmEmailWindow(ViewModel.User);  // запускаем окно подтверждения почты
                dialog.ShowDialog();
                Show();
            }
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stateUserData != StateData.Delete)  // если удаление аккаунта не было, то выводим пользователю вопрос
            {
                if (MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    mainLoginWindow.Show();  // снова показываем главное окно входа
                    return;
                }
            }
            e.Cancel = true;  // отменяем событие закрытия
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
                    if (await ViewModel.UpdateUser(dialog.User))  // сохраняем изменения, передаём объект User из окна аккаунта пользователя
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
            catch (Exception) { MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void BtnUserBasketProduct_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData.CheckUser.CheckIsConfirmedEmail(ViewModel.User))  // если почта подтверждена
            {
                Hide();
                var dialog = new BasketProductWindow(ViewModel.BPViewModel);  // запускаем окно корзины и передаём ViewModel для BasketProduct
                dialog.ShowDialog();
                Show();
                ViewModel.UpdateProductsChoiceCount();  // обновляем кол-во товаров
            }
            else if (MessageBox.Show("Почта не подтверждена.\nХотите подтвердить?", "Доступ ограничен", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                OpenConfirmEmailWindow();  // запускаем окно подтверждения почты
            }
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
                        ViewModel.ChoiceCategory = category;  // присваиваем новую выбранную категорию
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
                ProductViewModel? productVM = GetProductFromBorder(sender);  // получаем объект ProductViewModel
                if (productVM is not null)
                {
                    var dialog = new ProductInfoWindow(productVM);  // создаём информационное окно для продукта
                    if (dialog.ShowDialog() == true)  // если true, значит добавили в корзину
                    {
                        if (await ViewModel.AddProductInBasket(productVM))  // добавляем продукт в корзину
                        {
                            MessageBox.Show("Товар успешно добавлен в корзину!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ViewModel.UpdateProductChoiceCount(productVM);  // обновляем кол-во товара
                }
            }
            catch (Exception) { MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private async void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
        {
            bool isBadMessage = true;
            try
            {
                ProductViewModel? productVM = GuiBaseManipulation.GetProductFromButton(sender);  // получаем объект ProductViewModel
                if (productVM is not null)
                {
                    if (await ViewModel.AddProductInBasket(productVM))  // добавляем в корзину
                    {
                        ViewModel.UpdateProductChoiceCount(productVM);  // обновляем выбранное кол-во
                        isBadMessage = false;
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                if (isBadMessage) { MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
                else { MessageBox.Show("Товар успешно добавлен в корзину!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information); }
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
                if (isBadMessage) { MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
        }
    }
}

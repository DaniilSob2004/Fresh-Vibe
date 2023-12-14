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
using StoreExam.CheckData;
using static StoreExam.Formatting.ResourceHelper;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.LoadComboBoxLanguages(langComboBox);  // заполняем combobox доступными языками
        }


        private ProductViewModel? GetProductFromBorder(object sender)
        {
            if (sender is Border border)
            {
                if (border.DataContext is ProductViewModel productVM) { return productVM; }  // получаем объект ProductViewModel
            }
            return null;
        }


        private void OpenConfirmEmailWindow(string? text = null)
        {
            if (GuiBaseManipulation.OpenConfirmEmailWindow(this, ViewModel.User, text))  // запускаем окно подтверждения почты
            {
                Show();
            }
        }

        private void OpenBasketProductWindow()
        {
            Hide();
            var dialog = new BasketProductWindow(ViewModel.BPViewModel);  // запускаем окно корзины и передаём ViewModel для BasketProduct
            dialog.ShowDialog();
            Show();
            ViewModel.UpdateProductsChoiceCount();  // обновляем кол-во товаров
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (stateUserData != StateData.Delete)  // если удаление аккаунта не было, то выводим пользователю вопрос
            {
                if (GuiBaseManipulation.ShowQuestionWindow(MessageValues.ExitAccMess) == StateWindow.Yes)  // запускаем окно уведомлений
                {
                    mainLoginWindow.Show();  // показываем главное окно входа
                }
                else
                {
                    e.Cancel = true;  // отменяем событие закрытия
                }
            }
            else { mainLoginWindow.Show(); }  // показываем главное окно входа
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

                switch (stateUserData)
                {
                    case StateData.Delete:
                        if (await UserDal.Delete(ViewModel.User))  // если User-а успешно удалили
                        {
                            Close();
                        }
                        else
                        {
                            new MessageWindow(MessageValues.DelAccErrorMess).ShowDialog();  // запускаем окно уведомлений
                        }
                        break;

                    case StateData.Exit:
                        Close(); break;

                    case StateData.Cancel: break;

                    default:  // если StateData.Save или StateData.ChangeEmail
                        if (!await ViewModel.UpdateUser(dialog.User))  // сохраняем изменения, передаём объект User из окна аккаунта пользователя
                        {
                            new MessageWindow(MessageValues.UpdateAccErrorMess).ShowDialog();
                            return;
                        }

                        new MessageWindow(MessageValues.ChangeSucValueMess).ShowDialog();
                        if (stateUserData == StateData.ChangeEmail)
                        {
                            OpenConfirmEmailWindow();  // запускаем окно подтверждения почты
                        }
                        break;
                }
            }
            catch (Exception) { new MessageWindow(MessageValues.BaseErrorMess).ShowDialog(); }
        }

        private void BtnUserBasketProduct_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUser.CheckIsConfirmedEmail(ViewModel.User))  // если почта подтверждена
            {
                OpenBasketProductWindow();
            }
            else
            {
                OpenConfirmEmailWindow(MessageValues.ConfirmEmailQuestMess);  // запускаем окно подтверждения почты
                if (CheckUser.CheckIsConfirmedEmail(ViewModel.User))  // доп. проверка подтверждение почты
                {
                    OpenBasketProductWindow();
                }
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
                        ViewModel.UpdateProducts(listProducts, category);  // обновляем коллекцию продуктов
                    }
                    else
                    {
                        new MessageWindow(MessageValues.BaseErrorMess).ShowDialog();
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
                            GuiBaseManipulation.ShowInfoMessage(borderMessage);
                        }
                    }
                    ViewModel.UpdateProductChoiceCount(productVM);  // обновляем кол-во товара
                }
            }
            catch (Exception) { new MessageWindow(MessageValues.BaseErrorMess).ShowDialog(); }
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
                if (isBadMessage) { new MessageWindow(MessageValues.BaseErrorMess).ShowDialog(); }
                else { GuiBaseManipulation.ShowInfoMessage(borderMessage); }
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
                if (isBadMessage) { new MessageWindow(MessageValues.BaseErrorMess).ShowDialog(); }
            }
        }
    }
}

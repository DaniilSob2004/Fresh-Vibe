using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreExam.Views
{
    public partial class MainWindow : Window
    {
        public Data.Entity.User User { get; set; }
        public ObservableCollection<Data.Entity.Category>? Categories { get; set; }
        public Data.Entity.Category? ChoiceCategory { get; set; }
        public ObservableCollection<Data.Entity.Product> Products { get; set; }
        ICollectionView productsListView;
        public bool IsDelAccount;  // флаг, удалил ли пользователь аккаунт

        public MainWindow(Data.Entity.User user)
        {
            InitializeComponent();
            User = user;
            Categories = Data.DAL.CategoriesDal.GetCategories();  // получаем ObservableCollection 'Категорий' из БД

            Products = new ObservableCollection<Data.Entity.Product>();
            productsListView = CollectionViewSource.GetDefaultView(Products);

            IsDelAccount = false;
            this.DataContext = this;
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


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!IsDelAccount)  // если удаление аккаунта не было, то выводим пользователю вопрос
            {
                if (MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;  // отменяем событие закрытия
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserAccountWindow(User);  // в конструктор передаём ссылку User
            bool? dialogRes = dialog.ShowDialog();  // отображаем окно аккаунта пользователя
            if (dialogRes ?? false)  // если вернулось true, то закрываем окно (выходим из аккаунта)
            {
                if (dialog.isDelAccount)  // если пользователь удалил аккаунт
                {
                    if (Data.DAL.UserDal.Delete(User))  // если User-а успешно удалили
                    {
                        IsDelAccount = true;  // устанавливаем флаг удаления
                    }
                    else
                    {
                        MessageBox.Show("При удалении аккаунта, что-то пошло не так!\nПопробуйте чуть похже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                Close();
            }
            else  // сохранение свойств user-а
            {
                if (dialog.isSaveData)  // если User сохранил изменения
                {
                    User = dialog.User;  // сохраняем объект User из окна аккаунта пользователя
                    UpdateUIUserData();  // обновляем значения интерфейса

                    if (Data.DAL.UserDal.Update(User))  // обновляем данные в таблице
                    {
                        MessageBox.Show("Изменения сохранены!");
                    }
                    else
                    {
                        MessageBox.Show("При обновлении аккаунта, что-то пошло не так!\nПопробуйте чуть похже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void ListBoxItemCategories_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.Content is Data.Entity.Category category)  // получаем ссылку на объект Entity.Category
                {
                    var listProducts = Data.DAL.ProductsDal.GetByCategory(category.Id);
                    if (listProducts is not null)
                    {
                        ChoiceCategory = category;  // присваиваем новую выбранную категорию
                        UpdateUIChoiceProductData();  // обновляем интерфейс
                        UpdateProducts(listProducts);  // обновляем коллекцию продуктов
                    }
                    else
                    {
                        MessageBox.Show("Что-то пошло не так...");
                    }
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                if (border.DataContext is Data.Entity.Product product)
                {
                    var dialog = new ProductInfoWindow(product);  // создаём информационное окно для продукта
                    dialog.ShowDialog();
                }
            }
        }

        private void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.DataContext is Data.Entity.Product product)
                {
                    MessageBox.Show($"Добавляем в корзину: {product.Name}");
                }
            }
        }


        private void TextBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxGotFocus(sender);
        }

        private void TextBoxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxLostFocus(sender);
        }

        private void BtnSearch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Guid.TryParse(textBlockChoiceCategory.Tag.ToString(), out Guid idCat))  // преобразовываем string в Id(категории)
            {
                List<Data.Entity.Product>? listProducts;
                if (String.IsNullOrEmpty(textBoxSearch.Text))  // если поисковая строка пустая, то получаем все продукты категории
                {
                    listProducts = Data.DAL.ProductsDal.GetByCategory(idCat);
                }
                else
                {
                    listProducts = Data.DAL.ProductsDal.FindByName(textBoxSearch.Text, idCat);  // получаем продукты с учётом выбранной категории
                }
                if (listProducts is not null)  // если продукты нашлись, то выводим
                {
                    UpdateProducts(listProducts);
                }
                else MessageBox.Show("Что-то пошло не так!\nПопробуйте чуть похже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else MessageBox.Show("Что-то пошло не так!\nПопробуйте чуть похже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

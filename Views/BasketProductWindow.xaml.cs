using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreExam.ModelViews;

namespace StoreExam.Views
{
    public partial class BasketProductWindow : Window
    {
        public ObservableCollection<BasketProductViewModel> BasketProductsView { get; set; }  // ViewModel для BasketProduct (в котором есть IsSelected)
        public MainWindowModel ViewModel { get; set; }  // ссылка на ViewModel окна MainWindow

        public BasketProductWindow(ObservableCollection<Data.Entity.BasketProduct> basketProducts, MainWindowModel mainWindowModel)
        {
            InitializeComponent();
            DataContext = this;

            BasketProductsView = new();
            foreach (var product in basketProducts)
            {
                BasketProductsView.Add(new BasketProductViewModel(product));
            }
            ViewModel = mainWindowModel;
        }


        private async Task UpdateTotalBasketProductsPrice()
        {
            await Task.Run(() => ViewModel.TotalBasketProductsPrice = BasketProductsView.Sum(bp => bp.BasketProduct.Product.Price * bp.BasketProduct.Amounts));
        }


        private async void BtnAllDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                BasketProductsView.Clear();
                await UpdateTotalBasketProductsPrice();  // обновляем кол-во товаров
                // TODO: удаление из БД
            }
        }

        private async void BtnChoiceDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранные товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<BasketProductViewModel> delBpViewModels = new();
                for (int i = 0; i < BasketProductsView.Count; i++)
                {
                    if (BasketProductsView[i].IsSelected ?? false)  // если выбран
                    {
                        delBpViewModels.Add(BasketProductsView[i]);
                        BasketProductsView.Remove(BasketProductsView[i]);
                        i--;
                    }
                }
                await UpdateTotalBasketProductsPrice();  // обновляем кол-во товаров
                // TODO: удаление из БД
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBoxProducts.Items)  // перебираем каждый элемент
            {
                if (item is BasketProductViewModel bpViewModel)
                {
                    bpViewModel.IsSelected = checkBoxChoiceAll.IsChecked;  // устанавливаем новое значение чекбоксу
                }
            }
        }

        private void ListBoxItemBPViewModel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.Content is BasketProductViewModel bpViewModel)
                {
                    bpViewModel.IsSelected = !bpViewModel.IsSelected;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using StoreExam.Data.DAL;
using StoreExam.ModelViews;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class BasketProductWindow : Window
    {
        public BasketProductsViewModel BPViewModel { get; set; }  // ViewModel для BasketProduct (в котором есть IsSelected и другие методы)
        private bool flag = true;

        public BasketProductWindow(BasketProductsViewModel bPViewModel)
        {
            InitializeComponent();
            BPViewModel = bPViewModel;
            DataContext = BPViewModel;
            Task.Run(() => BPViewModel.UpdateTotalBasketProductsPrice()).Wait();  // обновляем цену товаров в корзине
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            BPViewModel.SetCheckBoxAllProduct(true);  // после закрытия, устанавливаем все чекбоксы вкл.
        }


        private BasketProductModel? GetBasketProductModelFromButton(object sender)
        {
            if (sender is Button btn)  // если это кнопка
            {
                if (btn.DataContext is BasketProductModel bpModel)  // получаем объект BasketProductModel
                {
                    return bpModel;
                }
            }
            return null;
        }

        private async Task<bool> ChangeOneValueAmount(object sender, bool isIncrease)
        {
            // меняем кол-во продукта, в зависимости от isIncrease (увеличиваем/уменьшаем)
            BasketProductModel? bpModel = GetBasketProductModelFromButton(sender);  // получаем объект BasketProductModel
            if (bpModel is not null)
            {
                if (GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, bpModel.BasketProduct.Product, isIncrease))  // меняем значение, передаём Product для дальнейшей проверки
                {
                    return await BPViewModel.UpdateAmountProduct(bpModel.BasketProduct, 1, isIncrease);  // обновляем значение кол-ва продукта
                }
            }
            return false;
        }


        private async void BtnAllDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (!await BPViewModel.DeleteAllProduct())  // удаляем все товары из корзины
                {
                    MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnChoiceDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранные товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (!await BPViewModel.DeleteChoiseProduct())  // удаляем выбранные товары из корзины
                {
                    MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CheckBoxChoiseAll_Click(object sender, RoutedEventArgs e)
        {
            BPViewModel.SetCheckBoxAllProduct(checkBoxChoiceAll.IsChecked);  // устанавливаем новые значения всем чекбоксам
        }

        private void ListBoxItemBPViewModel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Border && e.OriginalSource is not TextBlock) return;  // если не был нажат border или TextBlock, то выходим
            if (sender is ListBoxItem item)
            {
                if (item.Content is BasketProductModel bpModel)
                {
                    BPViewModel.SetCheckBoxProduct(bpModel);  // меняем состояние чекбокса
                    flag = false;
                }
            }
        }


        private async void BtnAddAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!await ChangeOneValueAmount(sender, true))
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnReduceAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!await ChangeOneValueAmount(sender, false))
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnDelProduct_Click(object sender, RoutedEventArgs e)
        {
            BasketProductModel? bpModel = GetBasketProductModelFromButton(sender);  // получаем объект BasketProductModel
            if (bpModel is not null)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить \"{bpModel.BasketProduct.Product.Name}\"?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (!await BPViewModel.DeleteBasketProduct(bpModel))  // удаление из БД и коллекции
                    {
                        MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else { MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }


        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            // 1. проверить, есть ли каждый выбранный товар в наличии (по кол-ву)
            // 2. если какие то не в наличие, то добавить их названия в строку и вывести
            // 3. 
            // 4. * товары не в наличии пометить как НЕ В НАЛИЧИИ (добавить свойство string в BasketProductModel и привязать к элементу XAML)
        }
    }
}

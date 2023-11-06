using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.Data.DAL;
using StoreExam.ViewModels;

namespace StoreExam.Views
{
    public partial class BasketProductWindow : Window
    {
        public BasketProductsViewModel BPViewModel { get; set; }  // ViewModel для BasketProduct

        public BasketProductWindow(BasketProductsViewModel bPViewModel)
        {
            InitializeComponent();
            BPViewModel = bPViewModel;
            DataContext = BPViewModel;
            Task.Run(async () => await BPViewModel.UpdateInStockProducts());  // проверка корзины на наличие товаров
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            BPViewModel.SetCheckBoxAllProduct(true);  // после закрытия, устанавливаем все чекбоксы вкл.
        }


        private BasketProductModel? GetBasketProductModelFromButton(object sender)
        {
            if (sender is Button btn)  // если это кнопка
            {
                if (btn.DataContext is BasketProductModel bpModel) { return bpModel; }  // получаем объект BasketProductModel
            }
            return null;
        }

        private async Task<bool> UpdateAmount(object sender, bool isIncrease)
        {
            BasketProductModel? bpModel = GetBasketProductModelFromButton(sender);  // получаем объект BasketProductModel
            if (bpModel is not null)
            {
                return await BPViewModel.CheckUpdateAmountProduct(bpModel, isIncrease);  // изменяем кол-во
            }
            return false;
        }

        private void OpenOrderWindow(List<BasketProductModel> listBuyBPModels, float totalPrice)
        {
            Hide();
            var dialog = new OrderWindow(BPViewModel.User, listBuyBPModels, totalPrice);  // TODO: передавать объект Order
            dialog.ShowDialog();
            ShowDialog();
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
                }
            }
        }


        private async void BtnAddAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!await UpdateAmount(sender, true))  // увеличиваем кол-во
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnReduceAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!await UpdateAmount(sender, false))  // уменьшаем кол-во
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


        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            await BPViewModel.UpdateInStockProducts();  // проверяем и обновляем наличие товаров

            if (BPViewModel.IsHaveProductNotInStock())  // хотя бы один товар из корзины не в наличии
            {
                if (MessageBox.Show("Некоторые товары не в наличии\nХотите продолжить?", "Нет в наличии", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            List<BasketProductModel> listBuyBPModels = BPViewModel.GetChoiceProducts();  // товары которые выбранны (купленны)
            int lastIndProduct = -1;  // откат кол-ва товаров, которые успели изменится, но была ошибка, и пользователь остановил обработку
            for (int i = 0; i < listBuyBPModels.Count; i++)
            {
                if (!await ProductsDal.UpdateCount(listBuyBPModels[i].BasketProduct.Product, listBuyBPModels[i].BasketProduct.Amounts, false))  // уменьшаем кол-во товаров в БД
                {
                    if (MessageBox.Show($"При обработке '{listBuyBPModels[i].BasketProduct.Product.Name}' что-то пошло не так...\nПродолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        listBuyBPModels.RemoveAt(i);  // удаляем из коллекции 'купленных' товаров
                        i--;
                    }
                    else
                    {
                        lastIndProduct = i;  // сохраняем индекс
                    }
                }
            }
            if (lastIndProduct != -1)  // если пользователь остановил обработку
            {
                for (int i = 0; i < lastIndProduct; i++)  // проходимся до того товара, на котором остановился пользователь
                {
                    await ProductsDal.UpdateCount(listBuyBPModels[i].BasketProduct.Product, listBuyBPModels[i].BasketProduct.Amounts, true);  // увеличиваем кол-во товаров в БД
                }
                return;
            }

            float totalPrice = BPViewModel.TotalBasketProductsPrice;  // запоминаем сумму
            await BPViewModel.UpdateInStockProducts();  // проверяем и обновляем наличие товаров

            OpenOrderWindow(listBuyBPModels, totalPrice);  // запуск окна заказа
        }
    }
}

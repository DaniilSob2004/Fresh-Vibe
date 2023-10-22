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
using StoreExam.CheckData;
using StoreExam.Data.DAL;
using StoreExam.ModelViews;
using StoreExam.Extensions;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class BasketProductWindow : Window
    {
        public BasketProductsViewModel BPViewModel { get; set; }  // ViewModel для BasketProduct (в котором есть IsSelected и другие методы)

        public BasketProductWindow(BasketProductsViewModel bPViewModel)
        {
            InitializeComponent();
            BPViewModel = bPViewModel;
            DataContext = BPViewModel;
            Task.Run(async () => await BPViewModel.CheckSetProductsNotInStock());  // проверка товаров в корзине в наличии, если нет, то добавляется текст "Нет в наличии"
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
                else return true;
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
                    if (bpModel.IsNotStock) return;  // если товар не в наличии
                    BPViewModel.SetCheckBoxProduct(bpModel);  // меняем состояние чекбокса
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
            if (await ChangeOneValueAmount(sender, false))
            {
                BasketProductModel? bpModel = GetBasketProductModelFromButton(sender);  // получаем объект BasketProductModel
                if (bpModel is not null)
                {
                    BPViewModel.CheckSetProductInNotStock(bpModel);  // проверяем и обновляем товар в наличии
                }
            }
            else { MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); }
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
            await BPViewModel.CheckSetProductsNotInStock();  // проверяем и обновляем наличие товаров

            if (BPViewModel.IsHaveProductNotInStock())  // хотя бы один товар из корзины не в наличии
            {
                if (MessageBox.Show("Некоторые товары не в наличии\nХотите продолжить?", "Нет в наличии", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            List<BasketProductModel> listBuyBPModels = BPViewModel.GetChoiceProducts();  // товары которые выбранны (купленны)
            int lastIndProduct = -1;  // откат кол-ва товаров, которые успели изменится, но была ошибка, и пользователь остановил обработку
            for (int i = 0; i < listBuyBPModels.Count; i++)
            {
                if (!await ProductsDal.ReduceCount(listBuyBPModels[i].BasketProduct.Product, listBuyBPModels[i].BasketProduct.Amounts))  // уменьшаем кол-во товаров в БД
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
                    await ProductsDal.AddCount(listBuyBPModels[i].BasketProduct.Product, listBuyBPModels[i].BasketProduct.Amounts);  // увеличиваем кол-во товаров в БД
                }
                return;
            }

            // TODO: ЗАПУСК ОКНА (вывод общей суммы, и кнопка для чека в pdf)
            if (MessageBox.Show($"Сумма к оплате: {BPViewModel.TotalBasketProductsPrice.Hrn()}\nНапечатать чек(в PDF)?", "Печатать чек", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FileWork.FilePdf filePdf = new();
                filePdf.ShowDialog();

                if (filePdf.PrintReceiptForBasketProducts(listBuyBPModels, BPViewModel.TotalBasketProductsPrice))
                {
                    MessageBox.Show("Чек успешно сохранился. Спасибо за заказ!", "Успешное сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Чек не удалось сохранить. Спасибо за заказ!", "Не удалось сохранить", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            await BPViewModel.CheckSetProductsNotInStock();  // проверяем и обновляем наличие товаров
        }
    }
}

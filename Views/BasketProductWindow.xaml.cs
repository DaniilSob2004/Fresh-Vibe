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
        public ObservableCollection<BasketProductViewModel> BasketProductsView { get; set; }  // ViewModel для BasketProduct (в котором есть IsSelected)
        public MainWindowModel ViewModel { get; set; }  // ссылка на ViewModel окна MainWindow

        public BasketProductWindow(MainWindowModel mainWindowModel)
        {
            InitializeComponent();
            DataContext = this;

            BasketProductsView = new();
            foreach (var product in mainWindowModel.BasketProducts)  // создаём коллекцию ViewModel для BasketProduct с чекбоксом
            {
                BasketProductsView.Add(new BasketProductViewModel(product));
            }
            ViewModel = mainWindowModel;
        }


        private BasketProductViewModel? GetBasketProductsViewFromButton(object sender)
        {
            if (sender is Button btn)  // если это кнопка
            {
                if (btn.DataContext is BasketProductViewModel bpViewModel)  // получаем объект BasketProductViewModel
                {
                    return bpViewModel;
                }
            }
            return null;
        }


        private async Task<bool> UpdateAmountProduct(BasketProductViewModel bpModel, bool isIncrease)
        {
            Data.Entity.BasketProduct? bp = await BasketProductsDal.GetBasketProduct(bpModel.BasketProduct.Id);  // находим BasketProduct
            if (bp is not null)
            {
                bp.Amounts = isIncrease ? bp.Amounts + 1 : bp.Amounts - 1;  // меняем значение
                await ViewModel.UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                return await BasketProductsDal.Update(bp);  // обновляем данные в БД
            }
            return false;
        }


        private async void BtnAllDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (await BasketProductsDal.DeleteAll())  // удаление из БД
                {
                    BasketProductsView.Clear();  // удаляем из коллекции UI
                    ViewModel.BasketProducts.Clear();  // обновляем коллекцию BasketProducts
                    await ViewModel.UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                }
                else
                {
                    MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnChoiceDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранные товары из корзины?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Data.Entity.BasketProduct> delBasketProducts = new();  // буфферная коллекция (для дальнейшего удаления из БД)
                foreach (var bpView in BasketProductsView)
                {
                    if (bpView.IsSelected ?? false)  // если выбран
                    {
                        delBasketProducts.Add(bpView.BasketProduct);  // добавляем в буфферную коллекцию
                    }
                }
                if (await BasketProductsDal.DeleteRange(delBasketProducts))  // удаление из БД
                {
                    foreach (var delBp in delBasketProducts)
                    {
                        BasketProductsView.Remove(BasketProductsView.FirstOrDefault(bp => bp.BasketProduct == delBp)!);  // удаляем из коллекции UI
                        ViewModel.BasketProducts.Remove(delBp);  // удаляем из коллекции BasketProducts
                    }
                    await ViewModel.UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                }
                else
                {
                    MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CheckBoxChoiseAll_Click(object sender, RoutedEventArgs e)
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
            if (e.OriginalSource is not Border) return;  // если была нажата кнопка или др. элемент кроме border, то выходим
            if (sender is ListBoxItem item)
            {
                if (item.Content is BasketProductViewModel bpViewModel)
                {
                    bpViewModel.IsSelected = !bpViewModel.IsSelected;  // меняем состояние чекбокса
                }
            }
        }

        private async void BtnAddAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            BasketProductViewModel? bpModel = GetBasketProductsViewFromButton(sender);  // получаем объект BasketProductViewModel
            if (bpModel is not null)
            {
                if (!GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, bpModel.BasketProduct.Product, true))  // увеличиваем значение, передаём Product для дальнейшей проверки
                {
                    return;  // если false значит значение увеличить неудалось
                }
                if (!await UpdateAmountProduct(bpModel, true))  // обновляем кол-во товара
                {
                    MessageBox.Show("При обновлении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnReduceAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            BasketProductViewModel? bpModel = GetBasketProductsViewFromButton(sender);  // получаем объект BasketProductViewModel
            if (bpModel is not null)
            {
                if (!GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, bpModel.BasketProduct.Product, false))  // уменьшаем значение, передаём Product для дальнейшей проверки
                {
                    return;  // если false значит значение увеличить неудалось
                }
                if (!await UpdateAmountProduct(bpModel, false))  // обновляем кол-во товара
                {
                    MessageBox.Show("При обновлении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnDelProduct_Click(object sender, RoutedEventArgs e)
        {
            BasketProductViewModel? bp = GetBasketProductsViewFromButton(sender);  // получаем объект Entity.Product
            if (bp is not null)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить \"{bp.BasketProduct.Product.Name}\"?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (await BasketProductsDal.Del(bp.BasketProduct.Id))  // удаление из БД
                    {
                        BasketProductsView.Remove(bp);  // удаляем из коллекции UI
                        ViewModel.BasketProducts.Remove(bp.BasketProduct);  // удаляем из коллекции BasketProducts
                        await ViewModel.UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                    }
                    else
                    {
                        MessageBox.Show("При удалении, что-то пошло не так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }


        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            // 1. проверить, есть ли каждый выбранный товар в наличии (по кол-ву)
            // 2. если какие то не в наличие, то добавить их названия в строку и вывести
            // 3. 
            // 4. * товары не в наличии пометить как НЕ В НАЛИЧИИ (добавить свойство string в BasketProductViewModel и привязать к элементу XAML)
            foreach (var bpView in BasketProductsView)
            {
                
            }
        }
    }
}

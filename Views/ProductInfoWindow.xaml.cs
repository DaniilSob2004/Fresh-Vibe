using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class ProductInfoWindow : Window
    {
        public Data.Entity.Product Product { get; set; }
        public int AddAmount { get; set; }

        public ProductInfoWindow(Data.Entity.Product product)
        {
            InitializeComponent();
            DataContext = this;
            Product = product;
            AddAmount = 0;
        }

        private void BtnAddAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, Product, true);  // увеличиваем значение, передаём Product для дальнейшей проверки
        }

        private void BtnReduceAmountProduct_Click(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBlockAmountProductChangeValue(sender, Product, false);  // уменьшаем значение, передаём Product для дальнейшей проверки
        }

        private void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
        {
            TextBlock? textBlockAmount = GuiBaseManipulation.FindTextBlockAmountsProductBtnBasket(sender);  // получаем TextBlock в котором хранится кол-во товара
            if (textBlockAmount is not null)
            {
                int amount;
                if (int.TryParse(textBlockAmount.Text, out amount))  // преобразовываем в int
                {
                    AddAmount = amount;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}

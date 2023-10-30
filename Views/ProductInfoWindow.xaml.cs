using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using StoreExam.ViewModels;

namespace StoreExam.Views
{
    public partial class ProductInfoWindow : Window
    {
        public ProductViewModel ProductVM { get; set; }

        public ProductInfoWindow(ProductViewModel productVM)
        {
            InitializeComponent();
            ProductVM = productVM;
            DataContext = ProductVM;
        }

        private void BtnAddProductToBasket_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

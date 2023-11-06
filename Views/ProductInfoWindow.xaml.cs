using System.Windows;
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

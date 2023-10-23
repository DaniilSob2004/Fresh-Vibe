using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using StoreExam.Extensions;

namespace StoreExam.Views
{
    public partial class OrderWindow : Window
    {
        private List<ModelViews.BasketProductModel> listBuyBPModels;
        private float totalPrice;

        public OrderWindow(List<ModelViews.BasketProductModel> listBuyBPModels, float totalPrice)
        {
            InitializeComponent();
            this.listBuyBPModels = listBuyBPModels;
            this.totalPrice = totalPrice;
            LoadTotalPrice();  // загрузка цены
        }

        private void LoadTotalPrice()
        {
            textBlockTotalPrice.Text = totalPrice.Hrn();  // вызов расширения для float
        }

        private void DownloadReceiptTB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // печатаем чек в pdf
            FileWork.FilePdf filePdf = new();
            filePdf.ShowDialog();
            string message = filePdf.PrintReceiptForBasketProducts(listBuyBPModels, totalPrice) ? "Чек успешно сохранился" : "Чек не удалось сохранить";
            MessageBox.Show(message, "Сохранение чека", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}

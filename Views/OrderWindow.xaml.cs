using System;
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

            if (!String.IsNullOrEmpty(filePdf.SelectFile))  // если пользователь выбрал файл
            {
                if (filePdf.PrintReceiptForBasketProducts(listBuyBPModels, totalPrice))  // если pdf успешно создался
                {
                    if (checkBoxOpenPdf.IsChecked == true)  // если выбранно чтобы нужно запустить pdf-файл
                    {
                        FileWork.BaseFileWork.StartFile(filePdf.SelectFile);  // запускаем процесс
                    }
                    else { MessageBox.Show("Чек успешно сохранился", "Сохранение чека", MessageBoxButton.OK, MessageBoxImage.Information); }
                    Close();
                }
                else { MessageBox.Show("Чек не удалось сохранить", "Сохранение чека", MessageBoxButton.OK, MessageBoxImage.Information); }
            }
        }
    }
}

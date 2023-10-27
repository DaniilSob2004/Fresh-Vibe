using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StoreExam.Extensions;

namespace StoreExam.Views
{
    public partial class OrderWindow : Window
    {
        private static ContentType pdfType = new("application/pdf");
        private readonly MailMessage _mailMessage;
        private List<ViewModels.BasketProductModel> listBuyBPModels;
        private float totalPrice;
        private FileWork.FilePdf filePdf;
        private Network.EmailWork emailWork;

        public OrderWindow(Data.Entity.User user, List<ViewModels.BasketProductModel> listBuyBPModels, float totalPrice)
        {
            InitializeComponent();

            this.listBuyBPModels = listBuyBPModels;
            this.totalPrice = totalPrice;
            filePdf = new();
            emailWork = new();
            _mailMessage = Formatting.UserFormat.GetMailMessageForSendPdfEmail(user, emailWork.Email);  // получаем готовый MailMessage

            LoadTotalPrice();  // загрузка цены
        }


        public async Task SendPdfReceiptToEmail()
        {
            _mailMessage.Attachments.Add(new(filePdf.SelectFile, pdfType));
            if (!await emailWork.SendEmail(_mailMessage))
            {
                MessageBox.Show("При отправке почты, что-то пошло не так...", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadTotalPrice()
        {
            textBlockTotalPrice.Text = totalPrice.Hrn();  // вызов расширения для float
        }

        private void DownloadReceiptTB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // печатаем чек в pdf
            filePdf.ShowDialog();

            if (!String.IsNullOrEmpty(filePdf.SelectFile))  // если пользователь выбрал файл
            {
                if (filePdf.PrintReceiptForBasketProducts(listBuyBPModels, totalPrice))  // если pdf успешно создался
                {
                    if (checkBoxSendPdfEmail.IsChecked == true)  // если нужно pdf-файл отправить на email
                    {
                        _ = SendPdfReceiptToEmail();  // отправка email
                    }

                    if (checkBoxOpenPdf.IsChecked == true)  // если нужно запустить pdf-файл
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

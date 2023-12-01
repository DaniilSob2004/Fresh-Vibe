using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StoreExam.Extensions;
using static StoreExam.Formatting.ResourceHelper;

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
            _mailMessage = Formatting.EmailHelper.GetMailMessageForSendPdfEmail(user, emailWork.Email);  // получаем готовый MailMessage

            LoadTotalPrice();  // загрузка цены
        }


        private void LoadTotalPrice()
        {
            textBlockTotalPrice.Text = totalPrice.Hrn();  // вызов расширения для float
        }

        public async Task SendPdfReceiptToEmail()
        {
            _mailMessage.Attachments.Add(new(filePdf.SelectFile, pdfType));  // прикрепляем pdf
            if (!await emailWork.SendEmail(_mailMessage))
            {
                new MessageWindow(MessageValues.SendEmailErrorMess).ShowDialog();
            }
        }

        private void WorkWithReceipt(string message, bool isSendEmail)
        {
            if (filePdf.PrintReceiptForBasketProducts(listBuyBPModels, totalPrice))  // если pdf успешно создался
            {
                if (isSendEmail)  // если нужно отправить на email, то отправляем
                {
                    _ = SendPdfReceiptToEmail();
                }

                if (checkBoxOpenPdf.IsChecked == true)  // если нужно запустить pdf-файл, то запускаем процесс
                {
                    FileWork.BaseFileWork.StartFile(filePdf.SelectFile);
                }
                else
                {
                    new MessageWindow(message).ShowDialog();
                }
                Close();
            }
            else { new MessageWindow(MessageValues.SaveCheckErrorMess).ShowDialog(); }
        }


        private void DownloadReceiptTB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (checkBoxSendPdfEmail.IsChecked == true)  // если отправка pdf-файла на email
            {
                filePdf.CreateTempFile();
                WorkWithReceipt(MessageValues.SendCheckMess, true);
            }
            else  // сохранение файла локально
            {
                filePdf.ShowDialog();
                if (!String.IsNullOrEmpty(filePdf.SelectFile))  // если пользователь выбрал файл
                {
                    WorkWithReceipt(MessageValues.SaveCheckMess, false);
                }
            }
        }
    }
}

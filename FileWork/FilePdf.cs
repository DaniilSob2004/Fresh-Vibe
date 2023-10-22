using System;
using System.Collections.Generic;
using Ookii.Dialogs.Wpf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Windows;
using StoreExam.Formatting;
using StoreExam.Extensions;

namespace StoreExam.FileWork
{
    public class FilePdf
    {
        private VistaSaveFileDialog saveFileDialog;
        private string selectFile;

        public string SelectFile { get => selectFile; }

        public FilePdf()
        {
            saveFileDialog = new()
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                DefaultExt = "pdf"
            };
            selectFile = String.Empty;
        }

        public void ShowDialog()
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                selectFile = saveFileDialog.FileName;  // сохраняем путь к файлу
            }
        }

        public bool PrintReceiptForBasketProducts(List<ModelViews.BasketProductModel> listBPModels, float totalPrice)
        {
            try
            {
                using Document doc = new();
                PdfWriter.GetInstance(doc, new FileStream(SelectFile, FileMode.Create));
                doc.Open();

                // настройка шрифта
                BaseFont baseFont = BaseFont.CreateFont(@"c:\windows\fonts\cour.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font font = new(baseFont, 10);
                Font headerFont = new(baseFont, 18);

                // вывод названия магазина
                string storeName = Application.Current.TryFindResource("StoreName").ToString()!;
                Paragraph paragraph = new(storeName, headerFont) { Alignment = Element.ALIGN_CENTER };
                doc.Add(paragraph);

                // вывод даты
                paragraph = new(DateTime.Now.ToString(), font) { Alignment = Element.ALIGN_CENTER };
                doc.Add(paragraph);

                // вывод товаров (форматируем коллекцию товаров в строку)
                paragraph = new(ProductFormat.GetStringProductsForReceipt(listBPModels), font) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 30f };
                doc.Add(paragraph);

                // вывод суммы (вызов расширения Hrn() у float)
                string price = $"Сумма к оплате:  {totalPrice.Hrn()}";
                paragraph = new(price, font) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 30f };
                doc.Add(paragraph);

                paragraph = new("Спасибо за покупку!)", font) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 15f };
                doc.Add(paragraph);

                doc.Close();
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}

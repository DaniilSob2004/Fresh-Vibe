﻿using System;
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
        private static string tempFile = String.Empty;
        private VistaSaveFileDialog saveFileDialog;
        private string selectFile;

        public string SelectFile { get => selectFile; }

        public FilePdf()
        {
            tempFile = BaseFileWork.GetPathTempFilePdf("Receipt.pdf");  // путь к временному файлу
            saveFileDialog = new()
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                DefaultExt = "pdf"
            };
            selectFile = String.Empty;
        }


        public void CreateTempFile()
        {
            BaseFileWork.Delete(tempFile);  // удаляем временный файл
            selectFile = tempFile;
        }

        public void ShowDialog()
        {
            selectFile = String.Empty;
            if (saveFileDialog.ShowDialog() == true)
            {
                selectFile = saveFileDialog.FileName;  // сохраняем путь к файлу
            }
        }

        public bool PrintReceiptForBasketProducts(List<ViewModels.BasketProductModel> listBPModels, float totalPrice)
        {
            bool isNotDeleteFile = true;
            try
            {
                using (FileStream fileStream = new(SelectFile, FileMode.Create))
                using (Document doc = new())
                {
                    PdfWriter.GetInstance(doc, fileStream);
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
                    paragraph = new(ProductFormat.GetStringProductsForReceipt(listBPModels), font)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        SpacingBefore = 30f,
                        IndentationLeft = 60f
                    };
                    doc.Add(paragraph);

                    // вывод суммы (вызов расширения Hrn() у float)
                    string price = $"Сумма к оплате:  {totalPrice.Hrn()}";
                    paragraph = new(price, font) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 30f };
                    doc.Add(paragraph);

                    paragraph = new("Спасибо за покупку!)", font) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 15f };
                    doc.Add(paragraph);

                    return true;
                }
            }
            catch (Exception) { return isNotDeleteFile = false; }
            finally
            {
                if (!isNotDeleteFile)  // если что-то пошло не так, то удаляем созданный файл pdf
                {
                    BaseFileWork.Delete(SelectFile);
                }
            }
        }
    }
}

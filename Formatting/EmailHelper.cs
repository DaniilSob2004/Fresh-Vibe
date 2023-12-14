using System.Net.Mail;
using StoreExam.FileWork;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.Formatting
{
    public static class EmailHelper
    {
        public static string GetStringForConfirmEmail(Data.Entity.User user)
        {
            // считываем из ресурсов файл html
            return BaseFileWork.ReadFile("Resources/confirm_code_template.html")
                .Replace("{ConfirmEmailText}", Texts.ConfirmEmailText)
                .Replace("{ConfirmEmailTextHtml}", EmailText.ConfirmEmailTextHtml)
                .Replace("{WarningEmailTextHtml}", EmailText.WarningEmailTextHtml)
                .Replace("{ConfirmCode}", user.ConfirmCode)
                .Replace("{StoreName}", Texts.StoreName);
        }

        public static string GetStringForSendPdfEmail()
        {
            // считываем из ресурсов файл html
            return BaseFileWork.ReadFile("Resources/send_receipt_template.html")
                .Replace("{ThanksBuyText}", Texts.ThanksBuyText)
                .Replace("{PdfEmailTextHtml}", EmailText.PdfEmailTextHtml);
        }

        public static MailMessage GetMailMessageForConfirmEmail(Data.Entity.User user, string emailFrom)
        {
            // создаём MailMessage для отправки email
            return new(emailFrom,
                       user.Email!,
                       Texts.ConfirmEmailText,
                       GetStringForConfirmEmail(user)  // получаем строку для подтверждение email
                   )
                   {
                       IsBodyHtml = true,
                   };
        }

        public static MailMessage GetMailMessageForSendPdfEmail(Data.Entity.User user, string emailFrom)
        {
            return new(emailFrom,
                       user.Email!,
                       Texts.CheckPdfFileText,
                       GetStringForSendPdfEmail()  // получаем строку для отправки pdf на email
                   )
                   {
                       IsBodyHtml = true,
                   };
        }
    }
}

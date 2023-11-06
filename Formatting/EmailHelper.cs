using System.Net.Mail;
using System.Windows;
using StoreExam.FileWork;

namespace StoreExam.Formatting
{
    public static class EmailHelper
    {
        public static string GetStringForConfirmEmail(Data.Entity.User user)
        {
            // считываем из ресурсов файл html
            return BaseFileWork.ReadFile("Resources/confirm_code_template.html")
                        .Replace("{ConfirmCode}", user.ConfirmCode)
                        .Replace("{StoreName}", Application.Current.TryFindResource("StoreName").ToString());
        }

        public static string GetStringForSendPdfEmail()
        {
            return BaseFileWork.ReadFile("Resources/send_receipt_template.html");
        }

        public static MailMessage GetMailMessageForConfirmEmail(Data.Entity.User user, string emailFrom)
        {
            // создаём MailMessage для отправки email
            return new(emailFrom,
                       user.Email!,
                       "Подтверждение почты",
                       GetStringForConfirmEmail(user)  // получаем строку для отправки email
                   )
                   {
                       IsBodyHtml = true,
                   };
        }

        public static MailMessage GetMailMessageForSendPdfEmail(Data.Entity.User user, string emailFrom)
        {
            return new(emailFrom,
                       user.Email!,
                       "Чек PDF-файл",
                       GetStringForSendPdfEmail()  // получаем строку для отправки email
                   )
                   {
                       IsBodyHtml = true,
                   };
        }
    }
}

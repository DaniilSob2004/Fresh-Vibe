using System.Net.Mail;
using System.Windows;

namespace StoreExam.Formatting
{
    public static class UserFormat
    {
        public static string GetStringForConfirmEmail(Data.Entity.User user)
        {
            return $"<h1>Подтвердите email</h1>" +
                   $"<p>Для подтверждения адреса электронной почты используйте код: <span style='color: tomato; font-weight: bold;'>{user.ConfirmCode}</span></p>" +
                   $"<p>Если вы не являетесь пользователем {Application.Current.TryFindResource("StoreName")}, не предпринимайте никаких действий</p>";
        }

        public static string GetStringForSendPdfEmail()
        {
            return $"<h1>Спасибо за покупку!)</h1>" +
                   $"<p>Ваш чек pdf-файлом</p>";
        }

        public static MailMessage GetMailMessageForConfirmEmail(Data.Entity.User user, string emailFrom)
        {
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

using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace StoreExam.Network
{
    public class EmailWork
    {
        private string host = null!;
        public string Host => host;

        private int port;
        public int Port => port;

        private string email = null!;
        public string Email => email;

        private string password = null!;
        public string Password => password;

        public bool ssl;
        public bool Ssl => ssl;

        private bool isConfigurationOk;  // можно ли работать с этим классом (все ли корректно с конфигурацией)

        public EmailWork()
        {
            isConfigurationOk = GetConfigurations();  // настраиваем данные
        }


        private bool GetConfigurations()
        {
            // получаем данные о host, port, email, password и ssl из конфигурации(json-файла)
            string? host = App.GetConfiguration("smtp:host");
            if (host is not null) { this.host = host; }
            else { return false; }

            string? portString = App.GetConfiguration("smtp:port");
            if (portString is null) { return false; }
            int port;
            try
            {
                port = Convert.ToInt32(portString);
                this.port = port;
            }
            catch { return false; }

            string? email = App.GetConfiguration("smtp:email");
            if (email is not null) { this.email = email; }
            else { return false; }

            string? password = App.GetConfiguration("smtp:password");
            if (password is not null) { this.password = password; }
            else { return false; }

            string? sslString = App.GetConfiguration("smtp:ssl");
            if (sslString is null) { return false; }
            bool ssl;
            try
            {
                ssl = Convert.ToBoolean(sslString);
                this.ssl = ssl;
            }
            catch { return false; }

            return true;
        }

        public SmtpClient? GetSmtpClient()
        {
            if (!isConfigurationOk)
            {
                return null;
            }

            // возвращаем новый экземпляр SmtpClient
            return new(Host, Port)
            {
                EnableSsl = Ssl,
                Credentials = new NetworkCredential(Email, Password)
            };
        }

        public async Task<bool> SendEmail(MailMessage mailMessage)
        {
            using (SmtpClient? smtpClient = GetSmtpClient())
            {
                if (smtpClient is null) { return false; }
                try
                {
                    using (mailMessage)  // для освобождения ресурсов
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                    return true;
                }
                catch (Exception) { return false; }
                finally { smtpClient.Dispose(); }
            }
        }
    }
}

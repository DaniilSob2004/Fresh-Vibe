using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net.Mail;
using StoreExam.CheckData;
using System.Windows.Input;

namespace StoreExam.Views
{
    public partial class ConfirmEmailWindow : Window
    {
        public Data.Entity.User User { get; set; }
        private Network.EmailWork emailWork;
        private readonly MailMessage _mailMessage;

        public ConfirmEmailWindow(Data.Entity.User user)
        {
            InitializeComponent();
            DataContext = this;
            User = user;
            emailWork = new();
            _mailMessage = Formatting.EmailHelper.GetMailMessageForConfirmEmail(user, emailWork.Email);  // получаем готовый MailMessage
        }


        public async Task<bool> SendCodeToEmail()
        {
            if (!await emailWork.SendEmail(_mailMessage))
            {
                MessageBox.Show("При отправке почты, что-то пошло не так...", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ShowErrorLabel()
        {
            textBlockShowError.Visibility = Visibility.Visible;
        }


        private void TextBlockSkip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private async void BtnConfirmCode_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                if (CheckUser.CheckConfirmCode(User, textBoxConfirmCode.Text))
                {
                    User.ConfirmCode = null;  // код подтверждён
                    if (await Data.DAL.UserDal.Update(User))  // обновление данных в БД
                    {
                        MessageBox.Show("Почта подтверждена", "Подтверждение почты", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
                else
                {
                    ShowErrorLabel();  // некорректный ввод
                }
            }
        }

        private async void TextBlockAgainSendCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (await SendCodeToEmail())  // отправка кода
            {
                MessageBox.Show("Код отправлен", "Подтверждение почты", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

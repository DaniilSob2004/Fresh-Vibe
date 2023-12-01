using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.UI_Settings;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.Views
{
    public partial class SignIn : Window
    {
        public static Window? mainLoginWindow;  // ссылка на окно родителя (MainLoginWindow)
        public Data.Entity.User User { get; set; }
        private CancellationTokenSource cts = null!;  // источник токенов

        public SignIn()
        {
            InitializeComponent();
            DataContext = this;
            User = new() { Email = DefaultValues.DefaultEmail, Password = DefaultValues.DefaultPassword };
        }

        private void Window_Clossing(object sender, EventArgs e)
        {
            if ((bool)mainLoginWindow!.Tag)  // если true, значит показываем окно
            {
                mainLoginWindow?.Show();  // показываем главное окно входа
            }
        }


        private void OpenMainWindow(Data.Entity.User user)
        {
            ShowMessage(MessageValues.WelcomeMess.Replace("{UserName}", user.Name));
            GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
            new MainWindow(user, mainLoginWindow!).ShowDialog();  // запускаем основное окно и передаём объект user
        }

        private void CancelLoadingSignInBtn()
        {
            cts?.Cancel();  // для отмены работы async метода
            GuiBaseManipulation.CancelLoadingButton(btnSignIn, Texts.SignInText);  // возвращаем исходное состояние
        }

        private void ShowMessage(string message)
        {
            CancelLoadingSignInBtn();  // возвращаем состояние кнопки в исходное
            new MessageWindow(message).ShowDialog();  // запускаем окно уведомлений
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно регистрации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == Texts.SignUpText)
                {
                    GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
                    new SignUp().ShowDialog();  // запускаем окно регистрации
                }
            }
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // при изменение TextBox, присваиваем значение в PasswordBox
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // при изменение PasswordBox, присваиваем значение в TextBox
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSignIn, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                Data.Entity.User? user = await Data.DAL.UserDal.Get(User.Email);  // находим user по email
                if (user is null) { ShowMessage(MessageValues.ErrorEmailMess); return; }

                if (!CheckUser.PasswordEntryVerification(user, User.Password)) { ShowMessage(MessageValues.ErrorPassMess); return; }

                if (!CheckUser.CheckIsDeletedUser(user))  // если аккаунт пользователя не удалён
                {
                    // запускаем выполнение загрузки категорий и продуктов
                    await Data.DAL.CategoriesDal.LoadData().ContinueWith(task => Data.DAL.ProductsDal.LoadData());
                    OpenMainWindow(user);  // запускаем главное окно
                }
                else
                {
                    ShowMessage(MessageValues.ErrorEmailMess);
                }
            }
            catch (Exception) { ShowMessage(MessageValues.BaseErrorMess); }
        }
    }
}

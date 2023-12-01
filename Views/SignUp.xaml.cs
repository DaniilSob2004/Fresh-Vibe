using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.Data.DAL;
using StoreExam.UI_Settings;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.Views
{
    public partial class SignUp : Window
    {
        public static Window? mainLoginWindow;  // ссылка на окно родителя (MainLoginWindow)
        public Data.Entity.User User { get; set; }
        private CancellationTokenSource cts = null!;  // источник токенов

        public SignUp()
        {
            InitializeComponent();
            DataContext = this;
            User = new()
            {
                Name = DefaultValues.DefaultName,
                Surname = DefaultValues.DefaultSurname,
                NumTel = DefaultValues.DefaultNumTel,
                Email = DefaultValues.DefaultEmail,
                Password = DefaultValues.DefaultPassword,
                ConfirmCode = DefaultValues.DefaultConfirmCode
            };
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if ((bool)mainLoginWindow!.Tag)  // если true, значит показываем окно
            {
                mainLoginWindow?.Show();  // показываем главное окно входа
            }
        }


        private async Task SuccessUserSignUp()
        {
            await UserDal.Add(User);  // добавление User в БД
            ShowMessage(MessageValues.WelcomeSignUpMess.Replace("{UserName}", User.Name));
            OpenConfirmEmailWindow();  // запускаем окно подтверждения почты
        }

        private void OpenConfirmEmailWindow()
        {
            if (GuiBaseManipulation.OpenConfirmEmailWindow(this, User))  // запускаем окно подтверждения почты
            {
                Close();  // закрываем окно регистрации (возвращаемся в главное окно входа)
            }
        }

        private void CancelLoadingSignUpBtn()
        {
            cts?.Cancel();  // для отмены работы ассинхроного метода
            GuiBaseManipulation.CancelLoadingButton(btnSignUp, Texts.SignUpText);  // возвращаем исходное состояние
        }

        private void ShowMessage(string message)
        {
            CancelLoadingSignUpBtn();  // возвращаем состояние кнопки в исходное
            new MessageWindow(message).ShowDialog();  // запускаем окно уведомлений
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно авторизации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == Texts.SignInText)
                {
                    GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
                    new SignIn().ShowDialog();  // запускаем окно авторизации
                }
            }
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border TextBox изменяется на красный
            GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // при изменение TextBox, присваиваем значение в PasswordBox
            GuiBaseManipulation.SetPasswordBox(textBoxShowPasswordCheck, passwordCheck);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border PasswordBox изменяется на красный
            GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // при изменение PasswordBox, присваиваем значение в TextBox
            GuiBaseManipulation.SetTextBox(textBoxShowPasswordCheck, passwordCheck);
        }

        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSignUp, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                // данные не корректны
                if (!CheckUser.CheckAllData(User)) { ShowMessage(MessageValues.NotWriteFieldMess); return; }

                // получаем названия полей, которые не уникальны
                string? notUniqueFields = await CheckUser.CheckUniqueUserInDB(User);
                if (notUniqueFields is not null) { ShowMessage(MessageValues.NotUniqueFieldMess.Replace("{NotUniqueFields}", $"\n{notUniqueFields}")); return; }

                if (CheckUser.CheckPasswordByString(User, passwordCheck.Password))  // пароль и пароль-подтверждения совпадают
                {
                    await SuccessUserSignUp();  // действие при успешной регистрации
                }
                else { ShowMessage(MessageValues.ErrorTwoPassMess); }
            }
            catch (Exception) { ShowMessage(MessageValues.BaseErrorMess); }
        }
    }
}

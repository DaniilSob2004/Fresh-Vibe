using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.Data.DAL;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class SignUp : Window
    {
        // статические поля, для хранения значений по умолчанию, которые хранятся в ресурсах UserDefault.xaml
        public static string DefaultName = Application.Current.TryFindResource("DefName").ToString()!;
        public static string DefaultSurname = Application.Current.TryFindResource("DefSurname").ToString()!;
        public static string DefaultNumTel = Application.Current.TryFindResource("DefNumTel").ToString()!;
        public static string DefaultEmail = Application.Current.TryFindResource("DefEmail").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public static string DefaultConfirmCode = Application.Current.TryFindResource("DefConfirmCode").ToString()!;
        public static string SignInText = Application.Current.TryFindResource("SignInText").ToString()!;
        public static string SignUpText = Application.Current.TryFindResource("SignUpText").ToString()!;

        public static Window? mainLoginWindow;  // ссылка на окно родителя (MainLoginWindow)
        public Data.Entity.User User { get; set; }
        private CancellationTokenSource cts = null!;  // источник токенов

        public SignUp()
        {
            InitializeComponent();
            DataContext = this;
            User = new() { Name = DefaultName, Surname = DefaultSurname, NumTel = DefaultNumTel, Email = DefaultEmail, Password = DefaultPassword, ConfirmCode = DefaultConfirmCode };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((bool)mainLoginWindow!.Tag)  // если true, значит показываем окно
            {
                mainLoginWindow?.Show();  // показываем главное окно входа
            }
        }


        private async Task SuccessUserSignUp()
        {
            CancelLoadingSignUpBtn();  // возвращаем состояние кнопки в исходное
            await UserDal.Add(User);  // добавление User в БД
            MessageBox.Show($"{User.Name}, вы успешно зарегистрировались!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
            GuiBaseManipulation.CancelLoadingButton(btnSignUp, SignUpText);  // возвращаем исходное состояние
        }

        private void ShowErrorMessage(string message)
        {
            CancelLoadingSignUpBtn();  // возвращаем состояние кнопки в исходное
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно авторизации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == SignInText)
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
                if (!CheckUser.CheckAllData(User)) { ShowErrorMessage($"Не все поля заполнены!\n(Пароль не менее {CheckUser.MinPassword} символов)"); return; }

                // получаем названия полей, которые не уникальны
                string? notUniqueFields = await CheckUser.CheckUniqueUserInDB(User);
                if (notUniqueFields is not null) { ShowErrorMessage($"Некоторые поля не уникальны:\n{notUniqueFields}"); return; }

                if (CheckUser.CheckPasswordByString(User, passwordCheck.Password))  // пароль и пароль-подтверждения совпадают
                {
                    await SuccessUserSignUp();  // действие при успешной регистрации
                }
                else { ShowErrorMessage("Пароли не совпадают!"); }
            }
            catch (Exception) { ShowErrorMessage("Что-то пошло нет так...\nПопробуйте позже!"); }
        }
    }
}

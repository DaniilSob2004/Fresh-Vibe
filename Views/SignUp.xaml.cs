using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.Data.DAL;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class SignUp : Window
    {
        // статические поля, для хранения значений по умолчанию для user, которые хранятся в ресурсах UserDefault.xaml
        public static string DefaultName = Application.Current.TryFindResource("DefName").ToString()!;
        public static string DefaultSurname = Application.Current.TryFindResource("DefSurname").ToString()!;
        public static string DefaultNumTel = Application.Current.TryFindResource("DefNumTel").ToString()!;
        public static string DefaultEmail = Application.Current.TryFindResource("DefEmail").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public static string DefaultConfirmCode = Application.Current.TryFindResource("DefConfirmCode").ToString()!;

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


        private async Task AddUserInDB()
        {
            CheckUser.CheckAndChangeDefaultEmail(User);  // проверка на email по-умолчанию (чтобы установить в null если стоит по-умолчанию)
            await UserDal.Add(User);  // добавление User в БД
            MessageBox.Show($"Вы успешно зарегистрировались, {User.Name}!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenConfirmEmailWindow()
        {
            if (User.Email is not null)  // если почта указана
            {
                Hide();
                var dialog = new ConfirmEmailWindow(User);  // запускаем окно подтверждения почты
                _ = dialog.SendCodeToEmail();  // отправка кода на email пользователя
                dialog.ShowDialog();
                Close();  // закрываем окно регистрации (возвращаемся в главное окно входа)
            }
        }

        private void CancelLoadingSignUpBtn()
        {
            cts?.Cancel();  // для отмены работы ассинхроного метода
            GuiBaseManipulation.CancelLoadingButton(btnSignUp, "SIGN UP");  // возвращаем исходное состояние
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно авторизации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == "Sign In")
                {
                    GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
                    new SignIn().ShowDialog();  // запускаем окно авторизации
                }
            }
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border TextBox изменяется на красный

            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // при изменение TextBox, присваиваем значение в PasswordBox
            if (passwordCheck.Password != textBoxShowPasswordCheck.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPasswordCheck, passwordCheck);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border TextBox изменяется на красный

            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // при изменение PasswordBox, присваиваем значение в TextBox
            if (passwordCheck.Password != textBoxShowPasswordCheck.Text)
                GuiBaseManipulation.SetTextBox(textBoxShowPasswordCheck, passwordCheck);
        }

        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSignUp, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                if (CheckUser.CheckAllData(User))  // данные корректны
                {
                    string? notUniqueFields = await CheckUser.CheckUniqueUserInDB(User);  // получаем названия полей, которые не уникальны
                    if (notUniqueFields is null)  // данные уникальны
                    {
                        if (CheckUser.CheckPasswordByString(User, passwordCheck.Password))  // пароль и пароль-подтверждения совпадают
                        {
                            CancelLoadingSignUpBtn();  // возвращаем состояние кнопки в исходное
                            await AddUserInDB();  // добавление User в БД
                            OpenConfirmEmailWindow();  // запускаем окно подтверждения почты
                        }
                        else ShowErrorMessage("Пароли не совпадают!");
                    }
                    else ShowErrorMessage($"Поля не уникальны:\n{notUniqueFields}");
                }
                else ShowErrorMessage($"Не все поля заполнены!\n(Пароль не менее {CheckUser.MinPassword} символов)");
            }
            catch (Exception) { ShowErrorMessage("Что-то пошло нет так...\nПопробуйте позже!"); }
            finally { CancelLoadingSignUpBtn(); }  // возвращаем состояние кнопки в исходное
        }
    }
}

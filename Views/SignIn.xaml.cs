using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class SignIn : Window
    {
        // статические поля, для хранения значений по умолчанию для user, которые хранятся в ресурсах UserDefault.xaml
        public static string DefaultNumTel = Application.Current.TryFindResource("DefNumTel").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public Data.Entity.User User { get; set; }
        private CancellationTokenSource cts = null!;  // источник токенов

        public SignIn()
        {
            InitializeComponent();
            DataContext = this;
            User = new() { NumTel = DefaultNumTel, Password = DefaultPassword };
        }


        private async Task OpenMainWindow(Data.Entity.User user)
        {
            Task loadProductTask = Data.DAL.ProductsDal.LoadData();  // запускаем выполнение загрузки продуктов
            Task loadCatTask = Data.DAL.CategoriesDal.LoadData();  // запускаем выполнение загрузки продуктов

            CancelLoadingSignInBtn();  // возвращаем состояние кнопки в исходное
            MessageBox.Show($"Добро пожаловать {user.Name}", "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();  // закрываем окно авторизации

            await loadProductTask;  // получаем результат
            await loadCatTask;  // получаем результат
            //await Data.DAL.CategoriesDal.LoadData();  // загружаем категории

            new MainWindow(user).ShowDialog();  // запускаем основное окно и передаём объект user
        }

        private void CancelLoadingSignInBtn()
        {
            cts?.Cancel();  // для отмены работы ассинхроного метода
            GuiBaseManipulation.CancelLoadingButton(btnSignIn, "SIGN IN");  // возвращаем исходное состояние
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно регистрации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == "Sign Up")
                {
                    Close();  // закрываем окно авторизации
                    new SignUp().ShowDialog();  // запускаем окно регистрации
                }
            }
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // при изменение TextBox, присваиваем значение в PasswordBox
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // при изменение PasswordBox, присваиваем значение в TextBox
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSignIn, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                Data.Entity.User? user = await Data.DAL.UserDal.GetUser(User.NumTel);  // находим user по номер тел.
                if (user is not null)  // если такой user есть
                {
                    if (CheckUser.PasswordEntryVerification(user, User.Password))  // если пароль введён верный
                    {
                        if (CheckUser.CheckIsDeletedUser(user))  // если аккаунт user-а удалён
                        {
                            ShowErrorMessage("Неверный номер телефона!");
                        }
                        else
                        {
                            await OpenMainWindow(user);  // запускаем главное окно
                        }
                    }
                    else ShowErrorMessage("Неверный пароль!");
                }
                else ShowErrorMessage("Неверный номер телефона!");
            }
            catch (Exception) { ShowErrorMessage("Что-то пошло нет так...\nПопробуйте позже!"); }
            finally { CancelLoadingSignInBtn(); }   // возвращаем состояние кнопки в исходное состояние
        }
    }
}

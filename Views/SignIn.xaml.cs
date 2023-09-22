using System;
using System.Collections.Generic;
using System.Text;
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
        // статические поля, для хранения значений по умолчанию для user, которые хранятся в ресурсах
        public static string DefaultNumTel = Application.Current.TryFindResource("DefNumTel").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public Data.Entity.User User { get; set; } = new() { NumTel = DefaultNumTel, Password = DefaultPassword };

        public SignIn()
        {
            InitializeComponent();
            DataContext = this;
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

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.Entity.User? user = Data.DAL.UserDal.GetUser(User.NumTel);  // находим user по номер тел.
                if (user is not null)  // если такой user есть
                {
                    if (CheckUser.PasswordEntryVerification(user, User.Password))  // если пароль введён верный
                    {
                        if (CheckUser.CheckIsDeletedUser(user))  // если аккаунт user-а удалён
                        {
                            MessageBox.Show("Неверный номер телефона!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Добро пожаловать {user.Name}", "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();  // закрываем окно авторизации
                            new MainWindow(user).ShowDialog();  // запускаем основное окно и передаём объект user
                        }
                    }
                    else MessageBox.Show("Неверный пароль!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else MessageBox.Show("Неверный номер телефона!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

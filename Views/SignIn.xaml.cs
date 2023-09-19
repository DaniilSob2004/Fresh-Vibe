using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using StoreExam.CheckData;

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
            ClearTextBox();
            DataContext = this;
        }

        private void ClearTextBox()
        {
            // очищаем поля для ввода
            numTel.Foreground = Brushes.Gray;
            numTel.Text = DefaultNumTel;
            password.Foreground = Brushes.Gray;
            password.Password = DefaultPassword;
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == "Sign Up")
                {
                    Close();  // закрываем окно авторизации
                    new SignUp().ShowDialog();  // запускаем окно регистрации
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxGotFocus(sender);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxLostFocus(sender);
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // чтобы значения двух полей для пароля совпадали
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // чтобы значения двух полей для пароля совпадали
        }

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            Data.Entity.User? user = Data.DAL.UserDal.GetUser(User.NumTel);
            if (user is not null)  // если такой user есть
            {
                if (Data.DAL.UserDal.CheckPassword(user, User.Password))  // если пароль введён верный
                {
                    if (CheckUser.CheckIsDeletedUser(user))  // если аккаунт user-а удалён
                    {
                        MessageBox.Show("Неверный номер телефона!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Добро пожаловать {user.Name}", "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();  // закрываем окно авторизации
                        new MainWindow(user).ShowDialog();  // запускаем основное окно и передаём объект User
                    }
                }
                else
                {
                    MessageBox.Show("Неверный пароль!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Неверный номер телефона!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

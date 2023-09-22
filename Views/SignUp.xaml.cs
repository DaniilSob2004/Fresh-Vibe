using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
        public Data.Entity.User User { get; set; } = new() { Name = DefaultName, Surname = DefaultSurname, NumTel = DefaultNumTel, Email = DefaultEmail, Password = DefaultPassword };

        public SignUp()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void AddUserInDB()
        {
            CheckUser.CheckAndChangeDefaultEmail(User);  // проверка на email по-умолчанию (чтобы установить в null если стоит по-умолчанию)
            UserDal.Add(User);  // добавление User в БД
            MessageBox.Show($"Вы успешно зарегистрировались, {User.Name}!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно авторизации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == "Sign In")
                {
                    Close();  // закрываем окно регистрации
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

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckUser.CheckAllData(User))  // данные корректны
                {
                    string? notUniqueFields = null;  // передаём ссылку в метод
                    if (CheckUser.CheckUniqueUserInDB(User, ref notUniqueFields))  // данные уникальны
                    {
                        if (CheckUser.CheckPasswordByString(User, passwordCheck.Password))  // пароль и пароль-подтверждения совпадают
                        {
                            AddUserInDB();  // добавление User в БД
                            Close();  // закрываем окно регистрации (возвращаемся в главное окно входа)
                        }
                        else MessageBox.Show("Пароли не совпадают!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else MessageBox.Show($"Данный {notUniqueFields}уже используются", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else MessageBox.Show($"Не все поля заполнены!\n(Пароль не менее {CheckUser.MinPassword} символов)", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло нет так...\nПопробуйте позже!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

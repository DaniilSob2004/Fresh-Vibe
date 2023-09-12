using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using StoreExam.CheckData;
using StoreExam.Data.DAL;

namespace StoreExam.Views
{
    public partial class SignUp : Window
    {
        // статические поля, для хранения значений по умолчанию для user, которые хранятся в ресурсах
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


        private void CheckCorrectData(object sender)
        {
            if (sender is TextBox textBox && textBox.Tag is not null)
            {
                // узнаём какое это поле
                string? tag = textBox.Tag.ToString();
                if (tag == DefaultName)
                {
                    // если в поле строка по умолчанию или данные неверно введены, то красим периметр красным цветом
                    if (User.Name != DefaultName && !CheckUser.CheckName(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == DefaultSurname)
                {
                    if (User.Surname != DefaultSurname && !CheckUser.CheckSurname(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == DefaultNumTel)
                {
                    if (User.NumTel != DefaultNumTel && !CheckUser.CheckNumTel(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == DefaultEmail)
                {
                    if (User.Email != DefaultEmail && !CheckUser.CheckEmail(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
            }
        }

        private bool CheckUniqueUserInDB()
        {
            // проверяем в таблице User поле(которое должно быть уникальным), и если такое значение уже есть, то не уникально
            string? notUniqueFields = null;

            bool isUnique = !UserDal.IsUniqueNumTel(User.NumTel);  // проверка номера тел. на уникальность
            if (!isUnique) notUniqueFields += "номер тел., ";

            if (User.Email != DefaultEmail)  // если значение email не по-умолчанию(пользователь не указал)
            {
                isUnique = !UserDal.IsUniqueEmail(User.Email);  // проверка email на уникальность
                if (!isUnique) notUniqueFields += "email, ";
            }

            if (!String.IsNullOrEmpty(notUniqueFields))
            {
                MessageBox.Show($"Данный {notUniqueFields}уже используются", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else return true;
        }

        private void AddUserInDB()
        {
            CheckUser.CheckAndChangeDefaultEmail(User);  // проверка на email по-умолчанию (чтобы установить в null если стоит по-умолч.)
            UserDal.Add(User);  // добавление User в БД
            MessageBox.Show($"Вы успешно зарегистрировались, {User.Name}!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            MessageBox.Show(User.Name + " - " + User.Surname + " - " + User.NumTel + " - " + User.Email + " - " + User.Password + " - " + User.Salt);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == "Sign In")
                {
                    Close();  // закрываем окно регистрации
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
            CheckCorrectData(sender);

            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // чтобы значения двух полей для пароля совпадали
            if (passwordCheck.Password != textBoxShowPasswordCheck.Text)
                GuiBaseManipulation.SetPasswordBox(textBoxShowPasswordCheck, passwordCheck);  // чтобы значения двух полей для доп.ввода пароля совпадали
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckCorrectData(sender);

            if (password.Password != textBoxShowPassword.Text)
                GuiBaseManipulation.SetTextBoxPassword(textBoxShowPassword, password);  // чтобы значения двух полей для пароля совпадали
            if (passwordCheck.Password != textBoxShowPasswordCheck.Text)
                GuiBaseManipulation.SetTextBoxPassword(textBoxShowPasswordCheck, passwordCheck);  // чтобы значения двух полей для доп.ввода пароля совпадали
        }

        private void ShowPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.Tag.ToString()!.EndsWith("Check"))  // проверка чтобы узнать какое поле пароля показывать
                    GuiBaseManipulation.TextBoxShowPassword(textBoxShowPasswordCheck);
                else
                    GuiBaseManipulation.TextBoxShowPassword(textBoxShowPassword);
            }
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUser.CheckAllData(User))  // данные корректны
            {
                if (CheckUniqueUserInDB())  // данные уникальны
                {
                    if (CheckUser.CheckPasswordByString(User, passwordCheck.Password))  // пароль и пароль-подтверждения совпадают
                    {
                        AddUserInDB();  // добавление User в БД
                        Close();
                    }
                    else MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else MessageBox.Show($"Не все поля заполнены!\n(Пароль не менее {(int)CheckUser.RegistrationCheck.MinPassword} символов)", "Ошибка",
                                 MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

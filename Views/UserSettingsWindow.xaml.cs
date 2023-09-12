using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StoreExam.CheckData;

namespace StoreExam.Views
{
    public partial class UserSettingsWindow : Window
    {
        public Data.Entity.User User { get; set; }  // копия user-a
        public Data.Entity.User origUser;  // объект user, до модификации
        public bool isDelAccount;  // флаг, удалил ли пользователь аккаунт
        public bool isSaveData;  // флаг, изменил ли пользователь свои данные

        public UserSettingsWindow(Data.Entity.User user)
        {
            InitializeComponent();
            User = user;
            origUser = CheckUser.GetClone(user);
            isDelAccount = false;
            isSaveData = false;
            this.DataContext = this;
        }


        private void CheckCorrectData(object sender)
        {
            if (sender is TextBox textBox && textBox.Tag is not null)
            {
                // узнаём какое это поле, сравнивая tag со значениями User-a по умолчанию
                string? tag = textBox.Tag.ToString();
                if (tag == Application.Current.TryFindResource("DefName").ToString()!)
                {
                    // если введено некорректное значение, то меняем цвет ободка textBox на красный
                    if (!CheckUser.CheckName(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == Application.Current.TryFindResource("DefSurname").ToString()!)
                {
                    if (!CheckUser.CheckSurname(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == Application.Current.TryFindResource("DefNumTel").ToString()!)
                {
                    if (!CheckUser.CheckNumTel(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else if (tag == Application.Current.TryFindResource("DefEmail").ToString()!)
                {
                    if (!CheckUser.CheckEmail(User)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                }
                else
                {
                    if (!CheckUser.CheckPassword(textBox.Text)) textBox.BorderBrush = Brushes.Red;
                    else textBox.BorderBrush = Brushes.Gray;
                    User.Password = textBox.Text;  // обновляем отдельно пароль user-а
                }
            }
        }

        private bool CheckUniqueUserInDB()
        {
            // проверяем в таблице User поле(которое должно быть уникальным), и если такое значение уже есть, то не уникально
            string? notUniqueFields = null;

            // если данные о номер тел. были изменены и они не уникальны
            // TODO: (если NumTel совпадает с NumTel удалёноого user-а, то все равно этот NumTel уникален)
            if (User.NumTel != origUser.NumTel && Data.DAL.UserDal.IsUniqueNumTel(User.NumTel))
            {
                notUniqueFields += "номер тел., ";
            }
            // если данные о email были изменены и они не уникальны
            if (User.Email != origUser.Email && Data.DAL.UserDal.IsUniqueEmail(User.Email))
            {
                notUniqueFields += "email, ";
            }

            if (!String.IsNullOrEmpty(notUniqueFields))
            {
                MessageBox.Show($"Данный {notUniqueFields}уже используются", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else return true;
        }

        private bool CheckNewPassword()
        {
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) &&
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text) &&
                   textBoxNewPassword.Text == textBoxConfirmNewPassword.Text;
        }

        private bool IsChangePassword()
        {
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxNewPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text);
        }


        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            CheckCorrectData(sender);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUser.CheckAllData(User))  // данные корректны
            {
                if (CheckUniqueUserInDB())  // данные уникальны
                {
                    if (IsChangePassword())  // если пароль был изменён, значит user его поменял
                    {
                        if (Data.DAL.UserDal.CheckPassword(origUser, textBoxOldPassword.Text))  // если пароль введён верный
                        {
                            if (CheckNewPassword())  // проверка двух полей для нового пароля
                            {
                                User.Password = PasswordHasher.HashPassword(User.Password, origUser.Salt);  // хэшируем новый пароль, на основе старой соли
                                isSaveData = true;  // флаг сохранения
                                DialogResult = false;  // сохранение свойств user-а
                            }
                            else MessageBox.Show("Новые пароли не совпадают!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else MessageBox.Show("Пароль подтверждения неверный!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        isSaveData = true;  // флаг сохранения
                        DialogResult = false;  // сохранение свойств user-а
                    }
                }
            }
            else MessageBox.Show($"Не все поля заполнены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;  // выход из аккаунта
        }

        private void BtnDelAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить аккаунт?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                isDelAccount = true;  // указываем что пользователь удалил аккаунт
                DialogResult = true;  // выход из аккаунта
            }
        }
    }
}

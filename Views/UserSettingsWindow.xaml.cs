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
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class UserSettingsWindow : Window
    {
        public Data.Entity.User User { get; set; }  // копия user-a
        public Data.Entity.User origUser;  // объект user, до модификации
        public StateData stateUserData;  // состояние данных пользователя

        public UserSettingsWindow(Data.Entity.User user)
        {
            InitializeComponent();
            User = user;
            origUser = CheckUser.GetClone(user);  // сохраняем копию
            stateUserData = StateData.Cancel;
            this.DataContext = this;
        }


        private bool CheckNewPassword()
        {
            // проверяем, совпадает ли новый пароль с подтверждённым
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) &&
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text) &&
                   textBoxNewPassword.Text == textBoxConfirmNewPassword.Text;
        }

        private bool IsChangePassword()
        {
            // если для какого-то поля для пароля был ввод, то true
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxNewPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text);
        }


        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border TextBox изменяется на красный
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckUser.CheckAllData(User))  // данные корректны
            {
                string? notUniqueFields = null;  // передаём ссылку в метод
                if (CheckUser.CheckUniqueUserInDB(User, ref notUniqueFields))  // данные уникальны
                {
                    if (IsChangePassword())  // если пароль был изменён, значит user его поменял
                    {
                        if (CheckUser.PasswordEntryVerification(origUser, textBoxOldPassword.Text))  // если пароль введён верный
                        {
                            if (CheckNewPassword())  // проверка двух полей для нового пароля
                            {
                                User.Password = PasswordHasher.HashPassword(User.Password, origUser.Salt);  // хэшируем новый пароль, на основе соли
                                stateUserData = StateData.Save;  // сохраняем состояние работы окна
                                DialogResult = true;  // закрываем окно
                            }
                            else MessageBox.Show("Новые пароли не совпадают!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else MessageBox.Show("Пароль подтверждения неверный!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        stateUserData = StateData.Save;  // сохраняем состояние работы окна
                        DialogResult = true;  // закрываем окно
                    }
                }
                else MessageBox.Show($"Данный {notUniqueFields}уже используются", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else MessageBox.Show($"Не все поля заполнены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            stateUserData = StateData.Exit;  // сохраняем состояние работы окна
            DialogResult = true;  // закрываем окно
        }

        private void BtnDelAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить аккаунт?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                stateUserData = StateData.Delete;  // сохраняем состояние работы окна
                DialogResult = true;  // закрываем окно
            }
        }
    }
}

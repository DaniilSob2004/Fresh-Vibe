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
    public partial class UserAccountWindow : Window
    {
        public Data.Entity.User User { get; set; }  // ссылка на объект User
        public bool isDelAccount;  // флаг, удалил ли пользователь аккаунт
        public bool isSaveData;  // флаг, изменил ли пользователь свои данные

        public UserAccountWindow(Data.Entity.User user)
        {
            InitializeComponent();
            User = user;
            isDelAccount = false;
            isSaveData = false;
            this.DataContext = this;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Data.Entity.User copyUser = CheckUser.GetClone(User);
            var dialog = new UserSettingsWindow(copyUser);  // в конструктор передаём копию User
            bool? dialogRes = dialog.ShowDialog();  // отображаем окно настройки пользователя и передаём копию объекта User
            if (dialogRes ?? false)  // если вернулось true, то выход из аккаунта
            {
                if (dialog.isDelAccount)  // если аккаунт удалён
                {
                    isDelAccount = true;  // указываем что пользователь удалил аккаунт
                }
                DialogResult = true;  // выход из аккаунта
            }
            else  // сохранение свойств user-а
            {
                if (dialog.isSaveData)  // если изменения сохранены
                {
                    User = copyUser;  // присваиваем user-у изменённую копию
                    isSaveData = true;  // указываем что пользователь сохранил изменения
                    DialogResult = false;  // сохранение свойств user-а
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;  // выход из аккаунта
        }
    }
}

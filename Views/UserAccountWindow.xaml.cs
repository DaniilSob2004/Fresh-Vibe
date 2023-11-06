using System.Windows;
using StoreExam.CheckData;
using StoreExam.Enums;

namespace StoreExam.Views
{
    public partial class UserAccountWindow : Window
    {
        public Data.Entity.User User { get; set; }  // ссылка на объект User
        public StateData stateUserData;  // состояние данных пользователя

        public UserAccountWindow(Data.Entity.User user)
        {
            InitializeComponent();
            DataContext = this;
            User = user;
            stateUserData = StateData.Cancel;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Data.Entity.User copyUser = CheckUser.GetClone(User);
            var dialog = new UserSettingsWindow(copyUser);  // в конструктор передаём копию User
            dialog.ShowDialog();  // отображаем окно настройки пользователя

            stateUserData = dialog.stateUserData;  // сохраняем состояние работы окна
            if (stateUserData != StateData.Cancel)
            {
                if (stateUserData == StateData.Save || stateUserData == StateData.ChangeEmail)
                {
                    User = copyUser;  // сохраняем измененного user-а
                }
                Close();  // закрываем окно
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            stateUserData = StateData.Exit;  // сохраняем состояние работы окна
            Close();  // закрываем окно
        }
    }
}

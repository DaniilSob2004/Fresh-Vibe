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

namespace StoreExam.Views
{
    public partial class MainLoginWindow : Window
    {
        public MainLoginWindow()
        {
            InitializeComponent();
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            Hide();  // скрываем окно
            new SignIn().ShowDialog();  // запускаем окно авторизации
            Show();  // показываем окно
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Hide();  // скрываем окно
            new SignUp().ShowDialog();  // запускаем окно регистрации
            Show();  // показываем окно
        }
    }
}

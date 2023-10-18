using System.Windows;

namespace StoreExam.Views
{
    public partial class MainLoginWindow : Window
    {
        public MainLoginWindow()
        {
            InitializeComponent();

            // инициализируем статические поля, для доступа к главному окну входа
            Tag = true;  // "флаг" для пометки состояния окна (если true - показываем окно)
            SignIn.mainLoginWindow = this;
            SignUp.mainLoginWindow = this;
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            Hide();  // скрываем окно
            new SignIn().ShowDialog();  // запускаем окно авторизации
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Hide();  // скрываем окно
            new SignUp().ShowDialog();  // запускаем окно регистрации
            Show();  // показываем окно
        }
    }
}

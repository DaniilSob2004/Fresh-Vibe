using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreExam.CheckData;
using StoreExam.UI_Settings;

namespace StoreExam.Views
{
    public partial class SignIn : Window
    {
        // статические поля, для хранения значений по умолчанию, которые хранятся в ресурсах UserDefault.xaml
        public static string DefaultEmail = Application.Current.TryFindResource("DefEmail").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public static string SignInText = Application.Current.TryFindResource("SignInText").ToString()!;
        public static string SignUpText = Application.Current.TryFindResource("SignUpText").ToString()!;

        public static Window? mainLoginWindow;  // ссылка на окно родителя (MainLoginWindow)
        public Data.Entity.User User { get; set; }
        private CancellationTokenSource cts = null!;  // источник токенов

        public SignIn()
        {
            InitializeComponent();
            DataContext = this;
            User = new() { Email = DefaultEmail, Password = DefaultPassword };
        }

        private void Window_Clossing(object sender, EventArgs e)
        {
            if ((bool)mainLoginWindow!.Tag)  // если true, значит показываем окно
            {
                mainLoginWindow?.Show();  // показываем главное окно входа
            }
        }


        private void OpenMainWindow(Data.Entity.User user)
        {
            CancelLoadingSignInBtn();  // возвращаем состояние кнопки в исходное
            MessageBox.Show($"Добро пожаловать {user.Name}", "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
            GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
            new MainWindow(user, mainLoginWindow!).ShowDialog();  // запускаем основное окно и передаём объект user
        }

        private void CancelLoadingSignInBtn()
        {
            cts?.Cancel();  // для отмены работы ассинхроного метода
            GuiBaseManipulation.CancelLoadingButton(btnSignIn, SignInText);  // возвращаем исходное состояние
        }

        private void ShowErrorMessage(string message)
        {
            CancelLoadingSignInBtn();  // возвращаем состояние кнопки в исходное состояние
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // переключение на окно регистрации
            if (sender is TextBlock textBlock)
            {
                if (textBlock.Text == SignUpText)
                {
                    GuiBaseManipulation.CloseWindow(this, mainLoginWindow!);  // закрываем окно без показа главного окна входа
                    new SignUp().ShowDialog();  // запускаем окно регистрации
                }
            }
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.SetPasswordBox(textBoxShowPassword, password);  // при изменение TextBox, присваиваем значение в PasswordBox
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.SetTextBox(textBoxShowPassword, password);  // при изменение PasswordBox, присваиваем значение в TextBox
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSignIn, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                Data.Entity.User? user = await Data.DAL.UserDal.Get(User.Email);  // находим user по email
                if (user is null) { ShowErrorMessage("Неверный email!"); return; }

                if (!CheckUser.PasswordEntryVerification(user, User.Password)) { ShowErrorMessage("Неверный пароль!"); return; }

                if (!CheckUser.CheckIsDeletedUser(user))  // если аккаунт пользователя не удалён
                {
                    // запускаем выполнение загрузки категорий и продуктов
                    await Data.DAL.CategoriesDal.LoadData().ContinueWith(task => Data.DAL.ProductsDal.LoadData());
                    OpenMainWindow(user);  // запускаем главное окно
                }
                else
                {
                    ShowErrorMessage("Неверный email!");
                }
                CancelLoadingSignInBtn();  // возвращаем состояние кнопки в исходное состояние
            }
            catch (Exception) { ShowErrorMessage("Что-то пошло нет так...\nПопробуйте позже!"); }
        }
    }
}

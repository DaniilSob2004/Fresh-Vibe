using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using StoreExam.CheckData;
using StoreExam.Enums;
using StoreExam.Views;

namespace StoreExam.UI_Settings
{
    // общие операции над интерфейсом, которые дублировались в окнах
    public static class GuiBaseManipulation
    {
        public static string DefaultName = Application.Current.TryFindResource("DefName").ToString()!;
        public static string DefaultSurname = Application.Current.TryFindResource("DefSurname").ToString()!;
        public static string DefaultNumTel = Application.Current.TryFindResource("DefNumTel").ToString()!;
        public static string DefaultEmail = Application.Current.TryFindResource("DefEmail").ToString()!;
        public static string DefaultPassword = Application.Current.TryFindResource("DefPassword").ToString()!;
        public static string LoadingText = Application.Current.TryFindResource("LoadingText").ToString()!;

        // Работа с метками сообщений (предупреждений)
        public static void ShowInfoMessage(Border borderMessage)
        {
            if (borderMessage.Opacity != 0) { return; }

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5f));  // появление
            fadeInAnimation.Completed += async (sender, e) =>
            {
                await Task.Delay(1000);
                var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5f));  // исчезновение
                borderMessage.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            };
            borderMessage.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }


        // Работа с Button для отображения загрузки и для возвращения в исходное состояние
        public static async void ShowLoadingButton(Button button, CancellationToken token)
        {
            // отображаем кнопку закрузки
            button.IsEnabled = false;
            button.Content = LoadingText;

            int countDots = 4, i = 0;
            while (!token.IsCancellationRequested)
            {
                if (i == countDots)
                {
                    button.Content = LoadingText;
                    i = 0;
                }
                else
                {
                    button.Content += ".";
                }
                i++;
                await Task.Delay(200);
            }
        }

        public static void CancelLoadingButton(Button button, string btnContent)
        {
            button.IsEnabled = true;
            button.Content = btnContent;
        }


        // Работа с Button (-/+) который отвечает за кол-во товара
        public static ViewModels.ProductViewModel? GetProductFromButton(object sender)
        {
            if (sender is Button btn)  // если это кнопка (-/+)
            {
                if (btn.DataContext is ViewModels.ProductViewModel productVM)  // получаем объект ProductViewModel
                {
                    return productVM;
                }
            }
            return null;
        }


        // Работа с TextBox и PasswordBox
        public static void TextBoxCheckCorrectUserData(object sender, Data.Entity.User user)
        {
            if (sender is TextBox textBox && textBox.Tag is not null)
            {
                string? tag = textBox.Tag.ToString();  // узнаём с помощью тега какое это поле
                bool isErrorInput = false;  // ошибочный ли ввод

                if (tag == DefaultName)
                {
                    // если в поле не строка по умолчанию и данные неверно введены, то красим border красным цветом
                    if (user.Name != DefaultName && !CheckUser.CheckName(user)) isErrorInput = true;
                }
                else if (tag == DefaultSurname)
                {
                    if (user.Surname != DefaultSurname && !CheckUser.CheckSurname(user)) isErrorInput = true;
                }
                else if (tag == DefaultNumTel)
                {
                    if (user.NumTel != DefaultNumTel && !CheckUser.CheckNumTel(user)) isErrorInput = true;
                }
                else if (tag == DefaultEmail)
                {
                    if (user.Email != DefaultEmail && !CheckUser.CheckEmail(user)) isErrorInput = true;
                }
                else if (tag == DefaultPassword)
                {
                    if (textBox.Text != DefaultPassword && !CheckUser.CheckPassword(textBox.Text)) isErrorInput = true;
                }

                if (isErrorInput) textBox.BorderBrush = Brushes.Red;
                else textBox.BorderBrush = Brushes.Gray;
            }
        }

        public static void TextBoxGotFocus(object sender)  // при получении фокуса для TextBox и PasswordBox
        {
            if (sender is Control control)
            {
                string? tag = control.Tag?.ToString();
                if (tag is not null)
                {
                    // если тэг и текст совпадают
                    if (control is TextBox tb && tb.Text == tag) { tb.Clear(); }
                    else if (control is PasswordBox pb && pb.Password == tag) { pb.Clear(); }
                    control.Foreground = Brushes.Black;
                }
            }
        }

        public static void TextBoxLostFocus(object sender)  // при потери фокуса для TextBox и PasswordBox
        {
            if (sender is Control control)
            {
                bool isUpdate = true;

                // если поле пустое, то устанавливаем значение тэга
                if (sender is TextBox tb && String.IsNullOrEmpty(tb.Text)) { tb.Text = tb.Tag.ToString(); }
                else if (sender is PasswordBox pb && String.IsNullOrEmpty(pb.Password)) { pb.Password = pb.Tag.ToString(); }
                else { isUpdate = false; }

                if (isUpdate)
                {
                    control.Foreground = Brushes.Gray;
                    control.BorderBrush = Brushes.Gray;
                }
            }
        }

        public static void SetPasswordBox(TextBox textBox, PasswordBox passwordBox)
        {
            if (passwordBox.Password != textBox.Text)
            {
                passwordBox.Password = textBox.Text;
            }
        }

        public static void SetTextBox(TextBox textBox, PasswordBox passwordBox)
        {
            if (textBox.Text != passwordBox.Password)
            {
                textBox.Text = passwordBox.Password;
            }
        }


        // Работа с окнами
        public static StateWindow ShowQuestionWindow(string message)
        {
            // запускается окно с вопросом
            var mesWindow = new MessageWindow(message, TypeMessWin.Question);
            mesWindow.ShowDialog();
            return mesWindow.StateWindow;
        }

        public static bool OpenConfirmEmailWindow(Window window, Data.Entity.User user, string? text = null)
        {
            // запускается окно подтверждения почты
            bool isShowWindow = true;

            if (text is not null)
            {
                if (ShowQuestionWindow(text) != StateWindow.Yes)
                {
                    isShowWindow = false;
                }
            }
            if (isShowWindow)
            {
                window.Hide();
                var dialog = new ConfirmEmailWindow(user);  // запускаем окно подтверждения почты
                if (text is null)
                {
                    _ = dialog.SendCodeToEmail();  // отправка кода на email пользователя
                }
                dialog.ShowDialog();
                return true;
            }
            return false;
        }

        public static void CloseWindow(Window closeWindow, Window settingWindow)
        {
            // закрытие окна без показа главного окна входа
            settingWindow!.Tag = false;  // не нужно показывать главное окно входа (MainLoginWindow)
            closeWindow.Close();  // закрываем окно (SignIn / SignUp)
            settingWindow!.Tag = true;  // выставляем по умолчанию (MainLoginWindow)
        }
    }
}

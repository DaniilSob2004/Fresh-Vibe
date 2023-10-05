using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StoreExam.CheckData;

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


        // Работа с Button для отображения загрузки и для возвращения в исходное состояние
        public static async void ShowLoadingButton(Button button, CancellationToken token)
        {
            // отображаем кнопку закрузки
            button.IsEnabled = false;
            button.Content = "Loading";

            int countDots = 4, i = 0;
            while (!token.IsCancellationRequested)
            {
                if (i == countDots)
                {
                    button.Content = "Loading";
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


        // Работа с TextBlock который отвечает за кол-во товара
        public static TextBlock? FindTextBlockAmountsProductBtnPlMi(object sender)
        {
            // находим TextBlock который отвечает за кол-во выбранного товара (для кнопок +/-)
            DependencyObject parent = VisualTreeHelper.GetParent((UIElement)sender);  // находим родителя
            if (parent is StackPanel stack)  // если это StackPanel (наш TextBlock находится в StackPanel)
            {
                if (stack.FindName("textBlockAmountProducts") is TextBlock textBlockAmount)  // если нашли TextBlock
                {
                    return textBlockAmount;
                }
            }
            return null;
        }

        public static TextBlock? FindTextBlockAmountsProductBtnBasket(object sender)
        {
            // находим TextBlock который отвечает за кол-во выбранного товара (для кнопок В КОРЗИНУ)
            DependencyObject parent = VisualTreeHelper.GetParent((UIElement)sender);  // находим родителя
            if (parent is Grid grid)  // если это Grid (наш TextBlock находится в Grid)
            {
                if (grid.FindName("textBlockAmountProducts") is TextBlock textBlockAmount)  // если нашли TextBlock
                {
                    return textBlockAmount;
                }
            }
            return null;
        }

        public static void TextBlockAmountProductChangeValue(object sender, Data.Entity.Product product, bool shouldIncrease)
        {
            TextBlock? textBlockAmount = FindTextBlockAmountsProductBtnPlMi(sender);  // находим TextBlock
            if (textBlockAmount is not null)
            {
                int amountProduct;
                if (int.TryParse(textBlockAmount.Text, out amountProduct))  // преобразовываем в int
                {
                    if (shouldIncrease)  // если true, значит увеличиваем
                    {
                        if (CheckProduct.CheckMaxValue(product, amountProduct))  // проверка перед тем как увеличить
                        {
                            textBlockAmount.Text = (++amountProduct).ToString();
                        }
                    }
                    else  // иначе уменьшаем
                    {
                        if (CheckProduct.CheckMinValue(product, amountProduct))  // проверка перед тем как уменьшить
                        {
                            textBlockAmount.Text = (--amountProduct).ToString();
                        }
                    }
                }
            }
        }


        // Работа с TextBox и PasswordBox
        public static void TextBoxCheckCorrectUserData(object sender, Data.Entity.User user)
        {
            if (sender is TextBox textBox && textBox.Tag is not null)
            {
                string? tag = textBox.Tag.ToString();  // узнаём с помощью тега какое это поле
                bool isErrorInput = false;  // флаг, ошибочный ли ввод

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
            string tag;
            if (sender is TextBox textBox)
            {
                tag = textBox.Tag.ToString()!;
                if (textBox.Text == tag)  // если тэг и текст textBox-а совпадают
                {
                    textBox.Clear();
                    textBox.Foreground = Brushes.Black;
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                tag = passwordBox.Tag.ToString()!;
                if (passwordBox.Password == tag)   // если тэг и текст passwordBox-а совпадают
                {
                    passwordBox.Clear();
                    passwordBox.Foreground = Brushes.Black;
                }
            }
        }

        public static void TextBoxLostFocus(object sender)  // при потери фокуса для TextBox и PasswordBox
        {
            if (sender is TextBox textBox)
            {
                if (String.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Foreground = Brushes.Gray;
                    textBox.BorderBrush = Brushes.Gray;
                    textBox.Text = textBox.Tag.ToString();
                }
            }
            else if (sender is PasswordBox passwordBox)
            {
                if (String.IsNullOrEmpty(passwordBox.Password))
                {
                    passwordBox.Foreground = Brushes.Gray;
                    passwordBox.BorderBrush = Brushes.Gray;
                    passwordBox.Password = passwordBox.Tag.ToString();
                }
            }
        }

        public static void SetPasswordBox(TextBox textBox, PasswordBox passwordBox)
        {
            passwordBox.Password = textBox.Text;
        }

        public static void SetTextBox(TextBox textBox, PasswordBox passwordBox)
        {
            textBox.Text = passwordBox.Password;
        }
    }
}

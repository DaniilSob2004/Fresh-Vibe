using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoreExam
{
    // общие операции над интерфейсом, которые дублировались в окнах
    public static class GuiBaseManipulation
    {
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

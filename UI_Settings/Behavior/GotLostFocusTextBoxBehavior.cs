using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace StoreExam.UI_Settings.Behavior
{
    public class GotLostFocusTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
            // добавляем обработчик события GotFocus и LostFocus для элемента управления TextBox
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.GotFocus -= OnLostFocus;
            // удаляем обработчик события GotFocus и LostFocus
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxGotFocus(sender);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxLostFocus(sender);
        }
    }
}

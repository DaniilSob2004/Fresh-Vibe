using System.Globalization;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace StoreExam.UI_Settings.Behavior
{
    public class SelectionChangedComboBoxLang : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += ChangeLanguage;  // добавляем обработчик события SelectionChanged для элемента ComboBox
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= ChangeLanguage;  // удаляем обработчик события SelectionChanged
        }

        private void ChangeLanguage(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem cbItem)  // получаем выбранный элемент у ComboBox
            {
                if (cbItem.Tag is CultureInfo lang)  // получаем объект CultureInfo
                {
                    App.Language = lang;  // меняем культуру
                }
            }
        }
    }
}

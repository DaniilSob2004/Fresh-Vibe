using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreExam.UI_Settings.Behavior
{
    // Behavior<Image> - базовый класс для всех поведений, которые могут быть применены к элементам управления Image
    // Поведения - позволяют инкапсулировать части функциональности пользовательского интерфейса в повторно используемые объекты.

    // Данный класс нужен для обработки события клика по Image, т.к. сам Image определён в шаблоне TextBox, и Image ссылается на данный класс
    // Сам шаблон определён в StartStyle.xamls
    public class MouseLeftButtonDownImageBehavior : Behavior<Image>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            // добавляем обработчик события MouseLeftButtonDown для элемента управления Image
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            // удаляем обработчик события MouseLeftButtonDown
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // обработчик события MouseLeftButtonDown
            if (sender is Image image)
            {
                TextBox? textBox = image.Tag as TextBox;
                if (textBox is not null)
                {
                    if (Panel.GetZIndex(textBox) == 0)  // если ZIndex равен 0, значит TextBox скрыт
                    {
                        Panel.SetZIndex(textBox, 1);  // отображаем (ставим повыше)
                    }
                    else
                    {
                        Panel.SetZIndex(textBox, 0);  // прячем (ставим пониже)
                    }
                }
            }
        }
    }
}

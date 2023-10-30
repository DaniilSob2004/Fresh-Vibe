using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace StoreExam.UI_Settings.Behavior
{
    public class ClickButtonReduceProduct : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += BtnClick;  // добавляем обработчик события BtnClick для элемента Button
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= BtnClick;  // удаляем обработчик события BtnClick
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            ViewModels.ProductViewModel? productVM = GuiBaseManipulation.GetProductFromButton(sender);  // получаем объект ProductViewModel
            if (productVM is not null)
            {
                productVM.ChoiceCount--;  // уменьшаем значение свойства
            }
        }
    }
}

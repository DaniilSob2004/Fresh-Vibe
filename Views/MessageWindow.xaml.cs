using StoreExam.Enums;
using System.Windows;
using System.Windows.Controls;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.Views
{
    public partial class MessageWindow : Window
    {
        public string Message { get; set; }
        public StateWindow StateWindow { get; set; }
        private TypeMessWin typeMessWin;  // тип окна

        public MessageWindow(string message, TypeMessWin typeMessWin = TypeMessWin.Message)
        {
            InitializeComponent();
            Message = message;
            this.typeMessWin = typeMessWin;
            StateWindow = StateWindow.Close;
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckTemplateWindow();
        }


        private void CreateQuestionTemplate()
        {
            Grid grid = new();
            grid.SetValue(Grid.RowProperty, 1);

            ColumnDefinition col1 = new(), col2 = new();

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            // в Tag записываем значение перечисления, которое соответ. этой кнопки
            Button btnYes = new() { Content = Texts.YesText, Tag = StateWindow.Yes, Style = (Style)FindResource("baseButtonMainStyle_") };
            Button btnNo = new() { Content = Texts.NoText, Tag = StateWindow.No, Style = (Style)FindResource("baseButtonMainStyle_") };
            btnYes.Click += Btn_Click;
            btnNo.Click += Btn_Click;

            Grid.SetColumn(btnNo, 1);

            grid.Children.Add(btnYes);
            grid.Children.Add(btnNo);

            mainGrid.Children.Add(grid);
        }

        private void CreateMessageTemplate()
        {
            // в Tag записываем значение перечисления, которое соответ. этой кнопки
            Button btnOk = new() { Content = Texts.OkText, Width = 150, Tag = StateWindow.Ok, Style = (Style)FindResource("baseButtonMainStyle_") };
            btnOk.SetValue(Grid.RowProperty, 1);
            btnOk.Click += Btn_Click;
            mainGrid.Children.Add(btnOk);
        }

        private void CheckTemplateWindow()
        {
            switch (typeMessWin)
            {
                case TypeMessWin.Question:
                    CreateQuestionTemplate();
                    break;

                case TypeMessWin.Message:
                    CreateMessageTemplate();
                    break;
            }
        }


        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                StateWindow = (StateWindow)btn.Tag;  // узнаём и сохраняем состояние окна
                Close();
            }
        }
    }
}

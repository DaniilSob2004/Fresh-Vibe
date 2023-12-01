using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using StoreExam.CheckData;
using StoreExam.Enums;
using StoreExam.Generate;
using StoreExam.UI_Settings;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.Views
{
    public partial class UserSettingsWindow : Window
    {
        public Data.Entity.User User { get; set; }  // копия user-a
        public Data.Entity.User origUser;  // объект user, до модификации
        public StateData stateUserData;  // состояние данных пользователя
        private CancellationTokenSource cts = null!;  // источник токенов

        public UserSettingsWindow(Data.Entity.User user)
        {
            InitializeComponent();
            DataContext = this;
            User = user;
            origUser = CheckUser.GetClone(user);  // сохраняем копию
            stateUserData = StateData.Cancel;
        }


        private bool CheckNewPassword()
        {
            // проверяем, совпадает ли новый пароль с подтверждённым
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) &&
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text) &&
                   textBoxNewPassword.Text == textBoxConfirmNewPassword.Text;
        }

        private bool IsChangePassword()
        {
            // если для какого-то поля для пароля был ввод, то true
            return !String.IsNullOrEmpty(textBoxOldPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxNewPassword.Text) ||
                   !String.IsNullOrEmpty(textBoxConfirmNewPassword.Text);
        }

        private bool IsChangeEmail()
        {
            return origUser.Email != User.Email;
        }

        private void CancelLoadingSaveBtn()
        {
            cts?.Cancel();  // для отмены работы ассинхроного метода
            GuiBaseManipulation.CancelLoadingButton(btnSave, Texts.SaveText);  // возвращаем исходное состояние
        }

        private void ShowMessage(string message)
        {
            CancelLoadingSaveBtn();  // возвращаем состояние кнопки в исходное
            new MessageWindow(message).ShowDialog();
        }

        private void SaveStateUserData(StateData stateData)
        {
            stateUserData = stateData;  // сохраняем состояние работы окна
            Close();
        }


        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            GuiBaseManipulation.TextBoxCheckCorrectUserData(sender, User);  // проверка ввода, если некорректный, то Border TextBox изменяется на красный
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();  // создаём новый источник токенов
                GuiBaseManipulation.ShowLoadingButton(btnSave, cts.Token);  // делаем кнопку загрузочной
                await Task.Delay(1000);  // для проверки

                // данные не корректны
                if (!CheckUser.CheckAllData(User)) { ShowMessage(MessageValues.NotWriteFieldMess); return; }

                // получаем названия полей, которые не уникальны
                string? notUniqueFields = await CheckUser.CheckUniqueUserInDB(User);
                if (notUniqueFields is not null) { ShowMessage(MessageValues.NotUniqueFieldMess.Replace("{NotUniqueFields}", $"\n{notUniqueFields}")); return; }

                if (IsChangePassword())  // если пароль был изменён
                {
                    if (CheckUser.PasswordEntryVerification(origUser, textBoxOldPassword.Text))  // если пароль введён верный
                    {
                        if (CheckNewPassword())  // проверка двух полей для нового пароля
                        {
                            User.Password = PasswordHasher.HashPassword(textBoxNewPassword.Text, origUser.Salt);  // хэшируем новый пароль, на основе соли
                        }
                        else { ShowMessage(MessageValues.ErrorNewTwoPassMess); return; }
                    }
                    else { ShowMessage(MessageValues.ErrorConfrirmPassMess); return; }
                }

                if (IsChangeEmail())  // если email был изменён
                {
                    User.ConfirmCode = CodeGenerator.GetCode();  // генерируем новый код подтверждения
                    SaveStateUserData(StateData.ChangeEmail);
                }
                else
                {
                    SaveStateUserData(StateData.Save);
                }
            }
            catch (Exception) { ShowMessage(MessageValues.BaseErrorMess); }
            finally { CancelLoadingSaveBtn(); }  // возвращаем состояние кнопки в исходное
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            SaveStateUserData(StateData.Exit);
        }

        private void BtnDelAccount_Click(object sender, RoutedEventArgs e)
        {
            if (GuiBaseManipulation.ShowQuestionWindow(MessageValues.DelAccQuestMess) == StateWindow.Yes)
            {
                SaveStateUserData(StateData.Delete);
            }
        }
    }
}

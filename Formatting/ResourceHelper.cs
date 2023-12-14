using System;
using System.Windows;

namespace StoreExam.Formatting
{
    public static class ResourceHelper
    {
        // статические классы и свойства, для хранения значений, которые хранятся в ресурсах DefaultValue.xaml
        private static string GetString(string key)
        {
            // получаем ресурс по ключу, иначе пустая строка
            object resource = Application.Current.TryFindResource(key);
            return resource is not null ? resource.ToString()! : String.Empty;
        }

        public static class DefaultValues
        {
            public static string DefaultName => GetString("DefName");
            public static string DefaultSurname => GetString("DefSurname");
            public static string DefaultNumTel => GetString("DefNumTel");
            public static string DefaultEmail => GetString("DefEmail");
            public static string DefaultPassword => GetString("DefPassword");
            public static string DefaultConfirmCode => GetString("DefConfirmCode");
        }

        public static class CorrectValues
        {
            public static int MinLenNameSurname => int.Parse(GetString("MinLenNameSurname"));
            public static int MinLenPassword => int.Parse(GetString("MinLenPassword"));
        }

        public static class Texts
        {
            public static string StoreName => GetString("StoreName");
            public static string DefSearch => GetString("DefSearch");
            public static string DefConfirmCode => GetString("DefConfirmCode");
            public static string NumTelText => GetString("NumTelText");
            public static string LoadingText => GetString("LoadingText");
            public static string SignInText => GetString("SignInText");
            public static string SignUpText => GetString("SignUpText");
            public static string SaveText => GetString("SaveText");
            public static string ExitText => GetString("ExitText");
            public static string DelAccText => GetString("DelAccText");
            public static string YesText => GetString("YesText");
            public static string NoText => GetString("NoText");
            public static string OkText => GetString("OkText");
            public static string ConfirmEmailText => GetString("ConfirmEmailText");
            public static string CheckPdfFileText => GetString("CheckPdfFileText");
            public static string SumOrderText => GetString("SumOrderText");
            public static string ThanksBuyText => GetString("ThanksBuyText");
            public static string CatalogText => GetString("CatalogText");
        }

        public static class EmailText
        {
            public static string ConfirmEmailTextHtml => GetString("ConfirmEmailTextHtml");
            public static string WarningEmailTextHtml => GetString("WarningEmailTextHtml");
            public static string PdfEmailTextHtml => GetString("PdfEmailTextHtml");
        }

        public static class MessageValues
        {
            public static string BaseErrorMess => GetString("BaseErrorMess");
            public static string ExitAccMess => GetString("ExitAccMess");
            public static string DelAccErrorMess => GetString("DelAccErrorMess");
            public static string DelAccQuestMess => GetString("DelAccQuestMess");
            public static string UpdateAccErrorMess => GetString("UpdateAccErrorMess");
            public static string ChangeSucValueMess => GetString("ChangeSucValueMess");
            public static string DelAllProdMess => GetString("DelAllProdMess");
            public static string DelProdMess => GetString("DelProdMess");
            public static string DelErrorMess => GetString("DelErrorMess");
            public static string NotProdInStockMess => GetString("NotProdInStockMess");
            public static string DelProdQuestMess => GetString("DelProdQuestMess");
            public static string ProcProdQuestMess => GetString("ProcProdQuestMess");
            public static string WelcomeMess => GetString("WelcomeMess");
            public static string WelcomeSignUpMess => GetString("WelcomeSignUpMess");
            public static string NotUniqueFieldMess => GetString("NotUniqueFieldMess");
            public static string SendEmailErrorMess => GetString("SendEmailErrorMess");
            public static string ConfirmEmailSuccMess => GetString("ConfirmEmailSuccMess");
            public static string SendEmailMess => GetString("SendEmailMess");
            public static string ConfirmEmailQuestMess => GetString("ConfirmEmailQuestMess");
            public static string ErrorEmailMess => GetString("ErrorEmailMess");
            public static string SaveCheckErrorMess => GetString("SaveCheckErrorMess");
            public static string SendCheckMess => GetString("SendCheckMess");
            public static string SaveCheckMess => GetString("SaveCheckMess");
            public static string NotWriteFieldMess => GetString("NotWriteFieldMess");
            public static string ErrorPassMess => GetString("ErrorPassMess");
            public static string ErrorTwoPassMess => GetString("ErrorTwoPassMess");
            public static string ErrorNewTwoPassMess => GetString("ErrorNewTwoPassMess");
            public static string ErrorConfrirmPassMess => GetString("ErrorConfrirmPassMess");
        }
    }
}

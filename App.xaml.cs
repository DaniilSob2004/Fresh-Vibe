using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace StoreExam
{
    public partial class App : Application
    {
        public Data.DataContext dataContext { get; private set; } = null!;

        // какие культуры поддерживает приложение
        private static List<CultureInfo> languages = new();
        public static List<CultureInfo> Languages => languages;


        public App()
        {
            App.LanguageChanged += App_LanguageChanged;

            languages.Clear();
            languages.Add(new CultureInfo("en-US"));  // нейтральная культура для этого проекта
            languages.Add(new CultureInfo("ru-RU"));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            dataContext = new();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string defLang = StoreExam.Resources.Settings.Default.DefLang;  // узнаём язык который установлен
            Language = new CultureInfo(defLang);  // создаём объект культуры
        }

        // обработчик события при изменении культуры
        private void App_LanguageChanged(object? sender, EventArgs e)
        {
            StoreExam.Resources.Settings.Default.DefLang = Language.Name;  // записываем в файл настройки (в свойство DefLang) название новой культуры
            StoreExam.Resources.Settings.Default.Save();
        }

        // событие для оповещения всех окон приложения при изменении языка
        public static event EventHandler LanguageChanged = null!;
        public static CultureInfo Language
        {
            get => Thread.CurrentThread.CurrentUICulture;  // культура UI для текущего потока выполнения
            set
            {
                if (value is null) { throw new ArgumentException("value"); }
                if (value == Thread.CurrentThread.CurrentUICulture) { return; }

                // 1. меняем язык приложения
                Thread.CurrentThread.CurrentUICulture = value;

                // 2. создаём ResourceDictionary для новой культуры
                ResourceDictionary resDict = new();
                switch (value.Name)
                {
                    case "ru-RU":
                        resDict.Source = new Uri($"Resources/DefaultValue.{value.Name}.xaml", UriKind.Relative);
                        break;
                    default:
                        resDict.Source = new Uri($"Resources/DefaultValue.xaml", UriKind.Relative);
                        break;
                }

                // 3. находим старую ResourceDictionary и удаляем его и добавляем новую
                ResourceDictionary oldResDict = (
                    from d in Application.Current.Resources.MergedDictionaries
                    where d.Source != null && d.Source.OriginalString.StartsWith("Resources/DefaultValue.")  // находим словарь в ресурсах у которого название начинается с DefaultValue.
                    select d
                ).First();
                if (oldResDict is not null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldResDict);  // находим индекс
                    Application.Current.Resources.MergedDictionaries.Remove(oldResDict);  // удаляем
                    Application.Current.Resources.MergedDictionaries.Insert(ind, resDict);  // вставляем новый
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(resDict);  // добавляем новый
                }

                // 4. вызываем событие для оповещения всех окон
                LanguageChanged?.Invoke(Application.Current, new EventArgs());
            }
        }

        private static string configFilename = "settings.json";
        private static JsonElement? settings = null;
        public static string? GetConfiguration(string name)
        {
            // проверка существует ли файл json
            if (settings is null)
            {
                if (!File.Exists(configFilename)) { return null; }
            }

            // преобразовываем весь текст из json в тип JsonElement
            try { settings ??= JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(configFilename)); }
            catch { return null; }

            JsonElement? jsonElement = settings;
            if (settings is not null)
            {
                try
                {
                    foreach (string key in name.Split(':'))
                    {
                        jsonElement = jsonElement?.GetProperty(key);
                    }
                }
                catch { return null; }
                return jsonElement?.GetString();
            }
            return null;
        }
    }
}

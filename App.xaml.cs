using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;

namespace StoreExam
{
    public partial class App : Application
    {
        public Data.DataContext dataContext { get; private set; } = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            dataContext = new();

            //CultureInfo russianCulture = new CultureInfo("ru-RU");
            //Thread.CurrentThread.CurrentCulture = russianCulture;
            //Thread.CurrentThread.CurrentUICulture = russianCulture;

            //// Применяем изменения культуры к элементу
            //FrameworkElement.LanguageProperty.OverrideMetadata(
            //    typeof(FrameworkElement),
            //    new FrameworkPropertyMetadata(
            //        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
            //    )
            //);
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

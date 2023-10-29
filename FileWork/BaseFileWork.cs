using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreExam.FileWork
{
    public static class BaseFileWork
    {
        public static string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception) { return String.Empty; }
        }

        public static string GetPathTempFilePdf(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), "Receipt.pdf");
        }

        public static bool Delete(string path)
        {
            // удаление файла
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool StartFile(string path)
        {
            // открытие файла приложения
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}

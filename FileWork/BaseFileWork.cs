using System;
using System.Diagnostics;
using System.IO;

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
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public static bool Delete(string path)
        {
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
            // открытие файла приложения (запуск процесса)
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

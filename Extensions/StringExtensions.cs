namespace StoreExam.Extensions
{
    // расширения для строк
    public static class StringExtensions
    {
        public static string Ellipsis(this string str, int maxLength)
        {
            // добавляются '...', если длина строки больше maxLength
            return str.Length > maxLength ? str[..(maxLength - 3)] + "..." : str;
        }
    }
}

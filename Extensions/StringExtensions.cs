namespace StoreExam.Extensions
{
    public static class StringExtensions
    {
        public static string Ellipsis(this string str, int maxLength)
        {
            return str.Length > maxLength ? str[..(maxLength - 3)] + "..." : str;  // добавляются '...', если длина строки больше maxLength
        }
    }
}

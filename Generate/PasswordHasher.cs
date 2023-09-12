using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    // Хэширование пароля и проверка пароля
    public static string HashPassword(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string saltedPassword = password + salt;  // пароль + соль
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));  // получаем хэш
            return BitConverter.ToString(hashedBytes).Replace("-", "");  // удаляем лишние '-' из хэша (для компактности)
        }
    }

    public static bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        return HashPassword(password, salt) == hashedPassword;  // хэшируем пароль с солью и сравниваем с другим хэшем
    }
}
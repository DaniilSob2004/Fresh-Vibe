using System;
using System.Security.Cryptography;

public static class SaltGenerator
{
    // Создание уникальной соли
    public static string GenerateSalt(int size = 16)
    {
        byte[] saltBytes = new byte[size];

        // используем криптографически безопасный генератор случайных чисел
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);  // преобразуем байты в строку
    }
}
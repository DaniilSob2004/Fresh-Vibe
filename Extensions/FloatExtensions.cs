﻿using System;

namespace StoreExam.Extensions
{
    public static class FloatExtensions
    {
        public static string Hrn(this float num)
        {
            return String.Format("{0:F1} ₴", num);  // добавляем знак 'грн' и округляем сумму
        }
    }
}

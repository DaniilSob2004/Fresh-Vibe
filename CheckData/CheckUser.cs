using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using StoreExam.Data.Entity;

namespace StoreExam.CheckData
{
    public static class CheckUser
    {
        public static int MinNameSurname = Convert.ToInt32(Application.Current.FindResource("MinLenNameSurname"));
        public static int MinPassword = Convert.ToInt32(Application.Current.FindResource("MinLenPassword"));
        public static string DefaultEmail = Application.Current.FindResource("DefEmail").ToString()!;
        private static Match result = null!;

        public static User GetClone(User user)
        {
            return new User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                NumTel = user.NumTel,
                Email = user.Email,
                Password = user.Password,
                Salt = user.Salt,
                CreateDt = user.CreateDt,
                DeleteDt = user.DeleteDt
            };
        }


        public static void CheckAndChangeDefaultEmail(User user)
        {
            // если email пользователя совпадает со значением по умолчанию, значит пользователь его не указал
            if (user.Email == DefaultEmail)
            {
                user.Email = null;
            }
        }

        public static bool CheckIsDeletedUser(User user)
        {
            return user.DeleteDt is not null;  // удалён ли аккаунт пользователь
        }

        public static bool PasswordEntryVerification(User user, string password)
        {
            return PasswordHasher.VerifyPassword(password, user.Salt, user.Password);
        }

        public async static Task<string?> CheckUniqueUserInDB(User user)
        {
            string? notUniqueFields = null;
            User? userFromDb = await Data.DAL.UserDal.GetUser(user.Id);

            // проверяем в таблице User поле(которое должно быть уникальным), и если такое значение уже есть, то не уникально
            bool isUnique;  
            if (userFromDb?.NumTel != user.NumTel)  // если номер телефона изменил
            {
                isUnique = !await Data.DAL.UserDal.IsUniqueNumTel(user.NumTel);  // проверка номера тел. на уникальность
                if (!isUnique) notUniqueFields += "номер тел., ";
            }

            if (user.Email != DefaultEmail && userFromDb?.Email != user.Email)  // если значение email не по-умолчанию(значит пользователь его указал) и если email изменил
            {
                isUnique = !await Data.DAL.UserDal.IsUniqueEmail(user.Email);  // проверка email на уникальность
                if (!isUnique) notUniqueFields += "email, ";
            }
            return notUniqueFields;  // возвращаем строку, которая содержит поля, которые не уникальны
        }


        public static bool CheckName(User user)
        {
            return user.Name.Length >= MinNameSurname;
        }

        public static bool CheckSurname(User user)
        {
            return user.Surname.Length >= MinNameSurname;
        }

        public static bool CheckNumTel(User user)
        {
            Regex reg = new Regex(@"^(\+38)?\d{10}$");
            result = reg.Match(user.NumTel);
            return result.Success;
        }

        public static bool CheckEmail(User user)
        {
            if (user.Email is not null)
            {
                if (user.Email == DefaultEmail || String.IsNullOrEmpty(user.Email))  // если это значение по умолчанию, то это не ошибка
                {
                    return true;
                }
                else
                {
                    Regex reg = new Regex(@"^[^\.]([\w\d_]\.?){1,18}[^\.]@[\w\d_]{1,20}\.\w{2,20}$");
                    result = reg.Match(user.Email);
                    return result.Success;
                }
            }
            return true;
        }

        public static bool CheckPassword(string password)
        {
            return String.IsNullOrEmpty(password) || password.Length >= MinPassword;
        }

        public static bool CheckAllData(User user)
        {
            return CheckName(user) && CheckSurname(user) && CheckNumTel(user) && CheckEmail(user) && CheckPassword(user.Password);
        }

        public static bool CheckPasswordByString(User user, string password)
        {
            return user.Password == password;
        }
    }
}

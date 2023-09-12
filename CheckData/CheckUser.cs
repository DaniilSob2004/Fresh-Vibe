using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace StoreExam.CheckData
{
    public static class CheckUser
    {
        public enum RegistrationCheck { MinNameSurname = 3, MinPassword = 8 };
        private static Match result = null!;

        public static Data.Entity.User GetClone(Data.Entity.User user)
        {
            return new Data.Entity.User
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

        public static void CheckAndChangeDefaultEmail(Data.Entity.User user)
        {
            if (user.Email == Application.Current.TryFindResource("DefEmail").ToString()!)
            {
                user.Email = null;
            }
        }

        public static bool CheckIsDeletedUser(Data.Entity.User user)
        {
            return user.DeleteDt is not null;
        }

        public static bool CheckName(Data.Entity.User user)
        {
            return user.Name.Length >= (int)RegistrationCheck.MinNameSurname;
        }

        public static bool CheckSurname(Data.Entity.User user)
        {
            return user.Surname.Length >= (int)RegistrationCheck.MinNameSurname;
        }

        public static bool CheckNumTel(Data.Entity.User user)
        {
            Regex reg = new Regex(@"^(\+38)?\d{10}$");
            result = reg.Match(user.NumTel);
            return result.Success;
        }

        public static bool CheckEmail(Data.Entity.User user)
        {
            if (user.Email is not null)
            {
                if (user.Email == Application.Current.TryFindResource("DefEmail").ToString() || String.IsNullOrEmpty(user.Email))  // если это значение по умолчанию, то это не ошибка
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
            return String.IsNullOrEmpty(password) || password.Length >= (int)RegistrationCheck.MinPassword;
        }

        public static bool CheckAllData(Data.Entity.User user)
        {
            return CheckName(user) && CheckSurname(user) && CheckNumTel(user) && CheckEmail(user) && CheckPassword(user.Password);
        }

        public static bool CheckPasswordByString(Data.Entity.User user, string password)
        {
            return user.Password == password;
        }
    }
}

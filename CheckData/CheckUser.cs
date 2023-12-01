using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using StoreExam.Data.Entity;
using StoreExam.Generate;
using static StoreExam.Formatting.ResourceHelper;

namespace StoreExam.CheckData
{
    public static class CheckUser
    {
        private static Match result = null!;

        public static User GetClone(User user)
        {
            return new()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                NumTel = user.NumTel,
                Email = user.Email,
                ConfirmCode = user.ConfirmCode,
                Password = user.Password,
                Salt = user.Salt,
                CreateDt = user.CreateDt,
                DeleteDt = user.DeleteDt
            };
        }


        public static bool CheckDefaultNumTel(User user)
        {
            // проверка на numTel по-умолчанию (чтобы установить в null если стоит по-умолчанию)
            if (String.IsNullOrEmpty(user.NumTel) || user.NumTel == DefaultValues.DefaultNumTel)
            {
                user.NumTel = null;
                return true;
            }
            return false;
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
            User? userFromDb = await Data.DAL.UserDal.Get(user.Id);

            // проверяем в таблице User поле(которое должно быть уникальным), и если такое значение уже есть, то не уникально
            bool isUnique;  
            if (DefaultValues.DefaultNumTel != user.NumTel && user.NumTel != userFromDb?.NumTel)  // если значение numTel не по-умолчанию(значит пользователь его указал) и если изменил
            {
                isUnique = await Data.DAL.UserDal.IsUniqueNumTel(user.NumTel!);  // проверка numTel на уникальность
                if (!isUnique) notUniqueFields += $" - {Texts.NumTelText}\n";
            }
            if (user.Email != userFromDb?.Email)  // если email изменил
            {
                isUnique = await Data.DAL.UserDal.IsUniqueEmail(user.Email);  // проверка email на уникальность
                if (!isUnique) notUniqueFields += $" - {DefaultValues.DefaultEmail}\n";
            }
            return notUniqueFields;  // возвращаем строку, которая содержит поля, которые не уникальны
        }


        public static bool CheckConfirmCode(User user, string code)
        {
            return user.ConfirmCode == code;
        }

        public static bool CheckIsConfirmedEmail(User user)
        {
            return user.ConfirmCode is null;
        }


        public static bool CheckName(User user)
        {
            return user.Name.Length >= CorrectValues.MinLenNameSurname;
        }

        public static bool CheckSurname(User user)
        {
            return user.Surname.Length >= CorrectValues.MinLenNameSurname;
        }

        public static bool CheckNumTel(User user)
        {
            if (!CheckDefaultNumTel(user))  // если это значение не по умолчанию или это не null
            {
                // проверка ввода
                Regex reg = new Regex(@"^(\+38)?\d{10}$");
                result = reg.Match(user.NumTel!);
                return result.Success;
            }
            return true;
        }

        public static bool CheckEmail(User user)
        {
            Regex reg = new Regex(@"^[^\.]([\w\d_]\.?){1,18}[^\.]@[\w\d_]{1,20}\.\w{2,20}$");
            result = reg.Match(user.Email);
            return result.Success;
        }

        public static bool CheckPassword(string password)
        {
            return !String.IsNullOrEmpty(password) && password != DefaultValues.DefaultPassword && password.Length >= CorrectValues.MinLenPassword;
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

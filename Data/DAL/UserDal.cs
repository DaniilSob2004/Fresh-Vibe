using StoreExam.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreExam.Data.DAL
{
    public static class UserDal
    {
        private static DataContext dataContext = new();

        public static User? GetUser(string numTel)
        {
            return dataContext.Users.FirstOrDefault(u => u.NumTel == numTel);
        }

        public static User? GetUser(Guid id)
        {
            return dataContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public static void Add(User user)
        {
            user.CreateDt = DateTime.Now;  // текущая дата
            user.Salt = SaltGenerator.GenerateSalt();  // генерируем соль
            user.Password = PasswordHasher.HashPassword(user.Password, user.Salt);  // генерация хеша, передаём пароль и соль
            dataContext.Users.Add(user);
            dataContext.SaveChanges();
        }

        public static bool Delete(User delUser)
        {
            User? user = GetUser(delUser.NumTel);  // находим пользователя по email
            if (user is not null)
            {
                user.DeleteDt = DateTime.Now;  // дата удаления пользователя
                dataContext.SaveChanges();
                return true;
            }
            return false;
        }

        public static bool Update(User updateUser)
        {
            User? user = GetUser(updateUser.Id);  // находим пользователя по Id
            if (user is not null)
            {
                // если значения полей различны, то обновляем
                if (user.Name != updateUser.Name) user.Name = updateUser.Name;
                if (user.Surname != updateUser.Surname) user.Surname = updateUser.Surname;
                if (user.NumTel != updateUser.NumTel) user.NumTel = updateUser.NumTel;
                if (user.Email != updateUser.Email) user.Email = updateUser.Email;
                if (user.Password != updateUser.Password) user.Password = updateUser.Password;

                dataContext.SaveChanges();
                return true;
            }
            return false;
        }

        public static bool CheckPassword(User user, string password)
        {
            return PasswordHasher.VerifyPassword(password, user.Salt, user.Password);
        }

        public static bool IsUniqueNumTel(string numTel)
        {
            return dataContext.Users.Any(u => u.NumTel == numTel);
        }

        public static bool IsUniqueEmail(string? email)
        {
            return dataContext.Users.Any(u => u.Email == email);
        }
    }
}

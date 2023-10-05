using Microsoft.EntityFrameworkCore;
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
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task<User?> GetUser(string numTel)
        {
            return await dataContext.Users.FirstOrDefaultAsync(u => u.NumTel == numTel);
        }

        public async static Task<User?> GetUser(Guid id)
        {
            return await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async static Task Add(User user)
        {
            user.CreateDt = DateTime.Now;  // текущая дата
            user.Salt = SaltGenerator.GenerateSalt();  // генерируем соль
            user.Password = PasswordHasher.HashPassword(user.Password, user.Salt);  // генерация хеша, передаём пароль и соль
            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();
        }

        public async static Task<bool> Delete(User delUser)
        {
            User? user = await GetUser(delUser.NumTel);  // находим пользователя по email
            if (user is not null)
            {
                user.DeleteDt = DateTime.Now;  // дата удаления пользователя
                await dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async static Task<bool> Update(User updateUser)
        {
            User? user = await GetUser(updateUser.Id);  // находим пользователя по Id
            if (user is not null)
            {
                // если значения полей различны, то обновляем
                if (user.Name != updateUser.Name) user.Name = updateUser.Name;
                if (user.Surname != updateUser.Surname) user.Surname = updateUser.Surname;
                if (user.NumTel != updateUser.NumTel) user.NumTel = updateUser.NumTel;
                if (user.Email != updateUser.Email) user.Email = updateUser.Email;
                if (user.Password != updateUser.Password) user.Password = updateUser.Password;

                await dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async static Task<bool> IsUniqueNumTel(string numTel)
        {
            return await dataContext.Users.AnyAsync(u => u.NumTel == numTel);
        }

        public async static Task<bool> IsUniqueEmail(string? email)
        {
            return await dataContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}

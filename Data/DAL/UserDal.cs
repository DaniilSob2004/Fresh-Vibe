using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Windows;
using StoreExam.Data.Entity;
using StoreExam.Generate;

namespace StoreExam.Data.DAL
{
    public static class UserDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task<User?> Get(string email)
        {
            return await dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async static Task<User?> Get(Guid id)
        {
            return await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async static Task Add(User user)
        {
            user.CreateDt = DateTime.Now;  // текущая дата
            user.Salt = SaltGenerator.GenerateSalt();  // генерируем соль
            user.Password = PasswordHasher.HashPassword(user.Password, user.Salt);  // генерация хеша, передаём пароль и соль
            user.ConfirmCode = CodeGenerator.GetCode();  // генерация кода подтверждения
            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();
        }

        public async static Task<bool> Delete(User delUser)
        {
            User? user = await Get(delUser.Email);  // находим пользователя по email
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
            User? user = await Get(updateUser.Id);  // находим пользователя по Id
            if (user is not null)
            {
                // если значения полей различны, то обновляем
                if (user.Name != updateUser.Name) user.Name = updateUser.Name;
                if (user.Surname != updateUser.Surname) user.Surname = updateUser.Surname;
                if (user.NumTel != updateUser.NumTel) user.NumTel = updateUser.NumTel;
                if (user.Email != updateUser.Email) user.Email = updateUser.Email;
                if (user.ConfirmCode != updateUser.ConfirmCode) user.ConfirmCode = updateUser.ConfirmCode;
                if (user.Password != updateUser.Password) user.Password = updateUser.Password;

                await dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async static Task<bool> IsUniqueNumTel(string? numTel)
        {
            if (numTel is not null)
            {
                return !await dataContext.Users.AnyAsync(u => u.NumTel == numTel);
            }
            return true;
        }

        public async static Task<bool> IsUniqueEmail(string email)
        {
            return !await dataContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}

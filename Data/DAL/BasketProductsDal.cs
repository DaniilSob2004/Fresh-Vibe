using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StoreExam.Data.Entity;

namespace StoreExam.Data.DAL
{
    public static class BasketProductsDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task<BasketProduct?> GetBasketProduct(Guid userId, Guid productId)
        {
            return await dataContext.BasketProducts.FirstOrDefaultAsync(bp => bp.UserId == userId && bp.ProductId == productId);
        }

        public async static Task<BasketProduct?> GetBasketProduct(Guid id)
        {
            return await dataContext.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id);
        }

        public async static Task<ObservableCollection<BasketProduct>?> GetBasketProductsByUser(User user)
        {
            try
            {
                await dataContext.BasketProducts.LoadAsync();  // загружаем записи из таблицы в память
                return new(user.BasketProducts);  // создаём ObservableCollection и возвращем коллекцию товаров в корзине
            }
            catch (Exception) { return null; }
        }

        public async static Task Add(BasketProduct basketProduct)
        {
            await dataContext.BasketProducts.AddAsync(basketProduct);
            await dataContext.SaveChangesAsync();
        }

        public async static Task<bool> Update(BasketProduct updateBasketProduct)
        {
            BasketProduct? basketProduct = await GetBasketProduct(updateBasketProduct.Id);  // находим объект корзины по Id
            if (basketProduct is not null)
            {
                // если значения полей различны, то обновляем
                if (basketProduct.Amounts != updateBasketProduct.Amounts)
                {
                    basketProduct.Amounts = updateBasketProduct.Amounts;
                }
                await dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

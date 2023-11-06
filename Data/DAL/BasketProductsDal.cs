using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StoreExam.Data.Entity;

namespace StoreExam.Data.DAL
{
    public static class BasketProductsDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task<BasketProduct?> Get(Guid userId, Guid productId)
        {
            return await dataContext.BasketProducts.FirstOrDefaultAsync(bp => bp.UserId == userId && bp.ProductId == productId);
        }

        public async static Task<BasketProduct?> Get(Guid id)
        {
            return await dataContext.BasketProducts.FirstOrDefaultAsync(bp => bp.Id == id);
        }

        public async static Task<List<BasketProduct>?> GetByUser(User user)
        {
            try
            {
                await dataContext.BasketProducts.LoadAsync();  // загружаем записи из таблицы в память
                return user.BasketProducts;  // возвращем коллекцию товаров в корзине
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
            BasketProduct? basketProduct = await Get(updateBasketProduct.Id);  // находим объект корзины по Id
            if (basketProduct is not null)
            {
                if (basketProduct.Amounts != updateBasketProduct.Amounts)
                {
                    basketProduct.Amounts = updateBasketProduct.Amounts;
                }
                await dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public static async Task<bool> Del(Guid id)
        {
            try
            {
                BasketProduct? bp = await Get(id);
                if (bp is not null)
                {
                    dataContext.BasketProducts.Remove(bp);
                    await dataContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }

        public async static Task<bool> DeleteAll()
        {
            try
            {
                var allItems = await dataContext.BasketProducts.ToListAsync();
                return await DeleteRange(allItems);
            }
            catch (Exception) { return false; }
        }

        public async static Task<bool> DeleteRange(List<BasketProduct> basketProducts)
        {
            try
            {
                if (basketProducts.Count > 0)
                {
                    dataContext.BasketProducts.RemoveRange(basketProducts);  // удаляем элементы из списка
                    await dataContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}

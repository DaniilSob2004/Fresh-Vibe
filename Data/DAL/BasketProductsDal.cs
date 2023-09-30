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

        public static BasketProduct? GetBasketProduct(Guid userId, Guid productId)
        {
            return dataContext.BasketProducts.FirstOrDefault(bp => bp.UserId == userId && bp.ProductId == productId);
        }

        public static BasketProduct? GetBasketProduct(Guid id)
        {
            return dataContext.BasketProducts.FirstOrDefault(bp => bp.Id == id);
        }

        public static ObservableCollection<BasketProduct>? GetBasketProductsByUser(User user)
        {
            try
            {
                dataContext.BasketProducts.Load();  // загружаем записи из таблицы в память
                return new(user.BasketProducts);  // создаём ObservableCollection и возвращем коллекцию товаров в корзине
            }
            catch (Exception) { return null; }
        }

        public static void Add(BasketProduct basketProduct)
        {
            dataContext.BasketProducts.Add(basketProduct);
            dataContext.SaveChanges();
        }

        public static bool Update(BasketProduct updateBasketProduct)
        {
            BasketProduct? basketProduct = GetBasketProduct(updateBasketProduct.Id);  // находим объект корзины по Id
            if (basketProduct is not null)
            {
                // если значения полей различны, то обновляем
                if (basketProduct.Amounts != updateBasketProduct.Amounts)
                {
                    basketProduct.Amounts = updateBasketProduct.Amounts;
                }
                dataContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StoreExam.Data.Entity;

namespace StoreExam.Data.DAL
{
    public static class ProductsDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task LoadData()
        {
            await dataContext.Products.LoadAsync();  // загружаем записи из таблицы в память
        }

        public static Task<Product?> Get(Guid id)
        {
            return dataContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async static Task<List<Product>?> GetByCategory(Guid idCat)
        {
            Category? category = await CategoriesDal.Get(idCat);  // получаем категорию по id
            return category?.Products;
        }

        public async static Task<bool> UpdateCount(Product product, int count, bool isIncrease)
        {
            if (count > 0)
            {
                Product? findProduct = await Get(product.Id);  // получаем объект из БД
                if (findProduct is not null)
                {
                    findProduct.Count = isIncrease ? findProduct.Count + count : findProduct.Count - count;  // изменяем кол-во
                    await dataContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

        public async static Task<List<Product>?> FindByName(string name, Guid idCat)
        {
            // находим товары определённой категории которые совпадают по названию
            Category? category = await CategoriesDal.Get(idCat);  // получаем категорию по id
            if (category is not null)
            {
                return await dataContext.Products.Where(p => p.IdCat == idCat && p.Name.Contains(name)).ToListAsync();
            }
            return null;
        }
    }
}

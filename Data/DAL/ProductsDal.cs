using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace StoreExam.Data.DAL
{
    public static class ProductsDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task LoadData()
        {
            await dataContext.Products.LoadAsync();  // загружаем записи из таблицы в память
        }

        public static Entity.Product? Get(Guid id)
        {
            return dataContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public async static Task<List<Entity.Product>?> GetByCategory(Guid idCat)
        {
            Entity.Category? category = await CategoriesDal.Get(idCat);  // получаем категорию по id
            if (category is not null)
            {
                return category.Products;
            }
            return null;
        }

        public async static Task<List<Entity.Product>> FindByName(string name, Guid idCat)
        {
            // находим товары определённой категории которые совпадают по названию
            Entity.Category? category = await CategoriesDal.Get(idCat);  // получаем категорию по id
            return dataContext.Products.Where(p => p.IdCat == idCat && p.Name.Contains(name)).ToList();
        }
    }
}

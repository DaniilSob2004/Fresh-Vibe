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
        private static DataContext dataContext = new();

        public static List<Entity.Product>? GetByCategory(Guid idCat)
        {
            Entity.Category? category = CategoriesDal.Get(idCat);  // получаем категорию по id
            if (category is not null)
            {
                return category.Products;
            }
            return null;
        }

        public static List<Entity.Product> FindByName(string name, Guid idCat)
        {
            // находим товары определённой категории которые совпадают по названию
            Entity.Category? category = CategoriesDal.Get(idCat);  // получаем категорию по id
            return dataContext.Products.Where(p => p.IdCat == idCat && p.Name.Contains(name)).ToList();
        }
    }
}

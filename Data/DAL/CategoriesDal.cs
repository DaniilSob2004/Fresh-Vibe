using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace StoreExam.Data.DAL
{
    public static class CategoriesDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public static ObservableCollection<Entity.Category>? GetCategories()
        {
            try
            {
                dataContext.Categories.Load();  // загружаем записи из таблицы в память
                return dataContext.Categories.Local.ToObservableCollection();  // преобразовываем коллекцию Entity в ObservableCollection
            }
            catch (Exception) { return null; }
        }

        public static Entity.Category? Get(Guid id)
        {
            // получаем категорию по Id
            return dataContext.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
        }
    }
}

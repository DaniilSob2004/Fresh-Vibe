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

        public async static Task LoadData()
        {
            await dataContext.Categories.LoadAsync();  // загружаем записи из таблицы в память
        }

        public async static Task<ObservableCollection<Entity.Category>?> GetCategories()
        {
            try
            {
                // преобразовываем коллекцию Entity в ObservableCollection
                var categories = await dataContext.Categories.ToListAsync();
                return new ObservableCollection<Entity.Category>(categories);
            }
            catch (Exception) { return null; }
        }

        public async static Task<Entity.Category?> Get(Guid id)
        {
            // получаем категорию по Id
            return await dataContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

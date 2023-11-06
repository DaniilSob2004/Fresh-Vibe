using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using StoreExam.Data.Entity;

namespace StoreExam.Data.DAL
{
    public static class CategoriesDal
    {
        private static DataContext dataContext = ((App)Application.Current).dataContext;

        public async static Task LoadData()
        {
            await dataContext.Categories.LoadAsync();  // загружаем записи из таблицы в память
        }

        public async static Task<ObservableCollection<Category>?> GetCategories()
        {
            try
            {
                // преобразовываем коллекцию Entity в ObservableCollection
                var categories = await dataContext.Categories.ToListAsync();
                return new ObservableCollection<Category>(categories);
            }
            catch (Exception) { return null; }
        }

        public async static Task<Category?> Get(Guid id)
        {
            return await dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

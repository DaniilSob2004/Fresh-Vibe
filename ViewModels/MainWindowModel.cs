using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using StoreExam.Data.DAL;

namespace StoreExam.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel(Data.Entity.User user)
        {
            User = user;

            // запускаем задачу для создания и загрузки BasketProductViewModel
            Task.Run(() => BPViewModel = new(user)).Wait();

            LoadCategories();  // загружаем категории
            Products = new();
            productsListView = CollectionViewSource.GetDefaultView(Products);
            BindingOperations.EnableCollectionSynchronization(Products, new object());  // позволяет безопасно обновлять коллекцию Products из других потоков
        }


        private Data.Entity.User user;
        public Data.Entity.User User
        {
            get => user;
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(User)));
                }
            }
        }
        public async Task<bool> UpdateUser(Data.Entity.User user)
        {
            User = user;  // сохраняем объект user
            return await UserDal.Update(User);  // обновляем данные в БД
        }


        public ObservableCollection<Data.Entity.Product> Products { get; set; }
        public ICollectionView productsListView;
        public void UpdateProducts(List<Data.Entity.Product> newListProducts)
        {
            // обновляем коллекцию продуктов
            Products.Clear();
            foreach (var product in newListProducts)
            {
                Products.Add(product);
            }
        }


        public ObservableCollection<Data.Entity.Category>? Categories { get; set; }
        public async void LoadCategories()
        {
            Categories = await CategoriesDal.GetCategories();  // получаем коллекцию категорий
        }


        private Data.Entity.Category? choiceCategory;
        public Data.Entity.Category? ChoiceCategory
        {
            get => choiceCategory;
            set
            {
                if (choiceCategory != value)
                {
                    choiceCategory = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChoiceCategory)));
                }
            }
        }


        public BasketProductsViewModel BPViewModel { get; set; } = null!;
        public async Task<bool> AddProductInBasket(Data.Entity.Product product, int amountAdd)
        {
            try
            {
                Data.Entity.BasketProduct? basketProduct = await BasketProductsDal.GetBasketProduct(User.Id, product.Id);  // находим объект
                if (basketProduct is not null)  // если товар уже в корзине, то просто добавляем кол-во
                {
                    // если при обновлении что-то пошло не так
                    if (!await BPViewModel.UpdateAmountProduct(basketProduct, amountAdd)) return false;
                }
                else
                {
                    BPViewModel.AddProduct(User.Id, product.Id, amountAdd);  // добавляем продукт в корзину
                }
                await BPViewModel.UpdateTotalBasketProductsPrice();  // обновляем сумму товаров в корзине через ViewModel корзины
                return true;
            }
            catch (Exception) { return false; }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

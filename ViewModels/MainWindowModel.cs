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
            ProductsModel = new();
            productsListView = CollectionViewSource.GetDefaultView(ProductsModel);
            BindingOperations.EnableCollectionSynchronization(ProductsModel, new object());  // позволяет безопасно обновлять коллекцию Products из других потоков
        }


        private Data.Entity.User user = null!;
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


        public ObservableCollection<ProductViewModel> ProductsModel { get; set; }
        public ICollectionView productsListView;
        public void UpdateProducts(List<Data.Entity.Product> newListProducts, Data.Entity.Category? category = null)
        {
            // обновляем категорию и коллекцию продуктов
            if (category is not null) { ChoiceCategory = category; }

            ProductsModel.Clear();
            foreach (var product in newListProducts)
            {
                ProductsModel.Add(new(product));
            }
        }
        public void UpdateProductChoiceCount(ProductViewModel productVM)
        {
            productVM.ChoiceCount = 1;
        }
        public void UpdateProductsChoiceCount()
        {
            foreach (var productVM in ProductsModel)
            {
                UpdateProductChoiceCount(productVM);
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
        public async Task<bool> AddProductInBasket(ProductViewModel productVM)
        {
            try
            {
                Data.Entity.BasketProduct? basketProduct = await BasketProductsDal.Get(User.Id, productVM.Product.Id);  // находим объект
                if (basketProduct is not null)  // если товар уже есть в корзине
                {
                    return await BPViewModel.UpdateAmountProduct(basketProduct, true, productVM.ChoiceCount);  // обновляем кол-во
                }
                BPViewModel.AddProduct(User.Id, productVM.Product.Id, productVM.ChoiceCount);  // добавляем продукт в корзину
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

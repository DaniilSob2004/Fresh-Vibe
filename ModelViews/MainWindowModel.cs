using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using StoreExam.Data.DAL;

namespace StoreExam.ModelViews
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel(Data.Entity.User user)
        {
            User = user;
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


        private float totalBasketProductsPrice;
        public float TotalBasketProductsPrice
        {
            get => totalBasketProductsPrice;
            set
            {
                if (totalBasketProductsPrice != value)
                {
                    totalBasketProductsPrice = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(TotalBasketProductsPrice)));
                }
            }
        }
        public async Task UpdateTotalBasketProductsPrice()
        {
            // обновляем сумму товаров в корзине
            await Task.Run(() => TotalBasketProductsPrice = BasketProducts.Sum(bp => bp.Product.Price * bp.Amounts));
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


        public ObservableCollection<Data.Entity.BasketProduct> BasketProducts { get; set; } = null!;
        public async Task LoadBasketProduct()
        {
            var bs = await BasketProductsDal.GetBasketProductsByUser(user);  // получаем коллекцию корзины или создаём новую
            BasketProducts = bs is null ? new() : bs;
        } 
        public void AddAmountBasketProduct(Data.Entity.BasketProduct bp, int amountAdd)
        {
            bp.Amounts += amountAdd;
        }
        public async Task<bool> AddProductInBasket(Data.Entity.Product product, int amount)
        {
            try
            {
                Data.Entity.BasketProduct? basketProduct = await BasketProductsDal.GetBasketProduct(User.Id, product.Id);  // находим объект
                if (basketProduct is not null)  // если товар уже в корзине, то просто добавляем кол-во
                {
                    AddAmountBasketProduct(basketProduct, amount);  // прибавляем кол-во
                    await BasketProductsDal.Update(basketProduct);  // обновляем данные в БД
                }
                else
                {
                    basketProduct = new()  // создаём новый объект для добавления продукта в корзину
                    {
                        Id = Guid.NewGuid(),
                        UserId = User.Id,
                        ProductId = product.Id,
                        Amounts = amount
                    };
                    BasketProducts.Add(basketProduct);  // добавляем в коллекцию
                    await BasketProductsDal.Add(basketProduct);  // добавляем в БД
                }
                await UpdateTotalBasketProductsPrice();  // запускаем обновление суммы в корзине
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using StoreExam.Data.DAL;

namespace StoreExam.ModelViews
{
    public class BasketProductsViewModel : INotifyPropertyChanged
    {
        public BasketProductsViewModel()
        {
            BasketProductsModel = new();
        }


        public ObservableCollection<BasketProductModel> BasketProductsModel { get; set; }
        public async Task LoadBasketProduct(Data.Entity.User user)
        {
            var listBp = await BasketProductsDal.GetBasketProductsByUser(user);  // получаем коллекцию корзины
            if (listBp is not null)
            {
                foreach (var bp in listBp)
                {
                    BasketProductsModel.Add(new(bp));
                }
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
            await Task.Run(() =>
            {
                var listBPModel = BasketProductsModel.Where(bp => bp.IsSelected == true);  // получаем список товаров которые выбранные

                // обновляем сумму корзины
                TotalBasketProductsPrice = listBPModel.Sum(bp => bp.BasketProduct.Product.Price * bp.BasketProduct.Amounts);

                // возвращаем кол-во
                Count = listBPModel.Count();
            });
        }


        private int count;
        public int Count
        {
            get => count;
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }


        public async Task<bool> UpdateAmountProduct(Data.Entity.BasketProduct basketProduct, int amountAdd, bool isIncrease = true)
        {
            Data.Entity.BasketProduct? bp = await BasketProductsDal.GetBasketProduct(basketProduct.Id);  // находим BasketProduct
            if (bp is not null)
            {
                bp.Amounts = isIncrease ? bp.Amounts + amountAdd : bp.Amounts - amountAdd;  // меняем значение
                if (await BasketProductsDal.Update(bp))  // обновляем данные в БД
                {
                    await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                    return true;
                }
            }
            return false;
        }


        public async void AddProduct(Guid userId, Guid productId, int amountAdd)
        {
            Data.Entity.BasketProduct bp = new()  // создаём новый объект для добавления продукта в корзину
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                Amounts = amountAdd
            };
            BasketProductsModel.Add(new(bp));  // добавляем в коллекцию
            await BasketProductsDal.Add(bp);  // добавляем в БД
        }


        public async void SetCheckBoxAllProduct(bool? isChecked)
        {
            foreach (var bpModel in BasketProductsModel)  // перебираем каждый элемент
            {
                bpModel.IsSelected = isChecked;  // устанавливаем новое значение чекбоксу
            }
            await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
        }
        public async void SetCheckBoxProduct(BasketProductModel bpModel)
        {
            bpModel.IsSelected = !bpModel.IsSelected;  // меняем состояние чекбокса
            await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
        }


        public void DeleteBasketProduct(Data.Entity.BasketProduct bp)
        {
            BasketProductModel? findBp = BasketProductsModel.FirstOrDefault(bpModel => bpModel.BasketProduct == bp);  // находим BasketProductModel
            if (findBp is not null)
            {
                BasketProductsModel.Remove(findBp);  // удаляем из коллекции
            }
        }
        public async Task<bool> DeleteBasketProduct(BasketProductModel bpModel)
        {
            if (await BasketProductsDal.Del(bpModel.BasketProduct.Id))  // удаление из БД
            {
                BasketProductsModel.Remove(bpModel);  // удаляем из коллекции
                await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteChoiseProduct()
        {
            List<Data.Entity.BasketProduct> delBasketProducts = new();  // буфферная коллекция (для дальнейшего удаления из БД)
            foreach (var bpModel in BasketProductsModel)
            {
                if (bpModel.IsSelected == true)  // если выбран
                {
                    delBasketProducts.Add(bpModel.BasketProduct);  // добавляем в буфферную коллекцию
                }
            }
            if (await BasketProductsDal.DeleteRange(delBasketProducts))  // удаление из БД
            {
                foreach (var delBp in delBasketProducts)
                {
                    DeleteBasketProduct(delBp);  // удаляем из коллекции BasketProductsView
                }
                await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteAllProduct()
        {
            if (await BasketProductsDal.DeleteAll())  // удаление из БД
            {
                BasketProductsModel.Clear();  // обновляем коллекцию BasketProductsView
                await UpdateTotalBasketProductsPrice();  // обновляем цену товаров
                return true;
            }
            return false;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

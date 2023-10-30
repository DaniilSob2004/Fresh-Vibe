using System.ComponentModel;
using StoreExam.CheckData;

namespace StoreExam.ViewModels
{
    public class BasketProductModel : INotifyPropertyChanged
    {
        public BasketProductModel(Data.Entity.BasketProduct basketProduct)
        {
            BasketProduct = basketProduct;
            IsSelected = true;
            ChoiceCount = basketProduct.Amounts;
        }


        public Data.Entity.BasketProduct BasketProduct { get; set; } = null!;


        private bool? isSelected;
        public bool? IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }


        private int choiceCount;
        public int ChoiceCount
        {
            get => choiceCount;
            set
            {
                if (choiceCount != value)
                {
                    choiceCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChoiceCount)));
                }
                IsNotStock = !CheckProduct.CheckInStock(BasketProduct.Product, ChoiceCount);  // проверка наличия
            }
        }


        private bool isNotStock;
        public bool IsNotStock
        {
            get => isNotStock;
            set
            {
                if (isNotStock != value)
                {
                    isNotStock = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsNotStock)));
                    IsSelected = false;
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

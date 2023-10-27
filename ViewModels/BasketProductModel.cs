using System.ComponentModel;

namespace StoreExam.ViewModels
{
    public class BasketProductModel : INotifyPropertyChanged
    {
        public BasketProductModel(Data.Entity.BasketProduct basketProduct)
        {
            BasketProduct = basketProduct;
            IsSelected = true;
        }


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
                }
            }
        }

        public bool isNotStock2 => BasketProduct.Amounts > BasketProduct.Product.Count;


        public Data.Entity.BasketProduct BasketProduct { get; set; } = null!;


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

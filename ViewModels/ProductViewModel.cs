using System.ComponentModel;
using StoreExam.CheckData;

namespace StoreExam.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        public ProductViewModel(Data.Entity.Product product)
        {
            Product = product;
            ChoiceCount = 1;
        }


        public Data.Entity.Product Product { get; set; } = null!;


        private int choiceCount;
        public int ChoiceCount
        {
            get => choiceCount;
            set
            {
                if (choiceCount != value && CheckProduct.CheckCount(Product, value))  // проверка перед изменением
                {
                    choiceCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChoiceCount)));
                }
                IsNotStock = !CheckProduct.CheckInStock(Product, ChoiceCount);  // проверка наличия
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


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

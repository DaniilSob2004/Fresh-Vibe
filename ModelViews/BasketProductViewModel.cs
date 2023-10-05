using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace StoreExam.ModelViews
{
    public class BasketProductViewModel : INotifyPropertyChanged
    {
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

        public Data.Entity.BasketProduct BasketProduct { get; set; } = null!;

        public BasketProductViewModel(Data.Entity.BasketProduct basketProduct)
        {
            BasketProduct = basketProduct;
            IsSelected = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

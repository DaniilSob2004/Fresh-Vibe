using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreExam.ModelViews
{
    public class MainWindowModel : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, e);
        }
    }
}

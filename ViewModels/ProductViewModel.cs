using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreExam.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        public ProductViewModel(Data.Entity.Product product)
        {
            Product = product;
        }

        public Data.Entity.Product Product { get; set; } = null!;


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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreExam.Views
{
    public partial class ProductInfoWindow : Window
    {
        public Data.Entity.Product Product { get; set; }

        public ProductInfoWindow(Data.Entity.Product product)
        {
            InitializeComponent();
            Product = product;
            this.DataContext = this;
        }
    }
}

using System.Windows;
using StoreExam.UI_Settings;
using StoreExam.Data.DAL;
using System.Threading.Tasks;

namespace StoreExam.CheckData
{
    public static class CheckProduct
    {
        public static bool CheckMaxValue(Data.Entity.Product product, int value)
        {
            return value < product.Count;
        }

        public static bool CheckMinValue(Data.Entity.Product product, int value)
        {
            return value > 1 && product.Count > 1;
        }
    }
}

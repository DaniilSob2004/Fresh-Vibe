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
            int amounts = ProductsDal.GetCount(product.Id);  // получаем кол-во продукта
            if (amounts != -1)
            {
                return value < amounts;
            }
            return false;
        }

        public static bool CheckMinValue(Data.Entity.Product product, int value)
        {
            int amounts = ProductsDal.GetCount(product.Id);  // получаем кол-во продукта
            if (amounts != -1)
            {
                return value > 1 && amounts > 1;
            }
            return false;
        }
    }
}

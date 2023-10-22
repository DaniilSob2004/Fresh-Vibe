using System.Collections.Generic;
using System.Text;
using StoreExam.Extensions;

namespace StoreExam.Formatting
{
    public static class ProductFormat
    {
        // форматирование коллекции продуктов в строку для чека
        public static string GetStringProductsForReceipt(List<ModelViews.BasketProductModel> listBPModels)
        {
            StringBuilder sbProducts = new();
            string num, product, amount, price;
            for (int i = 0; i < listBPModels.Count; i++)
            {
                num = $"{i + 1}.";
                product = listBPModels[i].BasketProduct.Product.Name.Ellipsis(30);
                amount = $"x{listBPModels[i].BasketProduct.Amounts}";
                price = $"{(listBPModels[i].BasketProduct.Amounts * listBPModels[i].BasketProduct.Product.Price).Hrn()}";
                sbProducts.AppendFormat("{0,-4} {1,-40} {2,-10} {3:F1}\n", num, product, amount, price);
            }
            return sbProducts.ToString();
        }
    }
}

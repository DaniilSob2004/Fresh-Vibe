namespace StoreExam.CheckData
{
    public static class CheckProduct
    {
        public static bool CheckMaxValue(Data.Entity.Product product, int value)
        {
            return value <= product.Count;
        }

        public static bool CheckMinValue(Data.Entity.Product product, int value)
        {
            return value >= 1 && product.Count >= 0;
        }

        public static bool CheckCount(Data.Entity.Product product, int value)
        {
            return product.Count > 0 && value > 0 && value <= product.Count;
        }

        public static bool CheckInStock(Data.Entity.Product? product, int amountBuy)
        {
            if (product is null) return true;
            return amountBuy > 0 && product.Count >= amountBuy;
        }
    }
}

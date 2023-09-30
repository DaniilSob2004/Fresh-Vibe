using System;

namespace StoreExam.Data.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid IdCat { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public string? Image { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }

        // -------------- Навигационные свойства --------------
        public Category Category { get; set; } = null!;
    }
}

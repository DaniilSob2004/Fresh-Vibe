using System;
using System.Collections.Generic;

namespace StoreExam.Data.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }

        // -------------- Навигационные свойства --------------
        public List<Product> Products { get; set; } = null!;
    }
}

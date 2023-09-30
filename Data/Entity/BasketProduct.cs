using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreExam.Data.Entity
{
    public class BasketProduct
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Amounts { get; set; }

        // -------------- Навигационные свойства --------------
        public Product Product { get; set; } = null!;
    }
}

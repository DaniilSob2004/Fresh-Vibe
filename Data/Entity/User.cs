using System;
using System.Collections.Generic;

namespace StoreExam.Data.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? NumTel { get; set; }
        public string Email { get; set; } = null!;
        public string? ConfirmCode { get; set; }
        public string Salt { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }

        // -------------- Навигационные свойства --------------
        public List<BasketProduct> BasketProducts { get; set; } = null!;
    }
}

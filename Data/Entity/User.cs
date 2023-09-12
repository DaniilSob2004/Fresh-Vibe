using System;

namespace StoreExam.Data.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string NumTel { get; set; } = null!;
        public string? Email { get; set; }
        public string Salt { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }
    }
}

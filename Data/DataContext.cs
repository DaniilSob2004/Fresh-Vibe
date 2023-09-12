using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoreExam.Data
{
    public class DataContext : DbContext
    {
        private const string DbName = "Store";
        public DbSet<Entity.User> Users { get; set; } = null!;
        public DbSet<Entity.Product> Products { get; set; } = null!;
        public DbSet<Entity.Category> Categories { get; set; } = null!;

        public DataContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                    $@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog={DbName};Integrated Security=True"
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Entity.User>()
                .HasIndex(u => u.NumTel)
                .IsUnique();

            modelBuilder  // настраиваем навигационные свойства Product.Category и Category.Products
                .Entity<Entity.Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.IdCat)
                .HasPrincipalKey(c => c.Id);
        }
    }
}

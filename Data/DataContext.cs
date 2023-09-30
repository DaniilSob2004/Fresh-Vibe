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
        public DbSet<Entity.BasketProduct> BasketProducts { get; set; } = null!;

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
                .IsUnique();  // делаем поле User.NumTel уникальным и устанавливаем индекс

            modelBuilder  // настраиваем навигационные свойства Product.Category и Category.Products
                .Entity<Entity.Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.IdCat)
                .HasPrincipalKey(c => c.Id);

            modelBuilder  // настраиваем навигационное свойство BasketProduct.Product
                .Entity<Entity.BasketProduct>()
                .HasOne(bp => bp.Product)
                .WithMany()
                .HasForeignKey(bp => bp.ProductId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder  // настраиваем навигационное свойство User.BasketProducts
                .Entity<Entity.User>()
                .HasMany(u => u.BasketProducts)
                .WithOne()
                .HasForeignKey(bp => bp.UserId)
                .HasPrincipalKey(u => u.Id);
        }
    }
}

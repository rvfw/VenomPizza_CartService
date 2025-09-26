using Microsoft.EntityFrameworkCore;
using System.Data;
using VenomPizzaCartService.src.model;

namespace VenomPizzaCartService.src.context;

public class CartsDbContext:DbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }

    public CartsDbContext(DbContextOptions<CartsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartProduct>()
            .HasKey(cp => new { cp.CartId, cp.ProductId, cp.PriceId });
        modelBuilder.Entity<CartProduct>()
            .HasOne(cp => cp.Cart)
            .WithMany(c => c.Products)
            .HasForeignKey(cp => cp.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cart>()
            .HasKey(c => c.Id);
    }
}

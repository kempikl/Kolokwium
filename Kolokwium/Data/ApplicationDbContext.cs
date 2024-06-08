using Microsoft.EntityFrameworkCore;
using Kolokwium.Models;

namespace Kolokwium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasKey(c => c.IdClient);
            modelBuilder.Entity<Subscription>().HasKey(s => s.IdSubscription);
            modelBuilder.Entity<Payment>().HasKey(p => p.IdPayment);
            modelBuilder.Entity<Discount>().HasKey(d => d.IdDiscount);
            modelBuilder.Entity<Sale>().HasKey(s => s.IdSale);

            modelBuilder.Entity<Client>()
                .HasMany<Sale>(c => c.Sales)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.IdClient);

            modelBuilder.Entity<Subscription>()
                .HasMany<Payment>(s => s.Payments)
                .WithOne(p => p.Subscription)
                .HasForeignKey(p => p.IdSubscription);

            modelBuilder.Entity<Discount>()
                .HasOne<Subscription>(d => d.Subscription)
                .WithMany(s => s.Discounts)
                .HasForeignKey(d => d.IdSubscription);

            modelBuilder.Entity<Sale>()
                .HasOne<Client>(s => s.Client)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.IdClient);

            modelBuilder.Entity<Sale>()
                .HasOne<Subscription>(s => s.Subscription)
                .WithMany(s => s.Sales)
                .HasForeignKey(s => s.IdSubscription);
        }
    }
}

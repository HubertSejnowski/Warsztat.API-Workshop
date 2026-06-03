using Microsoft.EntityFrameworkCore;
using Warsztat.API.Models;


namespace Warsztat.API.Data
{
    public class WorkshopDbContext : DbContext
    {
        public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options) : base(options)
        {
        }
            
          public DbSet<Customer> Customers { get; set; }
          public DbSet<Vehicle> Vehicles { get; set; }
          public DbSet<Workstation> Workstations { get; set; }
          public DbSet<WorkOrder> WorkOrders {  get; set; }
          public DbSet<Part> Parts {  get; set; }
          public DbSet<UsedPart> UsedParts { get; set;  }
          public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Part>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<UsedPart>()
                .HasOne(up => up.WorkOrder)
                .WithMany(wo => wo.UsedParts)
                .HasForeignKey(up => up.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UsedPart>()
                .HasOne(up => up.Part)
                .WithMany()
                .HasForeignKey(up => up.PartId)
                .OnDelete(DeleteBehavior.Restrict);

           modelBuilder.Entity<User>().HasData(
            new User
        {
        Id = 1,
        Username = "admin",
        PasswordHash = "$2a$11$N.ZpP5K8.iK6p35/sQ35qO.p08nB1C./3J9g8M3u5O6v3s8r5E.",
        Role = "Admin"
            } // Hasło to "admin123" (zhaszowane algorytmem BCrypt - dodamy go później, na razie wstawiamy gotowy hash dla testów)
              // Hash dla "admin123" to: $2a$11$N.ZpP5K8.iK6p35/sQ35qO.p08nB1C./3J9g8M3u5O6v3s8r5E.
            );


        }    
    }
}

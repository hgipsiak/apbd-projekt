using Microsoft.EntityFrameworkCore;
using projekt.Models;

namespace projekt.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Software> OperatingSystems { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<DiscountSoftware> DiscountOperatingSystems { get; set; }

    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(c =>
        {
            c.ToTable("Client");

            c.HasKey(e => e.IdClient);
            
            c.Property(e => e.Address).HasMaxLength(50).IsRequired();
            c.Property(e => e.Email).HasMaxLength(50).IsRequired();
            c.Property(e => e.PhoneNumber).HasMaxLength(9).IsRequired();
        });
        
        modelBuilder.Entity<Client>().HasQueryFilter(c => c.DeletionDate == null);

        modelBuilder.Entity<Person>(p =>
        {
            p.ToTable("Person");

            p.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            p.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            p.Property(e => e.Pesel).HasMaxLength(11).IsRequired();
        });

        modelBuilder.Entity<Company>(co =>
        {
            co.ToTable("Company");

            co.Property(e => e.CompanyName).IsRequired().HasMaxLength(50);
            co.Property(e => e.Krs).IsRequired().HasMaxLength(10);
        });

        modelBuilder.Entity<Software>(op =>
        {
            op.ToTable("OperatingSystem");

            op.HasKey(e => e.SoftwareId);
            op.Property(e => e.Name).IsRequired().HasMaxLength(50);
            op.Property(e => e.Version).IsRequired().HasColumnType("decimal(10,2)");
            op.Property(e => e.Description).IsRequired().HasMaxLength(200);
            op.Property(e => e.Category).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Discount>(d =>
        {
            d.ToTable("Discount");

            d.HasKey(e => e.DiscountId);

            d.Property(e => e.Name).IsRequired().HasMaxLength(100);
            d.Property(e => e.Description).IsRequired().HasMaxLength(200);
            d.Property(e => e.Value).IsRequired().HasColumnType("decimal(10,2)");
            d.Property(e => e.FromDate).IsRequired().HasColumnType("datetime");
            d.Property(e => e.ToDate).IsRequired().HasColumnType("datetime");
        });

        modelBuilder.Entity<DiscountSoftware>(dos =>
        {
            dos.ToTable("DiscountOperatingSystem");

            dos.HasKey(e => new { e.DiscountId, OperatingSystemId = e.SoftwareId });
        });
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projekt.Models;

namespace projekt.Data;

public class DatabaseContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<DiscountSoftware> DiscountSoftwares { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Instalment> Instalments { get; set; }

    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
            op.ToTable("Software");

            op.HasKey(e => e.SoftwareId);
            op.Property(e => e.Name).IsRequired().HasMaxLength(50);
            op.Property(e => e.Version).IsRequired().HasColumnType("decimal(10,2)");
            op.Property(e => e.Description).IsRequired().HasMaxLength(200);
            op.Property(e => e.Category).IsRequired().HasMaxLength(50);
            op.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
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
            dos.ToTable("DiscountSoftware");

            dos.HasKey(e => new { e.DiscountId, e.SoftwareId });
        });

        modelBuilder.Entity<Contract>(c =>
        {
            c.ToTable("Contract");

            c.HasKey(e => e.ContractId);
            c.Property(e => e.StartDate).IsRequired().HasColumnType("datetime");
            c.Property(e => e.EndDate).IsRequired().HasColumnType("datetime");
            c.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(10,2)");
            c.Property(e => e.UpdateYears).IsRequired();
            c.Property(e => e.SoftwareVersion).IsRequired().HasColumnType("decimal(10,2)");
            c.Property(e => e.IsInstalment).IsRequired();
            c.Property(e => e.IsFulfilled).IsRequired();
            c.Property(e => e.InstalmentsQuantity);
        });

        modelBuilder.Entity<Payment>(p =>
        {
            p.ToTable("Payment");

            p.HasKey(e => e.PaymentId);
            p.Property(e => e.PaymentDate).IsRequired().HasColumnType("datetime");
            p.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Instalment>(i =>
        {
            i.ToTable("Instalment");

            i.Property(e => e.InstalmentNumber).IsRequired();
        });
    }
}
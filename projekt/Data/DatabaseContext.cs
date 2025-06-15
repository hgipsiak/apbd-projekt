using Microsoft.EntityFrameworkCore;
using projekt.Models;

namespace projekt.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Company> Companies { get; set; }

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
    }
}
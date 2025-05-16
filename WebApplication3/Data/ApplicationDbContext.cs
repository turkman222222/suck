using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication3.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<bron> bron { get; set; }
    public DbSet<Carss> Carss { get; set; }
    public DbSet<compl> compl { get; set; }
    public DbSet<cveta> cveta { get; set; }
    public DbSet<izbr> izbr { get; set; }
    public DbSet<Marks> Marks { get; set; }
    public DbSet<rol> rol { get; set; }
    public DbSet<salonch> salonch { get; set; }
    public DbSet<strana> strana { get; set; }
    public DbSet<user> user { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure identity start value for user table
        modelBuilder.Entity<user>()
            .Property(u => u.id_user);
            

        // Configure relationships
        modelBuilder.Entity<bron>()
            .HasOne(b => b.Carss)
            .WithMany(c => c.bron)
            .HasForeignKey(b => b.id_car);

        modelBuilder.Entity<bron>()
            .HasOne(b => b.user)
            .WithMany(u => u.bron)
            .HasForeignKey(b => b.id_usr);

        modelBuilder.Entity<izbr>()
            .HasOne(i => i.Carss)
            .WithMany(c => c.izbr)
            .HasForeignKey(i => i.car_id);

        modelBuilder.Entity<izbr>()
            .HasOne(i => i.user)
            .WithMany(u => u.izbr)
            .HasForeignKey(i => i.user_id);

        modelBuilder.Entity<Carss>()
            .HasOne(c => c.Marks)
            .WithMany(m => m.Carss)
            .HasForeignKey(c => c.id_marki);

        modelBuilder.Entity<Carss>()
            .HasOne(c => c.strana)
            .WithMany(s => s.Carss)
            .HasForeignKey(c => c.id_str);

        modelBuilder.Entity<Carss>()
            .HasOne(c => c.cveta)
            .WithMany(cv => cv.Carss)
            .HasForeignKey(c => c.id_cvet);

        modelBuilder.Entity<Carss>()
            .HasOne(c => c.salonch)
            .WithMany(s => s.Carss)
            .HasForeignKey(c => c.id_salona);

        modelBuilder.Entity<Carss>()
            .HasOne(c => c.compl)
            .WithMany(co => co.Carss)
            .HasForeignKey(c => c.id_kompl);

        modelBuilder.Entity<user>()
            .HasOne(u => u.rol)
            .WithMany(r => r.user)
            .HasForeignKey(u => u.rol_id);
    }
}
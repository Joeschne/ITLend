using Microsoft.EntityFrameworkCore;
namespace API.Models;

public class ITLendDBContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Laptop> Laptops { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    public ITLendDBContext(DbContextOptions<ITLendDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

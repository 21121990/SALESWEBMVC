using Microsoft.EntityFrameworkCore;

namespace SalesWebMvc.Models
{
    public class SalesWebMvcContext : DbContext
    {
        public SalesWebMvcContext(DbContextOptions<SalesWebMvcContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Department { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<SalesRecord> SalesRecord { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<SalesRepository> SalesRepository { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Category> Category { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;

namespace N1285413.Models
{
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {

            }
            public DbSet<Customer> Customers { get; set; }
            public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        }

    }
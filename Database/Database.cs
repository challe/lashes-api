using Microsoft.EntityFrameworkCore;
using Lashes.Models;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Lashes.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() {
            this.Database.EnsureCreated();
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentLash> AppointmentLashes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string path = Path.Combine("./", "Lashes.db");
            optionsBuilder.UseSqlite("Filename=" + path);
        }
    }
}
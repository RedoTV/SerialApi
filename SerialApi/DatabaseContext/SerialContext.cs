using Microsoft.EntityFrameworkCore;
using SerialApi.Models;

namespace SerialApi.DatabaseContext
{
    public class SerialContext : DbContext
    {
        public DbSet<Serial> Serials { get; set; } = null!;
        public SerialContext(DbContextOptions<SerialContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SerialApi.Models;

namespace SerialApi.DatabaseContext
{
    public class AuthorizeContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public AuthorizeContext(DbContextOptions<AuthorizeContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

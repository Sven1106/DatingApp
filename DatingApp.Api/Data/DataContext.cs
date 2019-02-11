using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class DataContext : DbContext
    // We have to add DbContextOptions to our DataContext before it can use the DbContext in EF.
    // We do this by chaining the options onto our DataContext in the constructor
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using personwatcherapi.Models;

namespace personwatcherapi.Data
{
    public class PersonWatcherDbContext : DbContext
    {
        public PersonWatcherDbContext(DbContextOptions<PersonWatcherDbContext> options) : base(options) { }
        public DbSet<Place> Places { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
    
}

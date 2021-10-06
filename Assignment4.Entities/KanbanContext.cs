using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<User> Tasks {get; set;}
        public DbSet<User> Tags {get; set;}

    }
}

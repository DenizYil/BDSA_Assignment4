using System;
using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public DbSet<Developer> Users {get; set;}
        public DbSet<Task> Tasks {get; set;}
        public DbSet<Tag> Tags {get; set;}

        //public KanbanContext() {}

        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Task>()
                .Property(t => t.State)
                .HasConversion(new EnumToStringConverter<State>());
            
            modelBuilder
                .Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();
        }
    }
}

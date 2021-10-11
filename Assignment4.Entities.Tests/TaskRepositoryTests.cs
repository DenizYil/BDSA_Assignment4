using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repo;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            builder.EnableSensitiveDataLogging();

            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            _context = context;
            _repo = new TagRepository(_context);
        }
        
        //[Fact]
        //public void Create_

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

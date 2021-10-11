using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Assignment4.Core;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repo;

        public TagRepositoryTests()
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

        [Fact]
        public void Create_given_tag_returns_correct_response_and_id()
        {
            var tag = new TagCreateDTO {Name = "Complete"};

            var created = _repo.Create(tag);
            var expected = (Response.Created, 1);

            Assert.Equal(expected, created);
        }

        [Fact]
        public void Create_given_existing_name_returns_conflict()
        {
            var tag = new TagCreateDTO {Name = "Existing"};
            _repo.Create(tag);

            var expected = (Response.Conflict, -1);
            var actual = _repo.Create(tag);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_given_not_existing_tag_returns_notfound()
        {
            var expected = Response.NotFound;
            var actual = _repo.Delete(10);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Delete_given_existing_tag_returns_deleted()
        {
            var tag = new TagCreateDTO {Name = "About To Be Deleted"};
            
            (var response, var id) = _repo.Create(tag);
            
            var expected = Response.Deleted;
            var actual = _repo.Delete(id);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Read_given_existing_id_returns_tag()
        {
            _repo.Create(new TagCreateDTO {Name = "Complete"});

            var expected = new TagDTO(1, "Complete");
            var actual = _repo.Read(1);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Read_given_not_existing_id_returns_null()
        {
            Assert.Null(_repo.Read(10));
        }

        [Fact]
        public void Read_all_gives_empty_list()
        {
            Assert.Equal(0, _repo.ReadAll().Count);
        }
        
        [Fact]
        public void Read_all_gives_correct_list_given_added_elements()
        {
            _repo.Create(new TagCreateDTO {Name = "Element One"});
            _repo.Create(new TagCreateDTO {Name = "Element Two"});
            _repo.Create(new TagCreateDTO {Name = "Element Three"});

            var expected = new List<TagDTO>
            {
                new (1, "Element One"),
                new (2, "Element Two"),
                new (3, "Element Three")
            };
            var actual = _repo.ReadAll();
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Update_given_not_existing_id_returns_notfound()
        {
            var expected = Response.NotFound;
            var actual = _repo.Update(new TagUpdateDTO {Id = 10});
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Update_given_existing_id_returns_correct_name_and_response()
        {
            (var response, var id) = _repo.Create(new TagCreateDTO {Name = "About To Update"});

            var resp = _repo.Update(new TagUpdateDTO {Id = id, Name = "Updated!"});
            var updated = _repo.Read(id);
            
            Assert.Equal(Response.Updated, resp);
            Assert.Equal(new TagDTO(id, "Updated!"), updated);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
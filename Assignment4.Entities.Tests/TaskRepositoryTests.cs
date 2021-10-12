using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Sdk;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TaskRepository _repo;

        private readonly TaskCreateDTO DefaultTaskCreateDTO = new()
        {
            Title = "A",
            Description = "ABCDEF"
        };

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
            _repo = new TaskRepository(_context);
        }

        [Fact]
        public void Create_given_task_returns_correct_response_and_id()
        {
            var expected = (Response.Created, 1);
            var actual = _repo.Create(DefaultTaskCreateDTO);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_given_task_only_uses_existing_tags()
        {
            var first = new Tag
            {
                Id = 95,
                Name = "Tag 1"
            };

            var second = new Tag
            {
                Id = 96,
                Name = "Tag 2"
            };

            _context.Tags.AddRange(first, second);
            _context.SaveChanges();

            _repo.Create(new TaskCreateDTO
            {
                Title = "Tags Testing",
                Tags = new List<string> {"Tag 1", "Tag 2", "Tag 3", "Tag 4"}
            });

            var actual = _context.Tasks
                .FirstOrDefault(task => task.Id == 1);

            Assert.NotNull(actual);

            var expected = new Task
            {
                Title = "Tags Testing",
                Tags = new List<Tag> {first, second},
            };

            Assert.Equal(expected.Tags, actual.Tags);
        }

        [Fact]
        public void Create_given_task_sets_created_and_stateupdated_to_utcnow()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            var actual = _repo.Read(id);
            var expected = DateTime.UtcNow;

            Assert.Equal(actual.Created, actual.StateUpdated, TimeSpan.FromSeconds(1));
            Assert.Equal(expected, actual.Created, TimeSpan.FromSeconds(5));
            Assert.Equal(expected, actual.StateUpdated, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Read_returns_null_for_non_existant_id()
        {
            Assert.Null(_repo.Read(999));
        }

        [Fact]
        public void Read_returns_correct_task_by_id()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            var actual = _repo.Read(id);

            Assert.NotNull(actual);
            Assert.Equal(DefaultTaskCreateDTO.Title, actual.Title);
            Assert.Equal(DefaultTaskCreateDTO.Description, actual.Description);
        }

        [Fact]
        public void Update_returns_not_found_for_not_existing_task()
        {
            Assert.Equal(Response.NotFound, _repo.Update(new TaskUpdateDTO {Id = 999}));
        }

        [Fact]
        public void Update_updates_title_and_description()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            _repo.Update(new TaskUpdateDTO {Id = id, Title = "Title Updated!", Description = "Description Updated!"});
            var actual = _repo.Read(id);

            Assert.Equal("Title Updated!", actual.Title);
            Assert.Equal("Description Updated!", actual.Description);
        }

        [Fact]
        public void Update_updates_state()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            foreach (var state in new List<State> {State.Active, State.Closed, State.Removed, State.Resolved})
            {
                _repo.Update(new TaskUpdateDTO {Id = id, State = state});
                var actual = _repo.Read(id);

                Assert.Equal(state, actual.State);
            }
        }

        [Fact]
        public void Update_updates_tags()
        {
            var first = new Tag
            {
                Id = 95,
                Name = "Tag 1"
            };

            var second = new Tag
            {
                Id = 96,
                Name = "Tag 2"
            };

            _context.Tags.AddRange(first, second);
            _context.SaveChanges();

            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            // Should skip tag 3 as it's not registered
            _repo.Update(new TaskUpdateDTO {Id = id, Tags = new List<string> {"Tag 1", "Tag 2", "Tag 3"}});

            var expected = new List<string> {"Tag 1", "Tag 2"};
            var actual = _repo.Read(id).Tags;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_returns_not_found_for_not_existing_task()
        {
            Assert.Equal(Response.NotFound, _repo.Delete(999));
        }

        [Fact]
        public void Delete_returns_conflict_for_removed_resolved_and_closed_states()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);

            foreach (var state in new List<State> {State.Removed, State.Resolved, State.Closed})
            {
                _repo.Update(new TaskUpdateDTO {Id = id, State = state});
                Assert.Equal(Response.Conflict, _repo.Delete(id));
            }
        }

        [Fact]
        public void Delete_removes_task_with_new_state()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);
            _repo.Delete(id);

            Assert.Null(_repo.Read(id));
        }

        [Fact]
        public void Delete_changes_active_state_to_removed()
        {
            (var response, var id) = _repo.Create(DefaultTaskCreateDTO);
            _repo.Update(new TaskUpdateDTO {Id = id, State = State.Active});
            _repo.Delete(id);

            Assert.Equal(State.Removed, _repo.Read(id).State);
        }

        [Fact]
        public void ReadAll_returns_empty_list()
        {
            Assert.Equal(0, _repo.ReadAll().Count);
        }

        [Fact]
        public void ReadAll_returns_3_with_added_elements()
        {
            _repo.Create(DefaultTaskCreateDTO);
            _repo.Create(DefaultTaskCreateDTO);
            _repo.Create(DefaultTaskCreateDTO);
            Assert.Equal(3, _repo.ReadAll().Count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
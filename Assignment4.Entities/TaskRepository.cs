using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;

        // Converts a Task into a TaskDTO, this just saves a lot of duplicate code for select statements
        private static readonly Expression<Func<Task, TaskDTO>> TaskToTaskDTO = task => new TaskDTO(
            task.Id,
            task.Title,
            task.AssignedTo.Email,
            task.Tags
                .Select(tag => tag.Name)
                .ToImmutableList(),
            task.State
        );

        public TaskRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            // This feels wrong? How are tags validated.. Should this not pull from tag repo?
            var tags = new List<Tag>();

            foreach (var tag in task.Tags)
            {
                tags.Add(new Tag {Name = tag});
            }

            var created = _context.Tasks.Add(new Task
            {
                Title = task.Title,
                Description = task.Description,
                Tags = tags,
                State = State.New,
                Created = DateTime.UtcNow,
                StateUpdated = DateTime.UtcNow
            });
            _context.SaveChanges();

            return (Response.Created, created.Entity.Id);
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            return _context.Tasks
                .Select(TaskToTaskDTO)
                .ToImmutableList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            return ReadAllByState(State.Removed);
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            return _context.Tasks
                .Where(task => task.Tags.Any(t => t.Name == tag))
                .Select(TaskToTaskDTO)
                .ToImmutableList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            return _context.Tasks
                .Where(task => task.AssignedTo.Id == userId)
                .Select(TaskToTaskDTO)
                .ToImmutableList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            return _context.Tasks
                .Where(task => task.State == state)
                .Select(TaskToTaskDTO)
                .ToImmutableList();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            return _context.Tasks
                .Where(task => task.Id == taskId)
                .Select(task => new TaskDetailsDTO(
                    task.Id,
                    task.Title,
                    task.Description,
                    task.Created,
                    task.AssignedTo.Email,
                    task.Tags
                        .Select(tag => tag.Name)
                        .ToImmutableList(),
                    task.State,
                    task.StateUpdated
                ))
                .FirstOrDefault();
        }

        public Response Update(TaskUpdateDTO task)
        {
            throw new System.NotImplementedException();
        }

        public Response Delete(int taskId)
        {
            throw new System.NotImplementedException();
        }
    }
}
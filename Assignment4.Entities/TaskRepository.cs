using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using Assignment4.Core;
using Microsoft.VisualBasic;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;

        // Converts a Task into a TaskDTO, this just saves a lot of duplicate code for select statements
        private static readonly Expression<Func<Task, TaskDTO>> TaskToTaskDTO = task => new TaskDTO(
            task.Id,
            task.Title,
            task.AssignedTo.Name,
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
            // Only add existing tags...
            IEnumerable<Tag> tags = new List<Tag>();

            if (task.Tags != null)
            {
                tags = 
                    from name in task.Tags
                    from tag in _context.Tags
                    where tag.Name == name
                    select tag;
            }

            var created = _context.Tasks.Add(new Task
            {
                Title = task.Title,
                Description = task.Description,
                Tags = tags.ToImmutableList(),
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
                    task.AssignedTo.Name ?? null,
                    task.Tags
                        .Select(tag => tag.Name)
                        .ToImmutableList(),
                    task.State,
                    task.StateUpdated
                ))
                .FirstOrDefault();
        }

        public Response Update(TaskUpdateDTO update)
        {
            var task = _context.Tasks
                .FirstOrDefault(task => task.Id == update.Id);

            if (task == null)
            {
                return Response.NotFound;
            }

            if (update.Title != null)
            {
                task.Title = update.Title;
            }

            if (update.State != task.State)
            {
                task.State = update.State;
            }

            if (update.Description != null)
            {
                task.Description = update.Description;
            }

            if (update.AssignedToId != null)
            {
                // TODO: Do validation here
                task.AssignedTo = null;
            }

            if (update.Tags != null)
            {
                var tags =
                    from name in update.Tags
                    from tag in _context.Tags
                    where tag.Name == name
                    select tag;

                task.Tags = tags.ToImmutableList();
            }

            task.StateUpdated = DateTime.UtcNow;
            _context.SaveChanges();

            return Response.Updated;
        }

        public Response Delete(int taskId)
        {
            var task = _context.Tasks
                .FirstOrDefault(task => task.Id == taskId);

            if (task == null)
            {
                return Response.NotFound;
            }

            switch (task.State)
            {
                case State.Removed:
                case State.Resolved:
                case State.Closed:
                    return Response.Conflict;
                case State.New:
                    _context.Tasks.Remove(task);
                    _context.SaveChanges();
                    break;
                case State.Active:
                    task.State = State.Removed;
                    break;
            }

            _context.SaveChanges();

            return Response.Deleted;
        }
    }
}
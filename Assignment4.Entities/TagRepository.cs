using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext _context;

        public TagRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            var existing = _context.Tags
                .FirstOrDefault(t => t.Name == tag.Name);

            if (existing != null)
            {
                return (Response.Conflict, -1);
            }

            var entity = new Tag {Name = tag.Name};

            _context.Tags.Add(entity);
            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var tag = _context.Tags
                .FirstOrDefault(tag => tag.Id == tagId);

            if (tag == null)
            {
                return Response.NotFound;
            }

            _context.Tags.Remove(tag);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public TagDTO Read(int tagId)
        {
            return _context.Tags
                .Where(tag => tag.Id == tagId)
                .Select(tag => new TagDTO(tag.Id, tag.Name))
                .FirstOrDefault();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            return _context.Tags
                .Select(tag => new TagDTO(tag.Id, tag.Name))
                .ToImmutableList();
        }

        public Response Update(TagUpdateDTO tag)
        {
            var existing = _context.Tags
                .FirstOrDefault(t => t.Id == tag.Id);

            if (existing == null)
            {
                return Response.NotFound;
            }

            existing.Name = tag.Name;
            _context.SaveChanges();

            return Response.Updated;
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class Tag
    {
        public int ID {get; set;}

        [Required, Key]
        public string Name {get; set;}
        public ICollection<Task> Tasks {get; set;}
    }
}

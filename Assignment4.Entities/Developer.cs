using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class Developer
    {
        public int Id {get; set;}

        [Required]
        [StringLength(100)]
        public string Title {get; set;}

        [Required, EmailAddress, Key]
        [StringLength(100)]
        public string Email {get; set;}

        public IEnumerable<Task> Tasks {get; set;}
    }
}

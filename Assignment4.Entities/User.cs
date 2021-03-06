using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class User
    {
        public int Id {get; set;}

        [Required]
        [StringLength(100)]
        public string Name {get; set;}

        [Required]
        [Key]
        [EmailAddress]
        [StringLength(100)]
        public string Email {get; set;}

        public IEnumerable<Task> Tasks {get; set;}
    }
}

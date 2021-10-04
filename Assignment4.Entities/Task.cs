using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Assignment4.Core;

namespace Assignment4.Entities

{
    public class Task
    {
        public int ID {get; set;}

        [Required]
        public string Title {get; set;}

        [StringLength(int.MaxValue)]
        public User AssignedTo {get; set;}

        public string Description {get; set;}

        [Required]
        public State State {get; set;}

        public ICollection<Tag> Tags {get; set;}


    }
}

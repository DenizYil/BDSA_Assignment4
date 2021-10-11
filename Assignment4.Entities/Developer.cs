using System;
using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

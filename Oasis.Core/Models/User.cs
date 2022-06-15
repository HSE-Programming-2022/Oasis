using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public class User : Person
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Surname name cannot be longer than 50 characters.")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        public float Balance { get; set; }

        public User() : base("123", "456")
        {
            Name = "ivan";
            Surname = "Dolbilkin";
            Balance = 0;
        }

        public User(string login, string password, string name, string surname) : base(login, password)
        {
            Name = name;
            Surname = surname;
            Balance = 0;
        }
    }
}

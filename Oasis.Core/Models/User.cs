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
        public string Name { get; set; }

        public string Surname { get; set; }

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

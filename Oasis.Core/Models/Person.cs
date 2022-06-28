using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public abstract class Person
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Person(string login, string password)
        {
            Login = login;
            Password = password;
            RegistrationDate = DateTime.Now;
        }
    }
}

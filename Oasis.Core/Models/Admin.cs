using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public class Admin : Person
    {
        public Admin() : base("111", "222")
        {

        }

        public Admin(string login, string password) : base(login, password)
        {

        }
    }
}

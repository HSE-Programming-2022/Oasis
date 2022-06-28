using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public class Hall
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Price { get; set; }
        
        public ICollection<Seat> Seats { get; set; }

        public Hall()
        {
            Seats = new List<Seat>();
        }
    }
}

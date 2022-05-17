using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis.Core.Models
{
    public class Hall
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }

        public List<Seat> Seats { get; set; }
    }
}

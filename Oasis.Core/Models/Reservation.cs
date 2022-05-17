using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis.Core.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime stertTime { get; set; }
        public int Hours { get; set; }
        public Seat Seat { get; set; }
        public double Price { get; set; }

    }
}

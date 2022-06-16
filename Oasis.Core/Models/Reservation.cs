using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public class Reservation
    {

        public int Id { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        public User User { get; set; }

        public DateTime StartTime { get; set; }

        public int Hours { get; set; }

        [ForeignKey("Seat")]
        public int SeatId { get; set; }

        public Seat Seat { get; set; }

        public double Price { get; set; }

        public Reservation(User user, DateTime startTime, int hours, Seat seat, int price)
        {
            User = user;
            StartTime = startTime;
            Hours = hours;
            Seat = seat;
            Price = price;
        }
        public Reservation()
        {

        }
    }
}

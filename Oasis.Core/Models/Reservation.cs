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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required]
        public int Hours { get; set; }

        [Required]
        public Seat Seat { get; set; }

        [Required]
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

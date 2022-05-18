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
        [Key]
        [StringLength(50, ErrorMessage = "Hall name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Type name cannot be longer than 50 characters.")]
        public string Type { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public List<Seat> Seats { get; set; } //Спросить у Юры оставить list в hall или использовать фильтр через seats

        public Hall(string name, string type, int price, List<Seat> seats)
        {
            Name = name;
            Type = type;
            Price = price;
            Seats = seats;
        }
    }
}

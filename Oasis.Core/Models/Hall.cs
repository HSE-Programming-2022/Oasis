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

        
        public ICollection<Seat> Seats { get; set; } //Спросить у Юры оставить list в hall или использовать фильтр через seats

        public Hall()
        {
            Seats = new List<Seat>();
        }
        //public Hall(string name, string type, int price, List<Seat> seats)
        //{
        //    Name = name;
        //    Type = type;
        //    Price = price;
        //    Seats = seats;
        //}
    }
}

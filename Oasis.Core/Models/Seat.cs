using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oasis.Core.Models
{
    public class Seat
    {
        public int Id { get; set; }

        [ForeignKey("Hall")]
        public int? HallId { get; set; }

        public Hall Hall { get; set; }

        //public Seat()
        //{
            
        //}
    }
}

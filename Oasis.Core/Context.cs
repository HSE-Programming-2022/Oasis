using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Oasis.Core.Models;

namespace Oasis.Core
{
    public class Context : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }

        public Context() : base("DBConnection")
        {

        }
    }
}

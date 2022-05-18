using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using Oasis.Core;
using Oasis.Core.Models;

namespace Oasis.Design
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitialDb();
        }
        private void InitialPerson()
        {
            using (Context context = new Context())
            {
                Admin admin = new Admin("admin", "root");
                User user1 = new User("s1mple", "qwerty123", "Oleksandr", "Kostyliev");

                context.People.Add(admin);
                context.People.Add(user1);
                context.SaveChanges();

                MessageBox.Show("Saved");
            }
        }
        private void InitialDb()
        {
            using (Context context = new Context())
            {
                Admin admin = new Admin("admin", "root");
                User user1 = new User("s1mple", "qwerty123", "Oleksandr", "Kostyliev");

                context.People.Add(admin);
                context.People.Add(user1);
                List<Seat> seats = new List<Seat>
                {
                    new Seat(),
                    new Seat(),
                    new Seat(),
                    new Seat(),
                    new Seat()
                };
                Hall hall = new Hall("Bootcamp1", "PC", 150, seats);
                Reservation res = new Reservation(user1, Convert.ToDateTime("19:00:00 18-05-2022"), 1, seats[0], 150);
                context.Seats.AddRange(seats);
                context.Halls.Add(hall);
                context.Reservations.Add(res);
                context.SaveChanges();
                MessageBox.Show("Saved");
            }
        }
    }
}

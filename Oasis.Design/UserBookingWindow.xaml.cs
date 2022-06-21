using Oasis.Core;
using Oasis.Core.Models;
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
using System.Windows.Shapes;
using System.Data.Entity;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для UserBookingWindow.xaml
    /// </summary>
    public partial class UserBookingWindow : Window
    {
        string _selectedHall = "Bootcamp1";
        int _numberOfPeople = 1;
        DateTime _selectedDate = DateTime.Today;

        public UserBookingWindow(string type)
        {
            InitializeComponent();
            using(Context context = new Context())
            {
                if (type != "All")
                {
                    ComboBoxHall.ItemsSource = context.Halls.Where(x => x.Type == type).Select(x => x.Name).ToList();
                }
                else
                    ComboBoxHall.ItemsSource = context.Halls.Select(x => x.Name).ToList();
            }
            if (type == "PS")
                ComboBoxNumberOfPeople.Visibility = Visibility.Collapsed;
            //CheckingFreeTimeSlots();
        }

        private Dictionary<int, List<int>> CheckingFreeTimeSlots() // Выводит словарь {Номер временного слота - список свободных компьютеров}
        {
            using (Context context = new Context())
            {
                List<Reservation> reservations = context.Reservations.ToList();
                List<Seat> seats = SeatsOfHall();
                //foreach (var item in context.Reservations.ToList())
                //{
                //    resrvations.Add(item);
                //}
                var sortedReservations = reservations
                    .Where(res => res.Seat.HallId == context.Halls.Where(h => h.Name == _selectedHall).ToList()[0].Id)
                    .Where(res => res.StartTime.Date.CompareTo(_selectedDate.Date) == 0)
                    .ToList(); // получаем все резервации в нужную дату, в нужном зале

                var dicitionaryOfTime = GeneratingDitionaryOfFreeSeats();
                foreach (var item in sortedReservations)
                {
                    int hours = item.Hours;
                    int index = 1;
                    List<int> correctedSeats = dicitionaryOfTime[item.StartTime.Hour];
                    correctedSeats.Remove(item.Seat.Id);
                    dicitionaryOfTime[item.StartTime.Hour] = correctedSeats;
                    while (hours > 1)
                    {
                        List<int> correctedSeats1 = dicitionaryOfTime[item.StartTime.Hour + index];
                        correctedSeats.Remove(item.Seat.Id);
                        dicitionaryOfTime[item.StartTime.Hour + index] = correctedSeats1;
                        index++;
                        hours--;
                    }
                }
                return dicitionaryOfTime;
            }
        }

        private List<Seat> SeatsOfHall()
        {
            using (Context context = new Context())
            {

                //List<Seat> seats = new List<Seat>();
                //var seats = context.Seats.Include(x => x.Hall).ToList();
                //var a = context.Seats.ToList()[1].Hall.Name;
                //foreach (var item in context.Halls) 
                //{
                //    if (item.Name == _selectedHall)
                //    {
                //        foreach (var seat in item.Seats)
                //        {
                //            seats.Add(seat);
                //        }
                //    }
                //}
                return context.Seats.Include(x => x.Hall).Where(n => n.Hall.Name == _selectedHall).ToList(); // все ситы из выбранного хола
            }
        }

        private Dictionary<int, List<int>> GeneratingDitionaryOfFreeSeats() // Создает словарь {номер времяного слота - номера компьтеров}
        {
            List<Seat> seats = SeatsOfHall();
            var dicitionaryOfTime = new Dictionary<int, List<int>>();
            var enitialListOfSeats = new List<int>();
            for (int i = 0; i < seats.Count(); i++)
            {
                enitialListOfSeats.Add(i);
            }
            for (int i = 0; i < 23; i++)
            {
                dicitionaryOfTime.Add(i, enitialListOfSeats);
            }
            return dicitionaryOfTime;
        }

 
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OpenBigButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

       

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBoxHall_Selected(object sender, RoutedEventArgs e)
        {
            _selectedHall = ComboBoxHall.SelectedItem.ToString();
        }

        private void ComboBoxNumberOfPeople_Selected(object sender, RoutedEventArgs e)
        {
            _numberOfPeople = ComboBoxNumberOfPeople.SelectedIndex + 1;
        }

        private void ChoosingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
        }

       
    }
}

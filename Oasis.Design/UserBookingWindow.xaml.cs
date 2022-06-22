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
        string _selectedHall;
        int _numberOfPeople;
        DateTime _selectedDate;
        Dictionary<int, List<int>> SortedDictionary;
        Button[] allbuttons;
        bool IfFirstTimeChosen = false;

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
            {
                ComboBoxNumberOfPeople.Visibility = Visibility.Collapsed;
                _numberOfPeople = 1;
            }
            allbuttons = new Button[] { Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, Button10, Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19, Button20, Button21, Button22, Button23 };
        }

        private Dictionary<int, List<int>> CheckingFreeTimeSlots() // Выводит словарь {Номер временного слота - список свободных компьютеров}
        {
            using (Context context = new Context())
            {
                List<Reservation> reservations = context.Reservations.Include("Seat").Include("User").ToList();
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
                    int index = 0;
                    
                    while (hours != 0)
                    {
                        List<int> correctedSeats = dicitionaryOfTime[item.StartTime.Hour + index];
                        correctedSeats.Remove(item.Seat.Id);
                        dicitionaryOfTime[item.StartTime.Hour + index] = correctedSeats;
                        hours--;
                        index++;
                        //List<int> correctedSeats1 = dicitionaryOfTime[item.StartTime.Hour + index];
                        //correctedSeats.Remove(item.Seat.Id);
                        //dicitionaryOfTime[item.StartTime.Hour + index] = correctedSeats1;
                        //index++;
                        //hours--;
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
                enitialListOfSeats.Add(seats[i].Id);
            }
            for (int i = 0; i < 24; i++)
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ChoosingDatePicker_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _selectedDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
            if (ComboBoxNumberOfPeople.SelectedItem != null && ComboBoxHall.SelectedItem != null)
            {
                SortedDictionary = CheckingFreeTimeSlots();
                SetButtons();
            }
        }

        private void ComboBoxHall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedHall = ComboBoxHall.SelectedItem.ToString();
            using (Context context = new Context())
            {
                ComboBoxNumberOfPeople.ItemsSource = Enumerable.Range(1, context.Seats.Include(x => x.Hall).Where(n => n.Hall.Name == _selectedHall).ToList().Count()).ToArray();
            }
            if (ComboBoxNumberOfPeople.SelectedItem != null && ChoosingDatePicker.SelectedDate != null)
            {
                SortedDictionary = CheckingFreeTimeSlots();
                SetButtons();
            }
            ComboBoxNumberOfPeople.IsEnabled = true;
        }

        private void ComboBoxNumberOfPeople_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _numberOfPeople = ComboBoxNumberOfPeople.SelectedIndex + 1;
            if (ComboBoxHall.SelectedItem != null && ChoosingDatePicker.SelectedDate != null)
            {
                SortedDictionary = CheckingFreeTimeSlots();
                SetButtons();
            }
            _numberOfPeople = Convert.ToInt32(ComboBoxNumberOfPeople.SelectedItem.ToString());
        }


        private void SetButtons()
        {
            int currhour = 0;
            if (_selectedDate == DateTime.Today)
            {
                currhour = DateTime.Now.Hour + 1;
            }
            EnableAllButtons(false);
            for (int i = currhour; i < allbuttons.Count(); i++)
            {
                if (_numberOfPeople <= SortedDictionary[i].Count)
                    allbuttons[i].IsEnabled = true;
                allbuttons[i].Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
            }
        }

        private void EnableAllButtons(bool fl)
        {
            foreach (var i in allbuttons)
            {
                i.IsEnabled = fl;
            }
        }

        private void DisableButtonsTill(Button button)
        {
            foreach (var i in allbuttons)
            {
                if (i.Name == button.Name)
                    break;
                i.IsEnabled = false;
            }
        }

        private void PaintingButtons(Button button)
        {
            bool fl = false;
            bool endfl = false;
            foreach (var i in allbuttons)
            {
                if (i == button)
                {
                    endfl = true;
                    i.Background = Brushes.Indigo;
                }
                else if (i.Background == Brushes.Indigo && !endfl && button != i)
                    fl = true;
                else if (fl && button != i && !endfl)
                    i.Background = Brushes.Indigo;
                else if (fl && button == i && !endfl)
                {
                    i.Background = Brushes.Indigo;
                    endfl = true;
                }
                else if (endfl)
                    i.Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
            }
        }

        private void ClickForAll(Button button)
        {
            int inndex = Array.FindIndex(allbuttons, x => x.Name == button.Name);
            if (button.Background == Brushes.Indigo && allbuttons[Array.FindIndex(allbuttons, x => x.Name == button.Name) - 1].IsEnabled == false)
            {
                SetButtons();
                button.Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
                IfFirstTimeChosen = false;
            }
            else if (!IfFirstTimeChosen)
            {
                IfFirstTimeChosen = true;
                DisableButtonsTill(button);
                button.Background = Brushes.Indigo;
            }
            else if(IfFirstTimeChosen)
            {
                PaintingButtons(button);
            }
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button14_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button15_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button16_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button17_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button18_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button19_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button20_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button22_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }

        private void Button23_Click(object sender, RoutedEventArgs e)
        {
            ClickForAll(sender as Button);
        }
    }
}
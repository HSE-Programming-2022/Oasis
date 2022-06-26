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
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для UserBookingWindow.xaml
    /// </summary>
    public partial class UserBookingWindow : Window
    {
        User CurrentUser;
        string _selectedHall;
        int _numberOfPeople;
        DateTime _selectedDate;
        Dictionary<int, List<int>> SortedDictionary;
        Button[] allbuttons;
        bool IfFirstTimeChosen = false;
        List<int[]> ListOfPossibleSeatNumbers = new List<int[]> { };
        int price;
        string Type;
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 100,
                offsetY: 5);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public UserBookingWindow(string type, User user)
        {
            Type = type;
            InitializeComponent();
            using(Context context = new Context())
            {
                if (type != "All")
                {
                    ComboBoxHall.ItemsSource = context.Halls.Where(x => x.Type == type).Select(x => x.Name).ToList();
                }
                else
                    ComboBoxHall.ItemsSource = context.Halls.Select(x => x.Name).ToList();
                foreach (var item in context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Email == user.Email)
                        {
                            CurrentUser = (item as User);
                        }
                    }
                }
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
                    }
                }
                return dicitionaryOfTime;
            }
        }

        private List<Seat> SeatsOfHall()
        {
            using (Context context = new Context())
            {
                return context.Seats.Include(x => x.Hall).Where(n => n.Hall.Name == _selectedHall).ToList(); // все ситы из выбранного хола
            }
        }

        private Dictionary<int, List<int>> GeneratingDitionaryOfFreeSeats() // Создает словарь {номер времяного слота - номера компьтеров}
        {
            List<Seat> seats = SeatsOfHall();
            var dicitionaryOfTime = new Dictionary<int, List<int>>();
            for (int i = 0; i < 24; i++)
            {
                var enitialListOfSeats = new List<int>();
                for (int j = 0; j < seats.Count(); j++)
                {
                    enitialListOfSeats.Add(seats[j].Id);
                }
                dicitionaryOfTime.Add(i, enitialListOfSeats);
            }
            return dicitionaryOfTime;
        }

        private void OpenPreviousWindow()
        {
            if (Type == "All")
            {
                AdminWindow UsersInAdmin = new AdminWindow();
                UsersInAdmin.Show();
                UsersInAdmin.Admin.Content = new AdminUsersPage();
                UsersInAdmin.DashboardStickButton.Visibility = Visibility.Hidden;
                UsersInAdmin.UsersStickButton.Visibility = Visibility.Visible;
                UsersInAdmin.StatisticksStickButton.Visibility = Visibility.Hidden;
            }
            else
            {
                UserChoosingTypeofActivity UserWindow = new UserChoosingTypeofActivity(CurrentUser);
                UserWindow.Show();
            }
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPreviousWindow();
            Close();
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
            SetNewTotalPrice();
            IfFirstTimeChosen = false;
            ListOfPossibleSeatNumbers = new List<int[]> { };
        }

        private void ComboBoxHall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedHall = ComboBoxHall.SelectedItem.ToString();
            using (Context context = new Context())
            {
                ComboBoxNumberOfPeople.ItemsSource = Enumerable.Range(1, context.Seats.Include(x => x.Hall).Where(n => n.Hall.Name == _selectedHall).ToList().Count()).ToArray();
                price = context.Halls.Where(x => _selectedHall == x.Name).ToList()[0].Price;
            }
            if (ComboBoxNumberOfPeople.SelectedItem != null && ChoosingDatePicker.SelectedDate != null)
            {
                SortedDictionary = CheckingFreeTimeSlots();
                SetButtons();
            }
            ComboBoxNumberOfPeople.IsEnabled = true;
            SetNewTotalPrice();
            IfFirstTimeChosen = false;
            ListOfPossibleSeatNumbers = new List<int[]> { };
        }

        private void ComboBoxNumberOfPeople_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _numberOfPeople = Convert.ToInt32(ComboBoxNumberOfPeople.SelectedItem.ToString());
            if (ComboBoxHall.SelectedItem != null && ChoosingDatePicker.SelectedDate != null)
            {
                SortedDictionary = CheckingFreeTimeSlots();
                SetButtons();
            }
            SetNewTotalPrice();
            IfFirstTimeChosen = false;
            ListOfPossibleSeatNumbers = new List<int[]> { };
        }

        private void SetNewTotalPrice()
        {
            int totalPrice = 0;
            foreach (var button in allbuttons)
            {
                if (button.Background == Brushes.MediumSeaGreen)
                    totalPrice += price;
            }
            CurrentPrice.Text = $"{totalPrice * _numberOfPeople} р.";
        }

        private void SetButtons()
        {
            int currhour = 0;
            if (_selectedDate >= DateTime.Today)
            {
                if (_selectedDate == DateTime.Today)
                    currhour = DateTime.Now.Hour + 1;
                EnableAllButtons(false);
                for (int i = currhour; i < allbuttons.Count(); i++)
                {
                    if (_numberOfPeople <= SortedDictionary[i].Count && IfAnySeatsTogether(allbuttons[i]))
                        allbuttons[i].IsEnabled = true;
                    allbuttons[i].Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
                }
            }
            else
                EnableAllButtons(false);
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
                    i.Background = Brushes.MediumSeaGreen;
                }
                else if (i.Background == Brushes.MediumSeaGreen && !endfl && button != i)
                    fl = true;
                else if (fl && button != i && !endfl)
                    i.Background = Brushes.MediumSeaGreen;
                else if (fl && button == i && !endfl)
                {
                    i.Background = Brushes.MediumSeaGreen;
                    endfl = true;
                }
                else if (endfl)
                    i.Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
            }
        }

        private void CheckReservationPossibility(Button button)
        {
            List<int> IntersectListOfSeats = SortedDictionary[GetHourOfButton(button)];
            bool startfl = false;
            bool tryingToDeleteLastArrayOfSeats = false;
            for (int i = 0; i < allbuttons.Count(); i++)
            {
                if(startfl)
                {
                    for (int j = 0; j < ListOfPossibleSeatNumbers.Count(); j++)
                    {
                        if (SortedDictionary[i].Intersect(ListOfPossibleSeatNumbers[j]).Count() != ListOfPossibleSeatNumbers[j].Count())
                        {
                            if (ListOfPossibleSeatNumbers.Count() > 1)
                            {
                                ListOfPossibleSeatNumbers.Remove(ListOfPossibleSeatNumbers[j]);
                                j--;
                            }
                            else
                                tryingToDeleteLastArrayOfSeats = true;
                        }
                    }
                    if (tryingToDeleteLastArrayOfSeats == true)
                        startfl = false;
                }
                if (allbuttons[i].Name == button.Name)
                    startfl = true;
                if (!startfl)
                    allbuttons[i].IsEnabled = false;
            }
        }

        private bool IfAnySeatsTogether(Button button)
        {
            int hourOfButton = GetHourOfButton(button);
            for (int k = 0; k < SortedDictionary[hourOfButton].Count(); k++)
            {
                if (k + _numberOfPeople <= SortedDictionary[hourOfButton].Count())
                {
                    bool SeatsNotTogether = false;
                    int[] nearbyseats = new int[_numberOfPeople];
                    for (int j = 0; j < _numberOfPeople; j++)
                    {
                        if (j == 0)
                            nearbyseats[j] = SortedDictionary[hourOfButton][k + j];
                        else if (nearbyseats[j - 1] == SortedDictionary[hourOfButton][k + j] - 1)
                            nearbyseats[j] = SortedDictionary[hourOfButton][k + j];
                        else
                            SeatsNotTogether = true;
                    }
                    if (!SeatsNotTogether)
                        return true;
                }
            }
            return false;
        }

        private void SetListOfPossibleSeatNumbers(Button button)
        {
            int hourOfButton = GetHourOfButton(button);
            for (int k = 0; k < SortedDictionary[hourOfButton].Count(); k++)
            {
                if (k + _numberOfPeople <= SortedDictionary[hourOfButton].Count())
                {
                    bool SeatsNotTogether = false;
                    int[] nearbyseats = new int[_numberOfPeople];
                    for (int j = 0; j < _numberOfPeople; j++)
                    {
                        if (j == 0)
                            nearbyseats[j] = SortedDictionary[hourOfButton][k + j];
                        else if (nearbyseats[j - 1] == SortedDictionary[hourOfButton][k + j] - 1)
                            nearbyseats[j] = SortedDictionary[hourOfButton][k + j];
                        else
                            SeatsNotTogether = true;
                    }
                    if (!SeatsNotTogether)
                        ListOfPossibleSeatNumbers.Add(nearbyseats);
                }
            }
        }

        private void ClickForAll(Button button)
        {
            int indexofbutton = GetHourOfButton(button);
            if (indexofbutton - 1 > -1)
            {
                if (button.Background == Brushes.MediumSeaGreen && allbuttons[indexofbutton - 1].IsEnabled == false)
                {
                    SetButtons();
                    button.Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
                    IfFirstTimeChosen = false;
                    ListOfPossibleSeatNumbers = new List<int[]> { };
                }
                else if (!IfFirstTimeChosen)
                {
                    IfFirstTimeChosen = true;
                    DisableButtonsTill(button);
                    button.Background = Brushes.MediumSeaGreen;
                    SetListOfPossibleSeatNumbers(button);
                    CheckReservationPossibility(button);
                }
                else if (IfFirstTimeChosen)
                {
                    PaintingButtons(button);
                }
                SetNewTotalPrice();
            }
            else if (!IfFirstTimeChosen)
            {
                IfFirstTimeChosen = true;
                button.Background = Brushes.MediumSeaGreen;
                SetListOfPossibleSeatNumbers(button);
                CheckReservationPossibility(button);
            }
            else if (IfFirstTimeChosen)
            {
                SetButtons();
                button.Background = (Brush)(new BrushConverter()).ConvertFrom("#FF673AB7");
                IfFirstTimeChosen = false;
                ListOfPossibleSeatNumbers = new List<int[]> { };
            }

        }

        private int GetHourOfButton(Button button)
        {
            return Array.FindIndex(allbuttons, x => x.Name == button.Name);
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

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPrice = 0;
            int hours = 0;
            int startHour = -1;
            for (int i = 0; i < allbuttons.Count(); i++)
            {
                if (allbuttons[i].Background == Brushes.MediumSeaGreen)
                {
                    if (startHour == -1)
                        startHour = i;
                    totalPrice += price;
                    hours++;
                }
            }
            totalPrice = totalPrice * _numberOfPeople;
            if (CurrentUser.Balance >= totalPrice)
            {
                using (Context _context = new Context())
                {
                    foreach (var item in _context.People)
                    {
                        if (item is User)
                        {
                            if ((item as User).Email == CurrentUser.Email)
                            {
                                (item as User).Balance -= totalPrice;
                                CurrentUser = (item as User);
                            }
                        }
                    }
                    List<Reservation> newReservations = new List<Reservation> { };
                    foreach (var item in ListOfPossibleSeatNumbers[0])
                    {
                        Seat seat = _context.Seats.Where(x => x.Id == item).ToList()[0];
                        newReservations.Add(new Reservation(CurrentUser, _selectedDate.AddHours(startHour), hours, seat, price * hours));
                    }
                    _context.Reservations.AddRange(newReservations);
                    _context.SaveChanges();
                }
                OpenPreviousWindow();
                Close();


            }
            else
            {
                notifier.ShowWarning("Не достаточно денег на балансе");
            }




        }
    }
}
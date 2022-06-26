using LiveCharts.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для AdminDashboardPage.xaml
    /// </summary>
    public partial class AdminDashboardPage : Page
    {
        string _periodOfTime = "День";
        int _amountOfSeatsInClub;
        int _numberOfDays = 1;
        public AdminDashboardPage()
        {
            InitializeComponent();
            //CountingAllSeats();
            DataPanelsUpdate(FormingListOfValidReses());
            UpdatingGraphValues(CheckingFreeTimeSlots());

        }

        public void CountingAllSeats()
        {
            using (Context context = new Context())
            {
                _amountOfSeatsInClub = context.Seats.ToList().Count();
            }
        }

        public List<Reservation> FormingListOfValidReses()
        {
            using (Context context = new Context())
            {
                List<Reservation> reservations = context.Reservations.Include("Seat").Include("User").ToList();
                List<Reservation> validReservations = new List<Reservation>();
                DateTime dateTime = DateTime.Now;
                _numberOfDays = 0;
                if (_periodOfTime == "День")
                {
                    _numberOfDays = 1;
                    foreach (var res in reservations)
                    {
                        if (res.StartTime.Date == dateTime.Date)
                        {
                            validReservations.Add(res);
                        }
                    }
                }
                else if (_periodOfTime == "Неделя")
                {
                    while (true)
                    { 
                        foreach (var res in reservations)
                        {
                            if (res.StartTime.Date == dateTime.Date)
                            {
                                validReservations.Add(res);
                            }
                        }
                        _numberOfDays += 1;
                        dateTime = dateTime.Subtract(new TimeSpan(1, 0, 0, 0));
                        if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                        {
                            break;
                        }
                    }
                }
                else if (_periodOfTime == "Месяц")
                {
                    int month = dateTime.Month;
                    while (true)
                    {
                        foreach (var res in reservations)
                        {
                            if (res.StartTime.Date == dateTime.Date)
                            {
                                validReservations.Add(res);
                            }
                        }
                        _numberOfDays += 1;
                        dateTime = dateTime.Subtract(new TimeSpan(1, 0, 0, 0));
                        if (dateTime.Month != month)
                        {
                            break;
                        }
                    }
                }
                return validReservations;
            }
        }

        public int AmountOfSpentHours(List<Reservation> reservations)
        {
            int amountOfSpentHours = 0;
            foreach (var res in reservations)
            {
                amountOfSpentHours += res.Hours;
            }
            return amountOfSpentHours;
        }

        public double TotalRevenue(List<Reservation> reservations)
        {
            double totalRevenue = 0;
            foreach (var res in reservations)
            {
                totalRevenue += res.Hours * res.Price;
            }
            return totalRevenue;
        }

        public void DataPanelsUpdate(List<Reservation> reservations)
        {
            double totalRevenue = TotalRevenue(reservations);
            int amountOfSpentHours = AmountOfSpentHours(reservations);
            NumberOfSpentHours.Text = $"{amountOfSpentHours} ч.";
            TotalRevenue_TextBlock.Text = $"{totalRevenue} ₽";
            AmountOfClients_TextBlock.Text = $"{reservations.Count()} чел.";
            AmountOfClientsPerPeriod.Text = $"Статистика {_periodOfTime}";
            NumberOfSpentHoursPerPeriod.Text = $"Статистика {_periodOfTime}";
            TotalRevenuePerPeriod.Text = $"Статистика {_periodOfTime}";
        }

        private Dictionary<int, List<int>> CheckingFreeTimeSlots() // Выводит словарь {Номер временного слота - список занятых компьютеров}
        {
            var reservations = FormingListOfValidReses();
            using (Context context = new Context())
            {
                var dicitionaryOfTime = GeneratingDitionaryOfTimeSlots();
                foreach (var res in reservations)
                {
                    int hours = res.Hours;
                    int index = 0;

                    while (hours != 0)
                    {
                        List<int> correctedSeats = dicitionaryOfTime[res.StartTime.Hour + index];
                        correctedSeats.Add(res.Seat.Id);
                        dicitionaryOfTime[res.StartTime.Hour + index] = correctedSeats;
                        hours--;
                        index++;
                    }
                }
                return dicitionaryOfTime;
            }
        }

        public void UpdatingGraphValues(Dictionary<int, List<int>> dicitionaryOfTime)
        {
            List<int> listOfValues = new List<int>();
            List<int> ints = new List<int>();
            for (int i = 0; i < 24; i = i+4)
            {
                for (int j = 0; j < 4; j++)
                {
                    ints.Add(dicitionaryOfTime[i + j].Count());
                }
                listOfValues.Add(ints.Count());
                ints.Clear();
            }
            listOfValues.Insert(0, listOfValues[0]); // графику нужно какое-то 0-вое значение, мы приравниваем его к первому
            DashBoardGraph.Values = listOfValues.AsChartValues();
        }

        private Dictionary<int, List<int>> GeneratingDitionaryOfTimeSlots() // Создает словарь {номер времяного слота - пустой список с номерами занятых компьтеров}
        {
            var dicitionaryOfTime = new Dictionary<int, List<int>>();
            for (int i = 0; i < 24; i++)
            {
                var enitialListOfSeats = new List<int>();
                dicitionaryOfTime.Add(i, enitialListOfSeats);
            }
            return dicitionaryOfTime;
        }

        private void StatisticsForeDayButton_Click(object sender, RoutedEventArgs e)
        {

            StatisticsForeDayButton.Background = Brushes.DarkBlue;
            StatisticsForeMonthButton.Background = Brushes.Transparent;
            StatisticsForeYearButton.Background = Brushes.Transparent;
            _periodOfTime = "День";
            DataPanelsUpdate(FormingListOfValidReses());
            UpdatingGraphValues(CheckingFreeTimeSlots());
        }

        private void StatisticsForeMonthButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsForeDayButton.Background = Brushes.Transparent;
            StatisticsForeMonthButton.Background = Brushes.DarkBlue;
            StatisticsForeYearButton.Background = Brushes.Transparent;
            _periodOfTime = "Неделя";
            DataPanelsUpdate(FormingListOfValidReses());
            UpdatingGraphValues(CheckingFreeTimeSlots());
        }

        private void StatisticsForeYearButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsForeDayButton.Background = Brushes.Transparent;
            StatisticsForeMonthButton.Background = Brushes.Transparent;
            StatisticsForeYearButton.Background = Brushes.DarkBlue;
            _periodOfTime = "Месяц";
            DataPanelsUpdate(FormingListOfValidReses());
            UpdatingGraphValues(CheckingFreeTimeSlots());
        }
    }
}

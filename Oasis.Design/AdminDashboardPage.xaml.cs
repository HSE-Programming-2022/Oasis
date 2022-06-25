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
        public AdminDashboardPage()
        {
            InitializeComponent();
            DataPanelsUpdate(FormingListOfValidReses());


        }

        public List<Reservation> FormingListOfValidReses()
        {
            using (Context context = new Context())
            {
                List<Reservation> reservations = context.Reservations.Include("Seat").Include("User").ToList();
                List<Reservation> validReservations = new List<Reservation>();
                DateTime dateTime = DateTime.Now;
                if (_periodOfTime == "День")
                {
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
                        dateTime.AddDays(-1);
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
                        dateTime.AddDays(-1);
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
            AmountOfClients_TextBlock.Text = $"{reservations.Count()} ч.";
            AmountOfClientsPerPeriod.Text = $"Статистика {_periodOfTime}";
            NumberOfSpentHoursPerPeriod.Text = $"Статистика {_periodOfTime}";
            TotalRevenuePerPeriod.Text = $"Статистика {_periodOfTime}";
        }



        private void StatisticsForeDayButton_Click(object sender, RoutedEventArgs e)
        {

            StatisticsForeDayButton.Background = Brushes.DarkBlue;
            StatisticsForeMonthButton.Background = Brushes.Transparent;
            StatisticsForeYearButton.Background = Brushes.Transparent;
            _periodOfTime = "День";
        }

        private void StatisticsForeMonthButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsForeDayButton.Background = Brushes.Transparent;
            StatisticsForeMonthButton.Background = Brushes.DarkBlue;
            StatisticsForeYearButton.Background = Brushes.Transparent;
            _periodOfTime = "Неделя";
        }

        private void StatisticsForeYearButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsForeDayButton.Background = Brushes.Transparent;
            StatisticsForeMonthButton.Background = Brushes.Transparent;
            StatisticsForeYearButton.Background = Brushes.DarkBlue;
            _periodOfTime = "Месяц";
        }
    }
}

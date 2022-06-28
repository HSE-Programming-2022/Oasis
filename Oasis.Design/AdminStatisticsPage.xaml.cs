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
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Oasis.Core;
using Oasis.Core.Models;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для AdminStatisticsPage.xaml
    /// </summary>
    public partial class AdminStatisticsPage : Page
    {
        DateTime _startDate;
        DateTime _finalDate;
        
        public AdminStatisticsPage()
        {
            _finalDate = DateTime.Now;
            _startDate =  _finalDate.Subtract(new TimeSpan(30, 0, 0, 0)); // по умолчанию будет выводится статистика за последние 30 дней

            InitializeComponent();

            UpdatingPieChart(ProfitPerType());
            //UpdatingCartesianChart(AverageTimeOfUsingPerType());
        }

        private List<Reservation> FormingListOfValidReses()
        {
            using (Context context = new Context())
            {
                List<Reservation> reservations = context.Reservations.Include("Seat").Include("User").ToList();
                List<Reservation> validReservations = new List<Reservation>();
                foreach (var res in reservations)
                {
                    if ((res.StartTime.Date.CompareTo(_startDate) >= 0) && (res.StartTime.Date.CompareTo(_finalDate) <= 0))
                    {
                        validReservations.Add(res);
                    }
                }
                return validReservations;
            }
        }

        private Dictionary<string, int> FormingEmptyProfitPerTypeDictionary()
        {
            Dictionary<string, int> profitPerTypeEmptyDictionary = new Dictionary<string, int>();
            using (Context context = new Context())
            {
                List<Hall> halls = context.Halls.ToList();
                foreach (var hall in halls)
                {
                    if (!(profitPerTypeEmptyDictionary.ContainsKey($"{hall.Type}")))
                    {
                        profitPerTypeEmptyDictionary.Add(hall.Type, 0);
                    }
                }
                return profitPerTypeEmptyDictionary;
            }
        }

        private Dictionary<string, int> ProfitPerType()
        {
            var reservations = FormingListOfValidReses();
            Dictionary<string, int> profitPerType = FormingEmptyProfitPerTypeDictionary();
            using (Context context = new Context())
            {
                List<Hall> halls = context.Halls.ToList();
                foreach (var hall in halls)
                {
                    var sortedReservations = reservations
                    .Where(r => r.Seat.HallId == context.Halls.Where(h => h.Type == hall.Type).ToList()[0].Id)
                    .ToList();
                    foreach (var res in sortedReservations)
                    {
                        profitPerType[hall.Type] += (int)res.Price;
                    }
                }
            }
            return profitPerType;    
        }

        private int TotalProfit()
        {
            var reservations = FormingListOfValidReses();
            int totalProfit = 0;
            foreach (var res in reservations)
            {
                totalProfit += (int)res.Price;
            }
            return totalProfit;
        }

        private void UpdatingPieChart(Dictionary<string, int> profitPerHall)
        {
            int totalProfit = TotalProfit();
            int pcProfit = (int)profitPerHall["PC"];
            int vipPcProfit = (int)profitPerHall["VIP PC"];
            int psProfit = (int)profitPerHall["PS"];
            PCPartOfPieChart.Values = new ChartValues<int> { (int)((float)pcProfit / (float)totalProfit * 100) };
            VIP_PCPartOfPieChart.Values = new ChartValues<int> { (int)((float)vipPcProfit / totalProfit * 100) };
            PSPartOfPieChart.Values = new ChartValues<int> { (int)((float)psProfit / totalProfit * 100) };
        }


        private Dictionary<string, float> FormingEmptyAverageTimeOfUsingPerTypeDictionary()
        {
            Dictionary<string, float> profitPerTypeEmptyDictionary = new Dictionary<string, float>();
            using (Context context = new Context())
            {
                List<Hall> halls = context.Halls.ToList();
                foreach (var hall in halls)
                {
                    if (!(profitPerTypeEmptyDictionary.ContainsKey($"{hall.Type}")))
                    {
                        profitPerTypeEmptyDictionary.Add(hall.Type, 0);
                    }
                }
                return profitPerTypeEmptyDictionary;
            }
        }

        private Dictionary<string, float> AverageTimeOfUsingPerType()
        {
            var reservations = FormingListOfValidReses();
            Dictionary<string, float> averageTimeOfUsingPerType = FormingEmptyAverageTimeOfUsingPerTypeDictionary();
            using (Context context = new Context())
            {
                List<Hall> halls = context.Halls.ToList();
                foreach (var hall in halls)
                {
                    int amountOfReses = 0;
                    var sortedReservations = reservations
                    .Where(r => r.Seat.HallId == context.Halls.Where(h => h.Type == hall.Type).ToList()[0].Id)
                    .ToList();
                    foreach (var res in sortedReservations)
                    {
                        averageTimeOfUsingPerType[hall.Type] += (int)res.Price;
                        amountOfReses += 1;
                    }
                    averageTimeOfUsingPerType[hall.Type] = (float)averageTimeOfUsingPerType[hall.Type] / (float)amountOfReses;
                }
                return averageTimeOfUsingPerType;
            }
        }

        private void UpdatingCartesianChart(Dictionary<string, float> averageTimeOfUsingPerType)
        {
            List<float> listOfValues = new List<float>()
            {
                averageTimeOfUsingPerType["VIP PC"],
                averageTimeOfUsingPerType["PC"],
                averageTimeOfUsingPerType["PS"]
            };
            StatisticsGraph.Values = listOfValues.AsChartValues();
        }

        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChoosingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString()).CompareTo(_finalDate) <= 0)
            {
                _startDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
            }
            UpdatingPieChart(ProfitPerType());
            //UpdatingCartesianChart(AverageTimeOfUsingPerType());

        }

        private void ChoosingFinalDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateTime.Parse(ChoosingFinalDatePicker.SelectedDate.ToString()).CompareTo(_startDate) >= 0)
            {
                _finalDate = DateTime.Parse(ChoosingFinalDatePicker.SelectedDate.ToString());
            }
            UpdatingPieChart(ProfitPerType());
            //UpdatingCartesianChart(AverageTimeOfUsingPerType());
        }
    }
}

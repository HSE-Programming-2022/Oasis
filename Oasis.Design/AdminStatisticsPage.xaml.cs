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
                    .Where(r => r.Seat.HallId == context.Halls.Where(h => h.Name == hall.Name).ToList()[0].Id)
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

        public void UpdatingPieChart(Dictionary<string, int> profitPerHall)
        {
            int totalProfit = TotalProfit();
            int pcProfit = (int)profitPerHall["PC"];
            int vipPcProfit = (int)profitPerHall["VIP PC"];
            int psProfit = (int)profitPerHall["PS"];
            int qwdfwdsf = pcProfit / totalProfit * 100;
            PCPartOfPieChart.Values = new ChartValues<int> { (int)((float)pcProfit / (float)totalProfit * 100) };
            VIP_PCPartOfPieChart.Values = new ChartValues<int> { (int)((float)vipPcProfit / totalProfit * 100) };
            PSPartOfPieChart.Values = new ChartValues<int> { (int)((float)psProfit / totalProfit * 100) };
        }

        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChoosingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _startDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
            UpdatingPieChart(ProfitPerType());
        }

        private void ChoosingFinalDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _finalDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
            UpdatingPieChart(ProfitPerType());
        }
    }
}

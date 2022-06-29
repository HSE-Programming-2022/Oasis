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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для AdminStatisticsPage.xaml
    /// </summary>
    public partial class AdminStatisticsPage : Page
    {
        DateTime _startDate;
        DateTime _finalDate;

        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 5,
                offsetY: 5);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public AdminStatisticsPage()
        {
            _finalDate = DateTime.Now;
            _startDate =  _finalDate.Subtract(new TimeSpan(30, 0, 0, 0)); // по умолчанию будет выводится статистика за последние 30 дней

            InitializeComponent();

            UpdatingPieChart(ProfitPerType());
            UpdatingCartesianChart(AverageTimeOfUsingPerType());
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

        //Фун-ции для пайчарта

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
                foreach (var res in reservations)
                {
                    foreach (var hall in halls)
                    {
                        if (hall.Id == res.Seat.HallId)
                        {
                            profitPerType[hall.Type] += (int)res.Price;
                            break;
                        }
                    }
                    
                }
                //foreach (var hall in halls)
                //{
                //    var sortedReservations = reservations
                //    .Where(r => r.Seat.HallId == context.Halls.Where(h => h.Type == hall.Type).ToList()[0].Id)
                //    .ToList();
                //    foreach (var res in sortedReservations)
                //    {
                //        profitPerType[hall.Type] += (int)res.Price;
                //    }
                //}
            }
            return profitPerType;    
        }

        private void UpdatingPieChart(Dictionary<string, int> profitPerType)
        {
            int pcProfit = (int)profitPerType["PC"];
            int vipPcProfit = (int)profitPerType["VIP PC"];
            int psProfit = (int)profitPerType["PS"];
            PCPartOfPieChart.Values = new ChartValues<double> { (int)profitPerType["PC"] };
            VIP_PCPartOfPieChart.Values = new ChartValues<double> { (int)profitPerType["VIP PC"] };
            PSPartOfPieChart.Values = new ChartValues<double> { (int)profitPerType["PS"] };


        }
       
        // Фун-ции для графика

        private Dictionary<string, float> FormingEmptyAverageTimeOfUsingPerTypeDictionary()
        {
            Dictionary<string, float> emptyAverageTimeOfUsingPerTypeDictionary = new Dictionary<string, float>();
            using (Context context = new Context())
            {
                List<Hall> halls = context.Halls.ToList();
                foreach (var hall in halls)
                {
                    if (!(emptyAverageTimeOfUsingPerTypeDictionary.ContainsKey($"{hall.Type}")))
                    {
                        emptyAverageTimeOfUsingPerTypeDictionary.Add(hall.Type, 0);
                    }
                }
                return emptyAverageTimeOfUsingPerTypeDictionary;
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
                    if (sortedReservations.Count != 0)
                    {
                        foreach (var res in sortedReservations)
                        {
                            averageTimeOfUsingPerType[hall.Type] += (int)res.Hours;
                            amountOfReses += 1;
                        }
                        averageTimeOfUsingPerType[hall.Type] = (float)averageTimeOfUsingPerType[hall.Type] / amountOfReses;
                    }
                    else
                    {
                        averageTimeOfUsingPerType[hall.Type] = 0;
                    }
                }
                return averageTimeOfUsingPerType;
            }
        }

        private void UpdatingCartesianChart(Dictionary<string, float> averageTimeOfUsingPerType)
        {
            List<double> listOfValues = new List<double>()
            {
                //double.Parse("0"),
                Math.Round(averageTimeOfUsingPerType["VIP PC"], 2),
                Math.Round(averageTimeOfUsingPerType["PC"], 2),
                Math.Round(averageTimeOfUsingPerType["PS"], 2)
            };
            StatisticsGraph.Values = listOfValues.AsChartValues();
        }

        private bool CheckingDataExistens()
        {
            if (FormingListOfValidReses().Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void PieChart_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChoosingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _startDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
            if (_startDate.CompareTo(_finalDate) <= 0)
            {
                if (CheckingDataExistens())
                {
                    UpdatingPieChart(ProfitPerType());
                    UpdatingCartesianChart(AverageTimeOfUsingPerType());
                }
                else
                {
                    notifier.ShowWarning("Нет данных!");
                }
            }
            else
            {
                notifier.ShowWarning("Некорректный интервал!");
            }
        }

        private void ChoosingFinalDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _finalDate = DateTime.Parse(ChoosingFinalDatePicker.SelectedDate.ToString());
            if (_finalDate.CompareTo(_startDate) >= 0)
            {
                if (CheckingDataExistens())
                {
                    UpdatingPieChart(ProfitPerType());
                    UpdatingCartesianChart(AverageTimeOfUsingPerType());
                }
                else
                {
                    notifier.ShowWarning("Нет данных!");
                }
            }
            else
            {
                notifier.ShowWarning("Некорректный интервал!");
            }

        }
    }
}

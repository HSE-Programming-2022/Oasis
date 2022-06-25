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
using Oasis.Core;
using Oasis.Core.Models;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для HistoryOfUserReservations.xaml
    /// </summary>
    public partial class HistoryOfUserReservations : Window
    {
        public User CurrentUser { get; set; }

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

        public HistoryOfUserReservations(User user)
        {

            InitializeComponent();
            using (Context _context = new Context())
            {
                foreach (var item in _context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Email == user.Email)
                        {
                            CurrentUser = item as User;
                        }
                    }
                }
            }
            BalanceUserHistoryButton.Content = $"{CurrentUser.Balance} р.";
            FillGrid();
        }

        private void FillGrid()
        {
            string DBConnectionString = @"Data Source=vm-as35.staff.corp.local;Initial Catalog=OasisDB;User ID=student;Password=sql2020;Integrated Security=False";
            DataTable FullTable = new DataTable("UserReservationInfo");
            using (SqlConnection DBConnection = new SqlConnection(DBConnectionString))
            {
                try
                {
                    DBConnection.Open();
                    string GetFullInfoString = "SELECT Reservations.Id, Reservations.SeatId, Halls.Name, Reservations.StartTime, Reservations.Hours, Reservations.Price FROM Reservations JOIN Seats on Reservations.SeatId = Seats.Id JOIN Halls ON Seats.HallId = Halls.Id WHERE UserId = " + CurrentUser.Id;
                    SqlCommand GetFullInfoCommand = new SqlCommand(GetFullInfoString, DBConnection);
                    GetFullInfoCommand.ExecuteNonQuery();
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(GetFullInfoCommand);
                    DataAdapter.Fill(FullTable);
                    HistoryOfUserReservationsDataGrid.ItemsSource = FullTable.DefaultView;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    notifier.ShowWarning("На стороне сервера возникла ошибка");
                }
            }
        }

        private void DeleteReservationButton_Click(object sender, RoutedEventArgs e)
        {
            int ReservationId = Convert.ToInt32(((Button)sender).Tag);
            bool ReservationDeleted = false;
            using (Context _context = new Context())
            {
                foreach (var item in _context.Reservations)
                {
                    if (item.Id == ReservationId)
                    {
                        if (item.StartTime > DateTime.Now)
                        {
                            float ReservationPrice = (float)item.Price;
                            CurrentUser.Balance += ReservationPrice;
                            _context.People.Attach(CurrentUser);
                            _context.Entry(CurrentUser).Property(x => x.Balance).IsModified = true;
                            _context.Reservations.Remove(item);
                            ReservationDeleted = true;
                            notifier.ShowSuccess("Успешно отменено, деньги возвращены на ваш баланс");
                            break;
                        } 
                        else
                        {
                            notifier.ShowWarning("Отменить можно только не начавшуюся резервацию");
                        }
                    }
                }
                _context.SaveChanges();
            }
            if (ReservationDeleted)
            {
                HistoryOfUserReservations taskWindow = new HistoryOfUserReservations(CurrentUser);
                taskWindow.Show();
                Close();
            }
        }

        private void LogOutFromUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow taskWindow = new MainWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
        }

        private void MakeNewOrderUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            UserChoosingTypeofActivity taskWindow = new UserChoosingTypeofActivity(CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OpenBigButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                
            }
        }

        private void RemoveInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ReservationDate_Initialized(object sender, EventArgs e)
        {

        }

        private void ReservationStartTime_Initialized(object sender, EventArgs e)
        {

        }

        private void ReservationEndTime_Initialized(object sender, EventArgs e)
        {

        }

        private void TypeOfActivity_Initialized(object sender, EventArgs e)
        {

        }

        private void TypeOfHall_Initialized(object sender, EventArgs e)
        {

        }

        private void BalanceUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            TopUpBalance taskWindow = new TopUpBalance(CurrentUser, BalanceUserHistoryButton);

            taskWindow.Owner = this.Owner;
            taskWindow.ShowDialog();
        }

        private void ProfileInUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow taskWindow = new UserProfileWindow(CurrentUser);
            taskWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void HistoryOfUserReservationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

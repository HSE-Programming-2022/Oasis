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
using Oasis.Core;
using Oasis.Core.Models;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Data.SqlClient;
using System.Data;

namespace Oasis.Design
{
    public partial class AdminUsersPage : Page
    {

        private List<Person> AllPeople { get; set; }

        public AdminUsersPage()
        {
            InitializeComponent();
            DataGridBinding();
        }

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

        private void DataGridBinding()
        {

            SqlConnection DBConnection = new SqlConnection(@"Data Source=vm-as35.staff.corp.local;Initial Catalog=OasisDB;User ID=student;Password=sql2020;Integrated Security=False");

            DBConnection.Open();
            string cmd = "SELECT Login, Name, Surname, Balance FROM People WHERE Discriminator = 'User'"; // Из какой таблицы нужен вывод 
            SqlCommand createCommand = new SqlCommand(cmd, DBConnection);
            createCommand.ExecuteNonQuery();

            SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);
            DataTable dt = new DataTable("People"); // В скобках указываем название таблицы
            dataAdp.Fill(dt);
            UsersDataGrid.ItemsSource = dt.DefaultView; // Сам вывод 
            DBConnection.Close();
        }

        private void ChangeBalanceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            //UserBookingWindow window = new UserBookingWindow("All");
            //window.ShowDialog();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            //HistoryOfUserReservations window = new HistoryOfUserReservations(UsersListBox.SelectedItem as User);
            //window.ShowDialog();
        }

        private void SearchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SqlConnection DBConnection = new SqlConnection(@"Data Source=vm-as35.staff.corp.local;Initial Catalog=OasisDB;User ID=student;Password=sql2020;Integrated Security=False");

            DBConnection.Open();
            string cmd = $"SELECT Login, Name, Surname, Balance FROM People WHERE Discriminator = 'User'"; // Из какой таблицы нужен вывод 
            SqlCommand createCommand = new SqlCommand(cmd, DBConnection);
            createCommand.ExecuteNonQuery();
            SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);
            DataTable dt = new DataTable("People"); // В скобках указываем название таблицы
            dataAdp.Fill(dt);
            DataView SearchView = new DataView(dt);
            SearchView.RowFilter = string.Format("Login LIKE '%{0}%'", SearchingTextBox.Text);
            UsersDataGrid.ItemsSource = SearchView;
            DBConnection.Close();
        }
    }
}

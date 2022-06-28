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

        private User GetUserByLogin(string Login)
        {
            User SelectedUser = null;
            using (Context _context = new Context())
            {
                foreach (var item in _context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Login == Login)
                        {
                            SelectedUser = item as User;
                            break;
                        }
                    }
                }
            }
            return SelectedUser;
        }

        public AdminUsersPage()
        {
            InitializeComponent();
            DataGridBinding();
        }

        private void DataGridBinding()
        {
            string DBConnectionString = @"Data Source=vm-as35.staff.corp.local;Initial Catalog=OasisDB;User ID=student;Password=sql2020;Integrated Security=False";
            DataTable dt = new DataTable("People");
            using (SqlConnection DBConnection = new SqlConnection(DBConnectionString))
            {
                try
                {
                    DBConnection.Open();
                    string cmd = "SELECT Login, Name, Surname, Balance FROM People WHERE Discriminator = 'User'";
                    SqlCommand createCommand = new SqlCommand(cmd, DBConnection);
                    createCommand.ExecuteNonQuery();
                    SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);
                    dataAdp.Fill(dt);
                    UsersDataGrid.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    notifier.ShowWarning("На стороне сервера возникла ошибка");
                }
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedUserLogin = ((Button)sender).Tag.ToString();
            User SelectedUser = GetUserByLogin(SelectedUserLogin);
            if (SelectedUser == null)
            {
                notifier.ShowWarning("Пользователь не найден");
            }
            else
            {
                UserProfileWindow window = new UserProfileWindow(SelectedUser);
                window.ShowDialog();
            }
        }

        private void ChangeBalanceButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedUserLogin = ((Button)sender).Tag.ToString();
            Button NewButton = new Button();
            User SelectedUser = GetUserByLogin(SelectedUserLogin);
            if (SelectedUser == null)
            {
                notifier.ShowWarning("Пользователь не найден");
            }
            else
            {
                TopUpBalance window = new TopUpBalance(SelectedUser, NewButton);
                window.ShowDialog();
            }
        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedUserLogin = ((Button)sender).Tag.ToString();
            User SelectedUser = GetUserByLogin(SelectedUserLogin);
            if (SelectedUser == null)
            {
                notifier.ShowWarning("Пользователь не найден");
            }
            else
            {
                UserBookingWindow window = new UserBookingWindow("All", SelectedUser);
                window.ShowDialog();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedUserLogin = ((Button)sender).Tag.ToString();
            User SelectedUser = GetUserByLogin(SelectedUserLogin);
            if (SelectedUser == null)
            {
                notifier.ShowWarning("Пользователь не найден");
            }
            else
            {
                HistoryOfUserReservations window = new HistoryOfUserReservations(SelectedUser, true);
                window.ShowDialog();
            }
        }

        private void SearchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string DBConnectionString = @"Data Source=vm-as35.staff.corp.local;Initial Catalog=OasisDB;User ID=student;Password=sql2020;Integrated Security=False";
            DataTable dt = new DataTable("People");
            using (SqlConnection DBConnection = new SqlConnection(DBConnectionString))
            {
                try
                {
                    DBConnection.Open();
                    string cmd = "SELECT Login, Name, Surname, Balance FROM People WHERE Discriminator = 'User'";
                    SqlCommand createCommand = new SqlCommand(cmd, DBConnection);
                    createCommand.ExecuteNonQuery();
                    SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);
                    dataAdp.Fill(dt);
                    DataView SearchView = new DataView(dt);
                    SearchView.RowFilter = string.Format("Login LIKE '%{0}%'", SearchingTextBox.Text);
                    UsersDataGrid.ItemsSource = SearchView;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    notifier.ShowWarning("На стороне сервера возникла ошибка");
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchingTextBox.Text = "";
            DataGridBinding();
        }
    }
}

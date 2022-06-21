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

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для HistoryOfUserReservations.xaml
    /// </summary>
    public partial class HistoryOfUserReservations : Window
    {
        public User CurrentUser { get; set; }

        public HistoryOfUserReservations(User user)
        {
            InitializeComponent();
            using (Context _context = new Context())
            {
                foreach (var item in _context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Login == user.Login)
                        {
                            CurrentUser = item as User;
                        }
                    }
                }
            }
            BalanceUserHistoryButton.Content = $"{CurrentUser.Balance} р.";
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

        private void LogOutFromUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow taskWindow = new MainWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
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

        private void MakeNewOrderUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            UserChoosingTypeofActivity taskWindow = new UserChoosingTypeofActivity(CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
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
    }
}

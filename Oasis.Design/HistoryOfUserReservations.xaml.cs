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

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для HistoryOfUserReservations.xaml
    /// </summary>
    public partial class HistoryOfUserReservations : Window
    {
        public HistoryOfUserReservations()
        {
            InitializeComponent();
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
            UserChoosingTypeofActivity taskWindow = new UserChoosingTypeofActivity();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BalanceUserHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            TopUpBalance taskWindow = new TopUpBalance();

            taskWindow.Owner = this.Owner;
            taskWindow.ShowDialog();
        }
    }
}

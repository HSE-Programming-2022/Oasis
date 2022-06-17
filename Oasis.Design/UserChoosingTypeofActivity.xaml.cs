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
    /// Логика взаимодействия для UserChoosingTypeofActivity.xaml
    /// </summary>
    public partial class UserChoosingTypeofActivity : Window
    {
        public UserChoosingTypeofActivity()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void RemoveInUserChoosingTypeofActivityButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButton_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow taskWindow = new MainWindow();
            taskWindow.Show();
            Close();
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            TopUpBalance taskWindow = new TopUpBalance();
            taskWindow.ShowDialog();        
        }

        private void StatisticsInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryOfUserReservations taskWindow = new HistoryOfUserReservations();
            taskWindow.Show();
            Close();
        }

        private void ChooseTypeOfActivityPSButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("PS");
            taskWindow.ShowDialog();
        }

        private void ChooseTypeOfActivityVIPButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("PC VIP");
            taskWindow.ShowDialog();
        }

        private void ChooseTypeOfActivityPCButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("PC");
            taskWindow.ShowDialog();
        }

        private void ProfileInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow taskWindow = new UserProfileWindow();
            taskWindow.ShowDialog();
        }
    }
}

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
    /// Логика взаимодействия для UserChoosingTypeofActivity.xaml
    /// </summary>
    public partial class UserChoosingTypeofActivity : Window
    {
        public User CurrentUser { get; set; }

        public UserChoosingTypeofActivity(User user)
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
            BalanceButton.Content = $"{CurrentUser.Balance} р.";
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
            TopUpBalance taskWindow = new TopUpBalance(CurrentUser, BalanceButton);
            taskWindow.ShowDialog();
        }

        private void StatisticsInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryOfUserReservations taskWindow = new HistoryOfUserReservations(CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void ChooseTypeOfActivityPSButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("PS", CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void ChooseTypeOfActivityVIPButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("VIP PC", CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void ChooseTypeOfActivityPCButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow("PC", CurrentUser);
            taskWindow.Show();
            Close();
        }

        private void ProfileInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow taskWindow = new UserProfileWindow(CurrentUser);
            taskWindow.ShowDialog();
        }
    }
}

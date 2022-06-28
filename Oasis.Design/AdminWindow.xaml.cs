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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        bool IfMaximized = false;

        public AdminWindow()
        {
            InitializeComponent();
            Admin.Content = new AdminDashboardPage();
            DashboardStickButton.Visibility = Visibility.Visible;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void GoToDashboardPageButton_Click(object sender, RoutedEventArgs e)
        {
            Admin.Content = new AdminDashboardPage();
            DashboardStickButton.Visibility = Visibility.Visible;
            UsersStickButton.Visibility = Visibility.Hidden;
            StatisticksStickButton.Visibility = Visibility.Hidden;
        }

        private void GoToStatisticsPageButton_Click(object sender, RoutedEventArgs e)
        {
            Admin.Content = new AdminStatisticsPage();
            DashboardStickButton.Visibility = Visibility.Hidden;
            UsersStickButton.Visibility = Visibility.Hidden;
            StatisticksStickButton.Visibility = Visibility.Visible;
        }

        private void GoToUsersPageButton_Click(object sender, RoutedEventArgs e)
        {
            Admin.Content = new AdminUsersPage();
            DashboardStickButton.Visibility = Visibility.Hidden;
            UsersStickButton.Visibility = Visibility.Visible;
            StatisticksStickButton.Visibility = Visibility.Hidden;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RemoveInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OpenBigButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IfMaximized)
            {
                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;
                IfMaximized = true;
            }
            else
            {
                IfMaximized = false;
                this.Height = 550;
                this.Width = 900;
                this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
                this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
            }
        }

        private void LogOutAdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow taskWindow = new MainWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
        }
    }
}

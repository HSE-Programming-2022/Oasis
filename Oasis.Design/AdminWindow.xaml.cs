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
        public AdminWindow()
        {
            InitializeComponent();
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



        }

        private void GoToStatisticsPageButton_Click(object sender, RoutedEventArgs e)
        {
            Admin.Content = new AdminStatisticsPage();
        }

        private void GoToUsersPageButton_Click(object sender, RoutedEventArgs e)
        {
            Admin.Content = new AdminUsersPage();
        }
    }
}

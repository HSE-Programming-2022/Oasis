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
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private void RemoveInUserChoosingTypeofActivityButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
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

        private void ChooseTypeOfActivity1Button_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow taskWindow = new UserBookingWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();

        }
    }
}

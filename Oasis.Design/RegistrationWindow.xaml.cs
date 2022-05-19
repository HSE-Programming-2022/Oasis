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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }



        private void ExitInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RemoveInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitInPasswordrecoveryButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

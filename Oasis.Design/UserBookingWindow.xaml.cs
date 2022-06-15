using Oasis.Core;
using Oasis.Core.Models;
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
    /// Логика взаимодействия для UserBookingWindow.xaml
    /// </summary>
    public partial class UserBookingWindow : Window
    {
        string _selectedHall;
        int _numberOfPeople;
        DateTime _selectedDate;
        public UserBookingWindow()
        {
            InitializeComponent();
        }

        private void CheckingFreeTimeSlots()
        {
            using (Context context = new Context())
            {
                List<Reservation> resrvation = new List<Reservation>();
                foreach (var item in context.Reservations.ToList())
                {
                    resrvation.Add(item);
                }

            }

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
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

       

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBoxHall_Selected(object sender, RoutedEventArgs e)
        {
            _selectedHall = ComboBoxHall.SelectedItem.ToString();
        }

        private void ComboBoxNumberOfPeople_Selected(object sender, RoutedEventArgs e)
        {
            _numberOfPeople = int.Parse(ComboBoxNumberOfPeople.SelectedItem.ToString());
        }

        private void ChoosingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDate = DateTime.Parse(ChoosingDatePicker.SelectedDate.ToString());
        }
    }
}

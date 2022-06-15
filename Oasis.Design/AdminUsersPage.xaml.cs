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

namespace Oasis.Design
{
    public partial class AdminUsersPage : Page
    {
        private Context _context { get; set; }

        private List<Person> AllPeople { get; set; }

        public AdminUsersPage()
        {
            InitializeComponent();
            _context = new Context();
            AllPeople = _context.People.ToList();
            for(int i = 0; i < AllPeople.Count; i++)
            {
                if (AllPeople[i] is Admin)
                    AllPeople.Remove(AllPeople[i]);
            }
            UsersListBox.ItemsSource = AllPeople;
        }

        private void UserLogin_Initialized(object sender, EventArgs e)
        {
            TextBlock login = sender as TextBlock;
            User user = login.DataContext as User;
            login.Text = (user as User).Login;
        }

        private void UserName_Initialized(object sender, EventArgs e)
        {
            TextBlock name = sender as TextBlock;
            User user = name.DataContext as User;
            name.Text = (user as User).Name;
        }

        private void UserSurename_Initialized(object sender, EventArgs e)
        {
            TextBlock surename = sender as TextBlock;
            User user = surename.DataContext as User;
            surename.Text = (user as User).Surname;
        }

        private void Balance_Initialized(object sender, EventArgs e)
        {
            TextBlock balance = sender as TextBlock;
            User user = balance.DataContext as User;
            balance.Text = (user as User).Balance.ToString();
        }

        private void ChangeBalanceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            UserBookingWindow window = new UserBookingWindow();
            window.ShowDialog();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            for(int i = 0; i <AllPeople.Count; i++)
            {
                if((AllPeople[i] as User).Login.ToLower().Contains(SearchingTextBox.Text.ToLower()))
                {
                    Person found = AllPeople[i];
                    AllPeople.RemoveAt(i);
                    AllPeople.Insert(0, found);
                    count++;
                }
            }
            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = AllPeople;
            MessageBox.Show($"Найдено {count} пользователей");
        }
    }
}

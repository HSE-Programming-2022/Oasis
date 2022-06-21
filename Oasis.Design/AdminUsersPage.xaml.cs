﻿using System;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Oasis.Design
{
    public partial class AdminUsersPage : Page
    {

        private List<Person> AllPeople { get; set; }

        public AdminUsersPage()
        {
            InitializeComponent();
            using (Context _context = new Context())
            {
                AllPeople = _context.People.ToList();
                for (int i = 0; i < AllPeople.Count; i++)
                {
                    if (AllPeople[i] is Admin)
                        AllPeople.Remove(AllPeople[i]);
                }
                UsersListBox.ItemsSource = AllPeople;
            }
        }

        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 100,
                offsetY: 5);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

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
            UserBookingWindow window = new UserBookingWindow("All");
            window.ShowDialog();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            //HistoryOfUserReservations window = new HistoryOfUserReservations(UsersListBox.SelectedItem as User);
            //window.ShowDialog();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            for(int i = 0; i <AllPeople.Count; i++)
            {
                if((AllPeople[i] as User).Login.ToLower().Contains(SearchingTextBox.Text.ToLower()) && SearchingTextBox.Text != "")
                {
                    Person found = AllPeople[i];
                    AllPeople.RemoveAt(i);
                    AllPeople.Insert(0, found);
                    count++;
                }
            }
            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = AllPeople;
            if (count > 0)
            {
                notifier.ShowSuccess($"Найдено {count} пользователей");
            }
            else
            {
                notifier.ShowWarning($"Найдено {count} пользователей");
            }

        }
    }
}

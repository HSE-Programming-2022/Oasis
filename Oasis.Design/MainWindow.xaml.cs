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
using System.Data.SqlClient;
using Oasis.Core;
using Oasis.Core.Models;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Oasis.Design
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //InitialDb();
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
        //private void InitialPerson()
        //{
        //    using (Context context = new Context())
        //    {
        //        Admin admin = new Admin("admin", "root");
        //        User user1 = new User("s1mple", "qwerty123", "Oleksandr", "Kostyliev");

        //        context.People.Add(admin);
        //        context.People.Add(user1);
        //        context.SaveChanges();

        //        MessageBox.Show("Saved");
        //    }
        //}
        private void InitialDb()
        {
            using (Context context = new Context())
            {
                Admin admin = new Admin("admin", "root");
                User user1 = new User("s1mple", "qwerty123", "Oleksandr", "Kostyliev", "s1mple@gmail.com", "+79268971238");

                context.People.Add(admin);
                context.People.Add(user1);
                Hall hall = new Hall
                {
                    Name = "Bootcamp1",
                    Type = "PC",
                    Price = 150
                };

                context.Halls.Add(hall);
                context.SaveChanges();
                List<Seat> seats = new List<Seat>
                {
                    new Seat { Hall = hall},
                    new Seat { Hall = hall},
                    new Seat { Hall = hall},
                    new Seat { Hall = hall},
                    new Seat { Hall = hall}
                };
                context.Seats.AddRange(seats);
                context.SaveChanges();
                Reservation res = new Reservation(user1, Convert.ToDateTime("19:00:00 18-05-2022"), 1, seats[0], 150);
                context.Reservations.Add(res);
                context.SaveChanges();
                MessageBox.Show("Saved");
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (SwitcherToggleButton.IsChecked == false)
            {
                using (Context _context = new Context())
                {
                    bool UserExists = false;
                    foreach (var item in _context.People)
                    {
                        if (item is User)
                        {
                            User CurrentUser = item as User;
                            if (CurrentUser.Login == LoginOrEmailTextBox.Text || CurrentUser.Email == LoginOrEmailTextBox.Text)
                            {
                                UserExists = true;
                                if (CurrentUser.Password == PasswordBox.Password)
                                {
                                    UserChoosingTypeofActivity taskWindow = new UserChoosingTypeofActivity(CurrentUser);
                                    taskWindow.Show();
                                    Close();
                                }
                                else
                                {
                                    notifier.ShowWarning("Неверный пароль!");
                                    break;
                                }
                            }
                        }
                    }
                    if (!UserExists)
                        notifier.ShowWarning("Пользователь не найден!");
                }
            }
            if (SwitcherToggleButton.IsChecked == true)
            {
                using (Context _context = new Context())
                {
                    bool AdminExists = false;
                    foreach (var item in _context.People)
                    {
                        if (item is Admin)
                        {
                            Admin CurrentAdmin = item as Admin;
                            if (CurrentAdmin.Login == LoginOrEmailTextBox.Text)
                            {
                                AdminExists = true;
                                if (CurrentAdmin.Password == PasswordBox.Password)
                                {
                                    AdminWindow taskWindow = new AdminWindow();
                                    taskWindow.Show();
                                    Close();
                                }
                                else
                                {
                                    notifier.ShowWarning("Неверный пароль!");
                                    break;
                                }
                            }
                        }
                    }
                    if (!AdminExists)
                        notifier.ShowWarning("Пользователь не найден!");
                }
            }
        }




    
            
            
           

        private void RigistrationButton_Click(object sender, RoutedEventArgs e)
        {
            UserRegisteringWindow taskWindow = new UserRegisteringWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        private void ForgetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            UserForgetPasswordWindow taskWindow = new UserForgetPasswordWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        

        private void LoginOrEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GoToWebSiteButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ez-katka.ru/");

        }

        private void GoToVkButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/ezyas");
        }

        private void SwitcherToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            RigistrationButton.Visibility = Visibility.Hidden;
            ForgetPasswordButton.Visibility = Visibility.Hidden;
        }

        private void SwitcherToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RigistrationButton.Visibility = Visibility.Visible;
            ForgetPasswordButton.Visibility = Visibility.Hidden;
        }
    }
}

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
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 5,
                offsetY: -23);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (Context _context = new Context())
            {
                bool PersonExists = false;
                foreach (var item in _context.People)
                {
                    if (SwitcherToggleButton.IsChecked == false)
                    {
                        if (item is User)
                        {
                            User CurrentUser = item as User;
                            if (CurrentUser.Login == LoginOrEmailTextBox.Text || CurrentUser.Email == LoginOrEmailTextBox.Text)
                            {
                                PersonExists = true;
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
                    else
                    {
                        if (item is Admin)
                        {
                            Admin CurrentAdmin = item as Admin;
                            if (CurrentAdmin.Login == LoginOrEmailTextBox.Text)
                            {
                                PersonExists = true;
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
                }
                if (!PersonExists)
                    notifier.ShowWarning("Пользователь не найден!");
            }
        }

        private void RigistrationButton_Click(object sender, RoutedEventArgs e)
        {
            UserRegisteringWindow taskWindow = new UserRegisteringWindow();
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
            taskWindow.Show();
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
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
            ForgetPasswordButton.Visibility = Visibility.Visible;
        }
    }
}

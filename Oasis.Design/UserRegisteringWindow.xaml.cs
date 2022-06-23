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
    /// <summary>
    /// Логика взаимодействия для UserRegisteringWindow.xaml
    /// </summary>
    public partial class UserRegisteringWindow : Window
    {
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

        public UserRegisteringWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool LoginExists = false;
            string NewUserLogin = LoginTextBox.Text;
            string NewUserPassword = PasswordBox.Password;
            string NewUserConfirmPassword = ConfirmPasswordBox.Password;
            if (NewUserLogin == "" || NewUserPassword == "" || NewUserConfirmPassword == "")
            {
               notifier.ShowWarning("Необходимо заполнить все поля");
            }
            else
            {
                using (Context _context = new Context())
                {
                    foreach (var item in _context.People)
                    {
                        if (item is User)
                        {
                            if (item.Login == NewUserLogin)
                            {
                                LoginExists = true;
                                notifier.ShowWarning("Данный логин уже занят");
                                break;
                            }
                        }
                    }
                    if (!LoginExists)
                    {
                        if (NewUserPassword != NewUserConfirmPassword)
                        {
                            notifier.ShowWarning("Пароли не совпадают");
                        }
                        else
                        {
                            RegistrationWindow taskWindow = new RegistrationWindow(NewUserLogin, NewUserPassword);
                            taskWindow.Show();
                            Close();
                        }
                    }
                }
            }
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

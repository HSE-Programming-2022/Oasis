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
    /// Логика взаимодействия для CreatingNewPassword.xaml
    /// </summary>
    public partial class CreatingNewPassword : Window
    {
        string UserEmail;

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

        public CreatingNewPassword(string _UserEmail)
        {
            InitializeComponent();
            UserEmail = _UserEmail;
        }

        private void ExitInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void RemoveInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitInPasswordrecoveryButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void CreateNewPaswordButton_Click(object sender, RoutedEventArgs e)
        {
            string NewUserPassword = PasswordBox.Password;
            string NewUserConfirmPassword = ConfirmPasswordBox.Password;
            if (NewUserPassword == "" || NewUserConfirmPassword == "")
            {
                notifier.ShowWarning("Необходимо заполнить все поля");
            }
            else
            {
                if (NewUserPassword != NewUserConfirmPassword)
                {
                    notifier.ShowWarning("Пароли не совпадают");
                }
                else
                {
                    using (Context _context = new Context())
                    {
                        foreach (var item in _context.People)
                        {
                            if (item is User)
                            {
                                User CurrentUser = item as User;
                                if (CurrentUser.Email == UserEmail)
                                {
                                    CurrentUser.Password = NewUserPassword;
                                    _context.People.Attach(CurrentUser);
                                    _context.Entry(CurrentUser).Property(x => x.Password).IsModified = true;
                                    break;
                                }
                            }
                        }
                        _context.SaveChanges();
                    }
                    MainWindow taskWindow = new MainWindow();

                    taskWindow.Owner = this.Owner;
                    taskWindow.Show();
                    Close();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

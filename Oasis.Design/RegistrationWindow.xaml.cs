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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Net;
using System.Net.Mail;

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
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 100,
                offsetY: 20);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
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
            this.WindowState = WindowState.Minimized;
        }

        private void ExitInPasswordrecoveryButton_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            int randomnumber = 770238;
            SendCodeButton.Visibility = Visibility.Hidden;
            CodeConfirmationPasswordBox.Visibility = Visibility.Visible;
            ConfirmButton.Visibility = Visibility.Visible;

            if (EmailConfirmationTextBox.Text != "")
            {
                try
                {
                    SmtpClient Client = new SmtpClient();
                    Client.Credentials = new NetworkCredential("paveldzhankaraev@mail.ru", "RV6tbnrHQgCtovAkkfD8");
                    Client.Host = "smtp.mail.ru";
                    Client.Port = 587;
                    Client.EnableSsl = true;
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("paveldzhankaraev@mail.ru");
                    mail.To.Add(new MailAddress(EmailConfirmationTextBox.Text));
                    mail.Subject = "Восстановление пароля";
                    mail.IsBodyHtml = true;
                    mail.Body = "Ваш код восстановления: " + randomnumber;
                    Client.Send(mail);
                    notifier.ShowSuccess("Код для восстановления пароля успешно отправлен");
                }
                catch { }
            }
                

        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow taskWindow = new MainWindow();

            taskWindow.Owner = this.Owner;
            taskWindow.Show();
            Close();
           

        }
    }
}

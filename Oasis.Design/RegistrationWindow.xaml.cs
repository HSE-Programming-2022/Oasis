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
using MailKit.Net.Smtp;
using MimeKit;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        string NewUserLogin;
        string NewUserPassword;
        string NewUserEmail;
        string VerificationCode;

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

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        public RegistrationWindow(string _NewUserLogin, string _NewUserPassword)
        {
            InitializeComponent();
            NewUserLogin = _NewUserLogin;
            NewUserPassword = _NewUserPassword;
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
            this.WindowState = WindowState.Minimized;
        }

        private void ExitInPasswordrecoveryButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserEmail = EmailConfirmationTextBox.Text;
            bool EmailExists = false;
            if (NewUserEmail == "")
            {
                notifier.ShowWarning("Введите почту");
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
                            if (CurrentUser.Email == NewUserEmail)
                            {
                                EmailExists = true;
                                notifier.ShowWarning("Пользователь с данной почтой уже существует");
                                break;
                            }
                        }
                    }

                    if (!EmailExists)
                    {
                        SendCodeButton.Visibility = Visibility.Hidden;
                        CodeConfirmationTextBox.Visibility = Visibility.Visible;
                        ConfirmButton.Visibility = Visibility.Visible;

                        using (var smtp = new SmtpClient())
                        {
                            smtp.Connect("smtp.yandex.ru", 465, true);
                            smtp.Authenticate("oasis.computer.club@yandex.ru", "brbhekcpgskbzgfo");

                            VerificationCode = new Random().Next(1000, 9999).ToString("D4");
                            var BodyBldr = new BodyBuilder();
                            BodyBldr.TextBody = "Ваш код: " + VerificationCode;

                            var Message = new MimeMessage();
                            Message.Subject = "Подтверждение почты для регистрации";
                            Message.Body = BodyBldr.ToMessageBody();
                            Message.To.Add(MailboxAddress.Parse(NewUserEmail));
                            Message.From.Add(new MailboxAddress("Компьютерный клуб Oasis", "oasis.computer.club@yandex.ru"));

                            smtp.Send(Message);
                        }
                    }
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {            
            if (CodeConfirmationTextBox.Text == VerificationCode)
            {
                using (Context _context = new Context())
                {
                    User NewUser = new User(NewUserLogin, NewUserPassword, NewUserEmail);
                    _context.People.Add(NewUser);
                    _context.SaveChanges();
                }
                notifier.ShowWarning("Регистрация прошла успешно!");

                MainWindow taskWindow = new MainWindow();

                taskWindow.Owner = this.Owner;
                taskWindow.Show();
                Close();
            }
            else
            {
                notifier.ShowWarning("Неверный код");
            }
        }
    }
}

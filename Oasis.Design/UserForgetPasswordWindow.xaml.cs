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
    /// Логика взаимодействия для UserForgetPasswordWindow.xaml
    /// </summary>
    public partial class UserForgetPasswordWindow : Window
    {
        private string UserEmail;

        private string VerificationCode;

        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 100,
                offsetY: 30);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public UserForgetPasswordWindow()
        {
            InitializeComponent();
        }
        private bool IsEmailValid(string Email)
        {

            if (Email.EndsWith("."))
            {
                return false;
            }
            try
            {
                var ValidEmail = new System.Net.Mail.MailAddress(Email);
                return ValidEmail.Address == Email;
            }
            catch
            {
                return false;
            }
        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            UserEmail = EmailConfirmationTextBox.Text;
            bool EmailExists = false;
            if (UserEmail == "")
            {
                notifier.ShowWarning("Введите почту");
            }
            else
            {
                if (IsEmailValid(UserEmail))
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
                                    EmailExists = true;
                                    break;
                                }
                            }
                        }
                        if (EmailExists)
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
                                Message.Subject = "Восстановление забытого пароля";
                                Message.Body = BodyBldr.ToMessageBody();
                                Message.To.Add(MailboxAddress.Parse(UserEmail));
                                Message.From.Add(new MailboxAddress("Компьютерный клуб Oasis", "oasis.computer.club@yandex.ru"));

                                smtp.Send(Message);
                            }
                        }
                        else
                        {
                            notifier.ShowWarning("Пользователь с данной почтой отсутствует");
                        }
                        EmailConfirmationTextBox.IsReadOnly = true;
                    }
                }
                else
                {
                    notifier.ShowWarning("Введенная почта не соответствует формату");
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeConfirmationTextBox.Text == VerificationCode)
            {
                CreatingNewPassword taskWindow = new CreatingNewPassword(UserEmail);
                taskWindow.Show();
                Close();
            }
            else
            {
                notifier.ShowWarning("Неверный код");
            }
        }

        private void ExitInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void RemoveInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}

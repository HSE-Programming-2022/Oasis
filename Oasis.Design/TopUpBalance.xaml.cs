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
using Oasis.Core;
using Oasis.Core.Models;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для TopUpBalance.xaml
    /// </summary>
    public partial class TopUpBalance : Window
    {
        private User CurrentUser { get; set; }

        private Button BalanceButtonNew;

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

        public TopUpBalance(User user, Button BalanceButtonOld)
        {
            InitializeComponent();
            using (Context _context = new Context())
            {
                foreach (var item in _context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Email == user.Email)
                        {
                            CurrentUser = item as User;
                        }
                    }
                }
            }
            CurrentUserBalance.Text = $"{CurrentUser.Balance} р.";
            BalanceButtonNew = BalanceButtonOld;
        }

        private void ExitFromTopUpBalaneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void TopUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(SumOfUserTopUp.Text) < 100)
                    notifier.ShowWarning("Нельзя пополнить баланс меньше чем на 100 рублей");
                else
                {
                    using (Context _context = new Context())
                    {
                        foreach (var item in _context.People)
                        {
                            if (item is User)
                            {
                                if ((item as User).Email == CurrentUser.Email)
                                {
                                    (item as User).Balance += int.Parse(SumOfUserTopUp.Text);
                                    BalanceButtonNew.Content = $"{(item as User).Balance} р.";
                                }
                            }
                        }
                        _context.SaveChanges();
                    }
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                notifier.ShowWarning("Неправильный формат ввода");
            }
        }
    }
}

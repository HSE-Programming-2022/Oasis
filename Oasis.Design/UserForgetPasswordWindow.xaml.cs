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

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для UserForgetPasswordWindow.xaml
    /// </summary>
    public partial class UserForgetPasswordWindow : Window
    {
        public UserForgetPasswordWindow()
        {
            InitializeComponent();
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

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            CodeConfirmationTextBox.Visibility = Visibility.Visible;
            SendCodeButton.Visibility = Visibility.Hidden;
            ConfirmButton.Visibility = Visibility.Visible;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void RemoveInPasswordrecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}

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

namespace Oasis.Design
{
    /// <summary>
    /// Логика взаимодействия для UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        public UserProfileWindow(User user)
        {
            InitializeComponent();
            EmailText.Text = user.Email;
            PhoneText.Text = user.Phone;
            LoginText.Text = user.Login;
            NameText.Text = user.Name;
            SurenameText.Text = user.Surname;

            NewEmailTextBox.Text = user.Email;
            NewPhoneTextBox.Text = user.Phone;
            NewLoginTextBox.Text = user.Login;
            NewNameTextBox.Text = user.Name;
            NewSurenameTextBox.Text = user.Surname;


        }

        private void ExitFromTopUpBalaneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        

        private void ChangeProfileDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            EmailText.Visibility = Visibility.Hidden;
            PhoneText.Visibility = Visibility.Hidden;
            LoginText.Visibility = Visibility.Hidden;
            NameText.Visibility = Visibility.Hidden;
            SurenameText.Visibility = Visibility.Hidden;

            NewEmailTextBox.Visibility = Visibility.Visible;
            NewPhoneTextBox.Visibility = Visibility.Visible;
            NewLoginTextBox.Visibility = Visibility.Visible;
            NewNameTextBox.Visibility = Visibility.Visible;
            NewSurenameTextBox.Visibility = Visibility.Visible;

            ChangeProfileDetailsButton.Visibility = Visibility.Hidden;
            SaveChangefProfileDetailsButton.Visibility = Visibility.Visible;


            

        }

        private void SaveChangefProfileDetailsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

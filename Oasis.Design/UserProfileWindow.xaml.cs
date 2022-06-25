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
        User CurrentUser;

        public UserProfileWindow(User user)
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
            EmailText.Text = CurrentUser.Email;
            PhoneText.Text = CurrentUser.Phone;
            LoginText.Text = CurrentUser.Login;
            NameText.Text = CurrentUser.Name;
            SurenameText.Text = CurrentUser.Surname;

            NewPhoneTextBox.Text = CurrentUser.Phone;
            NewLoginTextBox.Text = CurrentUser.Login;
            NewNameTextBox.Text = CurrentUser.Name;
            NewSurenameTextBox.Text = CurrentUser.Surname;


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
            PhoneText.Visibility = Visibility.Hidden;
            LoginText.Visibility = Visibility.Hidden;
            NameText.Visibility = Visibility.Hidden;
            SurenameText.Visibility = Visibility.Hidden;

            NewPhoneTextBox.Visibility = Visibility.Visible;
            NewLoginTextBox.Visibility = Visibility.Visible;
            NewNameTextBox.Visibility = Visibility.Visible;
            NewSurenameTextBox.Visibility = Visibility.Visible;

            ChangeProfileDetailsButton.Visibility = Visibility.Hidden;
            SaveChangefProfileDetailsButton.Visibility = Visibility.Visible;
        }

        private void SaveChangefProfileDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            using (Context _context = new Context())
            {
                foreach (var item in _context.People)
                {
                    if (item is User)
                    {
                        if ((item as User).Email == CurrentUser.Email)
                        {
                            (item as User).Phone = NewPhoneTextBox.Text;
                            (item as User).Login = NewLoginTextBox.Text;
                            (item as User).Name = NewNameTextBox.Text;
                            (item as User).Surname = NewSurenameTextBox.Text;
                        }
                    }
                }
                _context.SaveChanges();
            }
            Close();
        }
    }
}

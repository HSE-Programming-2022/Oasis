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
    /// Логика взаимодействия для TopUpBalance.xaml
    /// </summary>
    public partial class TopUpBalance : Window
    {
        public User CurrentUser { get; set; }
        public Button main;

        public TopUpBalance( User user, Button win)
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
            main = win;
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

        private void TopUpButton_Click(object sender, RoutedEventArgs e)
        {
            using(Context _context = new Context())
            {
                foreach(var item in _context.People)
                {
                    if(item is User)
                    {
                        if((item as User).Email == CurrentUser.Email)
                        {
                            (item as User).Balance += int.Parse(SumOfUserTopUp.Text);
                            main.Content = $"{(item as User).Balance} р.";
                        }
                    }
                }
                _context.SaveChanges();
            }
            this.Close();
        }
    }
}

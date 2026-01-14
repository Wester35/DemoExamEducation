using Educat.Data;
using Educat.Models;
using System.Windows;
using System.Windows.Controls;

namespace Educat
{
    /// <summary>
    /// Interaction logic for Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;
            
            User user = DbHelper.Authorize(login, password);
            
            login = null;
            password = null;
            PasswordBox.Password = null;

            if (user != null) 
            {
                OpenMainWindow(user);
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMainWindow(null);
        }

        private void OpenMainWindow(User user)
        {
            MainWindow wnd = new MainWindow(user);
            wnd.Closed += (s, args) => this.Show();
            this.Hide();
            wnd.Show();
        }
    }
}

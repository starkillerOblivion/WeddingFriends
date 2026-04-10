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
using WeddingFriends.DB;

namespace WeddingFriends.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void btnReg_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new RegisterPage());

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string pass = pbPassword.Password;

            var user = DBClass.connect.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == pass && u.IsActive == true);
            if (user == null)
            {
                tbError.Text = "Неверный логин или пароль";
                return;
            }

            Session.CurrentUser = user;
            NavigationService?.Navigate(new CatalogPage());
        }
    }
}

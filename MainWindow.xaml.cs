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
using WeddingFriends.Pages;

namespace WeddingFriends
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Session.IsAuthenticated)
                MainFrame.Navigate(new CatalogPage());
            else
                MainFrame.Navigate(new LoginPage());
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CatalogPage());
        private void btnCart_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CartPage());
        private void btnProfile_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ProfilePage());
        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Session.IsAdmin)
                MainFrame.Navigate(new AdminPage());
            else
                MessageBox.Show("Доступ только для администратора");
        }
        private void btnCasting_Click(object sender, RoutedEventArgs e)
        {
            if (Session.IsCastingDirector || Session.IsAdmin)
                MainFrame.Navigate(new CastingDirectorPage());
            else
                MessageBox.Show("Доступ только для кастинг-директора");
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Session.Logout();
            MainFrame.Navigate(new LoginPage());
        }
    }
}

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
using WeddingFriends.Windows;

namespace WeddingFriends.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            if (!Session.IsAdmin) NavigationService?.GoBack();
            RefreshUsers();
            RefreshActors();
            RefreshRoles();
            RefreshScripts();
            RefreshLogs();
        }

        private void RefreshUsers() => lvUsers.ItemsSource = DBClass.connect.Users.ToList();
        private void RefreshActors() => lvActors.ItemsSource = DBClass.connect.Actors.ToList();
        private void RefreshRoles() => lvRoles.ItemsSource = DBClass.connect.ActorRoles.ToList();
        private void RefreshScripts() => lvScripts.ItemsSource = DBClass.connect.WeddingScripts.ToList();
        private void RefreshLogs() => lvLogs.ItemsSource = DBClass.connect.AdminLogs.OrderByDescending(l => l.ActionTime).ToList();

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            new EditUserWindow().ShowDialog();
            RefreshUsers();
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var user = DBClass.connect.Users.Find(id);
            new EditUserWindow(user).ShowDialog();
            RefreshUsers();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var user = DBClass.connect.Users.Find(id);
            if (MessageBox.Show($"Удалить {user.Login}? Все связанные данные будут удалены.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var orders = DBClass.connect.Orders.Where(o => o.Customers.UserID == id).ToList();
                foreach (var o in orders)
                {
                    var details = DBClass.connect.OrderDetails.Where(d => d.OrderID == o.OrderID).ToList();
                    foreach (var d in details)
                        DBClass.connect.OrderDetails.Remove(d);
                    DBClass.connect.Orders.Remove(o);
                }
                var customers = DBClass.connect.Customers.Where(c => c.UserID == id).ToList();
                foreach (var c in customers)
                    DBClass.connect.Customers.Remove(c);
                DBClass.connect.Users.Remove(user);
                DBClass.connect.SaveChanges();
                RefreshUsers();
            }
        }

        private void btnAddActor_Click(object sender, RoutedEventArgs e)
        {
            new EditActorWindow().ShowDialog();
            RefreshActors();
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            new EditRoleWindow().ShowDialog();
            RefreshRoles();
        }

        private void btnAddScript_Click(object sender, RoutedEventArgs e)
        {
            new EditScriptWindow().ShowDialog();
            RefreshScripts();
        }
    }
}

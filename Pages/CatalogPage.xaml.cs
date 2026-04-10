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
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private List<ActorViewModel> allActors;

        public CatalogPage()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        public bool IsAdminVisible => Session.IsAdmin;
        public bool IsCastingVisible => Session.IsCastingDirector;

        private void LoadData()
        {
            var actors = DBClass.connect.Actors.ToList();
            allActors = actors.Select(a => new ActorViewModel
            {
                ActorID = a.ActorID,
                FullName = a.FullName,
                Rating = Convert.ToDouble(a.Rating ?? 0),
                IsDrinkable = a.IsDrinkable ?? false,
                MainRole = string.Join(", ", a.ActorRoleMapping.Select(arm => arm.ActorRoles.RoleName))
            }).ToList();

            lvActors.ItemsSource = allActors;

            var roles = DBClass.connect.ActorRoles.Select(r => r.RoleName).Distinct().ToList();
            roles.Insert(0, "Все");
            cmbFilterRole.ItemsSource = roles;
            cmbFilterRole.SelectedIndex = 0;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = tbSearch.Text.Trim();
            if (search.Length > 0 && search != "Поиск по имени...")
            {
                var filtered = allActors.Where(a => a.FullName.ToLower().Contains(search.ToLower())).ToList();
                lvActors.ItemsSource = filtered;
            }
            else
                lvActors.ItemsSource = allActors;
        }

        private void cmbFilterRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = cmbFilterRole.SelectedItem as string;
            if (selected == null || selected == "Все")
                lvActors.ItemsSource = allActors;
            else
                lvActors.ItemsSource = allActors.Where(a => a.MainRole.Contains(selected)).ToList();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            NavigationService?.Navigate(new ActorDetailsPage(id));
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            CartHelper.AddToCart(id);
            MessageBox.Show("Актёр добавлен в корзину");
        }

        private void BtnEditActor_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsAdmin) return;
            int id = (int)((Button)sender).Tag;
            var actor = DBClass.connect.Actors.Find(id);
            new EditActorWindow(actor).ShowDialog();
            LoadData();
        }

        private void BtnDeleteActor_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsAdmin) return;
            int id = (int)((Button)sender).Tag;
            var actor = DBClass.connect.Actors.Find(id);
            if (MessageBox.Show($"Удалить актёра '{actor.FullName}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var details = DBClass.connect.OrderDetails.Where(od => od.ActorID == id).ToList();
                foreach (var d in details)
                    DBClass.connect.OrderDetails.Remove(d);
                var mappings = DBClass.connect.ActorRoleMapping.Where(arm => arm.ActorID == id).ToList();
                foreach (var m in mappings)
                    DBClass.connect.ActorRoleMapping.Remove(m);
                DBClass.connect.Actors.Remove(actor);
                DBClass.connect.SaveChanges();
                LoadData();
            }
        }

        private void BtnManageStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsCastingDirector) return;
            int id = (int)((Button)sender).Tag;
            var actor = DBClass.connect.Actors.Find(id);
            new ManageActorStatusWindow(actor).ShowDialog();
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsCastingDirector) return;
            int id = (int)((Button)sender).Tag;
            var actor = DBClass.connect.Actors.Find(id);
            new ActorStatisticsWindow(actor).ShowDialog();
        }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbSearch.Text == "Поиск по имени...")
                tbSearch.Text = "";
        }

        private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSearch.Text))
                tbSearch.Text = "Поиск по имени...";
        }
    }

    public class ActorViewModel
    {
        public int ActorID { get; set; }
        public string FullName { get; set; }
        public double Rating { get; set; }
        public bool IsDrinkable { get; set; }
        public string MainRole { get; set; }
    }
}

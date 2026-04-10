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
    /// Логика взаимодействия для CastingDirectorPage.xaml
    /// </summary>
    public partial class CastingDirectorPage : Page
    {
        public CastingDirectorPage()
        {
            InitializeComponent();
            if (!Session.IsCastingDirector && !Session.IsAdmin) NavigationService?.GoBack();
            cmbOrderStatus.ItemsSource = new[] { "Все", "Новый", "В обработке", "Завершён" };
            cmbOrderStatus.SelectedIndex = 0;
            RefreshOrders();
            RefreshOrdersForAssignment();
            RefreshBonuses();
        }

        private void RefreshOrders()
        {
            var all = DBClass.connect.Orders.ToList();
            string filter = cmbOrderStatus.SelectedItem as string;
            if (filter == null || filter == "Все")
                lvOrders.ItemsSource = all;
            else
                lvOrders.ItemsSource = all.Where(o => o.Status == filter).ToList();
        }

        private void RefreshOrdersForAssignment()
        {
            var orders = DBClass.connect.Orders.Where(o => o.Status != "Завершён").ToList();
            cmbOrderForAssignment.ItemsSource = orders;
            if (orders.Any())
                cmbOrderForAssignment.SelectedIndex = 0;
        }

        private void RefreshBonuses()
        {
            var testBonuses = new List<AdminLogs>
    {
        new AdminLogs { ActionTime = DateTime.Now, ActionType = "Bonus"},
        new AdminLogs { ActionTime = DateTime.Now, ActionType = "Fine"}
    };
            lvBonuses.ItemsSource = testBonuses;
        }

        private void cmbOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshOrders();

        private void ChangeOrderStatus_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var order = DBClass.connect.Orders.Find(id);
            string newStatus = "";
            if (order.Status == "Новый") newStatus = "В обработке";
            else if (order.Status == "В обработке") newStatus = "Завершён";
            else return;
            order.Status = newStatus;
            DBClass.connect.SaveChanges();
            RefreshOrders();
        }

        private void QualityCheck_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var order = DBClass.connect.Orders.Find(id);
            new QualityCheckWindow(order).ShowDialog();
            RefreshOrders();
        }

        private void cmbOrderForAssignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOrder = cmbOrderForAssignment.SelectedItem as Orders;
            if (selectedOrder == null) return;
            var assigned = DBClass.connect.OrderDetails.Where(od => od.OrderID == selectedOrder.OrderID).Select(od => od.ActorID).ToList();
            lvAssignedActors.ItemsSource = DBClass.connect.Actors.Where(a => assigned.Contains(a.ActorID)).ToList();
            lvAvailableActors.ItemsSource = DBClass.connect.Actors.Where(a => !assigned.Contains(a.ActorID)).ToList();
        }

        private void AssignActor_Click(object sender, RoutedEventArgs e)
        {
            int actorId = (int)((Button)sender).Tag;
            var order = cmbOrderForAssignment.SelectedItem as Orders;
            if (order == null) return;
            var actor = DBClass.connect.Actors.Find(actorId);
            var price = actor.ActorRoleMapping.FirstOrDefault()?.CustomRatePerHour ?? 500;
            var detail = new OrderDetails
            {
                OrderID = order.OrderID,
                ActorID = actorId,
                HoursCount = 1,
                PricePerHour = price
            };
            DBClass.connect.OrderDetails.Add(detail);
            DBClass.connect.SaveChanges();
            RefreshOrdersForAssignment();
            cmbOrderForAssignment_SelectionChanged(null, null);
        }

        private void AddBonusFine_Click(object sender, RoutedEventArgs e)
        {
            new BonusFineWindow().ShowDialog();
            RefreshBonuses();
        }
    }
}

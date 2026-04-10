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
    /// Логика взаимодействия для ActorDetailsPage.xaml
    /// </summary>
    public partial class ActorDetailsPage : Page
    {
        private int actorId;
        public ActorDetailsPage(int id)
        {
            InitializeComponent();
            actorId = id;
            LoadData();
        }

        private void LoadData()
        {
            var actor = DBClass.connect.Actors.FirstOrDefault(a => a.ActorID == actorId);
            if (actor == null) return;

            tbName.Text = actor.FullName;
            var roles = actor.ActorRoleMapping.Select(arm => arm.ActorRoles.RoleName);
            tbRoles.Text = "Роли: " + string.Join(", ", roles);
            tbRating.Text = $"Рейтинг: {actor.Rating:F1}";
            tbDrinkable.Text = actor.IsDrinkable == true ? "Умеет пить" : "Не пьёт";

            var orderDetails = DBClass.connect.OrderDetails.Where(od => od.ActorID == actorId).ToList();
            lvOrders.ItemsSource = orderDetails;

            var logs = DBClass.connect.AdminLogs
        .Where(l => l.TableName == "Actors" && l.RecordID == actorId)
        .OrderByDescending(l => l.ActionTime)
        .ToList();
            lvLogs.ItemsSource = logs;
        }
    }
}

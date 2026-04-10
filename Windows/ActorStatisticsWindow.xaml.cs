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
using WeddingFriends.DB;

namespace WeddingFriends.Windows
{
    /// <summary>
    /// Логика взаимодействия для ActorStatisticsWindow.xaml
    /// </summary>
    public partial class ActorStatisticsWindow : Window
    {
        private Actors actor;
        public ActorStatisticsWindow(Actors actor)
        {
            InitializeComponent();
            this.actor = actor;
            tbActorName.Text = actor.FullName;
            var details = DBClass.connect.OrderDetails.Where(od => od.ActorID == actor.ActorID).ToList();
            lvOrders.ItemsSource = details;
            tbOrdersCount.Text = details.Count.ToString();
            tbAvgRating.Text = "Нет оценок";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}

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
    /// Логика взаимодействия для ManageActorStatusWindow.xaml
    /// </summary>
    public partial class ManageActorStatusWindow : Window
    {
        private Actors actor;
        public ManageActorStatusWindow(Actors actor)
        {
            InitializeComponent();
            this.actor = actor;
            tbActorName.Text = actor.FullName;
            tbRating.Text = actor.Rating.ToString();
            chkDrinkable.IsChecked = actor.IsDrinkable;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            decimal newRating;
            if (!decimal.TryParse(tbRating.Text, out newRating))
                newRating = actor.Rating ?? 5;
            actor.Rating = newRating;
            actor.IsDrinkable = chkDrinkable.IsChecked;

            var log = new AdminLogs
            {
                AdminUserID = Session.CurrentUser.UserID,
                ActionType = "UPDATE_STATUS",
                TableName = "Actors",
                RecordID = actor.ActorID,
                ActionTime = DateTime.Now
            };
            DBClass.connect.AdminLogs.Add(log);
            DBClass.connect.SaveChanges();
            MessageBox.Show("Статус актёра обновлён");
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

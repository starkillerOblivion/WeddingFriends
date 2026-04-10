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
    /// Логика взаимодействия для EditActorWindow.xaml
    /// </summary>
    public partial class EditActorWindow : Window
    {
        private Actors actor;
        private bool isNew;

        public EditActorWindow(Actors existing = null)
        {
            InitializeComponent();
            lbRoles.ItemsSource = DBClass.connect.ActorRoles.ToList();

            if (existing == null)
            {
                isNew = true;
                actor = new Actors();
                dpHireDate.SelectedDate = DateTime.Today;
            }
            else
            {
                isNew = false;
                actor = existing;
                tbFullName.Text = actor.FullName;
                tbAge.Text = actor.Age.ToString();
                chkDrinkable.IsChecked = actor.IsDrinkable;
                tbRating.Text = actor.Rating.ToString();
                dpHireDate.SelectedDate = actor.HireDate;

                var selectedRoleIds = actor.ActorRoleMapping.Select(arm => arm.ActorRoleID).ToList();
                foreach (var item in lbRoles.Items)
                {
                    var role = item as ActorRoles;
                    if (selectedRoleIds.Contains(role.ActorRoleID))
                        lbRoles.SelectedItems.Add(role);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbAge.Text, out int age) || age < 18)
            {
                MessageBox.Show("Возраст должен быть целым числом не менее 18 лет", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(tbRating.Text, out decimal rating) || rating < 0 || rating > 5)
            {
                MessageBox.Show("Рейтинг должен быть числом от 0 до 5", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (lbRoles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            actor.FullName = tbFullName.Text.Trim();
            actor.Age = age;
            actor.IsDrinkable = chkDrinkable.IsChecked ?? false;
            actor.Rating = rating;
            actor.HireDate = dpHireDate.SelectedDate ?? DateTime.Today;

            if (isNew)
                DBClass.connect.Actors.Add(actor);
            DBClass.connect.SaveChanges();

            var existingMappings = DBClass.connect.ActorRoleMapping
                .Where(arm => arm.ActorID == actor.ActorID)
                .ToList();
            foreach (var mapping in existingMappings)
            {
                DBClass.connect.ActorRoleMapping.Remove(mapping);
            }

            foreach (var selected in lbRoles.SelectedItems)
            {
                var role = selected as ActorRoles;
                if (role != null)
                {
                    var mapping = new ActorRoleMapping
                    {
                        ActorID = actor.ActorID,
                        ActorRoleID = role.ActorRoleID,
                        CustomRatePerHour = role.BaseRatePerHour
                    };
                    DBClass.connect.ActorRoleMapping.Add(mapping);
                }
            }
            DBClass.connect.SaveChanges();

            DialogResult = true;
            Close();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

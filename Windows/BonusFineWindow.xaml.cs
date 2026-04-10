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
    /// Логика взаимодействия для BonusFineWindow.xaml
    /// </summary>
    public partial class BonusFineWindow : Window
    {
        public BonusFineWindow()
        {
            InitializeComponent();
            cmbActor.ItemsSource = DBClass.connect.Actors.ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbActor.SelectedValue == null)
            {
                MessageBox.Show("Выберите актёра");
                return;
            }
            int actorId = (int)cmbActor.SelectedValue;
            string type = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString();
            decimal amount;
            if (!decimal.TryParse(tbAmount.Text, out amount))
            {
                MessageBox.Show("Введите корректную сумму");
                return;
            }
            if (type == "Штраф") amount = -Math.Abs(amount);
            else amount = Math.Abs(amount);

            var log = new AdminLogs
            {
                AdminUserID = Session.CurrentUser.UserID,
                ActionType = type == "Премия" ? "BONUS" : "FINE",
                TableName = "Actors",
                RecordID = actorId,
                ActionTime = DateTime.Now
            };
            DBClass.connect.AdminLogs.Add(log);
            DBClass.connect.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

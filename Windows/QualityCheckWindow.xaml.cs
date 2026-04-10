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
    /// Логика взаимодействия для QualityCheckWindow.xaml
    /// </summary>
    public partial class QualityCheckWindow : Window
    {
        private Orders order;

        public QualityCheckWindow(Orders order)
        {
            InitializeComponent();
            this.order = order;
            tbOrderId.Text = order.OrderID.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            int rating = int.Parse((cmbRating.SelectedItem as ComboBoxItem).Content.ToString());
          
            var log = new AdminLogs
            {
                AdminUserID = Session.CurrentUser.UserID,
                ActionType = "QUALITY_CHECK",
                TableName = "Orders",
                RecordID = order.OrderID,
                ActionTime = DateTime.Now
            };
            DBClass.connect.AdminLogs.Add(log);
            DBClass.connect.SaveChanges();
            MessageBox.Show("Оценка качества сохранена");
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

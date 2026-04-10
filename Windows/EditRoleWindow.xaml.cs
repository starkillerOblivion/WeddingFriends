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
    /// Логика взаимодействия для EditRoleWindow.xaml
    /// </summary>
    public partial class EditRoleWindow : Window
    {
        private ActorRoles role;
        private bool isNew;

        public EditRoleWindow(ActorRoles existing = null)
        {
            InitializeComponent();
            if (existing == null)
            {
                isNew = true;
                role = new ActorRoles();
            }
            else
            {
                isNew = false;
                role = existing;
                tbRoleName.Text = role.RoleName;
                tbBaseRate.Text = role.BaseRatePerHour.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            role.RoleName = tbRoleName.Text;
            role.BaseRatePerHour = decimal.TryParse(tbBaseRate.Text, out decimal rate) ? rate : 0;

            if (isNew)
                DBClass.connect.ActorRoles.Add(role);
            DBClass.connect.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

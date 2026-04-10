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
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private Users user;
        private bool isNew;

        public EditUserWindow(Users existing = null)
        {
            InitializeComponent();
            cmbRole.ItemsSource = DBClass.connect.Roles.ToList();

            if (existing == null)
            {
                isNew = true;
                user = new Users();
            }
            else
            {
                isNew = false;
                user = existing;
                tbLogin.Text = user.Login;
                tbFullName.Text = user.FullName;
                tbPhone.Text = user.Phone;
                tbEmail.Text = user.Email;
                cmbRole.SelectedValue = user.RoleID;
                chkActive.IsChecked = user.IsActive;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            user.Login = tbLogin.Text;
            user.FullName = tbFullName.Text;
            user.Phone = tbPhone.Text;
            user.Email = tbEmail.Text;
            user.RoleID = (int)cmbRole.SelectedValue;
            user.IsActive = chkActive.IsChecked;

            if (!string.IsNullOrEmpty(pbPassword.Password))
                user.PasswordHash = pbPassword.Password;

            if (isNew)
                DBClass.connect.Users.Add(user);
            DBClass.connect.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private Users currentUser;
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (Session.CurrentUser == null) return;
            currentUser = DBClass.connect.Users.Find(Session.CurrentUser.UserID);
            if (currentUser == null) return;

            tbLogin.Text = currentUser.Login;
            tbFullName.Text = currentUser.FullName;
            tbPhone.Text = currentUser.Phone;
            tbEmail.Text = currentUser.Email;

            if (!string.IsNullOrEmpty(currentUser.AvatarPath) && File.Exists(currentUser.AvatarPath))
                imgAvatar.Source = new BitmapImage(new Uri(currentUser.AvatarPath));
            else
                imgAvatar.Source = new BitmapImage(new Uri("/Images/iconProfile.png", UriKind.Relative));

            var orders = DBClass.connect.Orders.Where(o => o.Customers.UserID == currentUser.UserID).ToList();
            lvOrders.ItemsSource = orders;
        }

        private void btnSelectAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Image files|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                string dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avatars");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string fileName = $"{currentUser.UserID}_{System.Guid.NewGuid()}{System.IO.Path.GetExtension(dialog.FileName)}";
                string dest = System.IO.Path.Combine(dir, fileName);
                File.Copy(dialog.FileName, dest, true);
                if (!string.IsNullOrEmpty(currentUser.AvatarPath) && File.Exists(currentUser.AvatarPath))
                    File.Delete(currentUser.AvatarPath);
                currentUser.AvatarPath = dest;
                DBClass.connect.SaveChanges();
                imgAvatar.Source = new BitmapImage(new Uri(dest));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            currentUser.FullName = tbFullName.Text.Trim();
            currentUser.Phone = tbPhone.Text.Trim();
            currentUser.Email = tbEmail.Text.Trim();
            if (!string.IsNullOrEmpty(pbPassword.Password))
                currentUser.PasswordHash = pbPassword.Password;
            DBClass.connect.SaveChanges();
            MessageBox.Show("Профиль обновлён");
        }
    }
}

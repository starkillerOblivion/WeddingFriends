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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string pass = pbPassword.Password;
            string confirm = pbPasswordConfirm.Password;
            string fullName = tbFullName.Text.Trim();
            string phone = tbPhone.Text.Trim();
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(fullName))
            {
                tbError.Text = "Заполните обязательные поля";
                return;
            }
            if (pass != confirm)
            {
                tbError.Text = "Пароли не совпадают";
                return;
            }
            if (DBClass.connect.Users.Any(u => u.Login == login))
            {
                tbError.Text = "Логин уже занят";
                return;
            }

            var newUser = new Users
            {
                Login = login,
                PasswordHash = pass,
                FullName = fullName,
                Phone = phone,
                Email = email,
                RoleID = 2,
                IsActive = true
            };
            DBClass.connect.Users.Add(newUser);
            DBClass.connect.SaveChanges();

            Session.CurrentUser = newUser;
            MessageBox.Show("Регистрация успешна!");
            NavigationService?.Navigate(new CatalogPage());
        }
    }
}

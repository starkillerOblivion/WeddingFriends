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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            RefreshCart();
        }

        private void RefreshCart()
        {
            var cart = CartHelper.GetCart();
            var items = new List<CartItemViewModel>(); 
            foreach (var c in cart)
            {
                var actor = DBClass.connect.Actors.Find(c.Key);
                if (actor == null)
                {
                    CartHelper.RemoveFromCart(c.Key);
                    continue;
                }

                items.Add(new CartItemViewModel
                {
                    ActorID = c.Key,
                    ActorFullName = actor.FullName,
                    Hours = c.Value.Hours,
                    PricePerHour = c.Value.PricePerHour
                });
            }

            lvCart.ItemsSource = items;
            tbTotal.Text = items.Sum(i => i.Total).ToString("C");
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            CartHelper.RemoveFromCart(id);
            RefreshCart();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsAuthenticated)
            {
                MessageBox.Show("Авторизуйтесь для оформления заказа");
                NavigationService.Navigate(new LoginPage());
                return;
            }

            var cart = CartHelper.GetCart();
            if (cart.Count == 0) return;

            var order = new Orders
            {
                CustomerID = GetCustomerId(Session.CurrentUser.UserID),
                OrderDate = System.DateTime.Now,
                Status = "Новый"
            };
            DBClass.connect.Orders.Add(order);
            DBClass.connect.SaveChanges();

            decimal total = 0;
            foreach (var item in cart)
            {
                var actor = DBClass.connect.Actors.Find(item.Key);
                var price = GetPriceForActor(actor);
                var detail = new OrderDetails
                {
                    OrderID = order.OrderID,
                    ActorID = actor.ActorID,
                    HoursCount = item.Value.Hours,
                    PricePerHour = price
                };
                DBClass.connect.OrderDetails.Add(detail);
                total += detail.HoursCount * detail.PricePerHour;
            }
            order.TotalCost = total;
            DBClass.connect.SaveChanges();

            CartHelper.ClearCart();
            MessageBox.Show("Заказ оформлен!");
            RefreshCart();
            NavigationService.Navigate(new CatalogPage());
        }

        private int GetCustomerId(int userId)
        {
            var customer = DBClass.connect.Customers.FirstOrDefault(c => c.UserID == userId);
            if (customer == null)
            {
                customer = new Customers
                {
                    UserID = userId,
                    WeddingDate = System.DateTime.Now.AddMonths(1),
                    WeddingVenue = "Не указано",
                    Budget = 0
                };
                DBClass.connect.Customers.Add(customer);
                DBClass.connect.SaveChanges();
            }
            return customer.CustomerID;
        }

        private decimal GetPriceForActor(Actors actor)
        {
            var mapping = actor.ActorRoleMapping.FirstOrDefault();
            if (mapping != null && mapping.CustomRatePerHour.HasValue)
                return mapping.CustomRatePerHour.Value;
            if (mapping != null && mapping.ActorRoles != null)
                return mapping.ActorRoles.BaseRatePerHour;
            return 500;
        }
    }
    public static class CartHelper
    {
        private static Dictionary<int, CartItem> cart = new Dictionary<int, CartItem>();

        public static void AddToCart(int actorId, int hours = 1, decimal pricePerHour = 0)
        {
            if (cart.ContainsKey(actorId))
                cart[actorId].Hours += hours;
            else
            {
                if (pricePerHour == 0)
                {
                    var actor = DBClass.connect.Actors.Find(actorId);
                    var mapping = actor.ActorRoleMapping.FirstOrDefault();
                    pricePerHour = mapping?.CustomRatePerHour ?? mapping?.ActorRoles.BaseRatePerHour ?? 500;
                }
                cart[actorId] = new CartItem { ActorID = actorId, Hours = hours, PricePerHour = pricePerHour };
            }
        }

        public static void RemoveFromCart(int actorId) => cart.Remove(actorId);
        public static Dictionary<int, CartItem> GetCart() => cart;
        public static void ClearCart() => cart.Clear();
    }

    public class CartItem
    {
        public int ActorID { get; set; }
        public int Hours { get; set; }
        public decimal PricePerHour { get; set; }
    }
    public class CartItemViewModel
    {
        public int ActorID { get; set; }
        public string ActorFullName { get; set; }
        public int Hours { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal Total => Hours * PricePerHour;
    }
}

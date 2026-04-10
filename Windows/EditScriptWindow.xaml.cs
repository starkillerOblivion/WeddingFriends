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
    /// Логика взаимодействия для EditScriptWindow.xaml
    /// </summary>
    public partial class EditScriptWindow : Window
    {
        private WeddingScripts script;
        private bool isNew;

        public EditScriptWindow(WeddingScripts existing = null)
        {
            InitializeComponent();
            lbRequiredRoles.ItemsSource = DBClass.connect.ActorRoles.ToList();

            if (existing == null)
            {
                isNew = true;
                script = new WeddingScripts();
            }
            else
            {
                isNew = false;
                script = existing;
                tbScriptName.Text = script.ScriptName;
                tbDescription.Text = script.Description;

                var selectedRoleIds = script.ScriptRequiredRoles.Select(sr => sr.ActorRoleID).ToList();
                foreach (var item in lbRequiredRoles.Items)
                {
                    var role = item as ActorRoles;
                    if (selectedRoleIds.Contains(role.ActorRoleID))
                        lbRequiredRoles.SelectedItems.Add(role);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            script.ScriptName = tbScriptName.Text;
            script.Description = tbDescription.Text;

            if (isNew)
                DBClass.connect.WeddingScripts.Add(script);
            DBClass.connect.SaveChanges();
            
            var existingRequirements = DBClass.connect.ScriptRequiredRoles.Where(sr => sr.ScriptID == script.ScriptID).ToList();
            foreach (var req in existingRequirements)
                DBClass.connect.ScriptRequiredRoles.Remove(req);

            foreach (var selected in lbRequiredRoles.SelectedItems)
            {
                var role = selected as ActorRoles;
                var requirement = new ScriptRequiredRoles
                {
                    ScriptID = script.ScriptID,
                    ActorRoleID = role.ActorRoleID,
                    QuantityRequired = 1
                };
                DBClass.connect.ScriptRequiredRoles.Add(requirement);
            }
            DBClass.connect.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

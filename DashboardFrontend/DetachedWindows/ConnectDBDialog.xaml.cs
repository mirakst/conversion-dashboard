using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using Microsoft.Data.SqlClient;
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

namespace DashboardFrontend.DetachedWindows
{
    public partial class ConnectDBDialog : Window
    {
        public ConnectDBDialog(UserSettings userSettings)
        {
            InitializeComponent();
            UserSettings = userSettings;
        }

        public UserSettings UserSettings { get; }

        private void OnButtonConnectDBClick(object sender, RoutedEventArgs e)
        {
            string userId = TextBoxUserId.Text;
            string password = TextBoxPassword.Password;

            if (UserSettings.ActiveProfile is null)
            {
                return;
            }

            UserSettings.ActiveProfile.BuildConnectionString(userId, password);
            using SqlConnection conn = new(UserSettings.ActiveProfile.ConnectionString);
            try
            {
                conn.Open();
                UserSettings.ActiveProfile.HasReceivedCredentials = true;
                Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnButtonBackClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using DashboardFrontend.Settings;
using Microsoft.Data.SqlClient;
using System.Windows;

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

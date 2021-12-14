using DashboardBackend.Settings;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    public partial class ConnectDBDialog : Window
    {
        public ConnectDBDialog(IUserSettings userSettings)
        {
            InitializeComponent();
            UserSettings = userSettings;
        }

        public IUserSettings UserSettings { get; }
        private readonly BackgroundWorker Worker = new()
        {
            WorkerSupportsCancellation = true
        };

        /// <summary>
        /// Opens a dialog to attempt to receive username and password for connection.
        /// </summary>
        private void OnButtonConnectDBClick(object sender, RoutedEventArgs e)
        {
            ButtonConnectDb.IsEnabled = false;
            string userId = TextBoxUserId.Text;
            string password = TextBoxPassword.Password;

            if (UserSettings.ActiveProfile is null)
            {
                return;
            }

            UserSettings.ActiveProfile.BuildConnectionString(userId, password);

            ControlLoadingAnim.Visibility = Visibility.Visible;
            Worker.DoWork += Worker_DoWork!;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted!;
            Worker.RunWorkerAsync();
        }

        /// <summary>
        /// Connecting to database cancelled.
        /// </summary>
        private void OnButtonBackClick(object sender, RoutedEventArgs e)
        {
            Worker.CancelAsync();
            Close();
        }

        /// <summary>
        /// Attempt connecting to the database.
        /// </summary>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            using SqlConnection conn = new(UserSettings.ActiveProfile!.ConnectionString);
            try
            {
                conn.Open();
                UserSettings.ActiveProfile.HasReceivedCredentials = true;
            }
            catch (SqlException ex)
            {
                if (Worker.CancellationPending) return;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// When connected to database.
        /// </summary>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ControlLoadingAnim.Visibility = Visibility.Collapsed;
            Close();
        }
    }
}

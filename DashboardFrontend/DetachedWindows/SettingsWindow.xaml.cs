using DashboardFrontend.ViewModels;
using DashboardSettings;
using System.Windows;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = UserSettings;
        }

        private UserSettingsViewModel UserSettings { get; set; } = new();

        private void Button_SaveAndClose(object sender, RoutedEventArgs e)
        {
            if (UserSettings.ActiveProfile is not null)
            {
                if (UserSettings.ActiveProfile == DashboardSettings.UserSettings.ActiveProfile ||
                    Confirm("Changing the active profile will stop the current monitoring process and clear all views. Continue?"))
                {
                    // Try to save config data to file and update the in-memory config. If successful, close the window...
                    Close();
                }
            }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_NewProfile(object sender, RoutedEventArgs e)
        {
            NewProfileWindow newProfileWindow = new(UserSettings);
            newProfileWindow.ShowDialog();
        }

        private void Button_DeleteProfile(object sender, RoutedEventArgs e)
        {
            Profile? profile = UserSettings.SelectedProfile;
            if (profile is not null && Confirm($"Delete profile [{profile.Name}]?"))
            {
                if (UserSettings.ActiveProfile == profile)
                {
                    UserSettings.ActiveProfile = null;
                }
                UserSettings.Profiles.Remove(profile);
            }
        }

        private bool Confirm(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Please confirm", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        private void Button_EditProfile(object sender, RoutedEventArgs e)
        {
            if (UserSettings.SelectedProfile is not null)
            {
                NewProfileWindow newProfileWindow = new(UserSettings, UserSettings.SelectedProfile);
                newProfileWindow.ShowDialog();
            }
        }
        
        private void Button_SetActiveProfile(object sender, RoutedEventArgs e)
        {
            if (UserSettings.SelectedProfile is not null)
            {
                UserSettings.ActiveProfile = UserSettings.SelectedProfile;
            }
        }
    }
}

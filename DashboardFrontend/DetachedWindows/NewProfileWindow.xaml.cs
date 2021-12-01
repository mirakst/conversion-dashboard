using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for NewProfileWindow.xaml
    /// </summary>
    public partial class NewProfileWindow : Window
    {
        private UserSettingsViewModel UserSettings { get; set; }
        private Profile Profile { get; set; } = new();

        public NewProfileWindow(UserSettingsViewModel userSettings)
        {
            InitializeComponent();
            UserSettings = userSettings;
            DataContext = Profile;
            Title = "Create new profile";
        }

        public NewProfileWindow(UserSettingsViewModel userSettings, Profile profile) : this(userSettings)
        {
            Profile = profile;
            DataContext = profile;
            Title = "Edit profile";
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            List<BindingExpression> bindings = new()
            {
                name.GetBindingExpression(TextBox.TextProperty),
                conversion.GetBindingExpression(TextBox.TextProperty),
                dataSrc.GetBindingExpression(TextBox.TextProperty),
                database.GetBindingExpression(TextBox.TextProperty),
                timeout.GetBindingExpression(TextBox.TextProperty)
            };

            if ((dataSrc.Text != Profile.DataSource || database.Text != Profile.Database) && Profile.HasStartedMonitoring)
            {
                if (!SettingsWindow.Confirm("Editing the data source or database of the active profile will stop ongoing monitoring and clear all views. Continue?"))
                {
                    Close();
                    return;
                }
                Profile.OnProfileChange();
                Profile.HasStartedMonitoring = false;
            }

            if (Profile.Equals(UserSettings.ActiveProfile))
            {
                Profile.HasReceivedCredentials = false;
                UserSettings.ActiveProfile = Profile;
            }

            foreach (BindingExpression binding in bindings)
            {
                binding.UpdateSource();
            }

            if (!UserSettings.Profiles.Contains(Profile))
            {
                UserSettings.Profiles.Add(Profile);
            }

            Close();
        }
    }
}

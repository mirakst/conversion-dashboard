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

            foreach(BindingExpression binding in bindings)
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

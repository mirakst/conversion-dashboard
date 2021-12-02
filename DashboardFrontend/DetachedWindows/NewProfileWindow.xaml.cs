using DashboardFrontend.Settings;
using DashboardFrontend.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
                TextBoxName.GetBindingExpression(TextBox.TextProperty),
                TextBoxConversion.GetBindingExpression(TextBox.TextProperty),
                TextBoxDataSrc.GetBindingExpression(TextBox.TextProperty),
                TextBoxDatabase.GetBindingExpression(TextBox.TextProperty),
                TextBoxTimeout.GetBindingExpression(TextBox.TextProperty)
            };

            foreach (BindingExpression binding in bindings)
            {
                binding.UpdateSource();
            }

            List<bool> inputValidations = new()
            {
                Validation.GetHasError(TextBoxName),
                Validation.GetHasError(TextBoxConversion),
                Validation.GetHasError(TextBoxDataSrc),
                Validation.GetHasError(TextBoxDatabase),
                Validation.GetHasError(TextBoxTimeout)
            };

            if (!inputValidations.Contains(true))
            {
                if (Profile.Equals(UserSettings.ActiveProfile))
                {
                    Profile.HasReceivedCredentials = false;
                    UserSettings.ActiveProfile = Profile;
                }

                if (!UserSettings.Profiles.Contains(Profile))
                {
                    UserSettings.Profiles.Add(Profile);
                }

                Close();
            }
            else
            {
                _ = MessageBox.Show("The specified properties are either empty or break validation rules", "Error");
            }
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}

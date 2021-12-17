using DashboardFrontend.ViewModels;
using DashboardBackend.Settings;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DashboardFrontend.DetachedWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly DashboardController _controller;

        public SettingsWindow(DashboardController controller)
        {
            InitializeComponent();
            Settings = controller.UserSettings;
            SettingsViewModel = new(controller.UserSettings);
            DataContext = SettingsViewModel;
            _controller = controller;
        }

        private UserSettingsViewModel SettingsViewModel { get; }
        private IUserSettings Settings { get; }

        /// <summary>
        /// Save and close settings, validate input.
        /// </summary>
        private void Button_SaveAndClose(object sender, RoutedEventArgs e)
        {
            List<bool> inputValidations = new()
            {
                Validation.GetHasError(TextBoxLoggingInterval),
                Validation.GetHasError(TextBoxHRInterval),
                Validation.GetHasError(TextBoxReconciliationInterval),
                Validation.GetHasError(TextBoxManagerInterval),
                Validation.GetHasError(TextBoxAllInterval)
            };

            if (inputValidations.Contains(true))
            {
                MessageBox.Show("The specified properties are either empty or break validation rules", "Error");
                return;
            }
            else if (SettingsViewModel.HasChangedActiveProfile && Settings.ActiveProfile?.HasStartedMonitoring is true)
            {
                if (Confirm("Changing the active profile will stop the current monitoring process and clear all views. Continue?"))
                {
                    Settings.ActiveProfile.HasStartedMonitoring = false;
                    _controller.Reset();
                }
                else
                {
                    return;
                }
            }
            try
            {
                Settings.Save(SettingsViewModel);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while saving settings\n\nDetails\n" + ex.Message);
            }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Opens the new profile dialog.
        /// </summary>
        private void Button_NewProfile(object sender, RoutedEventArgs e)
        {
            NewProfileWindow newProfileWindow = new(SettingsViewModel);
            newProfileWindow.Owner = this;
            newProfileWindow.ShowDialog();
        }

        /// <summary>
        /// Deletes the selected profile.
        /// </summary>
        private void Button_DeleteProfile(object sender, RoutedEventArgs e)
        {
            Profile? profile = SettingsViewModel.SelectedProfile;
            if (profile is not null && Confirm($"Delete profile [{profile.Name}]?"))
            {
                if (SettingsViewModel.ActiveProfile?.Equals(profile) == true)
                {
                    SettingsViewModel.ActiveProfile = null;
                }
                SettingsViewModel.Profiles.Remove(profile);
            }
        }

        /// <summary>
        /// Confirm message box, used to prompt user.
        /// </summary>
        /// <param name="message">The message to prompt the user.</param>
        /// <returns>True, if user confirms.</returns>
        public static bool Confirm(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Please confirm", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Opens the new profile window with the selected profile, to edit it.
        /// </summary>
        private void Button_EditProfile(object sender, RoutedEventArgs e)
        {
            if (SettingsViewModel.SelectedProfile is not null)
            {
                NewProfileWindow newProfileWindow = new(SettingsViewModel, SettingsViewModel.SelectedProfile);
                newProfileWindow.ShowDialog();
            }
        }
        
        /// <summary>
        /// Sets the active profile to the selected profile.
        /// </summary>
        private void Button_SetActiveProfile(object sender, RoutedEventArgs e)
        {
            if (SettingsViewModel.SelectedProfile is not null)
            {
                SettingsViewModel.ActiveProfile = SettingsViewModel.SelectedProfile;
                SettingsViewModel.HasChangedActiveProfile = !(SettingsViewModel.ActiveProfile.Equals(Settings.ActiveProfile));
            }
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// The close window command binding.
        /// </summary>
        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}

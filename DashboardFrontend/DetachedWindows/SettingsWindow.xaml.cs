using DashboardFrontend.ViewModels;
using DashboardFrontend.Settings;
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
        public SettingsWindow(UserSettings userSettings)
        {
            InitializeComponent();
            Settings = userSettings;
            SettingsViewModel = new(userSettings);
            DataContext = SettingsViewModel;
        }

        private UserSettingsViewModel SettingsViewModel { get; }
        private UserSettings Settings { get; }

        private void Button_SaveAndClose(object sender, RoutedEventArgs e)
        {
            List<bool> inputValidations = new()
            {
                Validation.GetHasError(TextBoxLoggingInterval),
                Validation.GetHasError(TextBoxHRInterval),
                Validation.GetHasError(TextBoxValidationInterval),
                Validation.GetHasError(TextBoxManagerInterval),
                Validation.GetHasError(TextBoxAllInterval)
            };

            if ((!SettingsViewModel.HasChangedActiveProfile || Confirm("Changing the active profile will stop the current monitoring process and clear all views. Continue?")) && !inputValidations.Contains(true))
            {
                try
                {
                    Settings.OverwriteAllAndSave(SettingsViewModel);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occured while saving settings\n\nDetails\n"+ex.Message);
                }
            }
            else
            {
                _ = MessageBox.Show("The specified properties are either empty or break validation rules", "Error");
            }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_NewProfile(object sender, RoutedEventArgs e)
        {
            NewProfileWindow newProfileWindow = new(SettingsViewModel);
            newProfileWindow.Owner = this;
            newProfileWindow.ShowDialog();
        }

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

        public static bool Confirm(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Please confirm", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        private void Button_EditProfile(object sender, RoutedEventArgs e)
        {
            if (SettingsViewModel.SelectedProfile is not null)
            {
                NewProfileWindow newProfileWindow = new(SettingsViewModel, SettingsViewModel.SelectedProfile);
                newProfileWindow.ShowDialog();
            }
        }
        
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

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}

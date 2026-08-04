using Avalonia.Controls;
using Avalonia.Interactivity;
using ODExplorer.Models;
using ODExplorer.ViewModels.ViewVMs;
using ODUtils.Models;
using System;

namespace ODExplorer.UI.Avalonia.Views;

public partial class DisplaySettingsView : UserControl
{
    public DisplaySettingsView()
    {
        InitializeComponent();

        CodexComboBox.ItemsSource = Enum.GetValues(typeof(CodexEntryHistory));
    }

    private void FlagCheckBox_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DisplaySettingsViewModel vm || sender is not CheckBox checkBox)
            return;

        var enabled = checkBox.IsChecked == true;

        if (checkBox.Tag is NotificationOptions notifyFlag)
        {
            vm.SetNotifyOptionsFlag(notifyFlag, enabled);
        }
        else if (checkBox.Tag is BodyNotification bodyFlag)
        {
            vm.SetBodyNotificationFlag(bodyFlag, enabled);
        }
    }
}

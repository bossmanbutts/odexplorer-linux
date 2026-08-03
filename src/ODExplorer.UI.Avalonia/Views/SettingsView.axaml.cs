using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ODExplorer.ViewModels.ViewVMs;
using System.Linq;
using System.Threading.Tasks;

namespace ODExplorer.UI.Avalonia.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private async void ChangeLogsFolder_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel vm)
            return;

        var folder = await PickFolderAsync("Select Journal Folder");
        if (folder is not null)
            vm.OnSetNewDir(folder.TryGetLocalPath() ?? folder.Path.ToString());
    }

    private async void ScanDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel vm)
            return;

        var folder = await PickFolderAsync("Select Directory to Scan");
        if (folder is not null)
            vm.OnScanNewDirectory(folder.TryGetLocalPath() ?? folder.Path.ToString());
    }

    private async Task<IStorageFolder?> PickFolderAsync(string title)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        return folders.FirstOrDefault();
    }
}

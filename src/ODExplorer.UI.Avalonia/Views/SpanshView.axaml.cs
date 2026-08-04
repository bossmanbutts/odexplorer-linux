using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ODExplorer.ViewModels.ViewVMs;
using System.Linq;

namespace ODExplorer.UI.Avalonia.Views;

public partial class SpanshView : UserControl
{
    public SpanshView()
    {
        InitializeComponent();
    }

    private async void ImportCsv_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SpanshViewModel vm)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Browse csv files",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("CSV files") { Patterns = new[] { "*.csv" } } }
        });

        var file = files.FirstOrDefault();
        if (file?.TryGetLocalPath() is { } path)
        {
            vm.ParseCSV(path);
        }
    }

    private async void SelectSound_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SpanshViewModel vm)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select custom alarm sound",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Sound files") { Patterns = new[] { "*.wav", "*.mp3" } } }
        });

        var file = files.FirstOrDefault();
        if (file?.TryGetLocalPath() is { } path)
        {
            vm.SetCustomFile(path);
        }
    }
}

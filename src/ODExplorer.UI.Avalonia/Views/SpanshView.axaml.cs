using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ODExplorer.Models;
using ODExplorer.ViewModels.ViewVMs;
using ODUtils.Spansh;
using System.IO;
using System.Linq;

namespace ODExplorer.UI.Avalonia.Views;

public partial class SpanshView : UserControl
{
    private SpanshViewModel? viewModel;

    public SpanshView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpanshViewModel vm)
        {
            viewModel = vm;
            vm.OnErrorProcessingCSV += OnErrorProcessingCSV;
        }
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (viewModel is not null)
        {
            viewModel.OnErrorProcessingCSV -= OnErrorProcessingCSV;
            viewModel = null;
        }
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

    private void OnErrorProcessingCSV(object? sender, SpanshCsvErrorEventArgs e)
    {
        if (e.ErrorType == SpanshCSVError.Parse)
        {
            ShowCsvTypeSelector(e.Filename);
        }
        else
        {
            MessageBoxRequester.Request(new MessageBoxEventArgsAsync("Unable to parse CSV", $"Error parsing {Path.GetFileName(e.Filename)}", MessageBoxButton.OK));
        }
    }

    private async void ShowCsvTypeSelector(string filename)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var owner = topLevel as Window;
        var dialog = new SpanshCSVSelectorWindow();

        if (owner is null)
        {
            dialog.Show();
            return;
        }

        await dialog.ShowDialog(owner);

        if (dialog.Result > CsvType.None)
        {
            viewModel?.ForceParseCSV(filename, dialog.Result);
        }
    }
}

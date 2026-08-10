using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ODExplorer.ViewModels.ViewVMs;
using System.Linq;
using System.Threading.Tasks;

namespace ODExplorer.UI.Avalonia.Views;

public partial class OnboardingWindow : Window
{
    private readonly MainViewModel viewModel;
    private bool finished;

    public OnboardingWindow()
    {
        InitializeComponent();
        viewModel = null!;
    }

    public OnboardingWindow(MainViewModel vm) : this()
    {
        viewModel = vm;
        DataContext = vm;
    }

    private async void OnSelectFolderClick(object? sender, RoutedEventArgs e)
    {
        if (finished)
            return;

        var folder = await PickFolderAsync("Select Elite Dangerous Journal Folder");
        if (folder is null)
            return;

        var path = folder.TryGetLocalPath() ?? folder.Path.ToString();

        StatusText.IsVisible = true;
        StatusText.Text = $"Scanning journal folder...\n{path}";

        SelectFolderButton.IsEnabled = false;
        SkipButton.IsEnabled = false;

        finished = true;
        viewModel.OnboardingFinishedWithDirectory(path);
        Close();
    }

    private void OnSkipClick(object? sender, RoutedEventArgs e)
    {
        if (finished)
            return;

        finished = true;
        viewModel.OnboardingFinishedSkip();
        Close();
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

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using ODExplorer.Models;
using ODExplorer.UI.Avalonia.Controls;
using ODExplorer.UI.Avalonia.Views;
using ODExplorer.ViewModels.ModelVMs;
using ODExplorer.ViewModels.ViewVMs;
using AvaloniaWindowState = global::Avalonia.Controls.WindowState;

namespace ODExplorer.UI.Avalonia;

public partial class MainWindow : Window
{
    private readonly ToastHost toastHost;
    private bool quitRequested;

    public MainWindow()
    {
        InitializeComponent();
        toastHost = new ToastHost(null!);
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;

        toastHost = new ToastHost(viewModel.SettingsStore)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = false,
        };
        ContentRoot.Children.Add(toastHost);

        viewModel.NotificationStore.OnToast += OnToast;
        viewModel.OpenPopoutRequested += OnOpenPopoutRequested;
        Closing += OnWindowClosing;
        PositionChanged += OnWindowGeometryChanged;
        Resized += OnWindowGeometryChanged;
        PropertyChanged += OnWindowPropertyChanged;

        ApplyWindowPosition();
    }

    private void OnToast(ToastMessage message)
    {
        Dispatcher.UIThread.Post(() => toastHost.Show(message));
    }

    private void OnOpenPopoutRequested(object? sender, ODExplorer.Models.PopOutBase popOut)
    {
        if (sender is not MainViewModel vm)
        {
            return;
        }

        var window = new PopOutWindow(vm, popOut);
        window.Show(this);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (DataContext is not MainViewModel vm || vm.SettingsStore.OnBoardingComplete)
        {
            return;
        }

        // First-run onboarding: welcome + guided journal folder selection.
        var onboarding = new OnboardingWindow(vm);
        _ = onboarding.ShowDialog(this);
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        if (quitRequested)
        {
            vm.OnClose();
            return;
        }

        vm.SettingsStore.SaveSettings();

        // With "minimise to tray" enabled, the close button hides the window
        // instead of terminating the app; use the tray "Quit" item to exit.
        if (vm.SettingsStore.MinimiseToTray)
        {
            e.Cancel = true;
            Hide();
        }
    }

    private void OnWindowGeometryChanged(object? sender, EventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var pos = vm.SettingsStore.WindowPosition ??= new WindowPositionViewModel();
        pos.Left = Position.X;
        pos.Top = Position.Y;
        pos.Width = ClientSize.Width;
        pos.Height = ClientSize.Height;
    }

    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != WindowStateProperty || DataContext is not MainViewModel vm)
        {
            return;
        }

        var pos = vm.SettingsStore.WindowPosition ??= new WindowPositionViewModel();
        pos.State = WindowState switch
        {
            AvaloniaWindowState.Maximized => Models.WindowState.Maximized,
            AvaloniaWindowState.Minimized => Models.WindowState.Minimized,
            _ => Models.WindowState.Normal,
        };
    }

    private void ApplyWindowPosition()
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        var pos = vm.SettingsStore.WindowPosition;
        if (pos == null || pos.Width <= 0 || pos.Height <= 0)
        {
            return;
        }

        Position = new PixelPoint((int)pos.Left, (int)pos.Top);
        Width = pos.Width;
        Height = pos.Height;
        WindowState = pos.State switch
        {
            Models.WindowState.Maximized => AvaloniaWindowState.Maximized,
            Models.WindowState.Minimized => AvaloniaWindowState.Minimized,
            _ => AvaloniaWindowState.Normal,
        };
    }

    public void RequestQuit()
    {
        quitRequested = true;
        Close();
    }
}

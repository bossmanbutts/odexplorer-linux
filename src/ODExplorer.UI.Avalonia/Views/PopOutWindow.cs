using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using ODExplorer.Controls;
using ODExplorer.Models;
using ODExplorer.UI.Avalonia.Views.PopOutOverlays;
using ODExplorer.ViewModels.ViewVMs;
using AvaloniaWindowState = global::Avalonia.Controls.WindowState;

namespace ODExplorer.UI.Avalonia.Views;

/// <summary>
/// Platform window hosting a core pop-out overlay. Consumes the pop-out's saved
/// geometry/mode params and reports close/resize back to MainViewModel so the
/// commander's PopOutParams stay in sync.
/// </summary>
public sealed class PopOutWindow : Window
{
    private readonly MainViewModel viewModel;
    private readonly PopOutBase popOut;
    private bool forceClosing;

    public PopOutWindow(MainViewModel viewModel, PopOutBase popOut)
    {
        this.viewModel = viewModel;
        this.popOut = popOut;

        Title = popOut.ShowTitle ? popOut.Title : string.Empty;
        DataContext = viewModel;
        Width = 640;
        Height = 480;
        Topmost = popOut.AlwaysOnTop;
        ShowInTaskbar = popOut.ShowInTaskBar;
        Content = BuildContent(popOut);

        popOut.ForceCloseRequested += OnForceCloseRequested;
        popOut.ResetRequested += OnResetRequested;
        Closing += OnWindowClosing;
        PositionChanged += OnGeometryChanged;
        Resized += OnGeometryChanged;
        PropertyChanged += OnPropertyChanged;

        ApplyPosition();
        ApplyMode();
    }

    private static Control BuildContent(PopOutBase popOut)
    {
        return popOut switch
        {
            SystemBodiesOverlay => new SystemBodiesOverlayView(),
            ExobiologyOverlay => new ExobiologyOverlayView(),
            _ => new TextBlock
            {
                Text = popOut.Title,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        ApplyPosition();
        ApplyMode();
    }

    private void ApplyPosition()
    {
        var pos = popOut.Position;
        if (pos == null)
        {
            return;
        }

        if (pos.Width > 0)
        {
            Width = pos.Width;
        }
        if (pos.Height > 0)
        {
            Height = pos.Height;
        }

        if (pos.IsZero == false)
        {
            Position = new PixelPoint((int)pos.Left, (int)pos.Top);
        }

        WindowState = pos.State switch
        {
            Models.WindowState.Maximized => AvaloniaWindowState.Maximized,
            Models.WindowState.Minimized => AvaloniaWindowState.Minimized,
            _ => AvaloniaWindowState.Normal,
        };
    }

    private void ApplyMode()
    {
        Opacity = popOut.Mode switch
        {
            PopOutMode.Semitransparent => 0.65,
            PopOutMode.Transparent => 0.2,
            _ => 1.0,
        };
    }

    private void OnResetRequested()
    {
        Dispatcher.UIThread.Post(() =>
        {
            ApplyPosition();
            ApplyMode();
        });
    }

    private void OnForceCloseRequested()
    {
        forceClosing = true;
        Dispatcher.UIThread.Post(Close);
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        popOut.ForceCloseRequested -= OnForceCloseRequested;
        popOut.ResetRequested -= OnResetRequested;

        // Force-close paths (parser offline) already persist Active=true for restore;
        // only user-initiated closes should persist the closed state.
        if (forceClosing == false)
        {
            viewModel.OnPopOutClose(popOut);
        }
    }

    private void OnGeometryChanged(object? sender, EventArgs e)
    {
        var pos = popOut.Position;
        if (pos == null)
        {
            return;
        }

        pos.Left = Position.X;
        pos.Top = Position.Y;
        pos.Width = ClientSize.Width;
        pos.Height = ClientSize.Height;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != WindowStateProperty)
        {
            return;
        }

        var pos = popOut.Position;
        if (pos == null)
        {
            return;
        }

        pos.State = WindowState switch
        {
            AvaloniaWindowState.Maximized => Models.WindowState.Maximized,
            AvaloniaWindowState.Minimized => Models.WindowState.Minimized,
            _ => Models.WindowState.Normal,
        };
    }
}

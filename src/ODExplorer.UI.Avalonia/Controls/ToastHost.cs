using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ODExplorer.Models;
using ODExplorer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using ToastNotifications.Position;

namespace ODExplorer.UI.Avalonia.Controls;

// Renders ToastMessage popups in a corner of the host window, honouring the
// NotificationSettings (position, offsets, size, display time, max count).
public sealed class ToastHost : Panel
{
    private readonly Canvas canvas = new();
    private readonly SettingsStore settingsStore;
    private readonly List<(Border Card, DispatcherTimer Timer)> active = new();

    public ToastHost(SettingsStore settingsStore)
    {
        this.settingsStore = settingsStore;
        Children.Add(canvas);
        IsHitTestVisible = false;
        SizeChanged += (_, _) => Reflow();
    }

    public void Show(ToastMessage message)
    {
        if (Dispatcher.UIThread.CheckAccess() == false)
        {
            Dispatcher.UIThread.Post(() => Show(message));
            return;
        }

        var settings = settingsStore.NotificationSettings;

        if (settings.NotificationsEnabled == false)
            return;

        if (settings.MaxNotificationCount <= 0)
            settings.MaxNotificationCount = 1;

        while (active.Count >= settings.MaxNotificationCount)
            RemoveOldest();

        var card = BuildCard(message);
        canvas.Children.Add(card);
        card.Opacity = 1;

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Math.Max(1, settings.DisplayTime)) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            FadeOut(card);
        };
        timer.Start();

        active.Add((card, timer));
        Reflow();
    }

    public void Clear()
    {
        foreach (var (card, timer) in active)
        {
            timer.Stop();
            canvas.Children.Remove(card);
        }
        active.Clear();
    }

    private Border BuildCard(ToastMessage message)
    {
        var settings = settingsStore.NotificationSettings;
        var width = settings.Size switch
        {
            NotificationSize.Small => 220.0,
            NotificationSize.Large => 380.0,
            _ => 300.0,
        };

        var title = new TextBlock
        {
            Text = message.Title,
            FontSize = settings.Size == NotificationSize.Small ? 13 : 14,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 4),
        };

        var body = new TextBlock
        {
            Text = message.Message,
            FontSize = settings.Size == NotificationSize.Small ? 12 : 13,
            Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
            TextWrapping = TextWrapping.Wrap,
        };

        var border = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(232, 32, 32, 32)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(140, 120, 120, 120)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12, 10),
            Child = new StackPanel { Children = { title, body } },
            Width = width,
            MinHeight = 48,
            Opacity = 0,
            RenderTransform = new TranslateTransform(0, -10),
        };

        border.Transitions = new Transitions
        {
            new DoubleTransition { Property = OpacityProperty, Duration = TimeSpan.FromMilliseconds(250) },
            new DoubleTransition { Property = TranslateTransform.YProperty, Duration = TimeSpan.FromMilliseconds(250) },
        };

        return border;
    }

    private void FadeOut(Border card)
    {
        var timer = active.FirstOrDefault(x => x.Card == card).Timer;
        timer?.Stop();

        if (card.RenderTransform is TranslateTransform translate)
            translate.Y = -8;
        card.Opacity = 0;

        DispatcherTimer.RunOnce(() =>
        {
            canvas.Children.Remove(card);
            active.RemoveAll(x => x.Card == card);
        }, TimeSpan.FromMilliseconds(300));
    }

    private void RemoveOldest()
    {
        var oldest = active.FirstOrDefault();
        if (oldest.Card is not null)
        {
            oldest.Timer?.Stop();
            canvas.Children.Remove(oldest.Card);
            active.RemoveAt(0);
        }
    }

    private void Reflow()
    {
        var settings = settingsStore.NotificationSettings;
        var width = Bounds.Width;
        var height = Bounds.Height;
        var right = settings.DisplayRegion is Corner.TopRight or Corner.BottomRight;
        var bottom = settings.DisplayRegion is Corner.BottomLeft or Corner.BottomRight;

        var cursor = bottom ? height - settings.YOffset : settings.YOffset;

        foreach (var (card, _) in active)
        {
            card.Measure(Size.Infinity);
            var cardHeight = card.DesiredSize.Height;
            var cardWidth = card.DesiredSize.Width;

            Canvas.SetLeft(card, right ? width - settings.XOffset - cardWidth : settings.XOffset);
            Canvas.SetTop(card, bottom ? cursor - cardHeight : cursor);

            cursor += bottom ? -(cardHeight + 10) : cardHeight + 10;
        }
    }
}

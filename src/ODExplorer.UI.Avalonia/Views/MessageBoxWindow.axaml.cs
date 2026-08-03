using Avalonia.Controls;
using ODExplorer.Models;
using System;
using System.Threading.Tasks;

namespace ODExplorer.UI.Avalonia.Views;

public partial class MessageBoxWindow : Window
{
    private readonly MessageBoxEventArgsAsync? _args;

    public MessageBoxWindow()
    {
        InitializeComponent();
    }

    public MessageBoxWindow(MessageBoxEventArgsAsync args) : this()
    {
        _args = args;
        Title = args.Title;
        MessageText.Text = args.Message;
        BuildButtons();
    }

    private void BuildButtons()
    {
        if (_args is null)
            return;

        switch (_args.Buttons)
        {
            case MessageBoxButton.OK:
                AddButton("OK", _args.CallbackYes);
                break;
            case MessageBoxButton.OKCancel:
                AddButton("OK", _args.CallbackYes);
                AddButton("Cancel", null);
                break;
            case MessageBoxButton.YesNo:
                AddButton("Yes", _args.CallbackYes);
                AddButton("No", _args.CallbackNo);
                break;
            case MessageBoxButton.YesNoCancel:
                AddButton("Yes", _args.CallbackYes);
                AddButton("No", _args.CallbackNo);
                AddButton("Cancel", null);
                break;
        }
    }

    private void AddButton(string text, Func<Task>? callback)
    {
        var button = new Button
        {
            Content = text,
            MinWidth = 90
        };

        button.Click += async (_, _) =>
        {
            IsEnabled = false;

            if (callback is not null)
            {
                try
                {
                    await callback();
                }
                catch
                {
                    // Callbacks must not prevent the dialog from closing.
                }
            }

            Close();
        };

        ButtonsPanel.Children.Add(button);
    }
}

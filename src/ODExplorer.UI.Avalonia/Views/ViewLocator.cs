using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using ODUtils.Dialogs.ViewModels;
using System;

namespace ODExplorer.UI.Avalonia.Views;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var className = data.GetType().Name;
        var viewName = className.Replace("ViewModel", "View");
        var viewType = Type.GetType($"ODExplorer.UI.Avalonia.Views.{viewName}, ODExplorer.UI.Avalonia");

        if (viewType is not null)
        {
            return (Control?)Activator.CreateInstance(viewType);
        }

        return new TextBlock
        {
            Text = $"{className} (view not ported yet)",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 16
        };
    }

    public bool Match(object? data) => data is OdViewModelBase;
}

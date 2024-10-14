using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

using ClipboardDecorator.ViewModels;
using ClipboardDecorator.Views;

using System;

namespace ClipboardDecorator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window mainWindow = new MainWindow();

            IClipboard clipboard = mainWindow.Clipboard ?? throw new InvalidOperationException("No clipboard!");
            IStorageProvider storageProvider = mainWindow.StorageProvider ?? throw new InvalidOperationException("No storage provider!");
            IClipboard myClipboad = OperatingSystem.IsLinux() ?
                new MyLinuxClipboard(clipboard, storageProvider) :
                new MyDefaultClipboard(clipboard);

            mainWindow.DataContext = new MainWindowViewModel(myClipboad);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
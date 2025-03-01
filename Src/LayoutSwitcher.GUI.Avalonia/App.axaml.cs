using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using LayoutSwitcher.ViewModels;

namespace LayoutSwitcher.GUI.Avalonia;

public class App : Application
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AppRoot _appRoot;
#pragma warning restore CS8618 

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            //DisableAvaloniaDataAnnotationValidation();
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            desktop.Exit += OnExit;

            _appRoot = new AppRoot();
            
            DataContext = new TrayViewModel(_appRoot.SettingsWindow, () => desktop.Shutdown());
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _appRoot.Dispose();
    }

    //private static void DisableAvaloniaDataAnnotationValidation()
    //{
    //    // Get an array of plugins to remove
    //    var dataValidationPluginsToRemove =
    //        BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

    //    // remove each entry found
    //    foreach (var plugin in dataValidationPluginsToRemove)
    //    {
    //        BindingPlugins.DataValidators.Remove(plugin);
    //    }
    //}
}
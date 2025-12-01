using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using HM.Presentation.WPF.Startup;
using HM.Presentation.WPF.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Presentation.WPF;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        IServiceCollection service = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<App>()
            .Build();


        service.AddWpfViewModels();
        service.AddSingleton<MainWindow>();
        
        service.AddScoped<Func<Type, BaseViewModel>>(_ => GetViewModel);
        
        _serviceProvider = service.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        base.OnStartup(e);
        mainWindow.Show();
    }

    private BaseViewModel GetViewModel(Type viewModelType)
    {
        return (BaseViewModel)_serviceProvider.GetRequiredService(viewModelType);
    }
}
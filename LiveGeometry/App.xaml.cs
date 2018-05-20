using LiveGeometry.Services;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry
{
    using System.Windows;

    using Catel.ApiCop;
    using Catel.ApiCop.Listeners;
    using Catel.IoC;
    using Catel.Logging;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            LogManager.AddDebugListener();
#endif
            Log.Info("Starting application");

            Log.Info("Registering custom types");
            var serviceLocator = ServiceLocator.Default;
            serviceLocator.RegisterType<IDrawingHostServices, DrawingHostServices>();

            Log.Info("Calling base.OnStartup");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Get advisory report in console
            ApiCopManager.AddListener(new ConsoleApiCopListener());
            ApiCopManager.WriteResults();

            base.OnExit(e);
        }
    }
}
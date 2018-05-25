using Catel;
using Catel.MVVM;
using LiveGeometry.CommandContainers;
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

            RegisterGlobalCommand();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Get advisory report in console
            ApiCopManager.AddListener(new ConsoleApiCopListener());
            ApiCopManager.WriteResults();

            base.OnExit(e);
        }

        private void RegisterGlobalCommand()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var commandManager = dependencyResolver.Resolve<ICommandManager>();

            RegisterFileCommand(commandManager);
            RegisterEditorCommand(commandManager);
            RegisterViewCommand(commandManager);

        }

        private static void RegisterViewCommand(ICommandManager commandManager)
        {
            commandManager.CreateCommandWithGesture(typeof(Commands), "FigureList");
        }

        private static void RegisterEditorCommand(ICommandManager commandManager)
        {
            commandManager.CreateCommandWithGesture(typeof(Commands), "Undo");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Redo");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Cut");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Copy");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Paste");
            commandManager.CreateCommandWithGesture(typeof(Commands), "PasteFrom");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Delete");
            commandManager.CreateCommandWithGesture(typeof(Commands), "Lock");
            commandManager.CreateCommandWithGesture(typeof(Commands), "SelectAll");
        }

        private static void RegisterFileCommand(ICommandManager commandManager)
        {
            commandManager.CreateCommandWithGesture(typeof(Commands), "NewProject");
            commandManager.CreateCommandWithGesture(typeof(Commands), "OpenProject");
            commandManager.CreateCommandWithGesture(typeof(Commands), "SaveProject");
        }
    }
}
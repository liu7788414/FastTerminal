using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Infragistics.Windows.Controls;
using Infragistics.Windows.DockManager;

using TradeStation.Infrastructure.Adapters;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Managers;
using TradeStation.Infrastructure.Views;

namespace TradeStationShell
{
    internal class TradeStationBootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new LogUtils();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();

            factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));

            return factory;
        }

        protected override void ConfigureAggregateCatalog()
        {
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(TradeStationBootstrapper).Assembly));

            // Load all DLL in application path.
            var catalog = new DirectoryCatalog(".");
            AggregateCatalog.Catalogs.Add(catalog);

        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();

            mappings.RegisterMapping(typeof(TabGroupPane), ServiceLocator.Current.GetInstance<IGItemsControlBasedRegionAdapter>());
            mappings.RegisterMapping(typeof(XamTabControl), ServiceLocator.Current.GetInstance<IGItemsControlBasedRegionAdapter>());

            return mappings;
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            var eventAggregator = new EventAggregator();

            // Create a thread to show and close splash screen.
            var thread = new Thread(() =>
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        TFSplashScreen splashScreen = new TFSplashScreen();

                        // Subscribe the event to close the splash.
                        eventAggregator.GetEvent<CloseSplashScreenEvent>().Subscribe(e =>
                        {
                            splashScreen.Dispatcher.BeginInvoke((Action)splashScreen.Close);
                        }, ThreadOption.PublisherThread, true);

                        splashScreen.Show();
                    }));

                Dispatcher.Run();
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // Show the text on splash screen.
            TFSplashScreenManager.Instance.Message = "正在初始化...";

            TimeKeeper.AppLaunched();

            base.Run(runWithDefaultConfiguration);

            // Raise the event to close the splash.
            eventAggregator.GetEvent<CloseSplashScreenEvent>().Publish(null);

            // Show the main window.
            Application.Current.MainWindow.Show();
        }
    }
}

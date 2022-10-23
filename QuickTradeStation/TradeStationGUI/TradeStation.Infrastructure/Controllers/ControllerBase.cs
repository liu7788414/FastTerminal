using System;
using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Infrastructure.Controllers
{
    public abstract class ControllerBase
    {
        #region Properties

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import]
        public IRegionManager RegionManager { get; set; }

        [Import]
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Constructors

        public ControllerBase()
        {
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            InitializeCommands();
            SubscribeEvents();
        }

        protected abstract void SubscribeEvents();

        protected abstract void InitializeCommands();

        public void AttachView(FrameworkElement viewObject, string regionName)
        {
            var region = RegionManager.Regions[regionName];

            if (null != region && !region.Views.Contains(viewObject))
            {
                region.Add(viewObject);
            }

            region.Activate(viewObject);
        }

        public void RemoveView(FrameworkElement viewObject, string regionName)
        {
            var region = RegionManager.Regions[regionName];

            if (null != region && region.Views.Contains(viewObject))
            {
                region.Remove(viewObject);
            }
        }

        #endregion
    }
}

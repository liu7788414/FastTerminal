using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System.ComponentModel.Composition;
using System.Windows.Input;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;

namespace TradeStation.Modules.Trade.ViewModels
{
    public abstract class TradeViewModelBase : BindableBase
    {
        public readonly IEventAggregator EventAggregator;

        [Import]
        public MarketDataService MarketDataService { get; set; }

        [Import]
        public UserSettings UserSettings { get; set; }

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public SecurityInfoMetadata SecurityInfoMetadata { get; set; }

        [Import]
        public HsStock Trader { get; set; }

        [Import]
        public MenubarViewModel MenuBar { get; set; }

        [ImportingConstructor]
        protected TradeViewModelBase(IEventAggregator eventAggr,SecurityCodeSearchProvider securitySearchProvider)
        {
            EventAggregator = eventAggr;
            SecuritySearchProvider = securitySearchProvider;
            ExportListCommand = new DelegateCommand(OnExportList);
        }

        protected abstract void SubscribeEvents();

        protected abstract void InitCommands();

        public ICommand AdvancedQueryCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ExportListCommand { get; set; }

        protected abstract void OnAdvancedQuery();
        protected abstract void OnRefresh();

        protected abstract void OnExportList();

        private SecurityCodeSearchProvider _securitySearchProvider;
        public SecurityCodeSearchProvider SecuritySearchProvider
        {
            get { return _securitySearchProvider; }
            set
            {
                if (_securitySearchProvider != value)
                {
                    SetProperty(ref _securitySearchProvider, value);
                }
            }
        }
    }
}

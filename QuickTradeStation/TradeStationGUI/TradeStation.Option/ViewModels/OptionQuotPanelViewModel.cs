using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Infrastructure.ViewModels;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Option.ViewModels
{
    [Export]
    public class OptionQuotPanelViewModel : ViewModelBase<OptionQuotPanel>
    {
        #region Private Fields

        private IEventAggregator _eventAggregator;

        private MarketDataService _marketDataService;
        private SecurityRefHttpService _securityRefHttpService;

        private DialogService _dialogService;

        #endregion

        #region Properties

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

        private SecurityInfo _selectedInputSecurityCode;
        public SecurityInfo SelectedInputSecurityCode
        {
            get { return _selectedInputSecurityCode; }
            set
            {
                if (_selectedInputSecurityCode != value)
                {
                    SetProperty(ref _selectedInputSecurityCode, value);
                }
            }
        }

        private SecurityInfo _selectedSecurityItem;
        public SecurityInfo SelectedSecurityItem
        {
            get { return _selectedSecurityItem; }
            set
            {
                SetProperty(ref _selectedSecurityItem, value);
            }
        }

        private string _selectedCombineNumber;
        public string SelectedCombineNumber
        {
            get { return _selectedCombineNumber; }
            set
            {
                SetProperty(ref _selectedCombineNumber, value);
            }
        }

        private ObservableCollection<RealTimeMarketDataPoint> _stockDataPoints;
        public ObservableCollection<RealTimeMarketDataPoint> StockDataPoints
        {
            get
            {
                return _stockDataPoints;
            }
            set
            {
                SetProperty(ref _stockDataPoints, value);
            }
        }

        private ObservableCollection<SecurityInfo> _securityInfoList;
        public ObservableCollection<SecurityInfo> SecurityInfoList
        {
            get { return _securityInfoList; }
            set
            {
                SetProperty(ref _securityInfoList, value);
            }
        }

        public MenubarViewModel MenubarViewModel { get; set; }

        #endregion

        #region Commands

        public ICommand AddSecurityToQuotListCommand { get; set; }
        public ICommand DoubleClickQuotCommand { get; set; }
        public ICommand SelectedSecurityChangedCommand { get; set; }
        public ICommand RemoveSecurityFromQuotListCommand { get; set; }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public OptionQuotPanelViewModel(
            OptionQuotPanel view,
            SecurityRefHttpService securityRefHttpService,
            MarketDataService marketDataService,
            DialogService dialogService,
            OptionSecurityCodeSearchProvider securitySearchProvider,
            IEventAggregator eventAggregator,
            MenubarViewModel menubarViewModel)
            : base(view)
        {
            _securityRefHttpService = securityRefHttpService;
            _marketDataService = marketDataService;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            MenubarViewModel = menubarViewModel;

            SecuritySearchProvider = securitySearchProvider;

            StockDataPoints = new ObservableCollection<RealTimeMarketDataPoint>();
            SecurityInfoList = new ObservableCollection<SecurityInfo>();

            View.DataContext = this;
            ((OptionQuotPanelViewModelProxy)View.Resources["ViewModelProxy"]).ViewModel = this;
        }

        #endregion
    }

    // For the removing button binding.
    public class OptionQuotPanelViewModelProxy
    {
        public OptionQuotPanelViewModel ViewModel { get; set; }
    }
}

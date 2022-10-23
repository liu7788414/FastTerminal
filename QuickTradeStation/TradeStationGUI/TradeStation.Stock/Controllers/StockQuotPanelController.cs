using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Infragistics.Windows.DockManager;

using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Controllers;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.ViewModels;
using TradeStation.Stock.ViewModels;
using TradeStation.Stock.Views;

namespace TradeStation.Stock.Controllers
{
    [Export]
    [Export(typeof(IReInitializable))]
    public class StockQuotPanelController : ControllerBase, IReInitializable
    {
        #region Private Fields

        private DialogService _dialogService;
        private MarketDataService _marketDataService;
        private SecurityRefHttpService _securityRefHttpService;

        private SaveLoadSecurityListHelper _saveLoadSecurityListHelper;

        private const string STOCK_LIST_FILE_NAME = "StockList.csv";
        private const string STOCK_LOCATION_LIST_FILE_NAME = "StockLocationList.csv";

        #endregion

        #region View Models

        public StockMainViewModel StockMainViewModel { get; set; }
        public StockQuotPanelViewModel QuotPanelViewModel { get; set; }

        public IList<StockRealTimePriceViewModel> StockRealTimePriceViewModelList { get; set; }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public StockQuotPanelController(
            StockMainViewModel stockMainViewModel,
            StockQuotPanelViewModel quotPanelViewModel,
            DialogService dialogService,
            MarketDataService marketDataService,
            SecurityRefHttpService securityRefHttpService,
            SaveLoadSecurityListHelper saveLoadSecurityListHelper)
        {
            _dialogService = dialogService;
            _marketDataService = marketDataService;
            _securityRefHttpService = securityRefHttpService;
            _saveLoadSecurityListHelper = saveLoadSecurityListHelper;

            StockMainViewModel = stockMainViewModel;
            QuotPanelViewModel = quotPanelViewModel;

            StockRealTimePriceViewModelList = new List<StockRealTimePriceViewModel>();
        }

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            AttachView(StockMainViewModel.View, RegionNames.MainTabPanelRegion);
            AttachView(QuotPanelViewModel.View, RegionNames.StockRealTimePricePanelRegion);

            LoadStockList();
        }

        protected override void SubscribeEvents()
        {
        }

        protected override void InitializeCommands()
        {
            QuotPanelViewModel.AddSecurityToQuotListCommand = new DelegateCommand(this.OnAddSecurityToQuotList);
            QuotPanelViewModel.RemoveSecurityFromQuotListCommand = new DelegateCommand(this.OnRemoveSecurityFromQuotList);
            QuotPanelViewModel.DoubleClickQuotCommand = new DelegateCommand(this.OnDoubleClickQuot);
            QuotPanelViewModel.SelectedSecurityChangedCommand = new DelegateCommand(this.OnSelectedSecurityChanged);

            // Handle the windows closing event.
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(SaveStockList));
        }

        public void DailyReInitialize()
        {
            DispatcherService.Invoke(() =>
            {
                RefreshSecurityInfoList();
            });
        }

        #endregion

        #region Private Methods

        private void OnAddSecurityToQuotList()
        {
            var selectedInputSecurityCode = QuotPanelViewModel.SelectedInputSecurityCode;

            if (selectedInputSecurityCode != null)
            {
                var existedCode = QuotPanelViewModel.SecurityInfoList.FirstOrDefault(x =>
                    x.ExID == selectedInputSecurityCode.ExID
                    && x.SecurityID == selectedInputSecurityCode.SecurityID);

                if (null != existedCode)
                {
                    _dialogService.ShowMessage("订阅失败", "证券已在订阅列表:" + existedCode.SecurityID);
                    return;
                }

                AddItemToQuotList(selectedInputSecurityCode);
            }
        }

        private void OnDoubleClickQuot()
        {
            var selectedSecurityItem = QuotPanelViewModel.SelectedSecurityItem;
            if (selectedSecurityItem.IsExpire)
            {
                if (_dialogService.ShowConfirmation("证券过期", "当前证券已经过期，点确认移除此证券"))
                {
                    this.OnRemoveSecurityFromQuotList();
                    return;
                }
                else
                {
                    return;
                }
            }

            ShowRealTimeView(selectedSecurityItem.ExID, selectedSecurityItem.SecurityID);
        }

        private void OnSelectedSecurityChanged()
        {
            EventAggregator.GetEvent<SelectDisplayStockQuotationEvent>().Publish(QuotPanelViewModel.SelectedSecurityItem);
        }

        private void AddItemToQuotList(SecurityInfo securityInfo)
        {
            // Subscribe the real time price information from market data service.
            _marketDataService.SubscribeSecQuot(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));

            if (!QuotPanelViewModel.SecurityInfoList.Any(x =>
                x.ExID == securityInfo.ExID
                && x.SecurityID == securityInfo.SecurityID))
            {
                // Add the security code information to the security list.
                QuotPanelViewModel.SecurityInfoList.Add(securityInfo);

                EventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity
                {
                    LogLevel = LogMessageLevel.INFO,
                    Message = "订阅行情代码:" + securityInfo.ExID + "." + securityInfo.SecurityID
                });
            }
        }

        private void OnRemoveSecurityFromQuotList()
        {
            var selectedSecurity = QuotPanelViewModel.SelectedSecurityItem;

            _marketDataService.UnSubscribeSecQuot(new ExSecID(selectedSecurity.ExID, selectedSecurity.SecurityID));
            QuotPanelViewModel.SecurityInfoList.Remove(selectedSecurity);
        }

        private void ShowRealTimeView(string exID, string securityID, Point? location = null)
        {
            var securityInfo = _marketDataService.GetSecurityInfo(exID, securityID);
            if (null == securityInfo || securityInfo.IsExpire)
            {
                return;
            }

            var selectedExSecID = new ExSecID(exID, securityID);

            var viewModel = StockRealTimePriceViewModelList.FirstOrDefault(x =>
                x.SecurityInfo.ExID == exID
                && x.SecurityInfo.SecurityID == securityID);

            var isNewViewModel = false;
            // Show the real time price diagram view.
            if (null == viewModel)
            {
                viewModel = ServiceLocator.GetInstance<StockRealTimePriceViewModel>();
                viewModel.Initialize(selectedExSecID);

                StockRealTimePriceViewModelList.Add(viewModel);
                isNewViewModel = true;
            }
            else
            {
                // TODO: Do not refresh when switch the chart?
                //viewModel.RefreshChart();
            }

            AttachView(viewModel.View, RegionNames.RootRealTimePricePanelRegion);

            // Hide the panel, before the initialize finished.
            viewModel.View.Visibility = Visibility.Visible;
            viewModel.View.Activate();

            // Popup the price panel, if it is new generated.
            if (isNewViewModel)
            {
                viewModel.InitializeRealTimeWindowStatus(false, location);
            }
        }

        // Save stock list into csv file.
        private void SaveStockList()
        {
            _saveLoadSecurityListHelper.SaveSecurityListToCSV(STOCK_LIST_FILE_NAME, QuotPanelViewModel.SecurityInfoList);

            SaveRealTimeWindowLocation();
        }

        // Load csv file into stock list.
        private void LoadStockList()
        {
            var securityList = _saveLoadSecurityListHelper.LoadSecurityListFromCSV(STOCK_LIST_FILE_NAME);

            if (securityList.Any())
            {
                foreach (var item in securityList)
                {
                    AddItemToQuotList(item);
                }
            }

            LoadRealTimeWindowLocation();
        }

        private void SaveRealTimeWindowLocation()
        {
            var panelLocationList = new List<SecurityPanelLocation>();

            foreach (var viewModel in StockRealTimePriceViewModelList)
            {
                if (viewModel.View.Visibility == Visibility.Visible)
                {
                    var contentPane = viewModel.View;
                    if (null != contentPane)
                    {
                        var paneParent = contentPane.Parent;

                        if (paneParent is SplitPane)
                        {
                            var splitPane = paneParent as SplitPane;
                            var location = XamDockManager.GetFloatingLocation(splitPane);

                            if (null != location)
                            {
                                panelLocationList.Add(new SecurityPanelLocation()
                                {
                                    ExSecID = new ExSecID(viewModel.SecurityInfo.ExID, viewModel.SecurityInfo.SecurityID),
                                    Location = location.Value,
                                });
                            }

                        }
                    }
                }
            }

            _saveLoadSecurityListHelper.SaveSecurityPanelLocationListToCSV(STOCK_LOCATION_LIST_FILE_NAME, panelLocationList);
        }

        private void LoadRealTimeWindowLocation()
        {
            var panelLocationList = _saveLoadSecurityListHelper.LoadSecurityPanelLocationList(STOCK_LOCATION_LIST_FILE_NAME);

            if (null != panelLocationList && panelLocationList.Any())
            {
                foreach (var panelLocation in panelLocationList)
                {
                    ShowRealTimeView(panelLocation.ExSecID.ExID, panelLocation.ExSecID.SecurityID, panelLocation.Location);
                }
            }
        }

        // Remove all security info from subscribed security info list,
        // then get new security info list from MarketDataService.
        private void RefreshSecurityInfoList()
        {
            var subscribedExSecIdList = new List<ExSecID>();
            foreach (var securityInfo in QuotPanelViewModel.SecurityInfoList)
            {
                subscribedExSecIdList.Add(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));
            }

            if (subscribedExSecIdList.Any())
            {
                QuotPanelViewModel.SecurityInfoList.Clear();

                foreach (var exSecId in subscribedExSecIdList)
                {
                    var securityInfo = _marketDataService.GetSecurityInfo(exSecId.ExID, exSecId.SecurityID);

                    if (null != securityInfo)
                    {
                        QuotPanelViewModel.SecurityInfoList.Add(securityInfo);
                    }
                }
            }
        }

        #endregion
    }
}

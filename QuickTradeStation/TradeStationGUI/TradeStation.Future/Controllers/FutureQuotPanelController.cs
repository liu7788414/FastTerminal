using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;

using Infragistics.Windows.DockManager;

using TradeStation.Future.ViewModels;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Controllers;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Future.Controllers
{
    [Export]
    [Export(typeof(IReInitializable))]
    public class FutureQuotPanelController : ControllerBase, IReInitializable
    {
        #region Private Fields

        private DialogService _dialogService;
        private MarketDataService _marketDataService;
        private SecurityRefHttpService _securityRefHttpService;

        private SaveLoadSecurityListHelper _saveLoadSecurityListHelper;

        private const string FUTURE_LIST_FILE_NAME = "FutureList.csv";
        private const string FUTURE_LOCATION_LIST_FILE_NAME = "FutureLocationList.csv";

        #endregion

        #region View Models

        public FutureMainViewModel FutureMainViewModel { get; set; }
        public FutureQuotPanelViewModel FutureQuotPanelViewModel { get; set; }

        public IList<FutureRealTimePriceViewModel> FutureRealTimePriceViewModelList { get; set; }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public FutureQuotPanelController(
            FutureMainViewModel futureMainViewModel,
            FutureQuotPanelViewModel futureQuotPanelViewModel,
            DialogService dialogService,
            MarketDataService marketDataService,
            SecurityRefHttpService securityRefHttpService,
            SaveLoadSecurityListHelper saveLoadSecurityListHelper)
        {
            _dialogService = dialogService;
            _marketDataService = marketDataService;
            _securityRefHttpService = securityRefHttpService;
            _saveLoadSecurityListHelper = saveLoadSecurityListHelper;

            FutureMainViewModel = futureMainViewModel;
            FutureQuotPanelViewModel = futureQuotPanelViewModel;

            FutureRealTimePriceViewModelList = new List<FutureRealTimePriceViewModel>();
        }

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            AttachView(FutureMainViewModel.View, RegionNames.MainTabPanelRegion);
            AttachView(FutureQuotPanelViewModel.View, RegionNames.FutureRealTimePricePanelRegion);

            this.FutureQuotPanelViewModel.Initialize();
            LoadFutureList();
        }

        protected override void SubscribeEvents()
        {
        }

        protected override void InitializeCommands()
        {
            FutureQuotPanelViewModel.AddSecurityToQuotListCommand = new DelegateCommand(this.OnAddSecurityToQuotList);
            FutureQuotPanelViewModel.DoubleClickQuotCommand = new DelegateCommand(this.OnDoubleClickQuot);
            FutureQuotPanelViewModel.SelectedSecurityChangedCommand = new DelegateCommand(this.OnSelectedSecurityChanged);
            FutureQuotPanelViewModel.RemoveSecurityFromQuotListCommand = new DelegateCommand(this.OnRemoveSecurityFromQuotList);

            // Handle the windows closing event.
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(this.SaveFutureList));
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
            var selectedInputSecurityCode = FutureQuotPanelViewModel.SelectedInputSecurityCode;

            if (selectedInputSecurityCode != null)
            {
                var existedCode = FutureQuotPanelViewModel.SecurityInfoList.FirstOrDefault(x =>
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
            var selectedSecurityItem = FutureQuotPanelViewModel.SelectedSecurityItem;
            if (selectedSecurityItem.IsExpire)
            {
                if (_dialogService.ShowConfirmation("期货过期", "当前期货已经过期，点确认移除此期货"))
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
            EventAggregator.GetEvent<SelectDisplayFutureQuotationEvent>().Publish(FutureQuotPanelViewModel.SelectedSecurityItem);
        }

        private void AddItemToQuotList(SecurityInfo securityInfo)
        {
            // Subscribe the real time price information from market data service.
            _marketDataService.SubscribeSecQuot(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));

            if (!FutureQuotPanelViewModel.SecurityInfoList.Any(x => x.ExID == securityInfo.ExID && x.SecurityID == securityInfo.SecurityID))
            {
                // Add the security code information to the security list.
                FutureQuotPanelViewModel.SecurityInfoList.Add(securityInfo);

                EventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity
                {
                    LogLevel = LogMessageLevel.INFO,
                    Message = "订阅行情代码:" + securityInfo.ExID + "." + securityInfo.SecurityID
                });
            }
        }

        private void OnRemoveSecurityFromQuotList()
        {
            var selectedSecurity = FutureQuotPanelViewModel.SelectedSecurityItem;

            _marketDataService.UnSubscribeSecQuot(new ExSecID(selectedSecurity.ExID, selectedSecurity.SecurityID));
            FutureQuotPanelViewModel.SecurityInfoList.Remove(selectedSecurity);
        }

        private void ShowRealTimeView(string exID, string securityID, Point? location = null)
        {
            var securityInfo = _marketDataService.GetSecurityInfo(exID, securityID);
            if (null == securityInfo || securityInfo.IsExpire)
            {
                return;
            }

            var selectedExSecID = new ExSecID(exID, securityID);

            var viewModel = FutureRealTimePriceViewModelList.FirstOrDefault(x =>
                x.SecurityInfo.ExID == exID
                && x.SecurityInfo.SecurityID == securityID);

            var isNewViewModel = false;
            // Show the real time price diagram view.
            if (null == viewModel)
            {
                viewModel = ServiceLocator.GetInstance<FutureRealTimePriceViewModel>();
                viewModel.Initialize(selectedExSecID);

                FutureRealTimePriceViewModelList.Add(viewModel);
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

        // Save future list into csv file.
        private void SaveFutureList()
        {
            _saveLoadSecurityListHelper.SaveSecurityListToCSV(FUTURE_LIST_FILE_NAME, FutureQuotPanelViewModel.SecurityInfoList);

            SaveRealTimeWindowLocation();
        }

        // Load csv file into future list.
        private void LoadFutureList()
        {
            var securityList = _saveLoadSecurityListHelper.LoadSecurityListFromCSV(FUTURE_LIST_FILE_NAME);

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

            foreach (var viewModel in FutureRealTimePriceViewModelList)
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

            _saveLoadSecurityListHelper.SaveSecurityPanelLocationListToCSV(FUTURE_LOCATION_LIST_FILE_NAME, panelLocationList);
        }

        private void LoadRealTimeWindowLocation()
        {
            var panelLocationList = _saveLoadSecurityListHelper.LoadSecurityPanelLocationList(FUTURE_LOCATION_LIST_FILE_NAME);

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
            foreach (var securityInfo in FutureQuotPanelViewModel.SecurityInfoList)
            {
                subscribedExSecIdList.Add(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));
            }

            if (subscribedExSecIdList.Any())
            {
                FutureQuotPanelViewModel.SecurityInfoList.Clear();

                foreach (var exSecId in subscribedExSecIdList)
                {
                    var securityInfo = _marketDataService.GetSecurityInfo(exSecId.ExID, exSecId.SecurityID);

                    if (null != securityInfo)
                    {
                        FutureQuotPanelViewModel.SecurityInfoList.Add(securityInfo);
                    }
                }
            }
        }

        #endregion
    }
}

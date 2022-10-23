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
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;
using TradeStation.Option.ViewModels;
using TradeStation.Future.ViewModels;

namespace TradeStation.Option.Controllers
{
    [Export]
    [Export(typeof(IReInitializable))]
    public class OptionQuotPanelController : ControllerBase, IReInitializable
    {
        #region Private Fields

        private DialogService _dialogService;
        private MarketDataService _marketDataService;
        private SecurityRefHttpService _securityRefHttpService;

        private SaveLoadSecurityListHelper _saveLoadSecurityListHelper;

        private const string OPTION_LIST_FILE_NAME = "OptionList.csv";
        private const string OPTION_LOCATION_LIST_FILE_NAME = "OptionLocationList.csv";

        #endregion

        #region View Models

        public OptionMainViewModel OptionMainViewModel { get; set; }
        public OptionQuotPanelViewModel OptionQuotPanelViewModel { get; set; }

        public IList<OptionRealTimePriceViewModel> OptionRealTimePriceViewModelList { get; set; }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public OptionQuotPanelController(
            OptionMainViewModel optionMainViewModel,
            OptionQuotPanelViewModel optionQuotPanelViewModel,
            DialogService dialogService,
            MarketDataService marketDataService,
            SecurityRefHttpService securityRefHttpService,
            SaveLoadSecurityListHelper saveLoadSecurityListHelper)
        {
            _dialogService = dialogService;
            _marketDataService = marketDataService;
            _securityRefHttpService = securityRefHttpService;
            _saveLoadSecurityListHelper = saveLoadSecurityListHelper;

            OptionMainViewModel = optionMainViewModel;
            OptionQuotPanelViewModel = optionQuotPanelViewModel;

            OptionRealTimePriceViewModelList = new List<OptionRealTimePriceViewModel>();
        }

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            AttachView(OptionMainViewModel.View, RegionNames.MainTabPanelRegion);
            AttachView(OptionQuotPanelViewModel.View, RegionNames.OptionRealTimePricePanelRegion);

            LoadOptionList();
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionInfoModelSelectedEvent>().Subscribe(OnOptionInfoModelSelected);
        }

        private void OnOptionInfoModelSelected(OptionInfoModel optionInfoModel)
        {
            ShowRealTimeView(optionInfoModel.ExID, optionInfoModel.SecurityID);
        }

        protected override void InitializeCommands()
        {
            OptionQuotPanelViewModel.AddSecurityToQuotListCommand = new DelegateCommand(this.OnAddSecurityToQuotList);
            OptionQuotPanelViewModel.DoubleClickQuotCommand = new DelegateCommand(this.OnDoubleClickQuot);
            OptionQuotPanelViewModel.SelectedSecurityChangedCommand = new DelegateCommand(this.OnSelectedSecurityChanged);
            OptionQuotPanelViewModel.RemoveSecurityFromQuotListCommand = new DelegateCommand(this.OnRemoveSecurityFromQuotList);

            // Handle the windows closing event.
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(this.SaveOptionList));
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
            var selectedInputSecurityCode = OptionQuotPanelViewModel.SelectedInputSecurityCode;

            if (selectedInputSecurityCode != null)
            {
                var existedCode = OptionQuotPanelViewModel.SecurityInfoList.FirstOrDefault(x =>
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
            var selectedSecurityItem = OptionQuotPanelViewModel.SelectedSecurityItem;
            if (selectedSecurityItem.IsExpire)
            {
                if (_dialogService.ShowConfirmation("期权过期", "当前期权已经过期，点确认移除此期权"))
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
            EventAggregator.GetEvent<SelectDisplayOptionQuotationEvent>().Publish(OptionQuotPanelViewModel.SelectedSecurityItem);
        }

        private void AddItemToQuotList(SecurityInfo securityInfo)
        {
            // Subscribe the real time price information from market data service.
            _marketDataService.SubscribeSecQuot(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));

            if (!OptionQuotPanelViewModel.SecurityInfoList.Any(x =>
                x.ExID == securityInfo.ExID
                && x.SecurityID == securityInfo.SecurityID))
            {
                // Add the security code information to the security list.
                OptionQuotPanelViewModel.SecurityInfoList.Add(securityInfo);

                EventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity
                {
                    LogLevel = LogMessageLevel.INFO,
                    Message = "订阅行情代码:" + securityInfo.ExID + "." + securityInfo.SecurityID
                });
            }
        }

        private void OnRemoveSecurityFromQuotList()
        {
            var selectedSecurity = OptionQuotPanelViewModel.SelectedSecurityItem;

            _marketDataService.UnSubscribeSecQuot(new ExSecID(selectedSecurity.ExID, selectedSecurity.SecurityID));
            OptionQuotPanelViewModel.SecurityInfoList.Remove(selectedSecurity);
        }

        private void ShowRealTimeView(string exID, string securityID, Point? location = null)
        {
            var securityInfo = _marketDataService.GetSecurityInfo(exID, securityID);
            if (null == securityInfo || securityInfo.IsExpire)
            {
                return;
            }

            var selectedExSecID = new ExSecID(exID, securityID);

            var viewModel = OptionRealTimePriceViewModelList.FirstOrDefault(x =>
                x.SecurityInfo.ExID == exID
                && x.SecurityInfo.SecurityID == securityID);

            var isNewViewModel = false;
            // Show the real time price diagram view.
            if (null == viewModel)
            {
                viewModel = ServiceLocator.GetInstance<OptionRealTimePriceViewModel>();
                viewModel.Initialize(selectedExSecID);

                OptionRealTimePriceViewModelList.Add(viewModel);
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

        // Save option list into csv file.
        private void SaveOptionList()
        {
            _saveLoadSecurityListHelper.SaveSecurityListToCSV(OPTION_LIST_FILE_NAME, OptionQuotPanelViewModel.SecurityInfoList);

            SaveRealTimeWindowLocation();
        }

        // Load csv file into option list.
        private void LoadOptionList()
        {
            var securityList = _saveLoadSecurityListHelper.LoadSecurityListFromCSV(OPTION_LIST_FILE_NAME);

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

            foreach (var viewModel in OptionRealTimePriceViewModelList)
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

            _saveLoadSecurityListHelper.SaveSecurityPanelLocationListToCSV(OPTION_LOCATION_LIST_FILE_NAME, panelLocationList);
        }

        private void LoadRealTimeWindowLocation()
        {
            var panelLocationList = _saveLoadSecurityListHelper.LoadSecurityPanelLocationList(OPTION_LOCATION_LIST_FILE_NAME);

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
            foreach (var securityInfo in OptionQuotPanelViewModel.SecurityInfoList)
            {
                subscribedExSecIdList.Add(new ExSecID(securityInfo.ExID, securityInfo.SecurityID));
            }

            if (subscribedExSecIdList.Any())
            {
                OptionQuotPanelViewModel.SecurityInfoList.Clear();

                foreach (var exSecId in subscribedExSecIdList)
                {
                    var securityInfo = _marketDataService.GetSecurityInfo(exSecId.ExID, exSecId.SecurityID);

                    if (null != securityInfo)
                    {
                        OptionQuotPanelViewModel.SecurityInfoList.Add(securityInfo);
                    }
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

using Infragistics.Windows.DockManager;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;
using TradeStation.Infrastructure.ViewModels;
using TradeStation.Modules.Trade.ViewModels;
using TradeStation.Modules.RealTimePrice.Views;

namespace TradeStation.Modules.RealTimePrice.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class RealTimePriceViewModelBase : ViewModelBase<RealTimePriceDiagramPanel>
    {
        #region Protected Fields

        private DateTime _latestGrabTime = DateTime.Now.Date.AddHours(15);
        private int _loadedCount = 200;

        protected IEventAggregator _eventAggregator;
        protected SecurityRefHttpService _securityRefHttpService;
        protected SecurityInfoMetadata _securityInfoMetadata;
        protected MarketDataService _marketDataService;
        protected DialogService _dialogService;
        protected RealTimeDataProcessor _realTimeDataProcessor;

        protected HsStock _tradeInfo;

        #endregion

        #region Properties

        public MenubarViewModel MenubarViewModel { get; set; }

        // 是否展开
        private bool _isExpandedChart;
        public bool IsExpandedChart
        {
            get { return _isExpandedChart; }
            set
            {
                SetProperty(ref _isExpandedChart, value);
            }
        }

        // 快速下单的投资类型
        private eInvestType _selectedInvestType;
        public eInvestType SelectedInvestType
        {
            get { return _selectedInvestType; }
            set
            {
                SetProperty(ref _selectedInvestType, value);
            }
        }

        // 快速下单的组合编号
        private string _selectedCombineNumber;
        public string SelectedCombineNumber
        {
            get { return _selectedCombineNumber; }
            set
            {
                SetProperty(ref _selectedCombineNumber, value);
            }
        }

        // 快速下单的委托数量
        private int _entrustAmount;
        public int EntrustAmount
        {
            get { return _entrustAmount; }
            set
            {
                SetProperty(ref _entrustAmount, value);
            }
        }

        // 证券最小下单单位
        private int _minEntrustChangeAmount;
        public int MinEntrustChangeAmount
        {
            get { return _minEntrustChangeAmount; }
            set
            {
                SetProperty(ref _minEntrustChangeAmount, value);
            }
        }

        private SecurityInfo _securityInfo;
        public SecurityInfo SecurityInfo
        {
            get { return _securityInfo; }
            set
            {
                SetProperty(ref _securityInfo, value);
            }
        }

        #region Real Time Chart Related

        private bool _isRealTimePrice;
        public bool IsRealTimePrice
        {
            get { return _isRealTimePrice; }
            set
            {
                SetProperty(ref _isRealTimePrice, value);
            }
        }

        private TimeSpan _realTimeChartStrokeInterval;
        public TimeSpan RealTimeChartStrokeInterval
        {
            get { return _realTimeChartStrokeInterval; }
            set
            {
                SetProperty(ref _realTimeChartStrokeInterval, value);
            }
        }

        private TimeSpan _realTimeChartTotalTime;
        public TimeSpan RealTimeChartTotalTime
        {
            get { return _realTimeChartTotalTime; }
            set
            {
                SetProperty(ref _realTimeChartTotalTime, value);
            }
        }

        private MarketPeriodRangeModel _minMaxTradingTime;
        public MarketPeriodRangeModel MinMaxTradingTime
        {
            get { return _minMaxTradingTime; }
            set
            {
                SetProperty(ref _minMaxTradingTime, value);
            }
        }

        private IList<MarketPeriodRangeModel> _tradingTimes;
        public IList<MarketPeriodRangeModel> TradingTimes
        {
            get { return _tradingTimes; }
            set
            {
                SetProperty(ref _tradingTimes, value);
            }
        }

        private IList<DateTime> _overlapLines;
        public IList<DateTime> OverlapLines
        {
            get { return _overlapLines; }
            set
            {
                SetProperty(ref _overlapLines, value);
            }
        }

        private RealTimeMarketData _RealTimeMarketData;
        public RealTimeMarketData RealTimeMarketData
        {
            get { return _RealTimeMarketData; }
            set
            {
                SetProperty(ref _RealTimeMarketData, value);
            }
        }

        #endregion

        #region K-line Chart Related

        private eDisplayedKLinePeriodType _kLinePeriodType;
        public eDisplayedKLinePeriodType KLinePeriodType
        {
            get { return _kLinePeriodType; }
            set
            {
                SetProperty(ref _kLinePeriodType, value);
            }
        }

        private int _selectedCount;
        public int SelectedCount
        {
            get { return _selectedCount; }
            set
            {
                SetProperty(ref _selectedCount, value);
            }
        }

        private RealTimeMarketData _kLineMarketData;
        public RealTimeMarketData KLineMarketData
        {
            get { return _kLineMarketData; }
            set
            {
                SetProperty(ref _kLineMarketData, value);
            }
        }

        #endregion

        protected abstract double FloatingPaneWidth { get; }
        protected abstract double FloatingPaneHeight { get; }
        protected abstract double FloatingExpandedPaneWidth { get; }
        protected abstract double FloatingExpandedPaneHeight { get; }

        #endregion

        #region Commands

        public ICommand PeriodChangedCommand { get; set; }
        public ICommand ExRightChangedCommand { get; set; }
        public ICommand IsVisibleResolvedChangedCommand { get; set; }
        public ICommand DiagramTypeChangedCommand { get; set; }
        public ICommand ExpandedChartButtonClickCommand { get; set; }
        public ICommand ZoomChangedCommand { get; set; }

        public ICommand CombineNoSelectionChangedCommand { get; set; }
        public ICommand QuickEntrustAmountCommand { get; set; }
        public ICommand FastBuyCommand { get; set; }
        public ICommand FastSellCommand { get; set; }

        #endregion

        #region Constructor

        public RealTimePriceViewModelBase(
            RealTimePriceDiagramPanel view,
            SecurityRefHttpService securityRefHttpService,
            SecurityInfoMetadata securityInfoMetadata,
            MarketDataService marketDataService,
            DialogService dialogService,
            RealTimeDataProcessor realTimeDataProcessor,
            IEventAggregator eventAggregator,
            HsStock tradeInfo,
            MenubarViewModel menubarViewModel)
            : base(view)
        {
            this.View.DataContext = this;

            _eventAggregator = eventAggregator;
            _securityRefHttpService = securityRefHttpService;
            _securityInfoMetadata = securityInfoMetadata;
            _marketDataService = marketDataService;
            _dialogService = dialogService;
            _realTimeDataProcessor = realTimeDataProcessor;
            _tradeInfo = tradeInfo;

            MenubarViewModel = menubarViewModel;

            PeriodChangedCommand = new DelegateCommand(this.OnPeriodChanged);
            ExRightChangedCommand = new DelegateCommand(this.OnExRightChanged);
            FastBuyCommand = new DelegateCommand<double?>(this.OnFastBuyCommand);
            FastSellCommand = new DelegateCommand<double?>(this.OnFastSellCommand);
            QuickEntrustAmountCommand = new DelegateCommand<string>(this.OnQuickEntrustAmountCommand);
            CombineNoSelectionChangedCommand = new DelegateCommand(this.OnCombineNoSelectionChangedCommand);
            IsVisibleResolvedChangedCommand = new DelegateCommand(this.OnIsVisibleResolvedChanged);
            DiagramTypeChangedCommand = new DelegateCommand(this.OnDiagramTypeChangedCommand);
            ExpandedChartButtonClickCommand = new DelegateCommand(this.OnExpandedChartButtonClickCommand);
            ZoomChangedCommand = new DelegateCommand(this.OnZoomChangedCommand);

            ((RealTimePriceViewModelBaseProxy)View.Resources["ViewModelProxy"]).ViewModel = this;
        }

        #endregion

        #region Initial Methods

        public virtual void Initialize(ExSecID selectedExSecID)
        {
            this.SecurityInfo = _marketDataService.GetSecurityInfo(selectedExSecID.ExID, selectedExSecID.SecurityID);

            // Initialize real time & k line data.
            var exSecID = new ExSecID(SecurityInfo.ExID, SecurityInfo.SecurityID);

            if (!_marketDataService.RealTimeDataMap.ContainsKey(exSecID))
            {
                // Generate real time market data model.
                _marketDataService.RealTimeDataMap[exSecID] = new RealTimeMarketData(exSecID);

                #region Get default period list
                IList<MarketPeriodRangeModel> periodList;
                if (!_securityInfoMetadata.ExchangeTradePeriodDictionary.TryGetValue(SecurityInfo.Variety, out periodList))
                {
                    periodList = new List<MarketPeriodRangeModel>();
                    periodList.Add(new MarketPeriodRangeModel()
                    {
                        StartTime = DateTime.Now.Date.AddHours(9).AddMinutes(30),
                        EndTime = DateTime.Now.Date.AddHours(11).AddMinutes(30),
                    });
                    periodList.Add(new MarketPeriodRangeModel()
                    {
                        StartTime = DateTime.Now.Date.AddHours(13).AddMinutes(0),
                        EndTime = DateTime.Now.Date.AddHours(15).AddMinutes(0),
                    });
                }
                #endregion

                _marketDataService.RealTimeDataMap[exSecID].InitializeDataForRealTime(periodList);
            }

            if (!_marketDataService.KLineMarketDataMap.ContainsKey(exSecID))
            {
                // Generate k line market data.
                _marketDataService.KLineMarketDataMap[exSecID] = new Dictionary<eKLinePeriodType, RealTimeMarketData>();

                var allKLineTypes = Enum.GetValues(typeof(eKLinePeriodType)).OfType<eKLinePeriodType>().ToList();
                // Initialize the real time data dictionary for subscription security.
                foreach (var kLineType in allKLineTypes)
                {
                    _marketDataService.KLineMarketDataMap[exSecID][kLineType] = new RealTimeMarketData(exSecID);
                }
            }

            this.RealTimeMarketData = _marketDataService.RealTimeDataMap[selectedExSecID];
            this.RealTimeMarketData.PreClosePrice = SecurityInfo.LastCpx;
            this.RealTimeMarketData.IsOpen = true;
            this.MinEntrustChangeAmount = 1;
            if (this.SecurityInfo.SecurityType == eCategory.股票
                || this.SecurityInfo.SecurityType == eCategory.基金
                || this.SecurityInfo.SecurityType == eCategory.债券
                || this.SecurityInfo.SecurityType == eCategory.债券回购)
            {
                this.MinEntrustChangeAmount = 100;
            }
            IntializeRealTimeChart();

            //this.RealTimeMarketData.PropertyChanged += RealTimeMarketData_PropertyChanged;

            this.KLineMarketData = _marketDataService.KLineMarketDataMap[selectedExSecID][(eKLinePeriodType)KLinePeriodType];

            // Create a unique name for each view.
            this.View.Name = selectedExSecID.ExID + selectedExSecID.SecurityID.Replace("-", "");

            if (null != this.MenubarViewModel.CombiNos
                && this.MenubarViewModel.CombiNos.Count > 0)
            {
                this.SelectedCombineNumber = this.MenubarViewModel.CombiNos.First();
            }

            RefreshHoldingPosition();

            // Async. To grab today historical data from redis service.
            Task.Run(() => FillRealTimeDataToCollection(selectedExSecID, this.RealTimeMarketData, eKLinePeriodType.MIN1, eExRightType.NORMAL));

            // Async. To grab k-line historical data from redis service.
            Task.Run(() => FillKLineDataToCollection(selectedExSecID, this.KLineMarketData, (eKLinePeriodType)this.KLinePeriodType, (eExRightType)this.KLineMarketData.ExRightType));
        }

        #endregion

        #region Private Methods

        private void IntializeRealTimeChart()
        {
            this.RealTimeChartStrokeInterval = new TimeSpan(0, 30, 0);

            MarketPeriodRangeModel minMaxTime;
            IList<MarketPeriodRangeModel> tradingTimes;

            if (_securityInfoMetadata.MarketVarietyMaxMinTradeTime.TryGetValue(SecurityInfo.Variety, out minMaxTime)
                && _securityInfoMetadata.ExchangeTradePeriodDictionary.TryGetValue(SecurityInfo.Variety, out tradingTimes))
            {
                this.MinMaxTradingTime = minMaxTime;
                this.TradingTimes = tradingTimes;
            }
            else
            {
                this.MinMaxTradingTime = new MarketPeriodRangeModel()
                {
                    StartTime = DateTime.Now.Date.AddHours(9).AddMinutes(30),
                    EndTime = DateTime.Now.Date.AddHours(15),
                };

                this.TradingTimes = new List<MarketPeriodRangeModel>();
                this.TradingTimes.Add(new MarketPeriodRangeModel()
                {
                    StartTime = DateTime.Now.Date.AddHours(9).AddMinutes(30),
                    EndTime = DateTime.Now.Date.AddHours(11).AddMinutes(30),
                });
                this.TradingTimes.Add(new MarketPeriodRangeModel()
                {
                    StartTime = DateTime.Now.Date.AddHours(13),
                    EndTime = DateTime.Now.Date.AddHours(15),
                });
            }

            this.RealTimeChartTotalTime = new TimeSpan(0);
            foreach (var time in this.TradingTimes)
            {
                RealTimeChartTotalTime += time.Interval;
            }
        }

        private async void FillRealTimeDataToCollection(ExSecID selectedExSecID, RealTimeMarketData targetToFill, eKLinePeriodType periodType, eExRightType exRightType)
        {
            // Always gets the current day's data in trading time.
            IList<KLineRecordModel> kLineDataList = await _securityRefHttpService.GetKLineRecordsWithStartEnd(
                selectedExSecID,
                periodType,
                this.MinMaxTradingTime.StartTime,
                this.MinMaxTradingTime.EndTime,
                eExRightType.NORMAL
            ).ConfigureAwait(false);
            if (null != kLineDataList && kLineDataList.Count > 0)
            {
                this.View.Dispatcher.Invoke(() =>
                {
                    _realTimeDataProcessor.UpdateRealTimeDataByHistoryData(targetToFill, this.SecurityInfo, kLineDataList);

                    RealTimePriceDateTimeConvertHelper.Instance.GenerateTimeFromOpen(targetToFill);

                    targetToFill.IsDataReady = true;
                });
            }
        }

        private async void FillKLineDataToCollection(ExSecID selectedExSecID, RealTimeMarketData targetToFill, eKLinePeriodType periodType, eExRightType exRightType, int count = 500, DateTime? endTime = null)
        {
            var realEndTime = endTime == null ? DateTime.Now.Date.AddHours(15) : endTime.Value;

            // Always gets the current day's data in trading time.
            // TODO: The last time need be changed.
            IList<KLineRecordModel> kLineDataList = await _securityRefHttpService.GetKLineRecordsWithCount(
                selectedExSecID,
                periodType,
                count,
                realEndTime,
                eExRightType.NORMAL
            ).ConfigureAwait(false);
            if (null != kLineDataList && kLineDataList.Count > 0)
            {
                this.View.Dispatcher.Invoke(() =>
                {
                    //targetToFill.RealTimeMarketDataPointSets.Clear();
                    // TODO: Change the trading date get method.
                    _realTimeDataProcessor.UpdateRealTimeDataByHistoryData(targetToFill, this.SecurityInfo, kLineDataList);
                    RefreshKLinePriceRatio();

                    if (null != kLineDataList && kLineDataList.Any())
                    {
                        _latestGrabTime = kLineDataList.Min(x => x.ExDateTime);
                    }
                    targetToFill.IsDataReady = true;
                });
            }
        }

        private void RealTimeMarketData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "LastTickData"
            //    && null != this.RealTimeMarketData.LastTickData
            //    && this.View.IsVisibleResolved)
            //{
            //    RefreshKLineChart();
            //}
        }

        private void RefreshKLineChart()
        {
            var selectedExSecID = new ExSecID(this.SecurityInfo.ExID, this.SecurityInfo.SecurityID);

            var targetChart = _marketDataService.KLineMarketDataMap[selectedExSecID][(eKLinePeriodType)KLinePeriodType];

            if (!targetChart.IsDataReady)
            {
                Task.Run(() => FillKLineDataToCollection(selectedExSecID, targetChart, (eKLinePeriodType)this.KLinePeriodType, (eExRightType)this.KLineMarketData.ExRightType));
            }

            this.KLineMarketData = targetChart;
        }

        private void OnPeriodChanged()
        {
            RefreshKLineChart();
        }

        private void OnExRightChanged()
        {
            RefreshKLineChart();

            RefreshKLinePriceRatio();
        }

        private void OnChartPreviewMouseWheelCommand()
        {
            // Do nothing.
        }

        private void OnIsVisibleResolvedChanged()
        {
            RefreshKLineChart();
        }

        private void OnDiagramTypeChangedCommand()
        {
            RefreshChart();
        }

        private void OnQuickEntrustAmountCommand(string commandParameter)
        {
            int entrustAmount = 0;
            if (!string.IsNullOrEmpty(commandParameter)
                && int.TryParse(commandParameter, out entrustAmount)
                && entrustAmount >= 0)
            {
                this.EntrustAmount = entrustAmount;
            }
        }

        private void RefreshKLinePriceRatio()
        {
            var selectedExSecID = new ExSecID(this.SecurityInfo.ExID, this.SecurityInfo.SecurityID);

            IList<ExrightRatioModel> ratios;
            if (!_securityInfoMetadata.ExrightRatiosMap.TryGetValue(selectedExSecID, out ratios))
            {
                ratios = new List<ExrightRatioModel>();
            }

            int skipCount = 0;

            for (int ix = KLineMarketData.RealTimeMarketDataPointSets.Count - 1; ix >= 0; ix--)
            {
                while (true)
                {
                    var ratio = ratios.Skip(skipCount).FirstOrDefault();

                    if (null == ratio)
                    {
                        KLineMarketData.RealTimeMarketDataPointSets[ix].PriceRatio = 1;

                        break;
                    }
                    else if (KLineMarketData.RealTimeMarketDataPointSets[ix].ExchangeTime >= ratio.TradeDate)
                    {
                        if (this.KLineMarketData.ExRightType == eDisplayedExRightType.前复权)
                        {
                            KLineMarketData.RealTimeMarketDataPointSets[ix].PriceRatio = ratio.ForwardFactor;
                        }
                        else if (this.KLineMarketData.ExRightType == eDisplayedExRightType.后复权)
                        {
                            KLineMarketData.RealTimeMarketDataPointSets[ix].PriceRatio = ratio.BackFactor;
                        }
                        else
                        {
                            KLineMarketData.RealTimeMarketDataPointSets[ix].PriceRatio = 1;
                        }

                        break;
                    }
                    else
                    {
                        skipCount++;
                    }
                }
            }
        }

        private void ResizeFloatingPane(ContentPane contentPane, Size size)
        {
            if (null != contentPane)
            {
                var paneParent = contentPane.Parent;

                if (paneParent is SplitPane)
                {
                    var splitPane = paneParent as SplitPane;
                    XamDockManager.SetFloatingSize(splitPane, size);
                }
            }
        }

        private void SetResizeFunction(ContentPane contentPane, bool isEnable)
        {
            if (null != contentPane)
            {
                var paneParent = contentPane.Parent;

                if (paneParent is SplitPane)
                {
                    var splitPane = paneParent as SplitPane;

                    if (null != splitPane && splitPane.Parent is PaneToolWindow)
                    {
                        var toolWindow = splitPane.Parent as PaneToolWindow;

                        toolWindow.ResizeMode = isEnable ? ResizeMode.CanResize : ResizeMode.NoResize;
                        toolWindow.AllowMaximize = isEnable;
                    }
                }
            }
        }

        private void SetPaneLocation(ContentPane contentPane, Point? point)
        {
            if (null != contentPane)
            {
                var paneParent = contentPane.Parent;

                if (paneParent is SplitPane && null != point)
                {
                    var splitPane = paneParent as SplitPane;
                    XamDockManager.SetFloatingLocation(splitPane, point);
                }
            }
        }

        protected virtual void OnExpandedChartButtonClickCommand()
        {
            SetRealTimeWindowStatus(this.IsExpandedChart, null);
        }

        private void SetRealTimeWindowStatus(bool isExpandedChart, Point? windowLocation = null)
        {
            this.IsExpandedChart = isExpandedChart;

            // Set the real time window floating only.
            this.View.ExecuteCommand(ContentPaneCommands.ChangeToFloatingOnly);

            // If it does not expand its chart,
            // then it need change to small size and un-resizable.
            if (!isExpandedChart)
            {
                ResizeFloatingPane(this.View, new Size(FloatingPaneWidth, FloatingPaneHeight));
                SetResizeFunction(this.View, false);
            }
            // If it expands its chart,
            // then it need change to big size and resizable.
            else
            {
                ResizeFloatingPane(this.View, new Size(FloatingExpandedPaneWidth, FloatingExpandedPaneHeight));
                SetResizeFunction(this.View, true);
            }

            if (null != windowLocation)
            {
                SetPaneLocation(this.View, windowLocation);
            }
        }

        protected void OnZoomChangedCommand()
        {
            if (this.View.xmZoombar.Range.Minimum == 0)
            {
                var selectedExSecID = new ExSecID(this.SecurityInfo.ExID, this.SecurityInfo.SecurityID);
                var targetChart = _marketDataService.KLineMarketDataMap[selectedExSecID][(eKLinePeriodType)KLinePeriodType];
                var distance = this.View.xmZoombar.Range.Maximum - this.View.xmZoombar.Range.Minimum;

                Task.Run(() => FillKLineDataToCollection(selectedExSecID, targetChart, (eKLinePeriodType)this.KLinePeriodType, (eExRightType)this.KLineMarketData.ExRightType, _loadedCount, _latestGrabTime));

                this.View.xmZoombar.Range.Minimum = 0.1;
                this.View.xmZoombar.Range.Maximum = 0.1 + distance;
            }
        }

        protected abstract void OnCombineNoSelectionChangedCommand();

        protected abstract void RefreshHoldingPosition();

        protected abstract void OnFastBuyCommand(double? price);

        protected abstract void OnFastSellCommand(double? price);

        #endregion

        public void RefreshChart()
        {
            if (IsRealTimePrice)
            {
                //RefreshRealTimeChart();
            }
            else
            {
                //RefreshKLineChart();
            }
        }

        public void InitializeRealTimeWindowStatus(bool isExpanded, Point? windowLocation = null)
        {
            SetRealTimeWindowStatus(isExpanded, windowLocation);
        }
    }

    // For the removing button binding.
    public class RealTimePriceViewModelBaseProxy
    {
        public RealTimePriceViewModelBase ViewModel { get; set; }
    }
}

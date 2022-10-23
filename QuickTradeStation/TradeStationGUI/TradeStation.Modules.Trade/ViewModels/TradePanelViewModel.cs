using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;

namespace TradeStation.Modules.Trade.ViewModels
{
    public abstract class TradePanelViewModelBase : TradeViewModelBase
    {
        [Import]
        protected MarketDataService MarketDataService { get; set; }

        [Import]
        protected DialogService DialogService { get; set; }

        #region Bindable Properties

        private eEntrustDirection _entrustDirection;

        public eEntrustDirection EntrustDirection
        {
            get { return _entrustDirection; }
            set { SetProperty(ref _entrustDirection, value); }
        }

        private eEntrustPriceType _entrustPriceType;

        public eEntrustPriceType EntrustPriceType
        {
            get { return _entrustPriceType; }
            set { SetProperty(ref _entrustPriceType, value); }
        }

        private string _selectedCombiNo;

        public string SelectedCombiNo
        {
            get { return _selectedCombiNo; }
            set { SetProperty(ref _selectedCombiNo, value); }
        }

        private double _entrustPrice;

        public double EntrustPrice
        {
            get { return _entrustPrice; }
            set { SetProperty(ref _entrustPrice, value); }
        }

        private int _entrustAmount;

        public int EntrustAmount
        {
            get { return _entrustAmount; }
            set { SetProperty(ref _entrustAmount, value); }
        }

        private eInvestType _investType;

        public eInvestType InvestType
        {
            get { return _investType; }
            set { SetProperty(ref _investType, value); }
        }

        private SecurityInfo _displaySecurityInfo;

        public SecurityInfo DisplaySecurityInfo
        {
            get { return _displaySecurityInfo; }
            set { SetProperty(ref _displaySecurityInfo, value); }
        }

        private eFuturesDirection _futuresDirection;

        public eFuturesDirection FuturesDirection
        {
            get { return _futuresDirection; }
            set { SetProperty(ref _futuresDirection, value); }
        }

        private ePurchaseWay _purchaseWay;

        public ePurchaseWay PurchaseWay
        {
            get { return _purchaseWay; }
            set { SetProperty(ref _purchaseWay, value); }
        }

        private double _entrustBalance;

        public double EntrustBalance
        {
            get { return _entrustBalance; }
            set { SetProperty(ref _entrustBalance, value); }
        }

        #endregion

        #region Commands

        public ICommand NewEntrustCommand { get; set; }
        public ICommand TestCommand { get; set; }

        #endregion

        [ImportingConstructor]
        protected TradePanelViewModelBase(IEventAggregator eventAgg,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }

        protected void NewEntrust(EntrustInfo ei)
        {
            var items = new List<MessageBoxItem>();
            MessageBoxItem item;
            WinDialog win;

            switch (ei.Category)
            {
                case eCategory.股票:
                {
                    if (ei.EntrustAmount > UserSettings.AmountLimitPerTradeStock)
                    {
                        item = new MessageBoxItem
                        {
                            Title = "错误",
                            Content = string.Format("单笔股票委托数量不得超过{0}!", UserSettings.AmountLimitPerTradeStock),
                            TextBrush = System.Windows.Media.Brushes.Red
                        };
                        items.Add(item);
                        win = new WinDialog(items, SystemIcons.Error.ToImageSource(), MessageBoxButton.OK);
                        win.ShowDialog();
                        return;
                    }
                    break;
                }
                case eCategory.期货:
                {
                    if (ei.EntrustAmount > UserSettings.AmountLimitPerTradeFuture)
                    {
                        item = new MessageBoxItem
                        {
                            Title = "错误",
                            Content = string.Format("单笔期货委托数量不得超过{0}!", UserSettings.AmountLimitPerTradeFuture),
                            TextBrush = System.Windows.Media.Brushes.Red
                        };
                        items.Add(item);
                        win = new WinDialog(items, SystemIcons.Error.ToImageSource(), MessageBoxButton.OK);
                        win.ShowDialog();
                        return;
                    }
                    break;
                }
                case eCategory.期权:
                {
                    if (ei.EntrustAmount > UserSettings.AmountLimitPerTradeOption)
                    {
                        item = new MessageBoxItem
                        {
                            Title = "错误",
                            Content = String.Format("单笔期权委托数量不得超过{0}!", UserSettings.AmountLimitPerTradeOption),
                            TextBrush = System.Windows.Media.Brushes.Red
                        };
                        items.Add(item);
                        win = new WinDialog(items, SystemIcons.Error.ToImageSource(), MessageBoxButton.OK);
                        win.ShowDialog();
                        return;
                    }
                    break;
                }
            }

            if (ei.EntrustAmount*ei.EntrustPrice > UserSettings.MoneyLimitPerTrade)
            {
                item = new MessageBoxItem
                {
                    Title = "错误",
                    Content = String.Format("单笔委托金额上限不得超过{0}!", UserSettings.MoneyLimitPerTrade),
                    TextBrush = System.Windows.Media.Brushes.Red
                };
                items.Add(item);
                win = new WinDialog(items, SystemIcons.Error.ToImageSource(), MessageBoxButton.OK);
                win.ShowDialog();
                return;
            }

            item = new MessageBoxItem
            {
                Title = "证券代码",
                Content = ei.SecurityID,
                TextBrush = System.Windows.Media.Brushes.Black
            };
            items.Add(item);

            item = new MessageBoxItem
            {
                Title = "交易市场",
                Content = ei.MarketType.ToString(),
                TextBrush = System.Windows.Media.Brushes.Black
            };
            items.Add(item);

            item = new MessageBoxItem
            {
                Title = "委托方向",
                Content = ei.EntrustDirection.ToString(),
                TextBrush =
                    ei.EntrustDirection == eEntrustDirection.买入
                        ? System.Windows.Media.Brushes.Red
                        : System.Windows.Media.Brushes.Green
            };
            items.Add(item);

            if (ei.CombiNo != null)
            {
                item = new MessageBoxItem
                {
                    Title = "组合编号",
                    Content = ei.CombiNo,
                    TextBrush = System.Windows.Media.Brushes.Black
                };
                items.Add(item);
            }
            else
            {
                MessageBox.Show("请先选择组合编号！");
                return;
            }

            if (ei is FutureEntrustInfo || ei is OptionEntrustInfo)
            {
                item = new MessageBoxItem
                {
                    Title = "开平仓",
                    Content = ei.FuturesDirection.ToString(),
                    TextBrush =
                        ei.FuturesDirection == eFuturesDirection.开仓
                            ? System.Windows.Media.Brushes.Red
                            : System.Windows.Media.Brushes.Green
                };
                items.Add(item);

                item = new MessageBoxItem
                {
                    Title = "投资类型",
                    Content = ei.InvestType.ToString(),
                    TextBrush = System.Windows.Media.Brushes.Black
                };
                items.Add(item);
            }

            if (ei is FundEntrustInfo)
            {
                item = new MessageBoxItem
                {
                    Title = "委托金额",
                    Content = ei.EntrustBalance.ToString(CultureInfo.InvariantCulture),
                    TextBrush = System.Windows.Media.Brushes.Black
                };
                items.Add(item);

                item = new MessageBoxItem
                {
                    Title = "申赎方式",
                    Content = ei.PurchaseWay.ToString(),
                    TextBrush = System.Windows.Media.Brushes.Black
                };
                items.Add(item);
            }

            if (!(ei is FundEntrustInfo))
            {
                item = new MessageBoxItem
                {
                    Title = "价格类型",
                    Content = ei.EntrustPriceType.ToString(),
                    TextBrush = System.Windows.Media.Brushes.Black
                };
                items.Add(item);
            }

            item = new MessageBoxItem
            {
                Title = "委托价格",
                Content = ei.EntrustPrice.ToString(CultureInfo.InvariantCulture),
                TextBrush = System.Windows.Media.Brushes.Black,
                IsEditable = true,
            };
            items.Add(item);
            var priceTitle = item.Title;

            item = new MessageBoxItem
            {
                Title = "委托数量",
                Content = ei.EntrustAmount.ToString(),
                TextBrush = System.Windows.Media.Brushes.Black,
                IsEditable = true,
            };
            items.Add(item);
            var amountTitle = item.Title;

            if (UserSettings.IsPromptEnabled)
            {
                win = new WinDialog(items, SystemIcons.Warning.ToImageSource()) {Title = "确认发出委托吗？"};

                var showDialog = win.ShowDialog();
                if (showDialog == null || !showDialog.Value)
                {
                    return;
                }
                else
                {
                    // Apply the new value on confirmation window.
                    var editableItems = items.Where(x => x.IsEditable);
                    var newPriceContent = items.FirstOrDefault(x => x.Title == priceTitle);
                    var newAmountContent = items.FirstOrDefault(x => x.Title == amountTitle);

                    double newPrice = 0;
                    int newAmount = 0;

                    if (editableItems.Count() > 0
                        && null != newPriceContent && null != newAmountContent
                        && double.TryParse(newPriceContent.Content, out newPrice)
                        && int.TryParse(newAmountContent.Content, out newAmount))
                    {
                        ei.EntrustPrice = newPrice;
                        ei.EntrustAmount = newAmount;
                    }
                }
            }

            Entrust entrust;

            if (ei is FundEntrustInfo)
            {
                entrust = new FundEntrust();
            }
            else if (ei is OptionEntrustInfo)
            {
                entrust = new OptionEntrust();
            }
            else if (ei is FutureEntrustInfo)
            {
                entrust = new FutureEntrust();
            }
            else
            {
                entrust = new Entrust();
            }

            entrust.StockCode = ei.SecurityID;
            entrust.MarketNo = CommonUtil.eMarketTypeToeMarketNo(ei.MarketType);
            entrust.CombiNo = ei.CombiNo;
            entrust.EntrustDirection = CommonUtil.eEntrustDirectionToEntrustDirection(ei.EntrustDirection);
            entrust.PriceType = CommonUtil.eEntrustPriceTypeToEntrustPriceType(ei.EntrustPriceType);
            entrust.EntrustPrice = ei.EntrustPrice;
            entrust.EntrustAmount = ei.EntrustAmount;
            entrust.FuturesDirection = CommonUtil.eFuturesDirectionToFuturesDirection(ei.FuturesDirection);
            entrust.EntrustBalance = ei.EntrustBalance;
            entrust.PurchaseWay = CommonUtil.ePurchaseWayToPurchaseWay(ei.PurchaseWay);
            entrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;

            if (ei is FundEntrustInfo)
            {
                Trader.InsertOrderFund(entrust as FundEntrust);
            }
            else
            {
                Trader.InsertOrder(entrust);
            }
        }

        protected void OnClosePositionInfoNotify(PositionInfoBase positionInfo)
        {
            var securityInfo = MarketDataService.GetSecurityInfo(
                CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(positionInfo.MarketType)),
                positionInfo.SecurityID);

            if (null == securityInfo)
            {
                securityInfo = new SecurityInfo
                {
                    ExID = CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(positionInfo.MarketType)),
                    SecurityID = positionInfo.SecurityID,
                    Quotation = new SecurityQuotation(),
                };
            }

            SelectedCombiNo = positionInfo.CombiNo;
            DisplaySecurityInfo = securityInfo;
            EntrustPriceType = eEntrustPriceType.限价;
            EntrustAmount = positionInfo.EnableAmount;

            if (null != securityInfo.Quotation)
            {
                EntrustPrice = securityInfo.Quotation.LastPx;
            }
            else
            {
                EntrustPrice = 0;
            }

            if (positionInfo is OptionPositionInfo || positionInfo is FuturePositionInfo)
            {
                if (positionInfo.PositionFlag == ePositionFlag.多头持仓)
                {
                    EntrustDirection = eEntrustDirection.卖出;
                    FuturesDirection = eFuturesDirection.平仓;
                }
                // Involved 空头持仓,备兑空头持仓
                else
                {
                    EntrustDirection = eEntrustDirection.买入;
                    FuturesDirection = eFuturesDirection.平仓;
                }
            }
            else
            {
                if (securityInfo.SecurityType == eCategory.债券回购)
                {
                    EntrustDirection = eEntrustDirection.融资回购;
                }
                else
                {
                    EntrustDirection = eEntrustDirection.卖出;
                }
            }

        }

        protected override void OnExportList()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TradePanelViewModelQueryBase : TradePanelViewModelBase, IPartImportsSatisfiedNotification
    {
        private List<eEntrustDirection> _stockEntrustDirection;

        public List<eEntrustDirection> StockEntrustDirectionList
        {
            get { return _stockEntrustDirection; }
            set { SetProperty(ref _stockEntrustDirection, value); }
        }

        [ImportingConstructor]
        public TradePanelViewModelQueryBase(IEventAggregator eventAgg,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            EntrustAmount = 100;
            PropertyChanged += TradePanelViewModel_PropertyChanged;
        }

        protected override void InitCommands()
        {
            NewEntrustCommand = new DelegateCommand<EntrustInfo>(OnNewEntrust);
        }

        protected override void SubscribeEvents()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<ClosePositionInfoNotifyEvent>()
                    .Subscribe(OnClosePositionInfoNotify, ThreadOption.UIThread);
                EventAggregator.GetEvent<SelectDisplayStockQuotationEvent>()
                    .Subscribe(OnDisplayQuotChange, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastBuyStockEvent>().Subscribe(OnFastBuyStock, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastSellStockEvent>().Subscribe(OnFastSellStock, ThreadOption.UIThread);
                EventAggregator.GetEvent<NewStockEntrustNotifyEvent>().Subscribe(OnNewEntrust, ThreadOption.UIThread);
            }
        }

        protected void TradePanelViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplaySecurityInfo")
            {
                if (DisplaySecurityInfo != null && DisplaySecurityInfo.SecurityType == eCategory.债券回购)
                {
                    StockEntrustDirectionList = MenuBar.BondsEntrustDirection;
                }
                else
                {
                    StockEntrustDirectionList = MenuBar.StockEntrustDirection;
                }

                if (!StockEntrustDirectionList.Contains(EntrustDirection))
                {
                    EntrustDirection = StockEntrustDirectionList.First();
                }
            }
        }

        private void OnDisplayQuotChange(SecurityInfo secCode)
        {
            DisplaySecurityInfo = secCode;
            EntrustPrice = 0;

            if (null != DisplaySecurityInfo.Quotation)
            {
                EntrustPrice = DisplaySecurityInfo.Quotation.LastPx;
            }
        }

        private void OnFastBuyStock(Tuple<eMarketType, string, double, string, int, eEntrustDirection> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputEntrustDirection = inputParameter.Item6;

            var ei = CommonUtil.BuildEntrustInfo(inputSecurityID, inputPrice, inputMarketType, inputCombineNo,
                inputEntrustAmount, inputEntrustDirection, eEntrustPriceType.限价);
            OnNewEntrust(ei);
        }

        private void OnFastSellStock(Tuple<eMarketType, string, double, string, int, eEntrustDirection> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputEntrustDirection = inputParameter.Item6;

            var ei = CommonUtil.BuildEntrustInfo(inputSecurityID, inputPrice, inputMarketType, inputCombineNo,
                inputEntrustAmount, inputEntrustDirection, eEntrustPriceType.限价);
            OnNewEntrust(ei);
        }

        private void OnNewEntrust(EntrustInfo ei)
        {
            if (ei == null)
            {
                if (DisplaySecurityInfo == null)
                {
                    DialogService.ShowMessage("错误", "请选择一支股票");
                    return;
                }

                ei = CommonUtil.BuildEntrustInfo(
                    DisplaySecurityInfo.SecurityID,
                    EntrustPrice,
                    DisplaySecurityInfo.MarketType,
                    SelectedCombiNo,
                    EntrustAmount,
                    EntrustDirection,
                    EntrustPriceType);
            }

            NewEntrust(ei);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {

        }

        public void OnImportsSatisfied()
        {
            StockEntrustDirectionList = MenuBar.StockEntrustDirection;
        }
    }

    // Newly added
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModel : TradePanelViewModelQueryBase
    {
        [ImportingConstructor]
        public TradePanelViewModel(IEventAggregator eventAgg,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModelFuture : TradePanelViewModelBase
    {
        [ImportingConstructor]
        public TradePanelViewModelFuture(IEventAggregator eventAgg,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            EntrustAmount = 1;
        }

        protected override void InitCommands()
        {
            NewEntrustCommand = new DelegateCommand<FutureEntrustInfo>(OnNewEntrust);
        }

        protected override void SubscribeEvents()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<CloseFuturePositionInfoNotifyEvent>()
                    .Subscribe(OnClosePositionInfoNotify, ThreadOption.UIThread);
                EventAggregator.GetEvent<SelectDisplayFutureQuotationEvent>()
                    .Subscribe(OnDisplayQuotChange, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastBuyFutureEvent>().Subscribe(OnFastBuyFuture, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastSellFutureEvent>().Subscribe(OnFastSellFuture, ThreadOption.UIThread);
                EventAggregator.GetEvent<NewFutureEntrustNotifyEvent>().Subscribe(OnNewEntrust, ThreadOption.UIThread);
            }
        }

        private void OnDisplayQuotChange(SecurityInfo secCode)
        {
            DisplaySecurityInfo = secCode;
            EntrustPrice = 0;

            if (null != DisplaySecurityInfo.Quotation)
            {
                EntrustPrice = DisplaySecurityInfo.Quotation.LastPx;
            }
        }

        private void OnNewEntrust(FutureEntrustInfo ei)
        {
            if (ei == null)
            {
                if (DisplaySecurityInfo == null)
                {
                    DialogService.ShowMessage("错误", "请选择一支期货");
                    return;
                }

                ei = (FutureEntrustInfo) CommonUtil.BuildEntrustInfo(
                    DisplaySecurityInfo.SecurityID,
                    EntrustPrice,
                    DisplaySecurityInfo.MarketType,
                    SelectedCombiNo,
                    EntrustAmount,
                    EntrustDirection,
                    EntrustPriceType,
                    eCategory.期货,
                    FuturesDirection,
                    InvestType);
            }

            NewEntrust(ei);
        }

        private void OnFastBuyFuture(Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputFuturesDirection = inputParameter.Item6;
            var inputInvestType = inputParameter.Item7;

            var ei = (FutureEntrustInfo) CommonUtil.BuildEntrustInfo(
                inputSecurityID,
                inputPrice,
                inputMarketType,
                inputCombineNo,
                inputEntrustAmount,
                eEntrustDirection.买入,
                eEntrustPriceType.限价,
                eCategory.期货,
                inputFuturesDirection,
                inputInvestType);

            NewEntrust(ei);
        }

        private void OnFastSellFuture(Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputFuturesDirection = inputParameter.Item6;
            var inputInvestType = inputParameter.Item7;

            var ei = (FutureEntrustInfo) CommonUtil.BuildEntrustInfo(
                inputSecurityID,
                inputPrice,
                inputMarketType,
                inputCombineNo,
                inputEntrustAmount,
                eEntrustDirection.卖出,
                eEntrustPriceType.限价,
                eCategory.期货,
                inputFuturesDirection,
                inputInvestType);

            NewEntrust(ei);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {

        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModelFund : TradePanelViewModelBase
    {
        [ImportingConstructor]
        public TradePanelViewModelFund(IEventAggregator eventAgg,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            EntrustDirection = eEntrustDirection.基金分拆;
        }

        protected override void InitCommands()
        {
            NewEntrustCommand = new DelegateCommand<FundEntrustInfo>(OnNewEntrust);
        }

        protected override void SubscribeEvents()
        {
        }

        protected void OnNewEntrust(FundEntrustInfo ei)
        {
            if (ei == null)
            {
                ei = (FundEntrustInfo) CommonUtil.BuildEntrustInfo(
                    DisplaySecurityInfo.SecurityID,
                    EntrustPrice,
                    DisplaySecurityInfo.MarketType,
                    SelectedCombiNo,
                    EntrustAmount,
                    EntrustDirection,
                    EntrustPriceType,
                    eCategory.基金分拆合并,
                    FuturesDirection,
                    InvestType,
                    EntrustBalance,
                    PurchaseWay);
            }

            NewEntrust(ei);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {

        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModelFundEtf : TradePanelViewModelFund
    {
        [ImportingConstructor]
        public TradePanelViewModelFundEtf(IEventAggregator eventAgg,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            EntrustDirection = eEntrustDirection.ETF申购;
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModelOption : TradePanelViewModelBase
    {
        [ImportingConstructor]
        public TradePanelViewModelOption(IEventAggregator eventAgg,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            EntrustAmount = 1;
        }

        protected override void InitCommands()
        {
            NewEntrustCommand = new DelegateCommand<OptionEntrustInfo>(OnNewEntrust);
            TestCommand = new DelegateCommand(OnTest);
        }

        protected override void SubscribeEvents()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<CloseOptionPositionInfoNotifyEvent>()
                    .Subscribe(OnClosePositionInfoNotify, ThreadOption.UIThread);
                EventAggregator.GetEvent<SelectDisplayOptionQuotationEvent>()
                    .Subscribe(OnDisplayQuotChange, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastBuyOptionEvent>().Subscribe(OnFastBuyOption, ThreadOption.UIThread);
                EventAggregator.GetEvent<FastSellOptionEvent>().Subscribe(OnFastSellOption, ThreadOption.UIThread);
                EventAggregator.GetEvent<NewOptionEntrustNotifyEvent>().Subscribe(OnNewEntrust, ThreadOption.UIThread);
            }
        }

        private void OnDisplayQuotChange(SecurityInfo secCode)
        {
            DisplaySecurityInfo = secCode;
            EntrustPrice = 0;

            if (null != DisplaySecurityInfo.Quotation)
            {
                EntrustPrice = DisplaySecurityInfo.Quotation.LastPx;
            }
        }

        private void OnNewEntrust(OptionEntrustInfo ei)
        {
            if (ei == null)
            {
                if (DisplaySecurityInfo == null)
                {
                    DialogService.ShowMessage("错误", "请选择一支期权");
                    return;
                }

                ei = (OptionEntrustInfo) CommonUtil.BuildEntrustInfo(
                    DisplaySecurityInfo.SecurityID,
                    EntrustPrice,
                    DisplaySecurityInfo.MarketType,
                    SelectedCombiNo,
                    EntrustAmount,
                    EntrustDirection,
                    EntrustPriceType,
                    eCategory.期权,
                    FuturesDirection,
                    InvestType);
            }

            NewEntrust(ei);
        }

        private void OnFastBuyOption(Tuple<eMarketType, string, double, string, int, eFuturesDirection> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputFuturesDirection = inputParameter.Item6;

            // Currently, the Invest Type is always 投机.
            var ei = (OptionEntrustInfo) CommonUtil.BuildEntrustInfo(
                inputSecurityID,
                inputPrice,
                inputMarketType,
                inputCombineNo,
                inputEntrustAmount,
                eEntrustDirection.买入,
                eEntrustPriceType.限价,
                eCategory.期权,
                inputFuturesDirection,
                eInvestType.投机);

            NewEntrust(ei);
        }

        private void OnFastSellOption(Tuple<eMarketType, string, double, string, int, eFuturesDirection> inputParameter)
        {
            var inputMarketType = inputParameter.Item1;
            var inputSecurityID = inputParameter.Item2;
            var inputPrice = inputParameter.Item3;
            var inputCombineNo = inputParameter.Item4;
            var inputEntrustAmount = inputParameter.Item5;
            var inputFuturesDirection = inputParameter.Item6;

            // Currently, the Invest Type is always 投机.
            var ei = (OptionEntrustInfo) CommonUtil.BuildEntrustInfo(
                inputSecurityID,
                inputPrice,
                inputMarketType,
                inputCombineNo,
                inputEntrustAmount,
                eEntrustDirection.卖出,
                eEntrustPriceType.限价,
                eCategory.期权,
                inputFuturesDirection,
                eInvestType.投机);

            NewEntrust(ei);
        }

        private void OnTest()
        {
            //trader.QryHoldingPosition(selectedCombiNo, eCategory.期权);
            //trader.QryOptionMargin(selectedCombiNo, CommonUtil.market_no_中金所);
            //trader.QryTradeResult(selectedCombiNo, eCategory.期权);
            //trader.QryEntrust(selectedCombiNo, eCategory.期权);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {

        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelViewModelBasket : TradePanelViewModelBase, IPartImportsSatisfiedNotification
    {
        private SecurityCodeSearchProvider _stockSearchProvider;
        private SecurityCodeSearchProvider _futureSearchProvider;

        [ImportingConstructor]
        public TradePanelViewModelBasket(IEventAggregator eventAgg,
            StockSecurityCodeSearchProvider stockSecuritySearchProvider,
            FutureSecurityCodeSearchProvider futureSecurityCodeSearchProvider)
            : base(eventAgg, stockSecuritySearchProvider)
        {
            _stockSearchProvider = stockSecuritySearchProvider;
            _futureSearchProvider = futureSecurityCodeSearchProvider;

            CategoryType = eBasketCategory.股票;
            PropertyChanged += TradePanelViewModel_PropertyChanged;
        }

        protected override void InitCommands()
        {
            NewEntrustCommand = new DelegateCommand(OnNewEntrust);
            AddEntrustCommand = new DelegateCommand(OnAddEntrust);
            DeleteEntrustCommand = new DelegateCommand(OnDeleteEntrust);
            SecurityTypeChangedCommand = new DelegateCommand(OnSecurityTypeChangedCommand);
            ExportEntrustListCommand = new DelegateCommand(OnExportEntrustList);
            ImportEntrustListCommand = new DelegateCommand(OnImportEntrustList);
        }

        protected override void SubscribeEvents()
        {

        }

        private EntrustInfo _selectedEntrust;

        public EntrustInfo SelectedEntrust
        {
            get { return _selectedEntrust; }
            set { SetProperty(ref _selectedEntrust, value); }
        }

        private eBasketCategory _categoryType;

        public eBasketCategory CategoryType
        {
            get { return _categoryType; }
            set { SetProperty(ref _categoryType, value); }
        }

        private string _limitEntrustRatio = "100%";
        public string LimitEntrustRatio
        {
            get { return _limitEntrustRatio; }
            set
            {
                SetProperty(ref _limitEntrustRatio, value);
            }
        }

        private IList<eMarketType> _basketStockMarketList;

        public IList<eMarketType> BasketStockMarketList
        {
            get { return _basketStockMarketList; }
            set { SetProperty(ref _basketStockMarketList, value); }
        }

        private IList<eMarketType> _basketFutureMarketList;

        public IList<eMarketType> BasketFutureMarketList
        {
            get { return _basketFutureMarketList; }
            set { SetProperty(ref _basketFutureMarketList, value); }
        }

        private ObservableCollection<EntrustInfo> _entrustList = new ObservableCollection<EntrustInfo>();

        public ObservableCollection<EntrustInfo> EntrustList
        {
            get { return _entrustList; }
            set { SetProperty(ref _entrustList, value); }
        }

        private List<eEntrustDirection> _stockEntrustDirection;

        public List<eEntrustDirection> StockEntrustDirectionList
        {
            get { return _stockEntrustDirection; }
            set { SetProperty(ref _stockEntrustDirection, value); }
        }

        private void OnNewEntrust()
        {
            if (!UserSettings.IsPromptEnabled ||
                (MessageBox.Show("确认发出篮子委托？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                 MessageBoxResult.Yes))
            {
                var listEntrust = new List<Entrust>();

                foreach (var ei in _entrustList)
                {
                    Entrust entrust;

                    if (ei is FutureEntrustInfo)
                    {
                        entrust = new FutureEntrust();
                    }
                    else
                    {
                        entrust = new Entrust();
                    }

                    entrust.StockCode = ei.SecurityID;
                    entrust.MarketNo = CommonUtil.eMarketTypeToeMarketNo(ei.MarketType);
                    entrust.CombiNo = ei.CombiNo;
                    entrust.EntrustDirection = CommonUtil.eEntrustDirectionToEntrustDirection(ei.EntrustDirection);
                    entrust.PriceType = CommonUtil.eEntrustPriceTypeToEntrustPriceType(ei.EntrustPriceType);
                    entrust.EntrustPrice = ei.EntrustPrice;
                    entrust.EntrustAmount = ei.EntrustAmount;                
                    entrust.FtrLimitEntrustRatio = entrust.LimitEntrustRatio = Convert.ToDouble(LimitEntrustRatio.Replace("%", ""));
                    entrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;

                    if (ei is FutureEntrustInfo)
                    {
                        ((FutureEntrust) entrust).FuturesDirection =
                            CommonUtil.eFuturesDirectionToFuturesDirection(((FutureEntrustInfo) ei).FuturesDirection);
                    }

                    listEntrust.Add(entrust);
                }

                if (listEntrust.Count > 0)
                {
                    Trader.InsertOrderBasket(listEntrust);
                    _entrustList.Clear();
                }
            }
        }

        public ICommand AddEntrustCommand { get; private set; }

        private void OnAddEntrust()
        {
            if (null != DisplaySecurityInfo)
            {
                EntrustInfo ei = null;

                if (ei == null)
                {
                    // Currently, it only supports stock and future.
                    if (CategoryType == eBasketCategory.期货)
                    {
                        // Future
                        ei = CommonUtil.BuildEntrustInfo(DisplaySecurityInfo.SecurityID, EntrustPrice,
                            DisplaySecurityInfo.MarketType, SelectedCombiNo, EntrustAmount, EntrustDirection,
                            EntrustPriceType, eCategory.期货, FuturesDirection, InvestType);
                    }
                    else
                    {
                        // Stock
                        ei = CommonUtil.BuildEntrustInfo(DisplaySecurityInfo.SecurityID, EntrustPrice,
                            DisplaySecurityInfo.MarketType, SelectedCombiNo, EntrustAmount, EntrustDirection,
                            EntrustPriceType);
                    }
                }

                var exID = CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(DisplaySecurityInfo.MarketType));

                ei.SecurityName = DisplaySecurityInfo.SecurityName;
                ei.SecurityQuotation =
                    MarketDataService.GetAndSubscribeSecurityQuote(new ExSecID(exID, DisplaySecurityInfo.SecurityID));

                _entrustList.Add(ei);
            }
        }

        public ICommand DeleteEntrustCommand { get; private set; }

        private void OnDeleteEntrust()
        {
            if (MessageBox.Show("确定要删除吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                _entrustList.Remove(_selectedEntrust);
            }
        }

        public ICommand SecurityTypeChangedCommand { get; set; }

        private void OnSecurityTypeChangedCommand()
        {
            switch (CategoryType)
            {
                case eBasketCategory.期货:
                    SecuritySearchProvider = _futureSearchProvider;
                    break;
                case eBasketCategory.股票:
                default:
                    SecuritySearchProvider = _stockSearchProvider;
                    break;
            }
        }

        [XmlIgnore]
        public ICommand ExportEntrustListCommand { get; set; }

        private void OnExportEntrustList()
        {
            CommonUtil.ExportToCsv("导出篮子交易列表...", _entrustList);
        }

        [XmlIgnore]
        public ICommand ImportEntrustListCommand { get; set; }

        private void OnImportEntrustList()
        {
            CommonUtil.ImportFromCsv("导入篮子交易列表...", _entrustList);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {

        }

        protected void TradePanelViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CategoryType == eBasketCategory.股票
                && e.PropertyName == "DisplaySecurityInfo")
            {
                if (null != DisplaySecurityInfo
                    && DisplaySecurityInfo.SecurityType == eCategory.债券回购)
                {
                    StockEntrustDirectionList = MenuBar.BondsEntrustDirection;
                }
                else
                {
                    StockEntrustDirectionList = MenuBar.StockEntrustDirection;
                }

                if (!StockEntrustDirectionList.Contains(EntrustDirection))
                {
                    EntrustDirection = StockEntrustDirectionList.First();
                }
            }
        }

        public void OnImportsSatisfied()
        {
            StockEntrustDirectionList = MenuBar.StockEntrustDirection;
        }
    }

    public abstract class QryHistoricalEntrustViewModelBase : TradePanelViewModelQueryBase
    {
        [ImportingConstructor]
        public QryHistoricalEntrustViewModelBase(IEventAggregator eventAgg,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
            PropertyChanged += TradePanelViewModel_PropertyChanged;
        }

        protected override void SubscribeEvents()
        {

        }

        private eEntrustState _entrustState;

        public eEntrustState EntrustState
        {
            get { return _entrustState; }
            set { SetProperty(ref _entrustState, value); }
        }

        private int _batchNo;

        public int BatchNo
        {
            get { return _batchNo; }
            set { SetProperty(ref _batchNo, value); }
        }

        private int _entrustNo;

        public int EntrustNo
        {
            get { return _entrustNo; }
            set { SetProperty(ref _entrustNo, value); }
        }

        private DateTime _startDate = DateTime.Now - new TimeSpan(TimeSpan.TicksPerDay*30);

        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTime _endDate = DateTime.Now;

        public DateTime EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private HistoricalEntrustInfo _selectedHistoricalEntrustInfo;

        public HistoricalEntrustInfo SelectedHistoricalEntrustInfo
        {
            get { return _selectedHistoricalEntrustInfo; }
            set { SetProperty(ref _selectedHistoricalEntrustInfo, value); }
        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        public ICommand QryCommand { get; set; }

        private void OnQry()
        {
            if (!string.IsNullOrEmpty(SelectedCombiNo))
            {
                Trader.EntrustInfoCollection.HistoricalStockEntrustInfoList.Clear();
                var combiNo = SelectedCombiNo;
                var accountCode = Trader.CombiNo2AccountCode[SelectedCombiNo];
                var startDate = Convert.ToInt32(StartDate.ToString("yyyyMMdd"));
                var endDate = Convert.ToInt32(EndDate.ToString("yyyyMMdd"));
                string stockCode = null;

                if (DisplaySecurityInfo != null)
                {
                    if (!string.IsNullOrEmpty(DisplaySecurityInfo.SecurityID))
                    {
                        stockCode = DisplaySecurityInfo.SecurityID;
                    }
                }

                if (!string.IsNullOrEmpty(stockCode))
                {
                    switch (DisplaySecurityInfo.SecurityType)
                    {
                        case eCategory.股票:
                        case eCategory.基金:
                        {
                            Trader.QryHistoricalEntrust(combiNo, accountCode, startDate, endDate, stockCode, null,
                                null);
                            break;
                        }
                        case eCategory.期货:
                        {
                            Trader.QryHistoricalEntrust(combiNo, accountCode, startDate, endDate, stockCode, null,
                                null, true);
                            break;
                        }
                    }

                }
                else //代码为空时，同时查询证券和期货
                {
                    Trader.QryHistoricalEntrust(combiNo, accountCode, startDate, endDate, null, null, null);
                    Trader.QryHistoricalEntrust(combiNo, accountCode, startDate, endDate, null, null, null, true);
                }
            }
            else
            {
                MessageBox.Show("必须提供组合编号！");
            }
        }
    }

    // Newly added.
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class QryHistoricalEntrustViewModel : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public QryHistoricalEntrustViewModel(IEventAggregator eventAgg,
            HistoricalSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
        }
    }


    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class QryHistoricalTradeResultViewModel : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public QryHistoricalTradeResultViewModel(IEventAggregator eventAgg,
            HistoricalSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {

        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            if (!string.IsNullOrEmpty(SelectedCombiNo))
            {
                Trader.TradeResultInfoCollection.HistoricalTradeResultInfoList.Clear();
                var combiNo = SelectedCombiNo;
                var accountCode = Trader.CombiNo2AccountCode[SelectedCombiNo];
                var startDate = Convert.ToInt32(StartDate.ToString("yyyyMMdd"));
                var endDate = Convert.ToInt32(EndDate.ToString("yyyyMMdd"));
                string stockCode = null;

                if (DisplaySecurityInfo != null)
                {
                    if (!string.IsNullOrEmpty(DisplaySecurityInfo.SecurityID))
                    {
                        stockCode = DisplaySecurityInfo.SecurityID;
                    }
                }

                if (!string.IsNullOrEmpty(stockCode))
                {
                    switch (DisplaySecurityInfo.SecurityType)
                    {
                        case eCategory.股票:
                        case eCategory.基金:
                        {
                            Trader.QryHistoricalTradeResult(combiNo, accountCode, startDate, endDate, stockCode, null);
                            break;
                        }
                        case eCategory.期货:
                        {
                            Trader.QryHistoricalTradeResult(combiNo, accountCode, startDate, endDate, stockCode, null,
                                true);
                            break;
                        }
                    }

                }
                else //代码为空时，同时查询证券和期货
                {
                    Trader.QryHistoricalTradeResult(combiNo, accountCode, startDate, endDate, null, null);
                    Trader.QryHistoricalTradeResult(combiNo, accountCode, startDate, endDate, null, null, true);
                }
            }
            else
            {
                MessageBox.Show("必须提供组合编号！");
            }
        }
    }


    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AdvancedQueryViewModelOption : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public AdvancedQueryViewModelOption(IEventAggregator eventAgg,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
        }

        private EntrustInfo _selectedEntrustInfo;

        public EntrustInfo SelectedEntrustInfo
        {
            get { return _selectedEntrustInfo; }
            set { SetProperty(ref _selectedEntrustInfo, value); }
        }

        private string _selectedEntrustDirection;

        public string SelectedEntrustDirection
        {
            get { return _selectedEntrustDirection; }
            set { SetProperty(ref _selectedEntrustDirection, value); }
        }

        private List<string> _optionEntrustDirectionList = new List<string>
        {
            null,
            "买入",
            "卖出"
        };

        public List<string> OptionEntrustDirectionList
        {
            get { return _optionEntrustDirectionList; }
            set { SetProperty(ref _optionEntrustDirectionList, value); }
        }

        private string _selectedEntrustState;

        public string SelectedEntrustState
        {
            get { return _selectedEntrustState; }
            set { SetProperty(ref _selectedEntrustState, value); }
        }

        private List<string> _entrustStateList = new List<string>
        {
            null,
            "未报",
            "待报",
            "正报",
            "已报",
            "废单",
            "部成",
            "已成",
            "部撤",
            "已撤",
            "待撤",
            "未撤",
            "正撤",
            "撤认",
            "撤废",
        };

        public List<string> EntrustStateList
        {
            get { return _entrustStateList; }
            set { SetProperty(ref _entrustStateList, value); }
        }

        private int _selectedStartHour = 9;

        public int SelectedStartHour
        {
            get { return _selectedStartHour; }
            set { SetProperty(ref _selectedStartHour, value); }
        }

        private int _selectedStartMinute;

        public int SelectedStartMinute
        {
            get { return _selectedStartMinute; }
            set { SetProperty(ref _selectedStartMinute, value); }
        }

        private int _selectedStartSecond;

        public int SelectedStartSecond
        {
            get { return _selectedStartSecond; }
            set { SetProperty(ref _selectedStartSecond, value); }
        }

        private int _selectedEndHour = 15;

        public int SelectedEndHour
        {
            get { return _selectedEndHour; }
            set { SetProperty(ref _selectedEndHour, value); }
        }

        private int _selectedEndMinute;

        public int SelectedEndMinute
        {
            get { return _selectedEndMinute; }
            set { SetProperty(ref _selectedEndMinute, value); }
        }

        private int _selectedEndSecond;

        public int SelectedEndSecond
        {
            get { return _selectedEndSecond; }
            set { SetProperty(ref _selectedEndSecond, value); }
        }

        private ObservableCollection<EntrustInfo> _result = new ObservableCollection<EntrustInfo>();

        public ObservableCollection<EntrustInfo> Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            IEnumerable<EntrustInfo> rest = Trader.EntrustInfoCollection.OptionEntrustInfoList;

            if (!string.IsNullOrEmpty(SelectedCombiNo))
            {
                rest = from ei in rest
                    where ei.CombiNo.Equals(SelectedCombiNo)
                    select ei;
            }

            if (DisplaySecurityInfo != null && !string.IsNullOrEmpty(DisplaySecurityInfo.SecurityID))
            {
                rest = from ei in rest where ei.SecurityID.Equals(DisplaySecurityInfo.SecurityID) select ei;
            }

            if (!string.IsNullOrEmpty(SelectedEntrustDirection))
            {
                var e = (eEntrustDirection) Enum.Parse(typeof (eEntrustDirection), SelectedEntrustDirection);
                rest = from ei in rest where ei.EntrustDirection == e select ei;
            }

            if (!string.IsNullOrEmpty(SelectedEntrustState))
            {
                var e = (eEntrustState) Enum.Parse(typeof (eEntrustState), SelectedEntrustState);
                rest = from ei in rest where ei.EntrustState == e select ei;
            }

            var dtStart =
                DateTime.Parse(
                    string.Format("{0}:{1}:{2}", SelectedStartHour, SelectedStartMinute, SelectedStartSecond),
                    CultureInfo.CurrentCulture);

            var dtEnd =
                DateTime.Parse(
                    string.Format("{0}:{1}:{2}", SelectedEndHour, SelectedEndMinute, SelectedEndSecond),
                    CultureInfo.CurrentCulture);

            if (dtStart > dtEnd)
            {
                MessageBox.Show("委托开始时间必须小于委托结束时间！");
                return;
            }

            rest = from ei in rest
                where
                    DateTime.Parse(ei.EntrustTime, CultureInfo.CurrentCulture) >= dtStart &&
                    DateTime.Parse(ei.EntrustTime, CultureInfo.CurrentCulture) <= dtEnd
                select ei;

            Result = new ObservableCollection<EntrustInfo>(rest);
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AdvancedQueryViewModelOptionTradeResult : AdvancedQueryViewModelOption
    {
        [ImportingConstructor]
        public AdvancedQueryViewModelOptionTradeResult(IEventAggregator eventAgg,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {

        }

        private TradeResultInfo _selectedTradeResultInfo;

        public TradeResultInfo SelectedTradeResultInfo
        {
            get { return _selectedTradeResultInfo; }
            set { SetProperty(ref _selectedTradeResultInfo, value); }
        }


        private ObservableCollection<TradeResultInfo> _result = new ObservableCollection<TradeResultInfo>();

        public new ObservableCollection<TradeResultInfo> Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            IEnumerable<TradeResultInfo> rest = Trader.TradeResultInfoCollection.OptionTradeResultInfoList;

            if (!string.IsNullOrEmpty(SelectedCombiNo))
            {
                rest = from ei in rest
                       where ei.CombiNo.Equals(SelectedCombiNo)
                       select ei;
            }

            if (DisplaySecurityInfo != null && !string.IsNullOrEmpty(DisplaySecurityInfo.SecurityID))
            {
                rest = from ei in rest where ei.SecurityID.Equals(DisplaySecurityInfo.SecurityID) select ei;
            }

            if (!string.IsNullOrEmpty(SelectedEntrustDirection))
            {
                var e = (eEntrustDirection)Enum.Parse(typeof(eEntrustDirection), SelectedEntrustDirection);
                rest = from ei in rest where ei.EntrustDirection == e select ei;
            }


            var dtStart =
                DateTime.Parse(
                    string.Format("{0}:{1}:{2}", SelectedStartHour, SelectedStartMinute, SelectedStartSecond),
                    CultureInfo.CurrentCulture);

            var dtEnd =
                DateTime.Parse(
                    string.Format("{0}:{1}:{2}", SelectedEndHour, SelectedEndMinute, SelectedEndSecond),
                    CultureInfo.CurrentCulture);

            if (dtStart > dtEnd)
            {
                MessageBox.Show("成交开始时间必须小于成交结束时间！");
                return;
            }

            rest = from ei in rest
                   where
                       DateTime.Parse(ei.DealTime, CultureInfo.CurrentCulture) >= dtStart &&
                       DateTime.Parse(ei.DealTime, CultureInfo.CurrentCulture) <= dtEnd
                   select ei;

            Result = new ObservableCollection<TradeResultInfo>(rest);
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AdvancedQueryViewModelOptionHoldingPosition : AdvancedQueryViewModelOption
    {
        [ImportingConstructor]
        public AdvancedQueryViewModelOptionHoldingPosition(IEventAggregator eventAgg,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {

        }

        private string _selectedOptionType;

        public string SelectedOptionType
        {
            get { return _selectedOptionType; }
            set { SetProperty(ref _selectedOptionType, value); }
        }

        private List<string> _optionTypeList = new List<string>
        {
            null,
            "认购期权",
            "认沽期权"
        };

        public List<string> OptionTypeList
        {
            get { return _optionTypeList; }
            set { SetProperty(ref _optionTypeList, value); }
        }

        private string _selectedPositionFlag;

        public string SelectedPositionFlag
        {
            get { return _selectedPositionFlag; }
            set { SetProperty(ref _selectedPositionFlag, value); }
        }

        private List<string> _positionFlagList = new List<string>
        {
            null,
            "多头持仓",
            "空头持仓",
        };

        public List<string> PositionFlagList
        {
            get { return _positionFlagList; }
            set { SetProperty(ref _positionFlagList, value); }
        }

        private OptionPositionInfo _selectedPositionInfo;

        public OptionPositionInfo SelectedPositionInfo
        {
            get { return _selectedPositionInfo; }
            set { SetProperty(ref _selectedPositionInfo, value); }
        }


        private ObservableCollection<OptionPositionInfo> _result = new ObservableCollection<OptionPositionInfo>();

        public new ObservableCollection<OptionPositionInfo> Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            IEnumerable<OptionPositionInfo> rest = Trader.PositionInfoCollection.OptionPositionInfoList;

            if (!string.IsNullOrEmpty(SelectedCombiNo))
            {
                rest = from ei in rest
                       where ei.CombiNo.Equals(SelectedCombiNo)
                       select ei;
            }

            if (DisplaySecurityInfo != null && !string.IsNullOrEmpty(DisplaySecurityInfo.SecurityID))
            {
                rest = from ei in rest where ei.SecurityID.Equals(DisplaySecurityInfo.SecurityID) select ei;
            }

            if (!string.IsNullOrEmpty(SelectedPositionFlag))
            {
                var e = (ePositionFlag)Enum.Parse(typeof(ePositionFlag), SelectedPositionFlag);
                rest = from ei in rest where ei.PositionFlag == e select ei;
            }

            if (!string.IsNullOrEmpty(SelectedOptionType))
            {
                var e = (eOptionType)Enum.Parse(typeof(eOptionType), SelectedOptionType);
                rest = from ei in rest where ei.OptionType == e select ei;
            }

            Result = new ObservableCollection<OptionPositionInfo>(rest);
        }
    }


    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class QryEtfStockListViewModel : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public QryEtfStockListViewModel(IEventAggregator eventAgg,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {
        }

        protected override void SubscribeEvents()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<EtfBaseInfoNotifyEvent>().Subscribe(OnReturnEtfBaseInfo, ThreadOption.UIThread);
            }
        }

        protected void OnReturnEtfBaseInfo(EtfBaseInfo etfBaseInfo)
        {
            if (etfBaseInfo != null)
            {
                DisplaySecurityInfo = MarketDataService.GetSecurityInfo(
                    CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(etfBaseInfo.MarketType)),
                    etfBaseInfo.EtfCode);

            }
        }
        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private void OnQry()
        {
            if (DisplaySecurityInfo != null)
            {
                Trader.EtfStockCollection.EtfStockList.Clear();
                Trader.QryEtfStockList(CommonUtil.eMarketTypeToeMarketNo(DisplaySecurityInfo.MarketType),
                    DisplaySecurityInfo.SecurityID);
            }
        }

        protected override void OnRefresh()
        {
            OnQry();
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出ETF成份股信息列表...", Trader.EtfStockCollection.EtfStockList);
        }

        public void AddStocks(string legNumber)
        {
            EventAggregator.GetEvent<AddEtfStocksNotifyEvent>().Publish(legNumber);
        }
    }


    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class QryEtfEntrustDetailViewModel : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public QryEtfEntrustDetailViewModel(IEventAggregator eventAgg,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {

        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            if (EntrustNo > 0)
            {
                Trader.EtfEntrustDetail.Clear();
                Trader.QryEtfEntrustDetail(EntrustNo);
            }
        }
    }


    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class QryEtfTradeResultDetailViewModel : QryHistoricalEntrustViewModelBase
    {
        [ImportingConstructor]
        public QryEtfTradeResultDetailViewModel(IEventAggregator eventAgg,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAgg, securitySearchProvider)
        {

        }

        protected override void InitCommands()
        {
            QryCommand = new DelegateCommand(OnQry);
        }

        private void OnQry()
        {
            if (EntrustNo > 0)
            {
                Trader.EtfTradeResultDetail.Clear();
                Trader.QryEtfTradeResultDetail(EntrustNo);
            }
        }
    }
}

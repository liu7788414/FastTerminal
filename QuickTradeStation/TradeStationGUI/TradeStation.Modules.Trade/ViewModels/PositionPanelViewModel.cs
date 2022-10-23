using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.Views;

namespace TradeStation.Modules.Trade.ViewModels
{
    public abstract class PositionPanelViewModelBase : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        protected PositionPanelViewModelBase(IEventAggregator eventAggr,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }


        private PositionInfoBase _selectedPosition;
        public PositionInfoBase SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                SetProperty(ref _selectedPosition, value);
            }
        }

        public ICommand QueryPositionCommand { get; set; }
        public ICommand ClosePositionCommand { get; set; }

        public void Process(PositionInfoBase positionInfo, ObservableCollection<PositionInfoBase> positionInfoList)
        {
            DispatcherService.Invoke(() =>
            {
                switch (positionInfo.MsgType)
                {
                    case ePositionInfoMsgType.查询:
                        {
                            var q = from pi in positionInfoList where pi.SecurityID.Equals(positionInfo.SecurityID) && pi.CombiNo.Equals(positionInfo.CombiNo) && pi.PositionFlag == positionInfo.PositionFlag select pi;

                            if (q.Count() > 0)
                            {
                                var pi = q.First();
                                var i = positionInfoList.IndexOf(pi);
                                positionInfoList.Remove(pi);
                                pi.Copy(positionInfo);

                                positionInfoList.Insert(i, pi);
                            }
                            else
                            {
                                positionInfoList.Add(positionInfo);
                            }
                            break;
                        }
                    case ePositionInfoMsgType.交易:
                        {
                            if ((positionInfo.EntrustDirection == eEntrustDirection.买入 && positionInfo.FuturesDirection == eFuturesDirection.开仓) || (positionInfo.EntrustDirection == eEntrustDirection.卖出 && positionInfo.FuturesDirection == eFuturesDirection.开仓))
                            {
                                Trader.SetTraderForSecurityQuote(positionInfo.MarketType, positionInfo.SecurityID);
                                var q = from pi in positionInfoList where pi.SecurityID.Equals(positionInfo.SecurityID) && pi.CombiNo.Equals(positionInfo.CombiNo) && pi.PositionFlag == (positionInfo.EntrustDirection == eEntrustDirection.买入 ? ePositionFlag.多头持仓 : ePositionFlag.空头持仓) select pi;

                                if (q.Count() > 0)
                                {
                                    var pi = q.First();
                                    var i = positionInfoList.IndexOf(pi);
                                    positionInfoList.Remove(pi);

                                    // TODO: Currently, the usage of CurrentAmount and EnableAmount is same.
                                    // However, they have some little different.
                                    // Need refine the code to distinguish these two field.
                                    pi.TodayBuyAmount += positionInfo.DealAmount;
                                    pi.TodayBuyBalance += positionInfo.DealAmount * positionInfo.DealPrice;

                                    var old = pi.CurrentCostPrice * pi.CurrentAmount;
                                    var New = positionInfo.DealPrice * positionInfo.DealAmount;
                                    if (pi.CurrentAmount + positionInfo.DealAmount != 0)
                                    {
                                        pi.CurrentCostPrice = (old + New) / (pi.CurrentAmount + positionInfo.DealAmount);
                                    }
                                    else
                                    {
                                        pi.CurrentCostPrice = 0;
                                    }

                                    pi.CurrentAmount += positionInfo.DealAmount;
                                    pi.EnableAmount += positionInfo.DealAmount;
                                    pi.TodayAmount += positionInfo.DealAmount;
                                    pi.TodayEnableAmount += positionInfo.DealAmount;

                                    //if (pi.CurrentAmount > 0)
                                    {
                                        positionInfoList.Insert(i, pi);
                                    }
                                }
                                else
                                {
                                    positionInfo.CurrentAmount = positionInfo.DealAmount;
                                    positionInfo.EnableAmount = positionInfo.DealAmount;
                                    positionInfo.TodayAmount = positionInfo.DealAmount;
                                    positionInfo.TodayEnableAmount = positionInfo.DealAmount;
                                    positionInfo.CurrentCostPrice = positionInfo.DealPrice;

                                    positionInfoList.Add(positionInfo);
                                }
                            }

                            if ((positionInfo.EntrustDirection == eEntrustDirection.买入 && positionInfo.FuturesDirection == eFuturesDirection.平仓) || (positionInfo.EntrustDirection == eEntrustDirection.卖出 && positionInfo.FuturesDirection == eFuturesDirection.平仓))
                            {
                                var q = from pi in positionInfoList where pi.SecurityID.Equals(positionInfo.SecurityID) && pi.CombiNo.Equals(positionInfo.CombiNo) && pi.PositionFlag == (positionInfo.EntrustDirection == eEntrustDirection.买入 ? ePositionFlag.空头持仓 : ePositionFlag.多头持仓) select pi;

                                if (q.Count() > 0)
                                {
                                    var pi = q.First();
                                    var i = positionInfoList.IndexOf(pi);
                                    positionInfoList.Remove(pi);

                                    // TODO: Currently, the usage of CurrentAmount and EnableAmount is same.
                                    // However, they have some little different.
                                    // Need refine the code to distinguish these two field.
                                    pi.TodaySellAmount += positionInfo.DealAmount;
                                    pi.TodaySellBalance += positionInfo.DealAmount * positionInfo.DealPrice;

                                    //平仓时，成本价保持不变，平仓盈亏变动
                                    pi.CurrentAmount -= positionInfo.DealAmount;
                                    pi.EnableAmount -= positionInfo.DealAmount;

                                    double currentProfit = 0;

                                    if (positionInfo.EntrustDirection == eEntrustDirection.卖出) //找的多仓
                                    {
                                        currentProfit = (positionInfo.DealPrice - pi.CurrentCostPrice) * positionInfo.DealAmount;
                                    }
                                    else// 找的空仓
                                    {
                                        currentProfit = (pi.CurrentCostPrice - positionInfo.DealPrice) * positionInfo.DealAmount;
                                    }

                                    pi.CloseProfit += currentProfit;
                                    pi.DynamicProfit -= currentProfit;

                                    // 上期所的规则为:先平今，再平昨
                                    // 期权没有昨仓概念，计算方法去期货一致
                                    if (pi.MarketType == eMarketType.上期所)
                                    {
                                        // 优先平今
                                        if (pi.TodayAmount >= positionInfo.DealAmount)
                                        {
                                            pi.TodayAmount -= positionInfo.DealAmount;
                                            pi.TodayEnableAmount -= positionInfo.DealAmount;
                                        }
                                        // 今仓不足，再平昨仓
                                        else
                                        {
                                            pi.LastdayAmount -= (positionInfo.DealAmount - positionInfo.TodayAmount);
                                            pi.LastdayEnableAmount -= (positionInfo.DealAmount - positionInfo.TodayEnableAmount);

                                            pi.TodayAmount = 0;
                                            pi.TodayEnableAmount = 0;
                                        }
                                    }
                                    else
                                    {
                                        // 优先平昨
                                        if (pi.LastdayAmount >= positionInfo.DealAmount)
                                        {
                                            pi.LastdayAmount -= positionInfo.DealAmount;
                                            pi.LastdayEnableAmount -= positionInfo.DealAmount;
                                        }
                                        // 昨仓不足，再平今仓
                                        else
                                        {
                                            pi.TodayAmount -= (positionInfo.DealAmount - positionInfo.LastdayAmount);
                                            pi.TodayEnableAmount -= (positionInfo.DealAmount - positionInfo.LastdayEnableAmount);

                                            pi.LastdayAmount = 0;
                                            pi.LastdayEnableAmount = 0;
                                        }
                                    }

                                    //if (pi.CurrentAmount > 0)
                                    {
                                        positionInfoList.Insert(i, pi);
                                    }
                                }
                                else
                                {
                                    Logger.Error(string.Format("{0}没有可平仓位!", positionInfo.SecurityID));
                                }
                            }

                            break;
                        }
                    case ePositionInfoMsgType.清空:
                        {
                            positionInfoList.Clear();
                            break;
                        }
                }

            });
        }

        public void DailyReInitialize()
        {
            DispatcherService.Invoke(() =>
            {
                OnRefresh();
            });
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PositionPanelViewModel : PositionPanelViewModelBase
    {
        [ImportingConstructor]
        public PositionPanelViewModel(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<PositionInfoNotifyEvent>().Subscribe(OnReturnPositionInfo);
        }

        private void OnReturnPositionInfo(PositionInfo positionInfo)
        {
            Logger.Debug("onReturnPositionInfo");

            var positionInfoList = Trader.PositionInfoCollection.StockPositionInfoList;

            DispatcherService.Invoke(() =>
            {
                switch (positionInfo.MsgType)
                {
                    case ePositionInfoMsgType.查询:
                        {
                            var q = from pi in positionInfoList where pi.SecurityID.Equals(positionInfo.SecurityID) && pi.CombiNo.Equals(positionInfo.CombiNo) select pi;

                            if (q.Count() > 0)
                            {
                                var pi = q.First();
                                var i = positionInfoList.IndexOf(pi);
                                positionInfoList.Remove(pi);
                                pi.Copy(positionInfo);

                                positionInfoList.Insert(i, pi);
                            }
                            else
                            {
                                positionInfoList.Add(positionInfo);
                            }
                            break;
                        }
                    case ePositionInfoMsgType.交易:
                        {
                            Trader.SetTraderForSecurityQuote(positionInfo.MarketType, positionInfo.SecurityID);
                            var q = from pi in positionInfoList where pi.SecurityID.Equals(positionInfo.SecurityID) && pi.CombiNo.Equals(positionInfo.CombiNo) select pi;

                            if (q.Count() > 0)
                            {
                                var pi = q.First();
                                var i = positionInfoList.IndexOf(pi);
                                positionInfoList.Remove(pi);

                                if (positionInfo.DealAmount >= 0) //买入
                                {
                                    pi.TodayBuyAmount += positionInfo.DealAmount;
                                    pi.TodayBuyBalance += positionInfo.DealAmount*positionInfo.DealPrice;
                                    pi.CurrentCostPrice = (Math.Abs(pi.CurrentAmount)*pi.CurrentCostPrice +
                                                           (Math.Abs(positionInfo.DealPrice)*
                                                            Math.Abs(positionInfo.DealAmount)))/
                                                           (Math.Abs(pi.CurrentAmount) + Math.Abs(positionInfo.DealAmount));
                                }
                                else //卖出
                                {
                                    pi.TodaySellAmount -= positionInfo.DealAmount;
                                    pi.TodaySellBalance -= positionInfo.DealAmount*positionInfo.DealPrice;
                                    pi.CloseProfit += Math.Abs(positionInfo.DealAmount)*
                                                      (Math.Abs(positionInfo.DealPrice) - pi.CurrentCostPrice);
                                }

                                pi.CurrentAmount += positionInfo.DealAmount;

                                if (positionInfo.DealAmount < 0)//小于0表示平仓，可用数量应该减少，大于0表示开仓，T+1时不能加入到可用数量中
                                {
                                    pi.EnableAmount += positionInfo.DealAmount;
                                }
                                
                                //if (pi.CurrentAmount > 0)
                                {
                                    positionInfoList.Insert(i, pi);
                                }
                            }
                            else
                            {
                                positionInfoList.Add(positionInfo);
                            }

                            EventAggregator.GetEvent<RelatedPositionItemChangedNotifyEvent>().Publish(positionInfo);
                            break;
                        }
                    case ePositionInfoMsgType.清空:
                        {
                            positionInfoList.Clear();
                            break;
                        }
                }

            });
        }

        protected override void InitCommands()
        {
            QueryPositionCommand = new DelegateCommand<PositionInfo>(OnReturnPositionInfo);
            ClosePositionCommand = new DelegateCommand(OnClosePosition);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private void OnClosePosition()
        {
            if (SelectedPosition != null)
            {
                EventAggregator.GetEvent<ClosePositionInfoNotifyEvent>().Publish(SelectedPosition as PositionInfo);
            }
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.PositionInfoCollection.StockPositionInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryHoldingPosition(combiNo);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出股票持仓列表...", Trader.PositionInfoCollection.StockPositionInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PositionPanelViewModelFuture : PositionPanelViewModelBase
    {
        [ImportingConstructor]
        public PositionPanelViewModelFuture(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FuturePositionInfoNotifyEvent>().Subscribe(OnReturnFuturePositionInfo);
        }


        private void OnReturnFuturePositionInfo(FuturePositionInfo positionInfo)
        {
            Logger.Debug(string.Format("OnReturnFuturePositionInfo:{0}", positionInfo));
            Process(positionInfo, Trader.PositionInfoCollection.FuturePositionInfoList);

            // Notify the position info changed through 交易.
            if (positionInfo.MsgType == ePositionInfoMsgType.交易)
            {
                EventAggregator.GetEvent<RelatedFuturePositionItemChangedNotifyEvent>().Publish(positionInfo);
            }
        }

        protected override void InitCommands()
        {
            QueryPositionCommand = new DelegateCommand<FuturePositionInfo>(OnReturnFuturePositionInfo);
            ClosePositionCommand = new DelegateCommand(OnClosePosition);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.PositionInfoCollection.FuturePositionInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryHoldingPosition(combiNo, eCategory.期货);
                }
            });
        }

        private void OnClosePosition()
        {
            if (SelectedPosition != null)
            {
                EventAggregator.GetEvent<CloseFuturePositionInfoNotifyEvent>().Publish(SelectedPosition as FuturePositionInfo);
            }
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期货持仓列表...", Trader.PositionInfoCollection.FuturePositionInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PositionPanelViewModelOption : PositionPanelViewModelBase
    {
        private SecurityInfoMetadata _securityInfoMetadata;

        [ImportingConstructor]
        public PositionPanelViewModelOption(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider,
            SecurityInfoMetadata securityInfoMetadata)
            : base(eventAggr, securitySearchProvider)
        {
            _securityInfoMetadata = securityInfoMetadata;
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionPositionInfoNotifyEvent>().Subscribe(OnReturnOptionPositionInfo);
            EventAggregator.GetEvent<FinishedGetOptionInformationEvent>().Subscribe(OnFinishedGetOptionInformation);
        }

        private void OnReturnOptionPositionInfo(OptionPositionInfo positionInfo)
        {
            Logger.Debug("onReturnOptionPositionInfo");
            ProcessOption(positionInfo, Trader.PositionInfoCollection.OptionPositionInfoList);

            // Notify the position info changed through 交易.
            if (positionInfo.MsgType == ePositionInfoMsgType.交易)
            {
                EventAggregator.GetEvent<RelatedOptionPositionItemChangedNotifyEvent>().Publish(positionInfo);
            }
        }

        private void OnFinishedGetOptionInformation(object payload)
        {
            foreach (var item in Trader.PositionInfoCollection.OptionPositionInfoList)
            {
                RefreshOptionInstrumentInfo(item);
            }
        }

        protected void ProcessOption(OptionPositionInfo positionInfo, ObservableCollection<OptionPositionInfo> positionInfoList)
        {
            DispatcherService.Invoke(() =>
            {
                switch (positionInfo.MsgType)
                {
                    case ePositionInfoMsgType.查询:
                        {
                            var q = positionInfoList.Where(pi =>
                                pi.SecurityID.Equals(positionInfo.SecurityID)
                                && pi.CombiNo.Equals(positionInfo.CombiNo)
                                && pi.PositionFlag == positionInfo.PositionFlag);

                            if (q.Count() > 0)
                            {
                                var pi = q.First();
                                var i = positionInfoList.IndexOf(pi);
                                positionInfoList.Remove(pi);
                                pi.Copy(positionInfo);

                                positionInfoList.Insert(i, pi);
                            }
                            else
                            {
                                positionInfoList.Add(positionInfo);
                            }
                            break;
                        }
                    case ePositionInfoMsgType.交易:
                        {
                            if ((positionInfo.EntrustDirection == eEntrustDirection.买入 && positionInfo.FuturesDirection == eFuturesDirection.开仓)
                                || (positionInfo.EntrustDirection == eEntrustDirection.卖出 && positionInfo.FuturesDirection == eFuturesDirection.开仓))
                            {
                                Trader.SetTraderForSecurityQuote(positionInfo.MarketType, positionInfo.SecurityID);

                                var q = positionInfoList.Where(pi =>
                                    pi.SecurityID.Equals(positionInfo.SecurityID)
                                    && pi.CombiNo.Equals(positionInfo.CombiNo)
                                    && pi.PositionFlag == (positionInfo.EntrustDirection == eEntrustDirection.买入 ? ePositionFlag.多头持仓 : ePositionFlag.空头持仓));

                                // Option only has current amount and enable amount.
                                // It does not have today amount or last day amount.
                                if (q.Count() > 0)
                                {
                                    var pi = q.First();
                                    var i = positionInfoList.IndexOf(pi);
                                    positionInfoList.Remove(pi);

                                    pi.TodayBuyAmount += positionInfo.DealAmount;
                                    pi.TodayBuyBalance += positionInfo.DealAmount * positionInfo.DealPrice;

                                    var old = pi.CurrentCostPrice * pi.CurrentAmount;
                                    var New = positionInfo.DealPrice * positionInfo.DealAmount;

                                    if (pi.CurrentAmount + positionInfo.DealAmount != 0)
                                    {
                                        pi.CurrentCostPrice = (old + New) / (pi.CurrentAmount + positionInfo.DealAmount);
                                    }
                                    else
                                    {
                                        pi.CurrentCostPrice = 0;
                                    }

                                    pi.CurrentAmount += positionInfo.DealAmount;
                                    pi.EnableAmount += positionInfo.DealAmount;

                                    //if (pi.CurrentAmount > 0)
                                    {
                                        positionInfoList.Insert(i, pi);
                                    }
                                }
                                else
                                {
                                    positionInfo.CurrentAmount = positionInfo.DealAmount;
                                    positionInfo.EnableAmount = positionInfo.DealAmount;
                                    positionInfo.CurrentCostPrice = positionInfo.DealPrice;

                                    positionInfoList.Add(positionInfo);
                                }
                            }

                            if ((positionInfo.EntrustDirection == eEntrustDirection.买入 && positionInfo.FuturesDirection == eFuturesDirection.平仓)
                                || (positionInfo.EntrustDirection == eEntrustDirection.卖出 && positionInfo.FuturesDirection == eFuturesDirection.平仓))
                            {
                                var q = positionInfoList.Where(pi =>
                                    pi.SecurityID.Equals(positionInfo.SecurityID)
                                    && pi.CombiNo.Equals(positionInfo.CombiNo)
                                    && pi.PositionFlag == (positionInfo.EntrustDirection == eEntrustDirection.买入 ? ePositionFlag.空头持仓 : ePositionFlag.多头持仓));

                                if (q.Count() > 0)
                                {
                                    var pi = q.First();
                                    var i = positionInfoList.IndexOf(pi);
                                    positionInfoList.Remove(pi);

                                    // Option only has current amount and enable amount.
                                    // It does not have today amount or last day amount.
                                    pi.TodaySellAmount += positionInfo.DealAmount;
                                    pi.TodaySellBalance += positionInfo.DealAmount * positionInfo.DealPrice;

                                    pi.CurrentAmount -= positionInfo.DealAmount;
                                    pi.EnableAmount -= positionInfo.DealAmount;

                                    double currentProfit = 0;

                                    if (positionInfo.EntrustDirection == eEntrustDirection.卖出) //找的多仓
                                    {
                                        currentProfit = (Math.Abs(positionInfo.DealPrice) -
                                                         Math.Abs(pi.CurrentCostPrice))*positionInfo.DealAmount*
                                                        CommonUtil.OptionAmountPerHand;
                                    }
                                    else // 找的空仓
                                    {
                                        currentProfit = (Math.Abs(pi.CurrentCostPrice) -
                                                         Math.Abs(positionInfo.DealPrice))*positionInfo.DealAmount*
                                                        CommonUtil.OptionAmountPerHand;
                                    }

                                    pi.CloseProfit += currentProfit;
                                    pi.DynamicProfit -= currentProfit;

                                    //if (pi.CurrentAmount > 0)
                                    {
                                        positionInfoList.Insert(i, pi);
                                    }
                                }
                                else
                                {
                                    Logger.Error(string.Format("{0}没有可平仓位!", positionInfo.SecurityID));
                                }
                            }

                            break;
                        }
                    case ePositionInfoMsgType.清空:
                        {
                            positionInfoList.Clear();
                            break;
                        }
                }

                RefreshOptionInstrumentInfo(positionInfo);
            });
        }

        private void RefreshOptionInstrumentInfo(OptionPositionInfo optionPosition)
        {
            if (null != optionPosition
                && null == optionPosition.OptionInstrumentInfo)
            {
                var instrumentInfo =
                    _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.FirstOrDefault(x => x.SecurityID == optionPosition.SecurityID);

                optionPosition.OptionInstrumentInfo = instrumentInfo;
            }
        }

        protected override void InitCommands()
        {
            QueryPositionCommand = new DelegateCommand<OptionPositionInfo>(OnReturnOptionPositionInfo);
            ClosePositionCommand = new DelegateCommand(OnClosePosition);
            RefreshCommand = new DelegateCommand(OnRefresh);
            AdvancedQueryCommand = new DelegateCommand(OnAdvancedQuery);
        }

        protected override void OnAdvancedQuery()
        {
            var win = ServiceLocator.Current.GetInstance<AdvancedQueryPanelOptionHoldingPosition>();
            win.Show();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.PositionInfoCollection.OptionPositionInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryHoldingPosition(combiNo, eCategory.期权);
                }
            });
        }

        private void OnClosePosition()
        {
            if (SelectedPosition != null)
            {
                EventAggregator.GetEvent<CloseOptionPositionInfoNotifyEvent>().Publish(SelectedPosition as OptionPositionInfo);
            }
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期权持仓列表...", Trader.PositionInfoCollection.OptionPositionInfoList);
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PositionPanelViewModelForBasket : PositionPanelViewModel
    {
        [ImportingConstructor]
        public PositionPanelViewModelForBasket(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            SubscribeEvents();
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }
        protected override void SubscribeEvents()
        { }

        protected override void OnRefresh()
        {
            base.OnRefresh();
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PositionPanelViewModelFutureForBasket : PositionPanelViewModelFuture
    {
        [ImportingConstructor]
        public PositionPanelViewModelFutureForBasket(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            SubscribeEvents();
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }
        protected override void SubscribeEvents()
        { }

        protected override void OnRefresh()
        {
            base.OnRefresh();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.Views;
using MessageBox = System.Windows.MessageBox;

namespace TradeStation.Modules.Trade.ViewModels
{
    // Newly added
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradePanelArbitrageViewModel : ArbitrageBase,IPartImportsSatisfiedNotification
    {
        public ConcurrentDictionary<string, SecurityInfo> UpLimitList = new ConcurrentDictionary<string, SecurityInfo>();
        public ConcurrentDictionary<string, SecurityInfo> DownLimitList = new ConcurrentDictionary<string, SecurityInfo>();
        public DispatcherTimer RefreshTimer = new DispatcherTimer();
        public DispatcherTimer SuperviseTimer = new DispatcherTimer();
        public DispatcherTimer SecondLegTimer = new DispatcherTimer();
        public DispatcherTimer RemoveFromLimitListTimer = new DispatcherTimer();
        public ICommand AddSecurityLeg1Command { get; set; }
        public ICommand DeleteSecurityLeg1Command { get; set; }

        public ICommand AddSecurityLeg2Command { get; set; }
        public ICommand DeleteSecurityLeg2Command { get; set; }

        public ICommand AddEtfStocksLeg1Command { get; set; }
        public ICommand AddEtfStocksLeg2Command { get; set; }

        public ICommand ClearSecurityLeg1Command { get; set; }
        public ICommand ClearSecurityLeg2Command { get; set; }

        public ICommand ImportArbitrageCommand { get; set; }
        public ICommand ExportArbitrageCommand { get; set; }

        public ICommand IssueEntrustCommand { get; set; }
        public ICommand ImmediateEntrustCommand { get; set; }
        public ICommand SuperviseCommand { get; set; }

        public ICommand ContinueArbitrageCommand { get; set; }

        public ICommand StopArbitrageCommand { get; set; }
        public ICommand DeleteArbitrageCommand { get; set; }

        public readonly IEventAggregator EventAggregator;
        private IServiceLocator _serviceLocator;

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
        public TradePanelArbitrageViewModel(IEventAggregator eventAgg,
            FullPresentSecurityCodeSearchProvider securitySearchProviderLeg1,
            FullPresentSecurityCodeSearchProvider securitySearchProviderLeg2, IServiceLocator serviceLocator)
        {
            EventAggregator = eventAgg;
            _serviceLocator = serviceLocator;

            SecuritySearchProviderLeg1 = securitySearchProviderLeg1;
            SecuritySearchProviderLeg2 = securitySearchProviderLeg2;
            InitCommands();
            SubscribeEvents();

            RefreshTimer.Tick += RefreshTimerOnTick;
            RefreshTimer.Interval = new TimeSpan(0, 0, 1);
            RefreshTimer.Start();


            SuperviseTimer.Tick += SuperviseTimerOnTick;
            SuperviseTimer.Interval = new TimeSpan(0, 0, 1);
            SuperviseTimer.Start();

            SecondLegTimer.Tick += SecondLegTimerOnTick;
            SecondLegTimer.Interval = new TimeSpan(0, 0, 1);
            SecondLegTimer.Start();

            RemoveFromLimitListTimer.Tick += RemoveFromLimitListTimerOnTick;
            RemoveFromLimitListTimer.Interval = new TimeSpan(0, 0, 0, 100);
            RemoveFromLimitListTimer.Start();
        }

        private void RemoveFromLimitListTimerOnTick(object sender, EventArgs e)
        {
            var keysToRemove = new List<string>();

            foreach (var kv in UpLimitList)
            {
                var quote = kv.Value.Quotation;

                if (quote.LastPx < quote.UpLimitPx)
                {
                    keysToRemove.Add(kv.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                Logger.Debug(string.Format("删除涨停证券{0}", key));
                SecurityInfo s;
                UpLimitList.TryRemove(key, out s);
            }

            keysToRemove = new List<string>();

            foreach (var kv in DownLimitList)
            {
                var quote = kv.Value.Quotation;

                if (quote.LastPx > quote.DownLimitPx)
                {
                    keysToRemove.Add(kv.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                Logger.Debug(string.Format("删除跌停证券{0}", key));
                SecurityInfo s;
                DownLimitList.TryRemove(key, out s);
            }
        }

        private bool CheckTriggerContition(Arbitrage arbitrage)
        {
            var trigger = false;

            switch (arbitrage.CompareSymbol)
            {
                case CompareSymbol.大于:
                {
                    trigger = arbitrage.DisplaySecurityInfoLeg1.Quotation.LastPx -
                              arbitrage.DisplaySecurityInfoLeg2.Quotation.LastPx > PriceDifference;
                    break;
                }
                case CompareSymbol.小于:
                {
                    trigger = arbitrage.DisplaySecurityInfoLeg1.Quotation.LastPx -
                              arbitrage.DisplaySecurityInfoLeg2.Quotation.LastPx < PriceDifference;
                    break;
                }
                default:
                {
                    break;
                }
            }

            return trigger;
        }
        private void SecondLegTimerOnTick(object sender, EventArgs eventArgs)
        {
            var removedKeys = new List<string>();

            foreach (var kv in Trader.LegDictionary)
            {
                var myNumber = kv.Key;
                var storedArbitrage = kv.Value;

                if (!string.IsNullOrEmpty(storedArbitrage.PreviousLegNumber))
                {
                    //买入的时候，忽略涨停板的证券；卖出的时候，忽略跌停板的证券。
                    var qPreviousStockEntrust = (from ei in Trader.EntrustInfoCollection.StockEntrustInfoList
                        where
                            ei.ThirdReff.Equals(storedArbitrage.PreviousLegNumber) &&
                            (ei.EntrustDirection == eEntrustDirection.买入
                                ? !UpLimitList.Keys.Contains(ei.SecurityID)
                                : !DownLimitList.Keys.Contains(ei.SecurityID))
                        select ei).ToList();

                    var qPreviousFutureEntrust = (from ei in Trader.EntrustInfoCollection.FutureEntrustInfoList
                        where ei.ThirdReff.Equals(storedArbitrage.PreviousLegNumber) &&
                              (ei.EntrustDirection == eEntrustDirection.买入
                                  ? !UpLimitList.Keys.Contains(ei.SecurityID)
                                  : !DownLimitList.Keys.Contains(ei.SecurityID))
                        select ei).ToList();

                    qPreviousStockEntrust.AddRange(qPreviousFutureEntrust);

                    if (qPreviousStockEntrust.Count > 0 &&
                        qPreviousStockEntrust.All(ei => ei.EntrustState == eEntrustState.已成))
                    {
                        var arbitrage = storedArbitrage.Arbitrage;

                        switch (arbitrage.BasketTradeMethod)
                        {
                            case ArbitrageTradeMethod.两腿同时:
                            {
                                var trigger = CheckTriggerContition(arbitrage);

                                if (trigger)
                                {
                                    var isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg1, arbitrage,
                                        arbitrage.SelectedCombiNoLeg1, arbitrage.EntrustDirectionLeg1,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg1,
                                        arbitrage.FuturesDirection);
                                    isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg2, arbitrage,
                                        arbitrage.SelectedCombiNoLeg2, arbitrage.EntrustDirectionLeg2,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg2,
                                        arbitrage.FuturesDirection);
                                }

                                break;
                            }
                            case ArbitrageTradeMethod.第一腿第二腿:
                            {
                                var trigger = CheckTriggerContition(arbitrage);

                                if (trigger && storedArbitrage.ArbitrageItemsLeg1 != null)
                                {
                                    var isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg1, arbitrage,
                                        arbitrage.SelectedCombiNoLeg1, arbitrage.EntrustDirectionLeg1,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg1,
                                        arbitrage.FuturesDirection);
                                }

                                if (storedArbitrage.ArbitrageItemsLeg2 != null)
                                {
                                    var isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg2, arbitrage,
                                        arbitrage.SelectedCombiNoLeg2, arbitrage.EntrustDirectionLeg2,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg2,
                                        arbitrage.FuturesDirection);
                                }
                                break;
                            }
                            case ArbitrageTradeMethod.第二腿第一腿:
                            {
                                var trigger = CheckTriggerContition(arbitrage);

                                if (storedArbitrage.ArbitrageItemsLeg1 != null)
                                {
                                    var isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg1, arbitrage,
                                        arbitrage.SelectedCombiNoLeg1, arbitrage.EntrustDirectionLeg1,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg1,
                                        arbitrage.FuturesDirection);
                                }

                                if (trigger && storedArbitrage.ArbitrageItemsLeg2 != null)
                                {
                                    var isSuccess = NewLegEntrust(storedArbitrage.ArbitrageItemsLeg2, arbitrage,
                                        arbitrage.SelectedCombiNoLeg2, arbitrage.EntrustDirectionLeg2,
                                        storedArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg2,
                                        arbitrage.FuturesDirection);
                                }
                                break;
                            }
                        }

                        arbitrage.CurrentTime = storedArbitrage.CurrentTime;

                        if (storedArbitrage.IsLast)
                        {
                            arbitrage.ArbitrageStatus = ArbitrageStatus.已执行;
                        }

                        removedKeys.Add(myNumber);
                    }
                }
            }

            //移除已经执行的延迟套利
            for (var i = 0; i < removedKeys.Count; i++)
            {
                LegNumberToArbitrage removed;
                Trader.LegDictionary.TryRemove(removedKeys[i], out removed);
            }
        }

        private List<eEntrustDirection> _stockEntrustDirection;

        public List<eEntrustDirection> StockEntrustDirectionList
        {
            get { return _stockEntrustDirection; }
            set { SetProperty(ref _stockEntrustDirection, value); }
        }

        private void SubscribeEvents()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<AddEtfStocksNotifyEvent>().Subscribe(OnAddStocks, ThreadOption.UIThread);
            }
        }

        private void OnAddStocks(string obj)
        {
            if (_leg1)
            {
                AddEtfStocksToLeg(ArbitrageItemListLeg1);
            }
            else
            {
                AddEtfStocksToLeg(ArbitrageItemListLeg2);
            }
        }

        private void AddEtfStocksToLeg(ICollection<ArbitrageItem> arbitrageItemList)
        {
            foreach (var etfStock in Trader.EtfStockCollection.EtfStockList)
            {
                var securityInfo =
                    MarketDataService.GetSecurityInfo(
                        CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(etfStock.MarketType)),
                        etfStock.SecurityID);

                if (securityInfo != null && securityInfo.SecurityID != null && securityInfo.ExID != null)
                {
                    var item = new ArbitrageItem {SecurityInfo = securityInfo, Amount = etfStock.StockAmount};
                    arbitrageItemList.Add(item);
                }
                else
                {
                    MessageBox.Show(string.Format("获取{0} {1}的信息失败!", etfStock.SecurityName, etfStock.SecurityID));
                }
            }
        }
        private void RefreshTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (MenuBar.CombiNos.Count > 0)
            {
                if (string.IsNullOrEmpty(SelectedCombiNoLeg1))
                {
                    SelectedCombiNoLeg1 = MenuBar.CombiNos[0];
                }

                if (string.IsNullOrEmpty(SelectedCombiNoLeg2))
                {
                    SelectedCombiNoLeg2 = MenuBar.CombiNos[0];
                }
            }

            RefreshLastPrice(ArbitrageItemListLeg1, true);
            RefreshLastPrice(ArbitrageItemListLeg2, false);
        }

        private object _locker = new object();

        private void ExcuteArbitrage(Arbitrage arbitrage, bool isImmediate)
        {
            var triggerArbitrage = false;
            if (isImmediate)
            {
                triggerArbitrage = true;
            }
            else
            {
                if (arbitrage.IssueNow)
                {
                    triggerArbitrage = true;
                    arbitrage.IssueNow = false;
                }
                else
                {
                    if (arbitrage.ArbitrageStatus == ArbitrageStatus.正在监控)
                    {
                        switch (arbitrage.CompareSymbol)
                        {
                            case CompareSymbol.大于:
                            {
                                if (CurrentPriceDifference > arbitrage.PriceDifference)
                                {
                                    triggerArbitrage = true;
                                }
                                break;
                            }
                            case CompareSymbol.小于:
                            {
                                if (CurrentPriceDifference < arbitrage.PriceDifference)
                                {
                                    triggerArbitrage = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }


            if (triggerArbitrage)
            {
                if (arbitrage.ExecutionTimes > 1)
                {
                    arbitrage.ArbitrageStatus = ArbitrageStatus.正在执行;
                }
                else
                {
                    arbitrage.ArbitrageStatus = ArbitrageStatus.已执行;
                }

                var executionTime2LegNumbers = new Dictionary<int, Tuple<int, int>>();
                var legNumbers = new List<int>();

                //生成腿号
                if (BasketTradeMethod == ArbitrageTradeMethod.两腿同时) //两腿同时，只生成一个腿号，两腿一样
                {
                    for (var i = 0; i < arbitrage.ExecutionTimes; i++)
                    {
                        legNumbers.Add(CommonUtil.ThirdReff++);
                    }
                }
                else //两腿异步，生成不同的腿号
                {
                    for (var i = 0; i < arbitrage.ExecutionTimes; i++)
                    {
                        var leg1 = CommonUtil.ThirdReff++;
                        var leg2 = CommonUtil.ThirdReff++;
                        var tuple = new Tuple<int, int>(leg1, leg2);
                        executionTime2LegNumbers[i] = tuple;
                    }
                }

                switch (BasketTradeMethod)
                {
                    case ArbitrageTradeMethod.两腿同时:
                    {
                        //保存需要延迟触发的套利
                        for (var i = 0; i < legNumbers.Count; i++)
                        {
                            var legNumberToArbitrage = new LegNumberToArbitrage
                            {
                                MyLegNumber = Convert.ToString(legNumbers[i]),
                                Arbitrage = arbitrage,
                                ArbitrageItemsLeg1 = arbitrage.ArbitrageItemListLeg1,
                                ArbitrageItemsLeg2 = arbitrage.ArbitrageItemListLeg2,
                                CurrentTime = i + 1
                            };

                            if (i == legNumbers.Count - 1)
                            {
                                legNumberToArbitrage.IsLast = true;
                            }

                            if (i > 0) //i > 0 说明是需要延迟的套利
                            {
                                legNumberToArbitrage.PreviousLegNumber = Convert.ToString(legNumbers[i - 1]);
                                Trader.LegDictionary[legNumberToArbitrage.MyLegNumber] = legNumberToArbitrage;
                            }
                            else //i == 0说明是第一次，直接触发
                            {
                                arbitrage.CurrentTime = i + 1;
                                var isSuccessful = NewLegEntrust(ArbitrageItemListLeg1, arbitrage,
                                    arbitrage.SelectedCombiNoLeg1, arbitrage.EntrustDirectionLeg1,
                                    legNumberToArbitrage.MyLegNumber, arbitrage.LimitEntrustRatioLeg1,
                                    arbitrage.FuturesDirection);

                                if (isSuccessful)
                                {
                                    NewLegEntrust(ArbitrageItemListLeg2, arbitrage, arbitrage.SelectedCombiNoLeg2,
                                        arbitrage.EntrustDirectionLeg2, legNumberToArbitrage.MyLegNumber,
                                        arbitrage.LimitEntrustRatioLeg2, arbitrage.FuturesDirection);
                                }
                            }
                        }

                        break;
                    }
                    case ArbitrageTradeMethod.第一腿第二腿:
                    {
                        for (var i = 0; i < executionTime2LegNumbers.Count; i++)
                        {
                            var currentLeg1Number = Convert.ToString(executionTime2LegNumbers[i].Item1);
                            var currentLeg2Number = Convert.ToString(executionTime2LegNumbers[i].Item2);

                            if (i > 0)
                            {
                                //触发下一轮第一腿的前提，是上一轮第二腿完成
                                var leg1NumberToArbitrage = new LegNumberToArbitrage
                                {
                                    PreviousLegNumber = Convert.ToString(executionTime2LegNumbers[i - 1].Item2),
                                    MyLegNumber = currentLeg1Number,
                                    Arbitrage = arbitrage,
                                    ArbitrageItemsLeg1 = arbitrage.ArbitrageItemListLeg1,
                                    CurrentTime = i + 1
                                };

                                Trader.LegDictionary[currentLeg1Number] = leg1NumberToArbitrage;

                                //触发第二腿的前提，是本轮第一腿完成
                                var leg2NumberToArbitrage = new LegNumberToArbitrage
                                {
                                    PreviousLegNumber = currentLeg1Number,
                                    MyLegNumber = currentLeg2Number,
                                    Arbitrage = arbitrage,
                                    ArbitrageItemsLeg2 = arbitrage.ArbitrageItemListLeg2,
                                    CurrentTime = i + 1
                                };

                                //最后完成的是最后一轮的第二腿
                                if (i == executionTime2LegNumbers.Count - 1)
                                {
                                    leg2NumberToArbitrage.IsLast = true;
                                }

                                Trader.LegDictionary[currentLeg2Number] = leg2NumberToArbitrage;
                            }
                            else //第一轮的第一腿直接触发
                            {
                                var isSuccessful = NewLegEntrust(arbitrage.ArbitrageItemListLeg1, arbitrage,
                                    arbitrage.SelectedCombiNoLeg1, arbitrage.EntrustDirectionLeg1, currentLeg1Number,
                                    arbitrage.LimitEntrustRatioLeg1, arbitrage.FuturesDirection);

                                if (isSuccessful)
                                {
                                    var legNumberToArbitrage = new LegNumberToArbitrage
                                    {
                                        PreviousLegNumber = currentLeg1Number,
                                        MyLegNumber = currentLeg2Number,
                                        Arbitrage = arbitrage,
                                        ArbitrageItemsLeg2 = arbitrage.ArbitrageItemListLeg2,
                                        CurrentTime = i + 1
                                    };

                                    Trader.LegDictionary[currentLeg2Number] = legNumberToArbitrage;
                                }
                            }
                        }

                        break;
                    }
                    case ArbitrageTradeMethod.第二腿第一腿:
                    {
                        for (var i = 0; i < executionTime2LegNumbers.Count; i++)
                        {
                            var currentLeg1Number = Convert.ToString(executionTime2LegNumbers[i].Item1);
                            var currentLeg2Number = Convert.ToString(executionTime2LegNumbers[i].Item2);

                            if (i > 0)
                            {
                                //触发下一轮第二腿的前提，是上一轮第一腿完成
                                var leg2NumberToArbitrage = new LegNumberToArbitrage
                                {
                                    PreviousLegNumber = Convert.ToString(executionTime2LegNumbers[i - 1].Item1),
                                    MyLegNumber = currentLeg2Number,
                                    Arbitrage = arbitrage,
                                    ArbitrageItemsLeg2 = arbitrage.ArbitrageItemListLeg2,
                                    CurrentTime = i + 1
                                };

                                Trader.LegDictionary[currentLeg2Number] = leg2NumberToArbitrage;

                                //触发第一腿的前提，是本轮第二腿完成
                                var leg1NumberToArbitrage = new LegNumberToArbitrage
                                {
                                    PreviousLegNumber = currentLeg2Number,
                                    MyLegNumber = currentLeg1Number,
                                    Arbitrage = arbitrage,
                                    ArbitrageItemsLeg1 = arbitrage.ArbitrageItemListLeg1,
                                    CurrentTime = i + 1
                                };

                                //最后完成的是最后一轮的第一腿
                                if (i == executionTime2LegNumbers.Count - 1)
                                {
                                    leg1NumberToArbitrage.IsLast = true;
                                }

                                Trader.LegDictionary[currentLeg1Number] = leg1NumberToArbitrage;
                            }
                            else //第一轮的第二腿直接触发
                            {
                                var isSuccessful = NewLegEntrust(arbitrage.ArbitrageItemListLeg2, arbitrage,
                                    arbitrage.SelectedCombiNoLeg2, arbitrage.EntrustDirectionLeg2, currentLeg2Number,
                                    arbitrage.LimitEntrustRatioLeg2, arbitrage.FuturesDirection);

                                if (isSuccessful)
                                {
                                    var legNumberToArbitrage = new LegNumberToArbitrage
                                    {
                                        PreviousLegNumber = currentLeg2Number,
                                        MyLegNumber = currentLeg1Number,
                                        Arbitrage = arbitrage,
                                        ArbitrageItemsLeg1 = arbitrage.ArbitrageItemListLeg1,
                                        CurrentTime = i + 1
                                    };

                                    Trader.LegDictionary[currentLeg1Number] = legNumberToArbitrage;
                                }
                            }
                        }

                        break;
                    }
                }

            }
        }

        private void SuperviseTimerOnTick(object sender, EventArgs eventArgs)
        {
            lock (_locker)
            {
                foreach (var arbitrage in ArbitrageList)
                {
                    ExcuteArbitrage(arbitrage, false);
                }
            }
        }

        /// <summary>
        /// 一腿的篮子委托
        /// </summary>
        /// <param name="arbitrageItemListLeg"></param>
        /// <param name="arbitrage"></param>
        /// <param name="combiNo"></param>
        /// <param name="entrustDirection"></param>
        /// <param name="legNumber"></param>
        /// <param name="limitEntrustRatio"></param>
        /// <param name="futuresDirection"></param>
        /// <returns></returns>
        private bool NewLegEntrust(ObservableCollection<ArbitrageItem> arbitrageItemListLeg, ArbitrageBase arbitrage,
            string combiNo, eEntrustDirection entrustDirection, string legNumber, string limitEntrustRatio, eFuturesDirection futuresDirection)
        {
            if (arbitrageItemListLeg == null)
            {
                return false;
            }

            var listEntrust = new List<Entrust>();

            foreach (var item in arbitrageItemListLeg)
            {
                //忽略停牌的证券
                if (item.SecurityInfo.IsSuspension)
                {
                    continue;
                }

                Entrust entrust;

                if (item.SecurityInfo.SecurityType == eCategory.期货)
                {
                    entrust = new FutureEntrust();
                }
                else
                {
                    if (item.SecurityInfo.SecurityType == eCategory.基金 ||
                        item.SecurityInfo.SecurityType == eCategory.股票)
                    {
                        entrust = new Entrust();
                    }
                    else
                    {
                        MessageBox.Show("只支持期货、场内基金、股票!");
                        return false;
                    }
                }

                entrust.StockCode = item.SecurityInfo.SecurityID;
                entrust.MarketNo = CommonUtil.eMarketTypeToeMarketNo(item.SecurityInfo.MarketType);
                entrust.CombiNo = combiNo;
                entrust.EntrustDirection =
                    CommonUtil.eEntrustDirectionToEntrustDirection(entrustDirection);
                entrust.PriceType = CommonUtil.eEntrustPriceTypeToEntrustPriceType(eEntrustPriceType.限价);

                var quote = MarketDataService.GetAndSubscribeSecurityQuote(item.SecurityInfo.ExID,
                    item.SecurityInfo.SecurityID);

                if (quote.LastPx > 0)
                {
                    entrust.EntrustPrice = quote.LastPx;
                }
                else
                {
                    entrust.EntrustPrice = 1;
                }

                entrust.EntrustAmount = item.Amount;
                entrust.LimitEntrustRatio = entrust.FtrLimitEntrustRatio = Convert.ToDouble(limitEntrustRatio.Replace("%", ""));
                entrust.FuturesDirection = CommonUtil.eFuturesDirectionToFuturesDirection(futuresDirection);
                entrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;
                entrust.ThirdReff = legNumber;

                listEntrust.Add(entrust);
            }

            Trader.InsertOrderBasket(listEntrust);

            return true;
        }

        private void RefreshLastPrice(IEnumerable<ArbitrageItem> arbitrageItemList, bool leg1)
        {
            if (leg1)
            {
                TotalValueLeg1 = 0;
            }
            else
            {
                TotalValueLeg2 = 0;
            }


            foreach (var item in arbitrageItemList)
            {
                var quote = MarketDataService.GetAndSubscribeSecurityQuote(item.SecurityInfo.ExID,
                    item.SecurityInfo.SecurityID);

                if (quote != null)
                {
                    item.LastPrice = quote.LastPx;

                    if (leg1)
                    {
                        switch (item.SecurityInfo.SecurityType)
                        {
                            case eCategory.基金:
                            case eCategory.股票:
                            case eCategory.债券:
                            {
                                TotalValueLeg1 += item.LastPrice*item.Amount;

                                if (item.SecurityInfo != null)
                                {
                                    var q = from pi in Trader.PositionInfoCollection.StockPositionInfoList
                                            where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg1)
                                            select pi.EnableAmount;

                                    if (q.Count() > 0)
                                    {
                                        item.EnabledAmount = q.First();
                                    }
                                    else
                                    {
                                        item.EnabledAmount = 0;
                                    }
                                }


                                break;
                            }
                            case eCategory.期货:
                            {
                                var securityId = item.SecurityInfo.SecurityID;
                                var q = from fi in Trader.InstrumentInfoCollection.InstrumentInfoList
                                    where fi.SecurityID.Equals(securityId) select fi;

                                if (q.Count() > 0)
                                {
                                    TotalValueLeg1 += item.LastPrice*item.Amount*q.First().Multiple;
                                }

                                if (item.SecurityInfo != null)
                                {
                                    IEnumerable<int> qq = new List<int>();

                                    if (FuturesDirection == eFuturesDirection.平仓)
                                    {                                 
                                        if (EntrustDirectionLeg1 == eEntrustDirection.买入) //找空仓
                                        {
                                            qq = from pi in Trader.PositionInfoCollection.FuturePositionInfoList
                                                     where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg1) && pi.PositionFlag.Equals(ePositionFlag.空头持仓)
                                                     select pi.EnableAmount;
                                        }
                                        else
                                        {
                                            qq = from pi in Trader.PositionInfoCollection.FuturePositionInfoList
                                                 where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg1) && pi.PositionFlag.Equals(ePositionFlag.多头持仓)
                                                 select pi.EnableAmount;
                                        }
                                    } 
                                    
                                    if (qq.Count() > 0)
                                    {
                                        item.EnabledAmount = qq.First();
                                    }
                                    else
                                    {
                                        item.EnabledAmount = 0;
                                    }

                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        switch (item.SecurityInfo.SecurityType)
                        {
                            case eCategory.基金:
                            case eCategory.股票:
                            case eCategory.债券:
                                {
                                    TotalValueLeg2 += item.LastPrice * item.Amount;

                                    if (item.SecurityInfo != null)
                                    {
                                        var q = from pi in Trader.PositionInfoCollection.StockPositionInfoList
                                                where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg2)
                                                select pi.EnableAmount;

                                        if (q.Count() > 0)
                                        {
                                            item.EnabledAmount = q.First();
                                        }
                                        else
                                        {
                                            item.EnabledAmount = 0;
                                        }
                                    }
                                    break;
                                }
                            case eCategory.期货:
                                {
                                    var securityId = item.SecurityInfo.SecurityID;
                                    var q = from fi in Trader.InstrumentInfoCollection.InstrumentInfoList
                                            where fi.SecurityID.Equals(securityId)
                                            select fi;

                                    if (q.Count() > 0)
                                    {
                                        TotalValueLeg2 += item.LastPrice * item.Amount * q.First().Multiple;
                                    }

                                    if (item.SecurityInfo != null)
                                    {
                                        IEnumerable<int> qq = new List<int>();

                                        if (FuturesDirection == eFuturesDirection.平仓)
                                        {
                                            if (EntrustDirectionLeg2 == eEntrustDirection.买入) //找空仓
                                            {
                                                qq = from pi in Trader.PositionInfoCollection.FuturePositionInfoList
                                                     where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg2) && pi.PositionFlag.Equals(ePositionFlag.空头持仓)
                                                     select pi.EnableAmount;
                                            }
                                            else
                                            {
                                                qq = from pi in Trader.PositionInfoCollection.FuturePositionInfoList
                                                     where pi.SecurityID.Equals(item.SecurityInfo.SecurityID) && pi.CombiNo.Equals(SelectedCombiNoLeg2) && pi.PositionFlag.Equals(ePositionFlag.多头持仓)
                                                     select pi.EnableAmount;
                                            }
                                        }

                                        if (qq.Count() > 0)
                                        {
                                            item.EnabledAmount = qq.First();
                                        }
                                        else
                                        {
                                            item.EnabledAmount = 0;
                                        }

                                    }
                                    break;
                                }
                        }
                    }
                }
            }

            foreach (var arbitrage in ArbitrageList)
            {
                arbitrage.CurrentPriceDifference = arbitrage.DisplaySecurityInfoLeg1.Quotation.LastPx -
                                                   arbitrage.DisplaySecurityInfoLeg2.Quotation.LastPx;
            }

            if (DisplaySecurityInfoLeg1 != null && DisplaySecurityInfoLeg2 != null)
            {
                CurrentPriceDifference = DisplaySecurityInfoLeg1.Quotation.LastPx -
                                         DisplaySecurityInfoLeg2.Quotation.LastPx;
            }
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshLastPrice(ArbitrageItemListLeg1, true);
            RefreshLastPrice(ArbitrageItemListLeg2, false);
        }

       

        private SecurityCodeSearchProvider _securitySearchProviderLeg1;

        public SecurityCodeSearchProvider SecuritySearchProviderLeg1
        {
            get { return _securitySearchProviderLeg1; }
            set { SetProperty(ref _securitySearchProviderLeg1, value); }
        }

        private SecurityCodeSearchProvider _securitySearchProviderLeg2;

        public SecurityCodeSearchProvider SecuritySearchProviderLeg2
        {
            get { return _securitySearchProviderLeg2; }
            set { SetProperty(ref _securitySearchProviderLeg2, value); }
        }


        private ArbitrageItem _selectedArbitrageItemLeg1;
        public ArbitrageItem SelectedArbitrageItemLeg1
        {
            get { return _selectedArbitrageItemLeg1; }
            set
            {
                SetProperty(ref _selectedArbitrageItemLeg1, value);
            }
        }

        private ArbitrageItem _selectedArbitrageItemLeg2;
        public ArbitrageItem SelectedArbitrageItemLeg2
        {
            get { return _selectedArbitrageItemLeg2; }
            set
            {
                SetProperty(ref _selectedArbitrageItemLeg2, value);
            }
        }

        private double _totalValueLeg1;
        public double TotalValueLeg1
        {
            get { return _totalValueLeg1; }
            set
            {
                SetProperty(ref _totalValueLeg1, value);
            }
        }

        private double _totalValueLeg2;
        public double TotalValueLeg2
        {
            get { return _totalValueLeg2; }
            set
            {
                SetProperty(ref _totalValueLeg2, value);
            }
        }

        private ObservableCollection<Arbitrage> _arbitrageList = new ObservableCollection<Arbitrage>();
        public ObservableCollection<Arbitrage> ArbitrageList
        {
            get { return _arbitrageList; }
            set
            {
                SetProperty(ref _arbitrageList, value);
            }
        }

        //protected override void SubscribeEvents()
        //{
        //    throw new NotImplementedException();
        //}

        protected void InitCommands()
        {
            AddSecurityLeg1Command = new DelegateCommand(OnAddSecurityLeg1);
            DeleteSecurityLeg1Command = new DelegateCommand(OnDeleteSecurityLeg1);
            AddSecurityLeg2Command = new DelegateCommand(OnAddSecurityLeg2);
            DeleteSecurityLeg2Command = new DelegateCommand(OnDeleteSecurityLeg2);
            AddEtfStocksLeg1Command = new DelegateCommand(OnAddEtfStocksLeg1);
            AddEtfStocksLeg2Command = new DelegateCommand(OnAddEtfStocksLeg2);
            ClearSecurityLeg1Command = new DelegateCommand(OnClearSecurityLeg1);
            ClearSecurityLeg2Command = new DelegateCommand(OnClearSecurityLeg2);
            IssueEntrustCommand = new DelegateCommand(OnIssueEntrust);
            ImmediateEntrustCommand = new DelegateCommand(OnImmediateEntrust);
            SuperviseCommand = new DelegateCommand(OnSupervise);
            ImportArbitrageCommand = new DelegateCommand(OnImportArbitrage);
            ExportArbitrageCommand = new DelegateCommand(OnExportArbitrage);
            StopArbitrageCommand = new DelegateCommand(OnStopArbitrage);
            ContinueArbitrageCommand = new DelegateCommand(OnContinueArbitrage);
            DeleteArbitrageCommand = new DelegateCommand(OnDeleteArbitrage);
        }

        private void OnIssueEntrust()
        {
            if (MessageBox.Show("确定要直接下单吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var arbitrage = new Arbitrage
                {
                    NameLeg1 = DisplaySecurityInfoLeg1.SecurityName,
                    NameLeg2 = DisplaySecurityInfoLeg2.SecurityName,
                    ExecutionTimes = ExecutionTimes,
                    PriceDifferenceType = PriceDifferenceType,
                    BasketTradeMethod = BasketTradeMethod,
                    CompareSymbol = CompareSymbol,
                    PriceDifference = PriceDifference,
                    DisplaySecurityInfoLeg1 = DisplaySecurityInfoLeg1,
                    DisplaySecurityInfoLeg2 = DisplaySecurityInfoLeg2,
                    EntrustAmountLeg1 = EntrustAmountLeg1,
                    EntrustAmountLeg2 = EntrustAmountLeg2,
                    SelectedCombiNoLeg1 = SelectedCombiNoLeg1,
                    SelectedCombiNoLeg2 = SelectedCombiNoLeg2,
                    EntrustDirectionLeg1 = EntrustDirectionLeg1,
                    EntrustDirectionLeg2 = EntrustDirectionLeg2,
                    ArbitrageItemListLeg1 = ArbitrageItemListLeg1,
                    ArbitrageItemListLeg2 = ArbitrageItemListLeg2,
                    LimitEntrustRatioLeg1 = LimitEntrustRatioLeg1,
                    LimitEntrustRatioLeg2 = LimitEntrustRatioLeg2,
                    CurrentPriceDifference =
                        DisplaySecurityInfoLeg1.Quotation.LastPx - DisplaySecurityInfoLeg2.Quotation.LastPx,
                    FuturesDirection = FuturesDirection,
                    IssueNow = true
                };

                ArbitrageList.Add(arbitrage);
            }
        }

        private void OnClearSecurityLeg2()
        {
            if (MessageBox.Show("确定要清空证券列表吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                ArbitrageItemListLeg2.Clear();
            }
        }

        private void OnClearSecurityLeg1()
        {
            if (MessageBox.Show("确定要清空证券列表吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                ArbitrageItemListLeg1.Clear();
            }
        }

        private void OnDeleteSecurityLeg2()
        {
            var items = ArbitrageItemListLeg2.Where(p => p.IsChecked).ToList();

            if (items.Count > 0)
            {
                if (MessageBox.Show("确定要删除该证券吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                    MessageBoxResult.Yes)
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        ArbitrageItemListLeg2.Remove(items[i]);
                    }
                }
            }
        }

        private void OnAddSecurityLeg2()
        {
            if (SelectedSecurityInfoLeg2 != null)
            {
                var item = CreateDefaultArbitrageItem(SelectedSecurityInfoLeg2);
                ArbitrageItemListLeg2.Add(item);
            }
        }

        private void OnDeleteSecurityLeg1()
        {
            var items = ArbitrageItemListLeg1.Where(p => p.IsChecked).ToList();

            if (items.Count > 0)
            {
                if (MessageBox.Show("确定要删除该证券吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                    MessageBoxResult.Yes)
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        ArbitrageItemListLeg1.Remove(items[i]);
                    }
                }
            }
        }

        private ArbitrageItem CreateDefaultArbitrageItem(SecurityInfo securityInfo)
        {
            var item = new ArbitrageItem { SecurityInfo = securityInfo };
            switch (item.SecurityInfo.SecurityType)
            {
                case eCategory.股票:
                    {
                        item.Amount = 100;
                        break;
                    }
                case eCategory.期货:
                case eCategory.基金:
                    {
                        item.Amount = 1;
                        break;
                    }
            }

            return item;
        }

        private void OnAddSecurityLeg1()
        {
            if (SelectedSecurityInfoLeg1 != null)
            {
                var item = CreateDefaultArbitrageItem(SelectedSecurityInfoLeg1); 
                ArbitrageItemListLeg1.Add(item);
            }
        }

        private void OnContinueArbitrage()
        {
            if (MessageBox.Show("确定要继续套利策略吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var items = ArbitrageList.Where(p => p.IsChecked);

                foreach (var item in items)
                {
                    item.ArbitrageStatus = ArbitrageStatus.正在监控;
                }
            }
        }

        private const string Separator = "***";
        private void OnExportArbitrage()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "导出套利配置...",
                    Filter = @"arbitrage files(*.arbitrage)|*.arbitrage|All files(*.*)|*.*",
                    RestoreDirectory = true
                };

                if (DisplaySecurityInfoLeg1 != null && DisplaySecurityInfoLeg2 != null)
                {
                    dialog.FileName = string.Format("{0}_{1}", DisplaySecurityInfoLeg1.SecurityName,
                        DisplaySecurityInfoLeg2.SecurityName);
                }
                else
                {
                    MessageBox.Show("请输入两腿参考品种");
                    return;
                }

                if (dialog.ShowDialog() != DialogResult.OK) return;
                var sw = new StreamWriter(dialog.FileName, false, Encoding.UTF8);

                sw.WriteLine(PriceDifferenceType);
                sw.WriteLine(BasketTradeMethod);
                sw.WriteLine(CompareSymbol);
                sw.WriteLine(PriceDifference);
                sw.WriteLine(FuturesDirection);

                //第一腿
                sw.WriteLine(DisplaySecurityInfoLeg1.ExID);
                sw.WriteLine(DisplaySecurityInfoLeg1.SecurityID);                
                sw.WriteLine(SelectedCombiNoLeg1);
                sw.WriteLine(EntrustDirectionLeg1);
                sw.WriteLine(LimitEntrustRatioLeg1);

                sw.WriteLine(Separator);
                //套利品种的具体信息
                foreach (var arbitrageItem in ArbitrageItemListLeg1)
                {
                    var s = string.Format("{0},{1},{2}", arbitrageItem.SecurityInfo.ExID,
                        arbitrageItem.SecurityInfo.SecurityID, arbitrageItem.Amount);
                    sw.WriteLine(s);
                }
                sw.WriteLine(Separator);

                //第二腿
                sw.WriteLine(DisplaySecurityInfoLeg2.ExID);
                sw.WriteLine(DisplaySecurityInfoLeg2.SecurityID);
                sw.WriteLine(SelectedCombiNoLeg2);
                sw.WriteLine(EntrustDirectionLeg2);
                sw.WriteLine(LimitEntrustRatioLeg2);

                sw.WriteLine(Separator);
                //套利品种的具体信息
                foreach (var arbitrageItem in ArbitrageItemListLeg2)
                {
                    var s = string.Format("{0},{1},{2}", arbitrageItem.SecurityInfo.ExID,
                        arbitrageItem.SecurityInfo.SecurityID, arbitrageItem.Amount);
                    sw.WriteLine(s);
                }
                sw.WriteLine(Separator);

                sw.Close();

                MessageBox.Show("导出套利信息文件成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information,
                    MessageBoxResult.OK);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
            }
        }

        private void OnImportArbitrage()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "导入套利配置...",
                    Filter = @"arbitrage files(*.arbitrage)|*.arbitrage|All files(*.*)|*.*"
                };

                if (dialog.ShowDialog() != DialogResult.OK) return;

                var sr = new StreamReader(dialog.FileName, Encoding.UTF8);
                try
                {
                    string line;
                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        PriceDifferenceType = (PriceDifferenceType) Enum.Parse(typeof (PriceDifferenceType), line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        BasketTradeMethod = (ArbitrageTradeMethod)Enum.Parse(typeof(ArbitrageTradeMethod), line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        CompareSymbol = (CompareSymbol)Enum.Parse(typeof(CompareSymbol), line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        PriceDifference = Convert.ToDouble(line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        FuturesDirection = (eFuturesDirection)Enum.Parse(typeof(eFuturesDirection), line);
                    }

                    //第一腿
                    string exId = null, securityId = null;
                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        exId = line;
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        securityId = line;
                    }

                    DisplaySecurityInfoLeg1 = MarketDataService.GetSecurityInfo(exId, securityId);

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (MenuBar.CombiNos.Contains(line))
                        {
                            SelectedCombiNoLeg1 = line;
                        }
                        else
                        {
                            if (MenuBar.CombiNos.Count > 0)
                            {
                                SelectedCombiNoLeg1 = MenuBar.CombiNos[0];
                            }
                        }
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        EntrustDirectionLeg1 = (eEntrustDirection)Enum.Parse(typeof(eEntrustDirection), line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        LimitEntrustRatioLeg1 = line;
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line) && line.Equals(Separator))
                    {
                        ArbitrageItemListLeg1.Clear();
                        while ((line = sr.ReadLine()) != Separator && line != null)
                        {
                            var s = line.Split(new[] { ',' }, StringSplitOptions.None);

                            var arbitrageItem = new ArbitrageItem
                            {
                                SecurityInfo = MarketDataService.GetSecurityInfo(s[0], s[1]),
                                Amount = Convert.ToInt32(s[2])
                            };
                            var quote = MarketDataService.GetAndSubscribeSecurityQuote(arbitrageItem.SecurityInfo.ExID,
                                arbitrageItem.SecurityInfo.SecurityID);
                            arbitrageItem.LastPrice = quote.LastPx;
                            ArbitrageItemListLeg1.Add(arbitrageItem);
                        }
                    }

                    
                    //第二腿
                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        exId = line;
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        securityId = line;
                    }

                    DisplaySecurityInfoLeg2 = MarketDataService.GetSecurityInfo(exId, securityId);

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (MenuBar.CombiNos.Contains(line))
                        {
                            SelectedCombiNoLeg2 = line;
                        }
                        else
                        {
                            if (MenuBar.CombiNos.Count > 0)
                            {
                                SelectedCombiNoLeg2 = MenuBar.CombiNos[0];
                            }
                        }
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        EntrustDirectionLeg2 = (eEntrustDirection)Enum.Parse(typeof(eEntrustDirection), line);
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        LimitEntrustRatioLeg2 = line;
                    }

                    line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line) && line.Equals(Separator))
                    {
                        ArbitrageItemListLeg2.Clear();
                        while ((line = sr.ReadLine()) != Separator && line != null)
                        {
                            var s = line.Split(new[] { ',' }, StringSplitOptions.None);

                            var arbitrageItem = new ArbitrageItem
                            {
                                SecurityInfo = MarketDataService.GetSecurityInfo(s[0], s[1]),
                                Amount = Convert.ToInt32(s[2])
                            };
                            var quote = MarketDataService.GetAndSubscribeSecurityQuote(arbitrageItem.SecurityInfo.ExID,
                                arbitrageItem.SecurityInfo.SecurityID);
                            arbitrageItem.LastPrice = quote.LastPx;
                            ArbitrageItemListLeg2.Add(arbitrageItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("反序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("反序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
            }
        }

        private void OnDeleteArbitrage()
        {
            if (MessageBox.Show("确定要删除套利策略吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var items = ArbitrageList.Where(p => p.IsChecked).ToList();

                for (var i = 0; i < items.Count; i++)
                {
                    ArbitrageList.Remove(items[i]);
                }
            }
        }

        private void OnStopArbitrage()
        {
            if (MessageBox.Show("确定要停止套利策略吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var items = ArbitrageList.Where(p => p.IsChecked);

                foreach (var item in items)
                {
                    item.ArbitrageStatus = ArbitrageStatus.停止;
                }
            }
        }

        /// <summary>
        /// 检查是否满足下单条件
        /// </summary>
        /// <param name="arbitrageItems"></param>
        /// <param name="leg1Number"></param>
        /// <param name="entrustDirection"></param>
        /// <param name="futuresDirection"></param>
        /// <returns>是否满足，标的数量，总市值，停牌，涨停，跌停</returns>
        private Tuple<bool, LegInfo> CheckSecurity(ObservableCollection<ArbitrageItem> arbitrageItems,
            string leg1Number,
            eEntrustDirection entrustDirection, eFuturesDirection futuresDirection)
        {
            if (leg1Number.Equals("第一腿"))
            {
                if (DisplaySecurityInfoLeg1 == null)
                {
                    MessageBox.Show(string.Format("{0}的参照品种尚未设置!", leg1Number));
                    return new Tuple<bool, LegInfo>(false, null);
                }
            }
            else
            {
                if (DisplaySecurityInfoLeg2 == null)
                {
                    MessageBox.Show(string.Format("{0}的参照品种尚未设置!", leg1Number));
                    return new Tuple<bool, LegInfo>(false, null);
                }
            }  

            if (arbitrageItems.Count <= 0)
            {
                MessageBox.Show(string.Format("{0}的列表中没有任何证券!", leg1Number));
                return new Tuple<bool, LegInfo>(false, null);
            }


            var legInfo = new LegInfo();

            legInfo.Number = arbitrageItems.Count;

            if (leg1Number.Equals("第一腿"))
            {
                legInfo.TotalValue = TotalValueLeg1;
            }
            else
            {
                legInfo.TotalValue = TotalValueLeg2;
            }           

            var q = from item in arbitrageItems where item.SecurityInfo.IsSuspension select item;

            if (q.Count() > 0)
            {
                var sb = new StringBuilder();

                foreach (var item in q)
                {
                    sb.Append(string.Format(" {0}{1} ", item.SecurityInfo.SecurityName, item.SecurityInfo.SecurityID));
                }

                legInfo.StoppedSecurity = sb.ToString();
            }

            q = from item in arbitrageItems where item.SecurityInfo.IsExpire select item;

            if (q.Count() > 0)
            {
                var sb = new StringBuilder();

                foreach (var item in q)
                {
                    sb.Append(string.Format(" {0}{1} ", item.SecurityInfo.SecurityName, item.SecurityInfo.SecurityID));
                }

                legInfo.CancelledSecurity = sb.ToString();
            }

            //股票\期货涨停
            if (entrustDirection == eEntrustDirection.买入)
            {
                q = from item in arbitrageItems
                    where
                        (item.SecurityInfo.SecurityType == eCategory.债券 ||
                         item.SecurityInfo.SecurityType == eCategory.基金 ||
                         item.SecurityInfo.SecurityType == eCategory.股票) && item.SecurityInfo.Quotation.LastPx > 0 &&
                        item.SecurityInfo.Quotation.LastPx.Equals(item.SecurityInfo.Quotation.UpLimitPx)
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}{1} ", item.SecurityInfo.SecurityName, item.SecurityInfo.SecurityID));
                        if (!UpLimitList.ContainsKey(item.SecurityInfo.SecurityID))
                        {
                            Logger.Debug(string.Format("增加涨停证券{0}", item.SecurityInfo.SecurityID));
                            UpLimitList[item.SecurityInfo.SecurityID] = item.SecurityInfo;
                        }
                    }

                    legInfo.StockUpperLimitSecurity = sb.ToString();
                }

                q = from item in arbitrageItems
                    where
                        (item.SecurityInfo.SecurityType == eCategory.期货) && item.SecurityInfo.Quotation.LastPx > 0 &&
                        item.SecurityInfo.Quotation.LastPx.Equals(item.SecurityInfo.Quotation.UpLimitPx)
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}", item.SecurityInfo.SecurityName));
                        if (!UpLimitList.ContainsKey(item.SecurityInfo.SecurityID))
                        {
                            Logger.Debug(string.Format("增加涨停证券{0}", item.SecurityInfo.SecurityID));
                            UpLimitList[item.SecurityInfo.SecurityID] = item.SecurityInfo;
                        }
                    }

                    legInfo.FuturesUpperLimitSecurity = sb.ToString();
                }
            }

            //股票\期货跌停
            if (entrustDirection == eEntrustDirection.卖出)
            {
                q = from item in arbitrageItems
                    where
                        (item.SecurityInfo.SecurityType == eCategory.债券 ||
                         item.SecurityInfo.SecurityType == eCategory.基金 ||
                         item.SecurityInfo.SecurityType == eCategory.股票) && item.SecurityInfo.Quotation.LastPx > 0 &&
                        item.SecurityInfo.Quotation.LastPx.Equals(item.SecurityInfo.Quotation.DownLimitPx)
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}{1} ", item.SecurityInfo.SecurityName, item.SecurityInfo.SecurityID));
                        if (!DownLimitList.ContainsKey(item.SecurityInfo.SecurityID))
                        {
                            Logger.Debug(string.Format("增加跌停证券{0}", item.SecurityInfo.SecurityID));
                            DownLimitList[item.SecurityInfo.SecurityID] = item.SecurityInfo;
                        }
                    }

                    legInfo.StockLowerLimitSecurity = sb.ToString();
                }

                q = from item in arbitrageItems
                    where
                        (item.SecurityInfo.SecurityType == eCategory.期货) && item.SecurityInfo.Quotation.LastPx > 0 &&
                        item.SecurityInfo.Quotation.LastPx.Equals(item.SecurityInfo.Quotation.DownLimitPx)
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}", item.SecurityInfo.SecurityName));
                        if (!DownLimitList.ContainsKey(item.SecurityInfo.SecurityID))
                        {
                            Logger.Debug(string.Format("增加跌停证券{0}", item.SecurityInfo.SecurityID));
                            DownLimitList[item.SecurityInfo.SecurityID] = item.SecurityInfo;
                        }
                    }

                    legInfo.FuturesLowerLimitSecurity = sb.ToString();
                }
            }

            //股票可用
            if (entrustDirection == eEntrustDirection.卖出)
            {
                q = from item in arbitrageItems
                    where
                        (item.SecurityInfo.SecurityType == eCategory.债券 ||
                         item.SecurityInfo.SecurityType == eCategory.基金 ||
                         item.SecurityInfo.SecurityType == eCategory.股票) && item.Amount > item.EnabledAmount
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}{1} ", item.SecurityInfo.SecurityName, item.SecurityInfo.SecurityID));
                    }

                    MessageBox.Show(string.Format("{0}中，{1}可用不足，无法交易!", leg1Number, sb));

                    return new Tuple<bool, LegInfo>(false, null);
                }
            }

            //期货可用
            if (futuresDirection == eFuturesDirection.平仓)
            {
                q = from item in arbitrageItems
                    where (item.SecurityInfo.SecurityType == eCategory.期货) && item.Amount > item.EnabledAmount
                    select item;

                if (q.Count() > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var item in q)
                    {
                        sb.Append(string.Format(" {0}", item.SecurityInfo.SecurityName));
                    }

                    MessageBox.Show(string.Format("{0}中，{1}可用不足，无法交易!", leg1Number, sb));

                    return new Tuple<bool, LegInfo>(false, null);
                }
            }

            return new Tuple<bool, LegInfo>(true, legInfo);
        }

        private void OnSupervise()
        {
            var check1 = CheckSecurity(ArbitrageItemListLeg1, "第一腿", EntrustDirectionLeg1, FuturesDirection);
            var check2 = CheckSecurity(ArbitrageItemListLeg2, "第二腿", EntrustDirectionLeg2, FuturesDirection);

            if (!check1.Item1 || !check2.Item1)
            {
                return;
            }

            var prompt = new StringBuilder();

            prompt.AppendLine("第一腿：");
            prompt.AppendLine(string.Format("操作：{0}", EntrustDirectionLeg1));
            prompt.AppendLine(string.Format("标的数量：{0}", check1.Item2.Number));
            prompt.AppendLine(string.Format("总市值：{0}", TotalValueLeg1));
            if (!string.IsNullOrEmpty(check1.Item2.StoppedSecurity))
            {
                prompt.AppendLine(string.Format("股票停牌：{0}", check1.Item2.StoppedSecurity));
            }

            if (!string.IsNullOrEmpty(check1.Item2.StockUpperLimitSecurity))
            {
                prompt.AppendLine(string.Format("股票涨停：{0}", check1.Item2.StockUpperLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check1.Item2.StockLowerLimitSecurity))
            {
                prompt.AppendLine(string.Format("股票跌停：{0}", check1.Item2.StockLowerLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check1.Item2.FuturesUpperLimitSecurity))
            {
                prompt.AppendLine(string.Format("期货涨停：{0}", check1.Item2.FuturesUpperLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check1.Item2.FuturesLowerLimitSecurity))
            {
                prompt.AppendLine(string.Format("期货跌停：{0}", check1.Item2.FuturesLowerLimitSecurity));
            }


            prompt.AppendLine("第二腿：");
            prompt.AppendLine(string.Format("操作：{0}", EntrustDirectionLeg2));
            prompt.AppendLine(string.Format("标的数量：{0}", check2.Item2.Number));
            prompt.AppendLine(string.Format("总市值：{0}", TotalValueLeg2));
            if (!string.IsNullOrEmpty(check2.Item2.StoppedSecurity))
            {
                prompt.AppendLine(string.Format("股票停牌：{0}", check2.Item2.StoppedSecurity));
            }

            if (!string.IsNullOrEmpty(check2.Item2.StockUpperLimitSecurity))
            {
                prompt.AppendLine(string.Format("股票涨停：{0}", check2.Item2.StockUpperLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check2.Item2.StockLowerLimitSecurity))
            {
                prompt.AppendLine(string.Format("股票跌停：{0}", check2.Item2.StockLowerLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check2.Item2.FuturesUpperLimitSecurity))
            {
                prompt.AppendLine(string.Format("期货涨停：{0}", check2.Item2.FuturesUpperLimitSecurity));
            }

            if (!string.IsNullOrEmpty(check2.Item2.FuturesLowerLimitSecurity))
            {
                prompt.AppendLine(string.Format("期货跌停：{0}", check2.Item2.FuturesLowerLimitSecurity));
            }

            if (MessageBox.Show(prompt.ToString(), "确认加入监控吗？", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var arbitrage = new Arbitrage
                {
                    NameLeg1 = DisplaySecurityInfoLeg1.SecurityName,
                    NameLeg2 = DisplaySecurityInfoLeg2.SecurityName,
                    ExecutionTimes = ExecutionTimes,
                    PriceDifferenceType = PriceDifferenceType,
                    BasketTradeMethod = BasketTradeMethod,
                    CompareSymbol = CompareSymbol,
                    PriceDifference = PriceDifference,
                    DisplaySecurityInfoLeg1 = DisplaySecurityInfoLeg1,
                    DisplaySecurityInfoLeg2 = DisplaySecurityInfoLeg2,
                    EntrustAmountLeg1 = EntrustAmountLeg1,
                    EntrustAmountLeg2 = EntrustAmountLeg2,
                    SelectedCombiNoLeg1 = SelectedCombiNoLeg1,
                    SelectedCombiNoLeg2 = SelectedCombiNoLeg2,
                    EntrustDirectionLeg1 = EntrustDirectionLeg1,
                    EntrustDirectionLeg2 = EntrustDirectionLeg2,
                    ArbitrageItemListLeg1 = ArbitrageItemListLeg1,
                    ArbitrageItemListLeg2 = ArbitrageItemListLeg2,
                    LimitEntrustRatioLeg1 = LimitEntrustRatioLeg1,
                    LimitEntrustRatioLeg2 = LimitEntrustRatioLeg2,
                    CurrentPriceDifference =
                        DisplaySecurityInfoLeg1.Quotation.LastPx - DisplaySecurityInfoLeg2.Quotation.LastPx,
                    FuturesDirection = FuturesDirection
                };


                ArbitrageList.Add(arbitrage);
            }

        }

        private void OnImmediateEntrust()
        {
            if (MessageBox.Show("确定要立即执行套利策略吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                var items = ArbitrageList.Where(p => p.IsChecked);

                foreach (var item in items)
                {
                    ExcuteArbitrage(item, true);
                }
            }
        }

        private bool _leg1 = true;

        private void OnAddEtfStocksLeg2()
        {
            _leg1 = false;
            var p = _serviceLocator.GetInstance<QryEtfStockListWindow>();
            p.Show();
        }

        private void OnAddEtfStocksLeg1()
        {
            _leg1 = true;
            var p = _serviceLocator.GetInstance<QryEtfStockListWindow>();
            p.Show();
        }

        public void OnImportsSatisfied()
        {
            StockEntrustDirectionList = MenuBar.StockEntrustDirection;
            ArbitrageItem.Trader = Trader;
        }
    }

}

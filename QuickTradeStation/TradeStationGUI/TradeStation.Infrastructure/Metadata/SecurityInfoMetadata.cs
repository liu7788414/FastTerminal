using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Metadata
{
    [Export]
    public class SecurityInfoMetadata : BindableBase
    {
        [Import]
        private MarketDataService marketDataService { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public bool IsTradingDate { get; set; }
        public DateTime LastTradingDate { get; set; }
        public DateTime NextTradingDate { get; set; }

        // 无风险利率
        public double RFRate { get; set; }

        public Dictionary<string, IList<MarketPeriodRangeModel>> ExchangeTradePeriodDictionary { get; private set; }
        public Dictionary<string, Dictionary<eKLinePeriodType, IList<KLinePeriod>>> KLineExchangeTradePeriodDictionary { get; private set; }
        public Dictionary<string, MarketPeriodRangeModel> MarketVarietyMaxMinTradeTime { get; private set; }
        public Dictionary<ExSecID, IList<ExrightRatioModel>> ExrightRatiosMap { get; private set; }

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public OptionInfoModelCollection OptionInfoModelCollection { get; set; }

        private Dictionary<ExSecID, SecurityInfo> securityInfoMap = new Dictionary<ExSecID, SecurityInfo>();

        protected bool isGetCodeTabeFinished = false;
        public bool IsGetCodeTabeFinished
        {
            get { return isGetCodeTabeFinished; }
            set
            {
                SetProperty(ref isGetCodeTabeFinished, value);
            }
        }

        public Dictionary<ExSecID, SecurityInfo> SecurityInfoMap
        {
            get { return securityInfoMap; }
        }

        public List<SecurityInfo> GetSecurityList()
        {
            return securityInfoMap.Values.ToList<SecurityInfo>();
        }

        [ImportingConstructor]
        public SecurityInfoMetadata(IEventAggregator eventAggr)
        {
            EventAggregator = eventAggr;

            IsTradingDate = false;

            LastTradingDate = DateTime.Now.Date.AddDays(-1);
            NextTradingDate = DateTime.Now.Date.AddDays(1);

            KLineExchangeTradePeriodDictionary = new Dictionary<string, Dictionary<eKLinePeriodType, IList<KLinePeriod>>>();
            MarketVarietyMaxMinTradeTime = new Dictionary<string, MarketPeriodRangeModel>();
            ExrightRatiosMap = new Dictionary<ExSecID, IList<ExrightRatioModel>>();
        }

        public void InitTradeDate(bool isTradingDate, long lastTradingDate, long nextTradingDate)
        {
            this.IsTradingDate = isTradingDate;

            this.LastTradingDate = DateTimeHelper.ConvertToDate(lastTradingDate);
            this.NextTradingDate = DateTimeHelper.ConvertToDate(nextTradingDate);

            TimeKeeper.UpdateNextInitializationTime(isTradingDate, this.NextTradingDate);
        }

        public void InitTradePeriodDictionary(IList<ExchangeTradePeriodModel> periods)
        {
            // Initial the metadata dictionary.
            if (null == ExchangeTradePeriodDictionary)
            {
                ExchangeTradePeriodDictionary = new Dictionary<string, IList<MarketPeriodRangeModel>>();
            }
            if (null == KLineExchangeTradePeriodDictionary)
            {
                KLineExchangeTradePeriodDictionary = new Dictionary<string, Dictionary<eKLinePeriodType, IList<KLinePeriod>>>();
            }
            if (null == MarketVarietyMaxMinTradeTime)
            {
                MarketVarietyMaxMinTradeTime = new Dictionary<string, MarketPeriodRangeModel>();
            }

            ExchangeTradePeriodDictionary.Clear();
            KLineExchangeTradePeriodDictionary.Clear();
            MarketVarietyMaxMinTradeTime.Clear();

            var kLineTypeList = Enum.GetValues(typeof(eKLinePeriodType)).OfType<eKLinePeriodType>().ToList();

            // Currently, it is useless. Lately, it will be configurable.
            //long systemShutDownTime = 160000;
            var systemShutDownTime = new TimeSpan(20, 0, 0);

            var currentDate = DateTime.Now.Date;
            var lastTradingDate = LastTradingDate;
            var nextTradingDate = NextTradingDate;
            var workingDate = LastTradingDate.AddDays(1);

            if (DateTime.Now.TimeOfDay >= systemShutDownTime)
            {
                lastTradingDate = currentDate;
                currentDate = NextTradingDate;
                workingDate = lastTradingDate.AddDays(1);
            }

            // Produce the min & max trading time.
            foreach (var period in periods)
            {
                var varietyId = (period.ExID + "_" + period.VarietyType + "_" + period.MarketTag).ToLower();
                var formattedPeriodList = new List<MarketPeriodRangeModel>();

                var minTradingTime = DateTime.MaxValue;
                var maxTradingTime = DateTime.MinValue;

                foreach (var timeRange in period.PeriodRanges)
                {
                    // For the stock, the targetDate always is currentDate.
                    var targetStartDate = timeRange.StartTime.TimeOfDay < systemShutDownTime ? currentDate : lastTradingDate;
                    var targetEndDate = timeRange.EndTime.TimeOfDay < systemShutDownTime ? currentDate : lastTradingDate;

                    if (timeRange.StartTime.TimeOfDay >= systemShutDownTime && timeRange.EndTime.TimeOfDay < systemShutDownTime)
                    {
                        targetEndDate = workingDate;
                    }

                    timeRange.StartTime = timeRange.StartTime
                        .AddYears(targetStartDate.Year - timeRange.StartTime.Year)
                        .AddMonths(targetStartDate.Month - timeRange.StartTime.Month)
                        .AddDays(targetStartDate.Day - timeRange.StartTime.Day);

                    timeRange.EndTime = timeRange.EndTime
                        .AddYears(targetEndDate.Year - timeRange.EndTime.Year)
                        .AddMonths(targetEndDate.Month - timeRange.EndTime.Month)
                        .AddDays(targetEndDate.Day - timeRange.EndTime.Day);

                    minTradingTime = minTradingTime <= timeRange.StartTime ? minTradingTime : timeRange.StartTime;
                    maxTradingTime = maxTradingTime >= timeRange.EndTime ? maxTradingTime : timeRange.EndTime;

                    formattedPeriodList.Add(new MarketPeriodRangeModel()
                    {
                        StartTime = timeRange.StartTime,
                        EndTime = timeRange.EndTime,
                    });
                }

                if (!MarketVarietyMaxMinTradeTime.ContainsKey(varietyId))
                {
                    ExchangeTradePeriodDictionary.Add(varietyId, formattedPeriodList);
                }

                if (MarketVarietyMaxMinTradeTime.ContainsKey(varietyId))
                {
                    // Do nothing.
                }
                else
                {
                    // Records the min & max trading time.
                    MarketVarietyMaxMinTradeTime.Add(
                        varietyId,
                        new MarketPeriodRangeModel()
                        {
                            StartTime = minTradingTime,
                            EndTime = maxTradingTime,
                        });
                }
            }

            // Intializes the real time model with the period model.
            foreach (var realTimeModel in marketDataService.RealTimeDataMap)
            {
                var exSecID = realTimeModel.Key;
                var realTimeMarketData = realTimeModel.Value;

                SecurityInfo securityInfo;
                if (securityInfoMap.TryGetValue(exSecID, out securityInfo))
                {
                    IList<MarketPeriodRangeModel> periodList;
                    if (ExchangeTradePeriodDictionary.TryGetValue(securityInfo.Variety, out periodList))
                    {
                        DispatcherService.Invoke(() =>
                        {
                            realTimeMarketData.InitializeDataForRealTime(periodList);
                        });
                    }
                    else
                    {
                        Logger.Debug(string.Format("Unexpected: Cannot find exchange trade period for {0} when initialize the real time model.", securityInfo.Variety));
                    }
                }
                else
                {
                    Logger.Debug(string.Format("Unexpected: Cannot find security info for {0}-{1} when initialize the real time model.", exSecID.ExID, exSecID.SecurityID));
                }
            }

            // Produce range of each K line types.
            foreach (var period in periods)
            {
                var varietyId = (period.ExID + "_" + period.VarietyType + "_" + period.MarketTag).ToLower();
                var kLinePeriodDictionary = new Dictionary<eKLinePeriodType, IList<KLinePeriod>>();

                foreach (var kLineType in kLineTypeList)
                {
                    // If the k line period is more than 1 hour, set a empty period list directly.
                    if ((int) kLineType > 60)
                    {
                        kLinePeriodDictionary.Add(kLineType, new List<KLinePeriod>());

                        continue;
                    }

                    long remainMitues = 0;
                    var kLinePeriodList = new List<KLinePeriod>();

                    for (int ix = 0; ix < period.PeriodRanges.Count; ix++)
                    {
                        var totalMinutes = (period.PeriodRanges[ix].EndTime - period.PeriodRanges[ix].StartTime).TotalMinutes;

                        remainMitues = (long)totalMinutes % (long)kLineType;

                        var rangeStartTime = period.PeriodRanges[ix].StartTime;
                        var rangeNextStartTime = period.PeriodRanges[ix].StartTime.AddMinutes((int)kLineType);

                        while (rangeNextStartTime <= period.PeriodRanges[ix].EndTime)
                        {
                            var itemRange = new MarketPeriodRangeModel()
                            {
                                StartTime = rangeStartTime,
                                EndTime = rangeNextStartTime.AddSeconds(-1),
                            };

                            kLinePeriodList.Add(new KLinePeriod()
                            {
                                KLineIndex = DateTimeHelper.ConvertToDateTimeInt(rangeNextStartTime),
                                Period = itemRange,
                            });

                            rangeStartTime = rangeNextStartTime;
                            rangeNextStartTime = rangeNextStartTime.AddMinutes((int)kLineType);
                        }

                        // This part is fit for future market.
                        if (remainMitues > 0)
                        {
                            // It is not the last period.
                            if (ix < period.PeriodRanges.Count - 1)
                            {
                                rangeNextStartTime = period.PeriodRanges[ix + 1].StartTime.AddMinutes(remainMitues);
                                period.PeriodRanges[ix + 1].StartTime = rangeNextStartTime;
                            }
                            // If it is the last period, always set the end time.
                            else
                            {
                                rangeNextStartTime = period.PeriodRanges[ix].EndTime;
                            }

                            var itemRange = new MarketPeriodRangeModel()
                            {
                                StartTime = rangeStartTime,
                                EndTime = rangeNextStartTime.AddSeconds(-1),
                            };

                            kLinePeriodList.Add(new KLinePeriod()
                            {
                                KLineIndex = DateTimeHelper.ConvertToDateTimeInt(rangeNextStartTime),
                                Period = itemRange,
                            });
                        }
                    }

                    kLinePeriodDictionary.Add(kLineType, kLinePeriodList);
                }

                if (KLineExchangeTradePeriodDictionary.ContainsKey(varietyId))
                {
                    Console.WriteLine(varietyId);
                }
                else
                {
                    KLineExchangeTradePeriodDictionary.Add(varietyId, kLinePeriodDictionary);
                }
            }
        }

        public void InitSecurityCodeMap(IList<SecurityInfo> codeList)
        {
            foreach (SecurityInfo code in codeList)
            {
                ExSecID exSecId = new ExSecID(code.ExID, code.SecurityID);
                securityInfoMap[exSecId] = code;
                InitSecurityPriceDigits(securityInfoMap[exSecId]);

                // Expired security will not have quotation and market real time data.
                if (!code.IsExpire)
                {
                    SecurityQuotation secQuot = new SecurityQuotation(code.ExID, code.SecurityID);
                    code.Quotation = secQuot;
                    marketDataService.AddOrUpdateSecurityQuotationToMap(code);
                }
                // If it is expired and existed in quotation map, then remove it.
                else if (marketDataService.SecurityQuotMap.ContainsKey(exSecId))
                {
                    marketDataService.SecurityQuotMap.Remove(exSecId);
                }
            }

            IsGetCodeTabeFinished = true;
        }

        public void InitExrightRatio(IList<ExrightRatioModel> exrightRatios)
        {
            if (null == ExrightRatiosMap)
            {
                ExrightRatiosMap = new Dictionary<ExSecID, IList<ExrightRatioModel>>();
            }

            ExrightRatiosMap.Clear();

            foreach (var ratio in exrightRatios)
            {
                var exSecID = new ExSecID(ratio.ExID, ratio.SecurityID);
                IList<ExrightRatioModel> targetList;

                if (!ExrightRatiosMap.TryGetValue(exSecID, out targetList))
                {
                    targetList = new List<ExrightRatioModel>();
                    ExrightRatiosMap.Add(exSecID, targetList);
                }

                targetList.Add(ratio);
            }

            foreach (var exrightRatioList in ExrightRatiosMap)
            {
                exrightRatioList.Value.OrderByDescending(x => x.TradeDate);
            }
        }

        public void InitOptionInfoCollection(IList<OptionInfo> optionInfoList)
        {
            OptionInfoModelCollection.OptionInfoList.Clear();

            foreach (var optionInfo in optionInfoList)
            {
                var optionDataModel = optionInfo.ToOptionInfoModel();
                var optionInfoModel = new OptionInfoModel
                {
                    MarketDataService = marketDataService,
                    NodeType = eNodeType.OptionInfoModelOriginal
                };

                // 取得期权合约简称最后一位字符
                // 若其为字母则表示标的已分红，将字母加到PriceTag后
                var optionExRightSymbol = optionDataModel.SecuritySymbol.Last().ToString();
                if (!Regex.IsMatch(optionExRightSymbol, "[A-Za-z]"))
                {
                    optionExRightSymbol = string.Empty;
                }

                if (null != optionDataModel)
                {
                    optionInfoModel.SecurityID = optionDataModel.SecurityID;
                    optionInfoModel.ExID = optionDataModel.ExID;
                    optionInfoModel.ContractID = optionDataModel.ContractID;
                    optionInfoModel.SecuritySymbol = optionDataModel.SecuritySymbol;
                    optionInfoModel.UnderlyingSecurityId = optionDataModel.UnderlyingSecurityId;
                    optionInfoModel.UnderlyingSymbol = optionDataModel.UnderlyingSymbol;
                    optionInfoModel.UnderlyingType = optionDataModel.UnderlyingType;
                    optionInfoModel.CallOrPut = optionDataModel.CallOrPut;
                    optionInfoModel.ContractMultiplierUnit = optionDataModel.ContractMultiplierUnit;
                    optionInfoModel.ExercisePrice = optionDataModel.ExercisePrice;
                    optionInfoModel.OptionExRightSymbol = optionExRightSymbol;
                    optionInfoModel.StartDate = optionDataModel.StartDate;
                    optionInfoModel.EndDate = optionDataModel.EndDate;
                    optionInfoModel.ExerciseDate = optionDataModel.ExerciseDate;
                    optionInfoModel.DeliveryDate = optionDataModel.DeliveryDate;
                    optionInfoModel.ExpireDate = optionDataModel.ExpireDate;
                    optionInfoModel.SettlePrice = optionDataModel.SettlePrice;
                    optionInfoModel.UnderlyingClosePx = optionDataModel.UnderlyingClosePx;
                    optionInfoModel.DailyPriceUpLimit = optionDataModel.DailyPriceUpLimit;
                    optionInfoModel.DailyPriceDownLimit = optionDataModel.DailyPriceDownLimit;
                    optionInfoModel.LimitOrderMaxFloor = optionDataModel.LimitOrderMaxFloor;
                    optionInfoModel.MarketOrderMaxFloor = optionDataModel.MarketOrderMaxFloor;
                }

                OptionInfoModelCollection.OptionInfoList.Add(optionInfoModel);
            }

            EventAggregator.GetEvent<OptionInfoReadyEvent>().Publish("Ready");
        }

        public void InitRFRate(double rate)
        {
            this.RFRate = rate / 100;
        }

        // Currently, only use the suspension status, and other suspension info will not be used.
        public void InitSuspensionInfo(IList<SuspensionInfoModel> suspensionInfoList)
        {
            // 获得当前本地的停牌信息
            var localSuspensionStockList = securityInfoMap.Where(x => x.Value.IsSuspension).Select(x => x.Value).ToList();

            // 设置已停牌证券的状态
            foreach (var suspensionInfo in suspensionInfoList)
            {
                var exSecID = new ExSecID(suspensionInfo.ExID, suspensionInfo.SecurityID);
                var securityInfo = localSuspensionStockList.FirstOrDefault(x => x.SecurityID == suspensionInfo.SecurityID);

                if (null != securityInfo
                    || securityInfoMap.TryGetValue(exSecID, out securityInfo))
                {
                    securityInfo.IsSuspension = true;

                    localSuspensionStockList.Remove(securityInfo);
                }
            }

            // 为重新复牌的证券设置状态
            foreach (var localSuspensionStock in localSuspensionStockList)
            {
                localSuspensionStock.IsSuspension = false;
            }
        }

        private void InitSecurityPriceDigits(SecurityInfo securityInfo)
        {
            if (0 != securityInfo.MinFloatingPrice)
            {
                double count = 0;

                while (Math.Pow(10, count) * securityInfo.MinFloatingPrice < 1)
                {
                    count++;
                }

                securityInfo.PriceDigits = (int)count;
            }
            else
            {
                if (securityInfo.SecurityType == eCategory.债券回购
                    || securityInfo.SecurityType == eCategory.基金
                    || securityInfo.SecurityType == eCategory.债券)
                {
                    securityInfo.PriceDigits = 3;
                }
                else
                {
                    securityInfo.PriceDigits = 2;
                }
            }
        }
    }
}

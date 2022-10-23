using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Services
{
    [Export]
    public class KLineProcessor
    {
        #region Private Fields

        private SecurityInfoMetadata _securityInfoMetadata;
        private MarketDataService _marketDataService;

        private Dictionary<string, RealTimeMarketDataPoint> preSavedKLinePoints { get; set; }

        #endregion

        [ImportingConstructor]
        public KLineProcessor(SecurityInfoMetadata securityInfoMetadata,
            MarketDataService marketDataService)
        {
            _securityInfoMetadata = securityInfoMetadata;
            _marketDataService = marketDataService;

            preSavedKLinePoints = new Dictionary<string, RealTimeMarketDataPoint>();
        }

        public void ProcessTickData(TickDataModel tickData)
        {
            var currentExSecID = new ExSecID(tickData.ExchangeID, tickData.SecurityID);
            Dictionary<eKLinePeriodType, RealTimeMarketData> kLineMarketDataDictionary;

            // Once the initialization is ready, the next progress can be processed.
            if (_marketDataService.KLineMarketDataMap.TryGetValue(currentExSecID, out kLineMarketDataDictionary)
                && null != kLineMarketDataDictionary[eKLinePeriodType.MIN1])
            {
                Dictionary<eKLinePeriodType, IList<KLinePeriod>> kLinePeriodDictionary;
                if (_securityInfoMetadata.KLineExchangeTradePeriodDictionary.TryGetValue(tickData.Variety, out kLinePeriodDictionary))
                {
                    foreach (var kLinePeriodPair in kLinePeriodDictionary)
                    {
                        var kLineType = kLinePeriodPair.Key;
                        var kLinePeriodList = kLinePeriodPair.Value;
                        var preSavedKLineDataKey = tickData.ExchangeID + "_" + tickData.SecurityID + "_" + kLineType.ToString();

                        // Once historical data of this K line type has been get from server, the next progress can be processed.
                        if (kLineMarketDataDictionary[kLineType].IsDataReady)
                        {
                            RealTimeMarketDataPoint preSavedKLineData;

                            // If it did not receive this K line data before, initialize it with the first tick data.
                            if (!preSavedKLinePoints.TryGetValue(preSavedKLineDataKey, out preSavedKLineData))
                            {
                                preSavedKLineData = new RealTimeMarketDataPoint()
                                {
                                    ExchangeTime = tickData.ExchangeTime,
                                    High = tickData.LastPrice,
                                    Low = tickData.LastPrice,
                                    Open = tickData.LastPrice,
                                    Close = tickData.LastPrice,
                                    Last = tickData.LastPrice,
                                    SumTurnover = tickData.Turnover,
                                    SumVolume = tickData.Volume,
                                };

                                var currentTradingDayKLines = kLineMarketDataDictionary[kLineType].RealTimeMarketDataPointSets
                                    .Where(x => x.ExchangeTime.Date == tickData.ExchangeTime.Date);

                                // 在交易时段进入，以最后一根K线的值来初始化preSavedKLineData
                                if (currentTradingDayKLines.Count() > 0)
                                {
                                    var lastKLine = currentTradingDayKLines.Last();

                                    preSavedKLineData.High = lastKLine.High;
                                    preSavedKLineData.Low = lastKLine.Low;
                                    preSavedKLineData.Open = lastKLine.Open;
                                }

                                preSavedKLineData.PreSumTurnover = currentTradingDayKLines.Sum(x => x.Turnover);
                                preSavedKLineData.PreSumVolume = currentTradingDayKLines.Sum(x => x.Volume);
                                preSavedKLineData.Turnover = preSavedKLineData.SumTurnover - preSavedKLineData.PreSumTurnover;
                                preSavedKLineData.Volume = preSavedKLineData.SumVolume - preSavedKLineData.PreSumVolume;
                                preSavedKLineData.LastOneInsert = -1;

                                MarketPeriodRangeModel minMaxTradeTime;
                                if (_securityInfoMetadata.MarketVarietyMaxMinTradeTime.TryGetValue(tickData.Variety, out minMaxTradeTime))
                                {
                                    preSavedKLineData.MinTradingTime = minMaxTradeTime.StartTime;
                                    preSavedKLineData.MaxTradingTime = minMaxTradeTime.EndTime;
                                }

                                preSavedKLinePoints.Add(preSavedKLineDataKey, preSavedKLineData);
                            }
                            // The K line data has received before, then start to process.
                            else
                            {
                                // If period is more than 1 hour, directly update the UIObject.
                                if ((int)kLineType > 60)
                                {
                                    // Currently, only update day k-line in long term type at real time.
                                    if (kLineType == eKLinePeriodType.DAY)
                                    {
                                        ApplyToLongPeriodUIObject(tickData, currentExSecID, kLineType);
                                    }

                                    continue;
                                }

                                long lastKLineIndex = 0;

                                // Market has not started trading.
                                if (tickData.ExchangeTime >= preSavedKLineData.ExchangeTime
                                    && tickData.ExchangeTime < preSavedKLineData.MinTradingTime)
                                {
                                    UpdateExistedKLineData(preSavedKLineData, tickData);
                                }
                                else if (tickData.ExchangeTime >= preSavedKLineData.ExchangeTime
                                    && tickData.ExchangeTime >= preSavedKLineData.MinTradingTime)
                                {
                                    foreach (var kLinePeriod in kLinePeriodList)
                                    {
                                        // 标准情况，数据处于K线过程中
                                        if (preSavedKLineData.ExchangeTime >= kLinePeriod.Period.StartTime && preSavedKLineData.ExchangeTime <= kLinePeriod.Period.EndTime
                                            && tickData.ExchangeTime >= kLinePeriod.Period.StartTime && tickData.ExchangeTime <= kLinePeriod.Period.EndTime)
                                        {
                                            UpdateExistedKLineData(preSavedKLineData, tickData);
                                            ApplyToUIObject(preSavedKLineData, currentExSecID, kLineType, kLinePeriod.KLineIndex, false);

                                            break;
                                        }
                                        // 标准情况，K线跨越到下一根
                                        else if (preSavedKLineData.ExchangeTime < kLinePeriod.Period.StartTime
                                            && tickData.ExchangeTime >= kLinePeriod.Period.StartTime && tickData.ExchangeTime <= kLinePeriod.Period.EndTime)
                                        {
                                            UpdateNewKLineData(preSavedKLineData, tickData);
                                            ApplyToUIObject(preSavedKLineData, currentExSecID, kLineType, kLinePeriod.KLineIndex, true);

                                            lastKLineIndex = kLinePeriod.KLineIndex;

                                            break;
                                        }
                                        // 盘中收盘
                                        else if (tickData.ExchangeTime > preSavedKLineData.MinTradingTime
                                            && tickData.ExchangeTime >= preSavedKLineData.ExchangeTime
                                            && tickData.ExchangeTime < kLinePeriod.Period.StartTime
                                            && preSavedKLineData.ExchangeTime < kLinePeriod.Period.StartTime)
                                        {
                                            UpdateExistedKLineData(preSavedKLineData, tickData);
                                            ApplyToUIObject(preSavedKLineData, currentExSecID, kLineType, lastKLineIndex, false);

                                            break;
                                        }
                                    }

                                    // 收盘后的数据处理，总是更新最后一根K线
                                    if (tickData.ExchangeTime >= preSavedKLineData.ExchangeTime
                                        && tickData.ExchangeTime >= preSavedKLineData.MaxTradingTime)
                                    {
                                        UpdateExistedKLineData(preSavedKLineData, tickData);
                                        ApplyToUIObject(preSavedKLineData, currentExSecID, kLineType, lastKLineIndex, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateExistedKLineData(RealTimeMarketDataPoint preSavedData, TickDataModel tickData)
        {
            preSavedData.ExchangeTime = tickData.ExchangeTime;
            //targetData.Sequence = tickData.Sequence;

            preSavedData.Close = tickData.LastPrice;
            preSavedData.High = preSavedData.High > tickData.LastPrice ? preSavedData.High : tickData.LastPrice;
            preSavedData.Low = preSavedData.Low < tickData.LastPrice ? preSavedData.Low : tickData.LastPrice;

            preSavedData.Turnover = tickData.Turnover - preSavedData.PreSumTurnover;
            preSavedData.Volume = tickData.Volume - preSavedData.PreSumVolume;
            preSavedData.SumTurnover = tickData.Turnover;
            preSavedData.SumVolume = tickData.Volume;

            //更新结算价等具体信息
            //preSavedData.PreClose = tickData.PreClosePrice;
            //preSavedData.PreSettlementPx = tickData.PreSettlementPx;
            //preSavedData.SettlementPx = tickData.SettlementPx;
            //preSavedData.UpLimitPx = tickData.UpLimitPx;
            //preSavedData.LowLimitPx = tickData.LowLimitPx;
            //preSavedData.PreOpenInterest = tickData.PreOpenInterest;
            //preSavedData.OpenInterest = tickData.OpenInterest;
            //preSavedData.PreDelta = tickData.PreDelta;
            //preSavedData.CurrDelta = tickData.CurrDelta;
        }

        private void UpdateNewKLineData(RealTimeMarketDataPoint preSavedData, TickDataModel tickData)
        {
            preSavedData.ExchangeTime = tickData.ExchangeTime;
            //preSavedData.Sequence = tickData.Sequence;

            //上一根K线的最新价作为新K线的前移收盘价
            preSavedData.PreClose = preSavedData.Last;
            preSavedData.Last = tickData.LastPrice;
            preSavedData.High = tickData.LastPrice;
            preSavedData.Low = tickData.LastPrice;
            preSavedData.Close = tickData.LastPrice;
            preSavedData.Open = tickData.LastPrice;

            //将上一根K线的累计值作为新K线的初始值
            preSavedData.PreSumTurnover = preSavedData.SumTurnover;
            preSavedData.PreSumVolume = preSavedData.SumVolume;
            preSavedData.Turnover = tickData.Turnover - preSavedData.PreSumTurnover;
            preSavedData.Volume = tickData.Volume - preSavedData.PreSumVolume;
            preSavedData.SumTurnover = tickData.Turnover;
            preSavedData.SumVolume = tickData.Volume;

            //更新结算价等具体信息，期货独有，无需处理
            //preSavedData.dayPreClose = tickData.dayPreClose;
            //preSavedData.PreSettlementPx = tickData.PreSettlementPx;
            //preSavedData.SettlementPx = tickData.SettlementPx;
            //preSavedData.UpLimitPx = tickData.UpLimitPx;
            //preSavedData.LowLimitPx = tickData.LowLimitPx;
            //preSavedData.PreOpenInterest = tickData.PreOpenInterest;
            //preSavedData.OpenInterest = tickData.OpenInterest;
            //preSavedData.PreDelta = tickData.PreDelta;
            //preSavedData.CurrDelta = tickData.CurrDelta;
        }

        private void ApplyToLongPeriodUIObject(TickDataModel tickData, ExSecID exSecID, eKLinePeriodType kLineType)
        {
            var targetKLines = _marketDataService.KLineMarketDataMap[exSecID][kLineType];

            targetKLines.DayHighPrice = targetKLines.DayHighPrice > tickData.HighPrice ? targetKLines.DayHighPrice : tickData.HighPrice;
            targetKLines.DayLowPrice = targetKLines.DayLowPrice < tickData.LowPrice ? targetKLines.DayLowPrice : tickData.LowPrice;
            targetKLines.TotalVolume = tickData.Volume;

            var targetKLine = targetKLines.RealTimeMarketDataPointSets.LastOrDefault(x => x.ExchangeTime == tickData.ExchangeTime.Date);

            if (null == targetKLine)
            {
                targetKLine = new RealTimeMarketDataPoint();
                targetKLine.ExchangeTime = tickData.ExchangeTime.Date;

                targetKLines.RealTimeMarketDataPointSets.Add(targetKLine);

                IList<ExrightRatioModel> ratioList;
                if (_securityInfoMetadata.ExrightRatiosMap.TryGetValue(exSecID, out ratioList))
                {
                    var lastRatio = ratioList.Last();

                    if (lastRatio != null && targetKLines.ExRightType == eDisplayedExRightType.前复权)
                    {
                        targetKLine.PriceRatio = lastRatio.ForwardFactor;
                    }
                    else if (lastRatio != null && targetKLines.ExRightType == eDisplayedExRightType.后复权)
                    {
                        targetKLine.PriceRatio = lastRatio.BackFactor;
                    }
                    else
                    {
                        targetKLine.PriceRatio = 1;
                    }
                }
            }

            targetKLine.Open = tickData.OpenPrice;
            targetKLine.Close = tickData.LastPrice;
            targetKLine.High = tickData.HighPrice;
            targetKLine.Low = tickData.LowPrice;
            targetKLine.Last = tickData.LastPrice;

            targetKLine.Turnover = tickData.Turnover;
            targetKLine.SumTurnover = tickData.Turnover;
            targetKLine.Volume = tickData.Volume;
            targetKLine.SumVolume = tickData.Turnover;
        }

        private void ApplyToUIObject(RealTimeMarketDataPoint preSavedData, ExSecID exSecID, eKLinePeriodType kLineType, long kLineIndex, bool isNew)
        {
            var targetKLines = _marketDataService.KLineMarketDataMap[exSecID][kLineType];

            targetKLines.DayHighPrice = targetKLines.DayHighPrice > preSavedData.High ? targetKLines.DayHighPrice : preSavedData.High;
            targetKLines.DayLowPrice = targetKLines.DayLowPrice < preSavedData.Low ? targetKLines.DayLowPrice : preSavedData.Low;
            targetKLines.TotalVolume = preSavedData.SumTurnover;

            var targetKLine = new RealTimeMarketDataPoint();
            var targetKLinePeriodTime = DateTimeHelper.ConvertToDateTime(kLineIndex * 100000);

            // If 'IsNew', then create a new K line data point.
            if (isNew)
            {
                targetKLine.ExchangeTime = targetKLinePeriodTime;

                targetKLines.RealTimeMarketDataPointSets.Add(targetKLine);
            }
            // If not 'IsNew', then find the target K line data point to update.
            else
            {
                // Find the K line data at the exactly time.
                targetKLine = targetKLines.RealTimeMarketDataPointSets.FirstOrDefault(x => x.ExchangeTime == targetKLinePeriodTime);
                if (null == targetKLine)
                {
                    // If it does not exist, then use the last data.
                    targetKLine = targetKLines.RealTimeMarketDataPointSets.LastOrDefault();
                }
            }

            // Update the fields with the pre-saved data information.
            if (null != targetKLine && targetKLine.ExchangeTime == targetKLinePeriodTime)
            {
                targetKLine.Open = preSavedData.Open;
                targetKLine.Close = preSavedData.Close;
                targetKLine.High = preSavedData.High;
                targetKLine.Low = preSavedData.Low;
                targetKLine.PreClose = preSavedData.PreClose;
                targetKLine.Last = preSavedData.Last;

                targetKLine.Turnover = preSavedData.Turnover;
                targetKLine.PreSumTurnover = preSavedData.PreSumTurnover;
                targetKLine.SumTurnover = preSavedData.SumTurnover;
                targetKLine.Volume = preSavedData.Volume;
                targetKLine.PreSumVolume = preSavedData.PreSumVolume;
                targetKLine.SumVolume = preSavedData.SumVolume;

                if (isNew)
                {
                    IList<ExrightRatioModel> ratioList;
                    if (_securityInfoMetadata.ExrightRatiosMap.TryGetValue(exSecID, out ratioList))
                    {
                        var lastRatio = ratioList.Last();

                        if (lastRatio != null && targetKLines.ExRightType == eDisplayedExRightType.前复权)
                        {
                            targetKLine.PriceRatio = lastRatio.ForwardFactor;
                        }
                        else if (lastRatio != null && targetKLines.ExRightType == eDisplayedExRightType.后复权)
                        {
                            targetKLine.PriceRatio = lastRatio.BackFactor;
                        }
                        else
                        {
                            targetKLine.PriceRatio = 1;
                        }
                    }
                }
            }
        }
    }
}

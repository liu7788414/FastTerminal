using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Services
{
    [Export]
    public class RealTimeDataProcessor
    {
        [Import]
        public InstrumentInfoCollection _futureInstrumentCollection;

        private SecurityInfoMetadata _securityInfoMetadata;

        private Dictionary<ExSecID, RealTimeMarketDataPoint> _lastTickPointDictionary;

        [ImportingConstructor]
        public RealTimeDataProcessor(SecurityInfoMetadata securityInfoMetadata)
        {
            _securityInfoMetadata = securityInfoMetadata;

            _lastTickPointDictionary = new Dictionary<ExSecID, RealTimeMarketDataPoint>();
        }

        // Updates real time data by history records.
        public void UpdateRealTimeDataByHistoryData(RealTimeMarketData targetData, SecurityInfo securityInfo, IList<KLineRecordModel> kLineRecords)
        {
            if (null == kLineRecords || kLineRecords.Count <= 0)
            {
                return;
            }

            if (kLineRecords.Any())
            {
                targetData.DayHighPrice = kLineRecords.Max(x => x.HighPx);
                targetData.DayLowPrice = kLineRecords.Min(x => x.LowPx);
            }

            if (null != securityInfo
                && null != securityInfo.Quotation
                && securityInfo.Quotation.PreClosePx != 0
                && targetData.PreClosePrice != securityInfo.Quotation.PreClosePx)
            {
                targetData.PreClosePrice = securityInfo.Quotation.PreClosePx;
            }

            // Only for future, if set multiplier for calculate average price.
            if (securityInfo.SecurityType == eCategory.期货 && !targetData.IsMultiplierReady
                && null != _futureInstrumentCollection && null != _futureInstrumentCollection.InstrumentInfoList)
            {
                var instrumentInfo = _futureInstrumentCollection.InstrumentInfoList.FirstOrDefault(x => x.SecurityID == securityInfo.SecurityID);
                if (null != instrumentInfo)
                {
                    targetData.VolumeMultiplier = instrumentInfo.Multiple;
                    targetData.IsMultiplierReady = true;
                }
            }

            // Caculates the range for the diagram.
            if (Math.Abs(targetData.DayHighPrice - targetData.PreClosePrice) > Math.Abs(targetData.DayLowPrice - targetData.PreClosePrice))
            {
                var highPercent = (targetData.DayHighPrice - targetData.PreClosePrice) / targetData.PreClosePrice;
                targetData.VisibleHighPrice = targetData.DayHighPrice;
                targetData.VisibleLowPrice = (1 - highPercent) * targetData.PreClosePrice;
                targetData.PriceInterval = ChineseMathRound((targetData.VisibleHighPrice - targetData.PreClosePrice) / CommonUtil.NUMBER_OF_DIAGRAM_INTERVAL, 4);
            }
            else
            {
                var lowPercent = (targetData.PreClosePrice - targetData.DayLowPrice) / targetData.PreClosePrice;
                targetData.VisibleLowPrice = targetData.DayLowPrice;
                targetData.VisibleHighPrice = (1 + lowPercent) * targetData.PreClosePrice;
                targetData.PriceInterval = ChineseMathRound((targetData.PreClosePrice - targetData.VisibleLowPrice) / CommonUtil.NUMBER_OF_DIAGRAM_INTERVAL, 4);
            }

            kLineRecords = kLineRecords.OrderBy(x => x.ExDateTime).ToList();

            double sumTurnover = 0;
            double sumVolume = 0;

            foreach (var kLineRecord in kLineRecords)
            {
                // Update the point if find one in the data collection.
                var targetPoint = targetData.RealTimeMarketDataPointSets.FirstOrDefault(x => x.ExchangeTime == kLineRecord.ExDateTime);

                if (null == targetPoint)
                {
                    targetPoint = new RealTimeMarketDataPoint()
                    {
                        ExchangeTime = kLineRecord.ExDateTime,
                    };
                    targetData.RealTimeMarketDataPointSets.Add(targetPoint);
                }

                sumTurnover += kLineRecord.Turnover;
                sumVolume += kLineRecord.Volume;

                targetPoint.High = kLineRecord.HighPx;
                targetPoint.Low = kLineRecord.LowPx;
                targetPoint.Open = kLineRecord.OpenPx;
                targetPoint.Close = kLineRecord.LastPx;
                targetPoint.Turnover = kLineRecord.Turnover;
                targetPoint.Volume = kLineRecord.Volume;
                targetPoint.SumTurnover = sumTurnover;
                targetPoint.SumVolume = sumVolume;
                targetPoint.RealTimeAverage = targetPoint.SumTurnover / (targetPoint.SumVolume * targetData.VolumeMultiplier);
                targetPoint.PriceChange = kLineRecord.LastPx - targetData.PreClosePrice;
                targetPoint.PriceChangeRate = targetPoint.PriceChange / targetData.PreClosePrice;
                targetPoint.IsTrueDataPoint = true;
            }
        }

        // Updates real time data by tick data.
        public void UpdateRealTimeDataByTickData(RealTimeMarketData targetData, TickDataModel tickData)
        {
            //LastTickData = tickData;

            //UpLimitPrice = tickData.UpLimitPx;
            //DownLimitPrice = tickData.DownLimitPx;
            targetData.PreClosePrice = tickData.PreClosePrice;
            targetData.DayHighPrice = tickData.HighPrice;
            targetData.DayLowPrice = tickData.LowPrice;

            // Caculates the range for the diagram.
            if (Math.Abs(targetData.DayHighPrice - tickData.PreClosePrice) > Math.Abs(targetData.DayLowPrice - tickData.PreClosePrice))
            {
                var highPercent = (targetData.DayHighPrice - tickData.PreClosePrice) / tickData.PreClosePrice;
                targetData.VisibleHighPrice = targetData.DayHighPrice;
                targetData.VisibleLowPrice = (1 - highPercent) * tickData.PreClosePrice;
                targetData.PriceInterval = ChineseMathRound((targetData.VisibleHighPrice - tickData.PreClosePrice) / CommonUtil.NUMBER_OF_DIAGRAM_INTERVAL, 4);
            }
            else
            {
                var lowPercent = (tickData.PreClosePrice - targetData.DayLowPrice) / tickData.PreClosePrice;
                targetData.VisibleLowPrice = targetData.DayLowPrice;
                targetData.VisibleHighPrice = (1 + lowPercent) * tickData.PreClosePrice;
                targetData.PriceInterval = ChineseMathRound((tickData.PreClosePrice - targetData.VisibleLowPrice) / CommonUtil.NUMBER_OF_DIAGRAM_INTERVAL, 4);
            }

            // Only for future, if set multiplier for calculate average price.
            if (tickData is FutureDataModel && !targetData.IsMultiplierReady
                && null != _futureInstrumentCollection && null != _futureInstrumentCollection.InstrumentInfoList)
            {
                var instrumentInfo = _futureInstrumentCollection.InstrumentInfoList.FirstOrDefault(x => x.SecurityID == tickData.SecurityID);
                if (null != instrumentInfo)
                {
                    targetData.VolumeMultiplier = instrumentInfo.Multiple;
                    targetData.IsMultiplierReady = true;
                }
            }

            MarketPeriodRangeModel tradingPeriod;
            // Get the max trade time for the security.
            if (_securityInfoMetadata.MarketVarietyMaxMinTradeTime.TryGetValue(tickData.Variety, out tradingPeriod))
            {
                // Make sure the last data can be accept.
                // For the data which received after trading time.
                if (tickData.ExchangeTime >= tradingPeriod.EndTime)
                {
                    tickData.ExchangeTime = tradingPeriod.EndTime;
                }
            }

            // Format tick data time to remove the seconds and format to corresponding k-line item for add 1 minute.
            var tickDataExDateTime = tickData.ExchangeTime.AddSeconds(-1).AddMinutes(1);
            tickDataExDateTime = new DateTime(tickDataExDateTime.Year, tickDataExDateTime.Month, tickDataExDateTime.Day, tickDataExDateTime.Hour, tickDataExDateTime.Minute, 0);
            // Find the target point in the data collection.
            var targetPoint = targetData.RealTimeMarketDataPointSets.FirstOrDefault(x => x.ExchangeTime == tickDataExDateTime);

            var currentExSecID = new ExSecID(tickData.ExchangeID, tickData.SecurityID);

            RealTimeMarketDataPoint lastTickPoint;
            // Initialize last tick point data.
            if (!_lastTickPointDictionary.TryGetValue(currentExSecID, out lastTickPoint))
            {
                lastTickPoint = new RealTimeMarketDataPoint();

                var currentTradingDayPoints = targetData.RealTimeMarketDataPointSets.Where(x => x.ExchangeTime.Date == tickData.ExchangeTime.Date && x.ExchangeTime != tickData.ExchangeTime);

                lastTickPoint.SumTurnover = tickData.Turnover;
                lastTickPoint.SumVolume = tickData.Volume;
                lastTickPoint.PreSumTurnover = currentTradingDayPoints.Where(x => x.IsTrueDataPoint).Sum(x => x.Turnover);
                lastTickPoint.PreSumVolume = currentTradingDayPoints.Where(x => x.IsTrueDataPoint).Sum(x => x.Volume);
                lastTickPoint.Turnover = lastTickPoint.SumTurnover - lastTickPoint.PreSumTurnover;
                lastTickPoint.Volume = lastTickPoint.SumVolume - lastTickPoint.PreSumVolume;

                _lastTickPointDictionary.Add(currentExSecID, lastTickPoint);
            }

            // Update the point if find one in the data collection.
            if (null != targetPoint && targetPoint.IsTrueDataPoint)
            {
                targetPoint.Close = tickData.LastPrice;
                targetPoint.PriceChange = tickData.LastPrice - targetData.PreClosePrice;
                targetPoint.PriceChangeRate = targetPoint.PriceChange / targetData.PreClosePrice;
                targetPoint.Turnover = tickData.Turnover - lastTickPoint.PreSumTurnover;
                targetPoint.Volume = tickData.Volume - lastTickPoint.PreSumVolume;
                targetPoint.RealTimeAverage = tickData.Turnover / (tickData.Volume * targetData.VolumeMultiplier);

                lastTickPoint.SumTurnover = tickData.Turnover;
                lastTickPoint.SumVolume = tickData.Volume;

                if (targetPoint.Close > targetPoint.High)
                {
                    targetPoint.High = targetPoint.Close;
                }
                else if (targetPoint.Close < targetPoint.Low)
                {
                    targetPoint.Low = targetPoint.Close;
                }
            }
            // Add a new point to the data collection.
            else
            {
                lastTickPoint.PreSumTurnover = lastTickPoint.SumTurnover;
                lastTickPoint.PreSumVolume = lastTickPoint.SumVolume;
                lastTickPoint.Turnover = tickData.Turnover - lastTickPoint.PreSumTurnover;
                lastTickPoint.Volume = tickData.Volume - lastTickPoint.PreSumVolume;
                lastTickPoint.SumTurnover = tickData.Turnover;
                lastTickPoint.SumVolume = tickData.Volume;

                var exSecID = new ExSecID(tickData.ExchangeID, tickData.SecurityID);

                if (null == targetPoint)
                {
                    targetPoint = new RealTimeMarketDataPoint(); ;

                    targetData.RealTimeMarketDataPointSets.Add(targetPoint);
                }

                targetPoint.Open = tickData.LastPrice;
                targetPoint.Close = tickData.LastPrice;
                targetPoint.High = tickData.LastPrice;
                targetPoint.Low = tickData.LastPrice;
                targetPoint.PriceChange = tickData.LastPrice - targetData.PreClosePrice;
                targetPoint.PriceChangeRate = (tickData.LastPrice - targetData.PreClosePrice) / targetData.PreClosePrice;
                targetPoint.ExchangeTime = tickDataExDateTime;
                targetPoint.TimeFromOpen = RealTimePriceDateTimeConvertHelper.Instance.ConvertToTimeFromOpen(tickDataExDateTime, exSecID);
                targetPoint.Turnover = tickData.Volume - lastTickPoint.PreSumTurnover;
                targetPoint.Volume = tickData.Volume - lastTickPoint.PreSumVolume;
                targetPoint.RealTimeAverage = tickData.Turnover / (tickData.Volume * targetData.VolumeMultiplier);
                targetPoint.IsTrueDataPoint = true;
            }
        }

        // Supports Chinese Math.Round method.
        private double ChineseMathRound(double value, int digits)
        {
            if (digits < 0)
            {
                digits = 0;
            }

            // For use the Math.Floor function, need change value to the integer to calculate.
            var numberBase = Math.Pow(10, digits);
            value *= numberBase;

            // To be chinese round.
            value = Math.Floor(value + 0.5);

            return value / numberBase;
        }
    }
}

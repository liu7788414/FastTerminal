using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Helpers
{
    [Export]
    public class RealTimePriceDateTimeConvertHelper
    {
        [Import]
        private SecurityInfoMetadata _securityInfoMetadata { get; set; }

        private static RealTimePriceDateTimeConvertHelper _instance;
        public static RealTimePriceDateTimeConvertHelper Instance
        {
            get
            {
                if (null == RealTimePriceDateTimeConvertHelper._instance)
                {
                    RealTimePriceDateTimeConvertHelper._instance = ServiceLocator.Current.GetInstance<RealTimePriceDateTimeConvertHelper>();
                }
                return RealTimePriceDateTimeConvertHelper._instance;
            }
        }

        public DateTime ConvertToTimeFromOpen(DateTime actualTime, ExSecID exSecID)
        {
            IList<MarketPeriodRangeModel> periods = GetMarketPeriodRangeList(exSecID);

            return ConvertToTimeFromOpen(actualTime, periods);
        }

        public DateTime ConvertToTimeFromOpen(DateTime actualTime, IList<MarketPeriodRangeModel> periods)
        {
            if (null == actualTime || null == periods || !periods.Any())
            {
                return DateTime.MinValue;
            }

            if (actualTime < periods.Min(x => x.StartTime) || actualTime > periods.Max(x => x.EndTime))
            {
                return DateTime.MinValue;
            }

            var orderedPeriods = periods.OrderBy(x => x.StartTime);
            var maxDate = periods.Max(x => x.EndTime).Date;
            var sumOfInverval = new TimeSpan(0);

            foreach (var period in periods)
            {
                if (period.StartTime <= actualTime && period.EndTime >= actualTime)
                {
                    sumOfInverval += actualTime - period.StartTime;
                    break;
                }
                else if (period.EndTime < actualTime)
                {
                    sumOfInverval += period.Interval;
                }
            }

            return new DateTime(maxDate.Year, maxDate.Month, maxDate.Day, sumOfInverval.Hours, sumOfInverval.Minutes, sumOfInverval.Seconds);
        }

        public DateTime ConvertToActualTime(DateTime timeFromOpen, ExSecID exSecID)
        {
            IList<MarketPeriodRangeModel> periods = GetMarketPeriodRangeList(exSecID);

            return ConvertToActualTime(timeFromOpen, periods);
        }

        public DateTime ConvertToActualTime(DateTime timeFromOpen, IList<MarketPeriodRangeModel> periods)
        {
            if (null == timeFromOpen || null == periods || !periods.Any())
            {
                return DateTime.MinValue;
            }

            var sumOfInverval = new TimeSpan(0);
            DateTime actualTime = DateTime.MinValue;

            foreach (var period in periods)
            {
                if (sumOfInverval + period.Interval >= timeFromOpen.TimeOfDay)
                {
                    var periodInterval = timeFromOpen.TimeOfDay - sumOfInverval;

                    actualTime = period.StartTime + periodInterval;

                    break;
                }
                else
                {
                    sumOfInverval += period.Interval;
                }
            }

            return actualTime;
        }

        public void GenerateTimeFromOpen(RealTimeMarketData realTimeMarketData)
        {
            var periods = GetMarketPeriodRangeList(realTimeMarketData.ExSecID);

            foreach (var point in realTimeMarketData.RealTimeMarketDataPointSets)
            {
                point.TimeFromOpen = ConvertToTimeFromOpen(point.ExchangeTime, periods);
            }
        }

        private IList<MarketPeriodRangeModel> GetMarketPeriodRangeList(ExSecID exSecID)
        {
            SecurityInfo securityInfo;
            IList<MarketPeriodRangeModel> periods = null;

            if (_securityInfoMetadata.SecurityInfoMap.TryGetValue(exSecID, out securityInfo))
            {
                if (null != securityInfo)
                {
                    _securityInfoMetadata.ExchangeTradePeriodDictionary.TryGetValue(securityInfo.Variety, out periods);
                }
            }

            return periods;
        }
    }
}

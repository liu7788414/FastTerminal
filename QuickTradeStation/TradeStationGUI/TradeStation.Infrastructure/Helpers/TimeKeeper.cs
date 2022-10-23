using System;
using System.Configuration;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Helpers
{
    public class TimeKeeper
    {
        public static DateTime ApplicationLaunchTime { get; private set; }
        public static DateTime NextInitializationTime { get; private set; }

        public static TimeSpan DailyInitializationTime { get; private set; }

        static TimeKeeper()
        {
            DailyInitializationTime = new TimeSpan(9, 10, 0);
        }

        public static void AppLaunched()
        {
            TimeKeeper.ApplicationLaunchTime = DateTime.Now;

            var currentTime = DateTime.Now;

            if (TimeKeeper.ApplicationLaunchTime.TimeOfDay >= DailyInitializationTime)
            {
                NextInitializationTime = currentTime.Date.AddDays(1).AddTicks(DailyInitializationTime.Ticks);
            }
            else
            {
                NextInitializationTime = currentTime.Date.AddTicks(DailyInitializationTime.Ticks);
            }
        }

        public static void UpdateNextInitializationTime(bool isTradingDate, DateTime nextTradingDate)
        {
            var nextTradingDayInitTime = nextTradingDate.Date.AddTicks(DailyInitializationTime.Ticks);

            // 当前日为交易日
            if (isTradingDate)
            {
                var currentTime = DateTime.Now;
                var currentDayInitTime = currentTime.Date.AddTicks(DailyInitializationTime.Ticks);

                // 小于默认初始化时间时，设置下次初始化时间为当前日
                if (currentTime < currentDayInitTime)
                {
                    NextInitializationTime = currentDayInitTime;
                }
                else
                {
                    NextInitializationTime = nextTradingDayInitTime;
                }
            }
            // 当前日不为交易日，则直接设置其初始化时间为下一交易日
            else
            {
                NextInitializationTime = nextTradingDayInitTime;
            }
        }

        public static void DailyInitialized()
        {
            NextInitializationTime = NextInitializationTime.AddDays(1);
        }
    }
}

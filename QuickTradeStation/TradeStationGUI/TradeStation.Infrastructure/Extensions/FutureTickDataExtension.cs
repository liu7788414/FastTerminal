using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TFMkdtCS;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Extensions
{
    public static class FutureTickDataExtension
    {
        public static FutureDataModel ToFutureDataModel(this FuturesTickData futureData)
        {
            var dataModel = new FutureDataModel()
            {
                ExchangeID = futureData.ExID,
                SecurityID = futureData.SecID,
                Sequence = futureData.Seq,
                Type = futureData.Type,

                OpenPrice = (double)futureData.OpenPx / CommonUtil.TICK_PX_MULTIPLIER,
                ClosePrice = (double)futureData.ClosePx / CommonUtil.TICK_PX_MULTIPLIER,
                HighPrice = (double)futureData.HighPx / CommonUtil.TICK_PX_MULTIPLIER,
                LowPrice = (double)futureData.LowPx / CommonUtil.TICK_PX_MULTIPLIER,
                LastPrice = (double)futureData.LastPx / CommonUtil.TICK_PX_MULTIPLIER,
                // Currently, future's pre-close price is just pre-settlement price.
                PreClosePrice = (double)futureData.PreSettlementPx / CommonUtil.TICK_PX_MULTIPLIER,
                UpLimitPrice = (double)futureData.UpLimitPx / CommonUtil.TICK_PX_MULTIPLIER,
                DownLimitPrice = (double)futureData.DownLimitPx / CommonUtil.TICK_PX_MULTIPLIER,

                OpenInterest = futureData.OpenInterest,
                PreOpenInterest = futureData.PreOpenInterest,

                CurrentDelta = futureData.CurrDelta,
                PreDelta = futureData.PreDelta,

                SettlementGroupID = futureData.SettlementGroupID,
                SettlementID = futureData.SettlementID,
                SettlementPrice = (double)futureData.SettlementPx / CommonUtil.TICK_PX_MULTIPLIER,
                PreSettlementPrice = (double)futureData.PreSettlementPx / CommonUtil.TICK_PX_MULTIPLIER,

                Turnover = futureData.Turnover,
                Volume = futureData.Volume,

                ExchangeTime = DateTimeHelper.ConvertToDateTime(futureData.TradeDate, futureData.ExTime),
                LocalTime = DateTimeHelper.ConvertToDateTime(futureData.TradeDate, futureData.LocalTime),

                BidPrice = new double[10],
                BidVolume = new long[10],
                AskPrice = new double[10],
                AskVolume = new long[10],
            };

            // Currently, only has 5 kinds of price.
            for (int ix = 0; ix < 5; ix++)
            {
                dataModel.BidPrice[ix] = (double)futureData.BidPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.BidVolume[ix] = futureData.BidVol[ix];
                dataModel.AskPrice[ix] = (double)futureData.OfferPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.AskVolume[ix] = futureData.OfferVol[ix];
            }

            return dataModel;
        }
    }
}

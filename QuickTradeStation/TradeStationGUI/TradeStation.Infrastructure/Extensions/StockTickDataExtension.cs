using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TFMkdtCS;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Extensions
{
    public static class StockTickDataExtension
    {
        public static StockTickDataModel ToStockTickDataModel(this StockTickData tickData)
        {
            var dataModel = new StockTickDataModel()
            {
                Type = tickData.Type,
                Sequence = tickData.Seq,

                ExchangeID = tickData.ExID,
                SecurityID = tickData.SecID,

                ExchangeTime = DateTimeHelper.ConvertToDateTime(tickData.TradeDate, tickData.ExTime),
                LocalDateTime = DateTimeHelper.ConvertToDateTime(tickData.TradeDate, tickData.LocalTime),

                PreClosePrice = (double)tickData.PreClosePx / CommonUtil.TICK_PX_MULTIPLIER,
                OpenPrice = (double)tickData.OpenPx / CommonUtil.TICK_PX_MULTIPLIER,
                HighPrice = (double)tickData.HighPx / CommonUtil.TICK_PX_MULTIPLIER,
                LowPrice = (double)tickData.LowPx / CommonUtil.TICK_PX_MULTIPLIER,
                ClosePrice = (double)tickData.LastPx / CommonUtil.TICK_PX_MULTIPLIER,
                LastPrice = (double)tickData.LastPx / CommonUtil.TICK_PX_MULTIPLIER,
                UpLimitPx = (double)tickData.UpLimitPx / CommonUtil.TICK_PX_MULTIPLIER,
                DownLimitPx = (double)tickData.DownLimitPx / CommonUtil.TICK_PX_MULTIPLIER,

                Turnover = tickData.Turnover,
                Volume = tickData.Volume,

                BidPrice = new double[10],
                BidVolume = new long[10],
                AskPrice = new double[10],
                AskVolume = new long[10],
            };

            for (int ix = 0; ix < 10; ix++)
            {
                dataModel.BidPrice[ix] = (double)tickData.BidPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.BidVolume[ix] = tickData.BidVol[ix];
                dataModel.AskPrice[ix] = (double)tickData.OfferPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.AskVolume[ix] = tickData.OfferVol[ix];
            }

            return dataModel;
        }
    }
}

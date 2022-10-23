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
    public static class OptionDataExtension
    {
        public static OptionDataModel ToOptionDataModel(this OptionData optionData)
        {
            var dataModel = new OptionDataModel()
            {
                ExchangeID = optionData.ExID,
                SecurityID = optionData.SecID,
                Sequence = optionData.Seq,
                Type = optionData.Type,

                TotalLongPosition = optionData.TotalLongPosition,

                TradingPhaseCode = optionData.TradingPhaseCode,

                OpenPrice = (double)optionData.OpenPx / CommonUtil.TICK_PX_MULTIPLIER,
                ClosePrice = (double)optionData.ClosePx / CommonUtil.TICK_PX_MULTIPLIER,
                HighPrice = (double)optionData.HighPx / CommonUtil.TICK_PX_MULTIPLIER,
                LowPrice = (double)optionData.LowPx / CommonUtil.TICK_PX_MULTIPLIER,
                LastPrice = (double)optionData.LastPx / CommonUtil.TICK_PX_MULTIPLIER,
                PreClosePrice = (double)optionData.PreSettlePx / CommonUtil.TICK_PX_MULTIPLIER,

                AuctionPrice = (double)optionData.AuctionPrice / CommonUtil.TICK_PX_MULTIPLIER,
                AuctoinQty = optionData.AuctionQty,

                Turnover = optionData.Turnover,
                Volume = optionData.Volume,

                ExchangeTime = DateTimeHelper.ConvertToDateTime(optionData.TradeDate, optionData.ExTime),
                LocalTime = DateTimeHelper.ConvertToDateTime(optionData.TradeDate, optionData.LocalTime),
                OrigTime = DateTimeHelper.ConvertToDateTime(optionData.TradeDate, optionData.OrigTime),

                BidPrice = new double[10],
                BidPriceImpliedVolatility = new double[10],
                BidVolume = new long[10],
                AskPrice = new double[10],
                AskPriceImpliedVolatility = new double[10],
                AskVolume = new long[10],
            };

            // Currently, only has 5 kinds of price.
            for (int ix = 0; ix < 5; ix++)
            {
                dataModel.BidPrice[ix] = (double)optionData.BidPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.BidVolume[ix] = optionData.BidVol[ix];
                dataModel.AskPrice[ix] = (double)optionData.OfferPx[ix] / CommonUtil.TICK_PX_MULTIPLIER;
                dataModel.AskVolume[ix] = optionData.OfferVol[ix];
            }

            return dataModel;
        }
    }
}

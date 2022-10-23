using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Payloads.RefDataPayloads;

namespace TradeStation.Infrastructure.Extensions
{
    public static class KLineRecordsQueryResponseExtension
    {
        public static KLineRecordModel ToStockTickDataModel(this KLineRecord kLineRecord)
        {
            if (null == kLineRecord)
            {
                return null;
            }

            return new KLineRecordModel()
            {
                ExID = kLineRecord.ExchangeId,
                SecID = kLineRecord.SecurityId,
                Seq = kLineRecord.Sequence,
                ExDateTime = DateTimeHelper.ConvertToDateTime(kLineRecord.TradeDateTime * 100000),
                OpenPx = (double)kLineRecord.OpenPrice / CommonUtil.TICK_PX_MULTIPLIER,
                LastPx = (double)kLineRecord.ClosePrice / CommonUtil.TICK_PX_MULTIPLIER,
                HighPx = (double)kLineRecord.HighestPrice / CommonUtil.TICK_PX_MULTIPLIER,
                LowPx = (double)kLineRecord.LowestPrice / CommonUtil.TICK_PX_MULTIPLIER,
                Volume = kLineRecord.Volume,
                Turnover = kLineRecord.TurnOver,
            };
        }
    }
}

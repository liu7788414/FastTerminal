using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models.Local
{
    public class OptionDataModel : TickDataModel
    {
        public long TotalLongPosition { get; set; }

        public string TradingPhaseCode { get; set; }

        public double AuctionPrice { get; set; }
        public long AuctoinQty { get; set; }

        public double[] BidPrice { get; set; }
        public long[] BidVolume { get; set; }

        public double[] AskPrice { get; set; }
        public long[] AskVolume { get; set; }

        public double[] BidPriceImpliedVolatility { get; set; }
        public double[] AskPriceImpliedVolatility { get; set; }

        public DateTime LocalTime { get; set; }
        public DateTime OrigTime { get; set; }

        public override string Variety
        {
            get
            {
                return (ExchangeID + "_all").ToLower() + "_300";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models.Local
{
    public class StockTickDataModel : TickDataModel
    {
        public DateTime LocalDateTime { get; set; }

        public double[] BidPrice { get; set; }
        public long[] BidVolume { get; set; }

        public double[] AskPrice { get; set; }
        public long[] AskVolume { get; set; }

        public override string Variety
        {
            get
            {
                return (ExchangeID + "_all").ToLower() + "_100";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models.Local
{
    public class FutureDataModel : TickDataModel
    {
        public double UpLimitPrice { get; set; }
        public double DownLimitPrice { get; set; }

        public long OpenInterest { get; set; }
        public long PreOpenInterest { get; set; }

        public double CurrentDelta { get; set; }
        public double PreDelta { get; set; }

        public string SettlementGroupID { get; set; }
        public int SettlementID { get; set; }
        public double SettlementPrice { get; set; }
        public double PreSettlementPrice { get; set; }

        public double[] BidPrice { get; set; }
        public long[] BidVolume { get; set; }

        public double[] AskPrice { get; set; }
        public long[] AskVolume { get; set; }

        public DateTime LocalTime { get; set; }

        public override string Variety
        {
            get
            {
                return (ExchangeID + "_" + Regex.Replace(SecurityID, @"\d", "")).ToLower() + "_500";
            }
        }
    }
}

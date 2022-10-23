using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models.Local
{
    public class TickDataModel
    {
        public string ExchangeID { get; set; }
        public string SecurityID { get; set; }
        public uint Sequence { get; set; }
        public int Type { get; set; }

        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double LastPrice { get; set; }
        public double PreClosePrice { get; set; }

        public double Turnover { get; set; }
        public long Volume { get; set; }
        public DateTime ExchangeTime { get; set; }

        public double UpLimitPx { get; set; }
        public double DownLimitPx { get; set; }
        public virtual string Variety
        {
            get
            {
                return ExchangeID.ToLower() + "_all";
            }
        }
    }
}

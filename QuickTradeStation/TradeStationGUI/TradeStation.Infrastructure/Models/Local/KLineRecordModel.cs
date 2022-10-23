using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models.Local
{
    public class KLineRecordModel
    {
        public string ExID { get; set; }
        public string SecID { get; set; }
        public uint Seq { get; set; }

        public DateTime ExDateTime { get; set; }

        public double OpenPx { get; set; }
        public double LastPx { get; set; }
        public double HighPx { get; set; }
        public double LowPx { get; set; }

        public double Turnover { get; set; }
        public long Volume { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class KLineRecordsQueryResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public IList<KLineRecord> KLineRecords { get; set; }
    }

    [DataContract]
    public class KLineRecord
    {
        [DataMember(Name = "tdate")]
        public int TradeDate { get; set; }

        // Currently, K Line score is treated as trade date time.
        [DataMember(Name = "sco")]
        public long TradeDateTime { get; set; }

        [DataMember(Name = "seq")]
        public uint Sequence { get; set; }

        [DataMember(Name = "pid")]
        public eKLinePeriodType PeriodType { get; set; }

        [DataMember(Name = "exch")]
        public string ExchangeId { get; set; }

        [DataMember(Name = "sec")]
        public string SecurityId { get; set; }

        [DataMember(Name = "op")]
        public long OpenPrice { get; set; }

        [DataMember(Name = "cp")]
        public long ClosePrice { get; set; }

        [DataMember(Name = "hp")]
        public long HighestPrice { get; set; }

        [DataMember(Name = "lp")]
        public long LowestPrice { get; set; }

        [DataMember(Name = "vol")]
        public long Volume { get; set; }

        [DataMember(Name = "to")]
        public double TurnOver { get; set; }
    }
}

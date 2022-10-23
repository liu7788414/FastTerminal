using System;
using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class LastTradeDateResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public long LastTradeDate { get; set; }
    }
}

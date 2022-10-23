using System;
using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class NextTradeDateResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public long NextTradeDate { get; set; }
    }
}

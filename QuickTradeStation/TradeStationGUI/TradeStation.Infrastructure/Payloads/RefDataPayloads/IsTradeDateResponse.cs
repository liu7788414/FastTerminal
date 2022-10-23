using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class IsTradeDateResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public bool IsTradeDate { get; set; }
    }
}

using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class BenchMarkRateResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public double RateValue { get; set; }
    }
}

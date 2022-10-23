using System.Collections.Generic;
using System.Runtime.Serialization;

using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class ExrightRatioQueryResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public IList<ExrightRatio> ExrightRatioList { get; set; }
    }
}

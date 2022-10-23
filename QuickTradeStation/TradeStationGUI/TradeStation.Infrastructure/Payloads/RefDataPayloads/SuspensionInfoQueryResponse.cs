using System.Collections.Generic;
using System.Runtime.Serialization;

using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class SuspensionInfoQueryResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public IList<SuspensionInfo> SuspensionInfoList { get; set; }
    }
}

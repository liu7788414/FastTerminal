using System.Collections.Generic;
using System.Runtime.Serialization;

using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads 
{
    [DataContract]
    public class OptionInformationTableQueryResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public List<OptionInfo> OptionList;
    }
}

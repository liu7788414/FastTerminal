using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads 
{
    [DataContract]
    public class SecurityCodeTableQueryResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public List<SecurityInfo> SecurityList;
    }
}

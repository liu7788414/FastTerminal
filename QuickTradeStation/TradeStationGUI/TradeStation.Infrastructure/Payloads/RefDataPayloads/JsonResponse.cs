using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class JsonResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errCode")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "errMsg")]
        public string Message { get; set; }
    }
}

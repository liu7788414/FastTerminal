using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Payloads
{
    public enum ResponseType
    {

    }
    
    [DataContract]
    public class ServerResponse
    {
        [DataMember(Name = "T")]
        public ResponseType Type
        {
            get;
            set;
        }

        [DataMember(Name = "re", IsRequired = false)]
        public int ReSequence
        {
            get;
            set;
        }
    }

    [DataContract]
    public class ServerErrorNotify : ServerResponse
    {
        [DataMember(Name = "errorCode")]
        public int ErrorCode
        {
            get;
            set;
        }

        [DataMember(Name = "errorMsg")]
        public string ErrorMessage
        {
            get;
            set;
        }
    }
}

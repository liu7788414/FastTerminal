using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class ExrightRatio
    {
        [DataMember(Name = "exId")]
        public string ExID { get; set; }

        [DataMember(Name = "securityId")]
        public string SecurityID { get; set; }

        [DataMember(Name = "tradeDate")]
        public long TradeDate { get; set; }

        [DataMember(Name = "exrightType")]
        public string ExrightType { get; set; }

        [DataMember(Name = "backFactor")]
        public double BackFactor { get; set; }

        [DataMember(Name = "forwardFactor")]
        public double ForwardFactor { get; set; }
    }
}

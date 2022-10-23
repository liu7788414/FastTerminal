using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class SuspensionInfo
    {
        [DataMember(Name = "sInfoWindCode")]
        public string WindCode { get; set; }

        [DataMember(Name = "suspendFlag")]
        public int SuspendType { get; set; }

        [DataMember(Name = "suspendDesc")]
        public string SuspendDescription { get; set; }

        [DataMember(Name = "sDqSuspendDate")]
        public int? SuspendDate { get; set; }

        [DataMember(Name = "suspendTime")]
        public string SuspendTime { get; set; }

        [DataMember(Name = "sDqResumeDate")]
        public int? ResumeDate { get; set; }

        [DataMember(Name = "resumeTime")]
        public string ResumeTime { get; set; }
    }
}

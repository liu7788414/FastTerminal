using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Models.Local
{
    public class SuspensionInfoModel
    {
        public string ExID { get; set; }

        public string SecurityID { get; set; }

        public eSuspendType SuspendType { get; set; }

        public string SuspendDescription { get; set; }
        
        public DateTime SuspendDate { get; set; }
        
        public TimeSpan SuspendTime { get; set; }
        
        public DateTime ResumeDate { get; set; }
        
        public TimeSpan ResumeTime { get; set; }
    }
}

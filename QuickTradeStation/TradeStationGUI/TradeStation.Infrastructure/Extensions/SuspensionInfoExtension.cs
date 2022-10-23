using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Extensions
{
    public static class SuspensionInfoExtension
    {
        public static SuspensionInfoModel ToSuspensionInfoModel(this SuspensionInfo suspensionInfo)
        {
            if (null == suspensionInfo)
            {
                return null;
            }

            var exID = string.Empty;
            var securityID = string.Empty;
            DateTime suspendDate = DateTime.MinValue;
            DateTime resumeDate = DateTime.MinValue;
            TimeSpan suspendTime;
            TimeSpan resumeTime;

            // Formats exID & securityID.
            var securityInfoArray = suspensionInfo.WindCode.Split('.');
            if (securityInfoArray.Count() == 2)
            {
                if (securityInfoArray[1] == CommonUtil.上交所
                    || securityInfoArray[1] == CommonUtil.深交所)
                {
                    exID = securityInfoArray[1];
                    securityID = securityInfoArray[0];
                }
            }

            // Formats suspend date.
            if (null != suspensionInfo.SuspendDate)
            {
                suspendDate = DateTimeHelper.ConvertToDateTime(suspensionInfo.SuspendDate.Value, 0);
            }

            // Format resume date.
            if (null != suspensionInfo.ResumeDate)
            {
                resumeDate = DateTimeHelper.ConvertToDateTime(suspensionInfo.ResumeDate.Value, 0);
            }

            // Formats suspend time.
            if (!TimeSpan.TryParse(suspensionInfo.SuspendTime, out suspendTime))
            {
                suspendTime = new TimeSpan();
            }

            // Format resume time.
            if (TimeSpan.TryParse(suspensionInfo.ResumeTime, out resumeTime))
            {
                resumeTime = new TimeSpan();
            }

            return new SuspensionInfoModel()
            {
                ExID = exID,
                SecurityID = securityID,
                SuspendType = (eSuspendType)suspensionInfo.SuspendType,
                SuspendDescription = suspensionInfo.SuspendDescription,
                SuspendDate = suspendDate,
                SuspendTime = suspendTime,
                ResumeDate = resumeDate,
                ResumeTime = resumeTime,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Extensions
{
    public static class ExrightRatioExtension
    {
        public static ExrightRatioModel ToExrightRatioModel(this ExrightRatio exrightRatio)
        {
            if (null == exrightRatio)
            {
                return null;
            }

            return new ExrightRatioModel()
            {
                ExID = exrightRatio.ExID,
                SecurityID = exrightRatio.SecurityID,
                ExrightType = exrightRatio.ExrightType,
                TradeDate = DateTimeHelper.ConvertToDate(exrightRatio.TradeDate),
                ForwardFactor = exrightRatio.ForwardFactor,
                BackFactor = exrightRatio.BackFactor,
            };
        }
    }
}

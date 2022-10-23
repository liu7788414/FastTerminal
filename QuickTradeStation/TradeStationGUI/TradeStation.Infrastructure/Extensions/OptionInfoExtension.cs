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
    public static class OptionInfoExtension
    {
        public static OptionInfoDataModel ToOptionInfoModel(this OptionInfo optionInfo)
        {
            if (null == optionInfo)
            {
                return null;
            }

            return new OptionInfoDataModel()
            {
                SecurityID = optionInfo.SecurityID,
                ContractID = optionInfo.ContractID,
                ExID = optionInfo.ExID,
                SecuritySymbol = optionInfo.SecuritySymbol,
                UnderlyingSecurityId = optionInfo.UnderlyingSecurityId,
                UnderlyingSymbol = optionInfo.UnderlyingSymbol,
                UnderlyingType = optionInfo.UnderlyingType == "EBS" ? eUnderlyingType.ETF : eUnderlyingType.A股,
                OptionType = optionInfo.OptionType == "E" ? eOptionStyleType.欧式 : eOptionStyleType.美式,
                CallOrPut = optionInfo.CallOrPut == "C" ? eOptionType.认购期权 : eOptionType.认沽期权,
                ContractMultiplierUnit = optionInfo.ContractMultiplierUnit,
                ExercisePrice = optionInfo.ExercisePrice,
                StartDate = DateTimeHelper.ConvertToDate(optionInfo.StartDate),
                EndDate = DateTimeHelper.ConvertToDate(optionInfo.EndDate),
                ExerciseDate = DateTimeHelper.ConvertToDate(optionInfo.ExerciseDate),
                DeliveryDate = DateTimeHelper.ConvertToDate(optionInfo.DeliveryDate),
                ExpireDate = DateTimeHelper.ConvertToDate(optionInfo.ExpireDate),
                UpdateVersion = optionInfo.UpdateVersion,
                TotalLongPosition = optionInfo.TotalLongPosition,
                SecurityClosePx = optionInfo.SecurityClosePx,
                SettlePrice = optionInfo.SettlePrice,
                UnderlyingClosePx = optionInfo.UnderlyingClosePx,
                PriceLimitType = optionInfo.PriceLimitType,
                DailyPriceUpLimit = optionInfo.DailyPriceUpLimit,
                DailyPriceDownLimit = optionInfo.DailyPriceDownLimit,
                MarginUnit = optionInfo.MarginUnit,
                MarginRatioParam1 = optionInfo.MarginRatioParam1,
                MarginRatioParam2 = optionInfo.MarginRatioParam2,
                RoundLot = optionInfo.RoundLot,
                LimitOrderMinFloor = optionInfo.LimitOrderMinFloor,
                LimitOrderMaxFloor = optionInfo.LimitOrderMaxFloor,
                MarketOrderMinFloor = optionInfo.MarketOrderMinFloor,
                MarketOrderMaxFloor = optionInfo.MarketOrderMaxFloor,
                TickSize = optionInfo.TickSize,
                SecurityStatusFlag = optionInfo.SecurityStatusFlag,
            };
        }
    }
}

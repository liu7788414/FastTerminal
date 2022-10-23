using System.Runtime.Serialization;

namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class OptionInfo
    {
        [DataMember(Name = "securityId")]
        public string SecurityID { get; set; }

        [DataMember(Name = "exId")]
        public string ExID { get; set; }

        [DataMember(Name = "contractId")]
        public string ContractID { get; set; }

        [DataMember(Name = "securitySymbol")]
        public string SecuritySymbol { get; set; }

        [DataMember(Name = "underlyingSecurityId")]
        public string UnderlyingSecurityId { get; set; }

        [DataMember(Name = "underlyingSymbol")]
        public string UnderlyingSymbol { get; set; }

        [DataMember(Name = "underlyingType")]
        public string UnderlyingType { get; set; }

        [DataMember(Name = "optionType")]
        public string OptionType { get; set; }

        [DataMember(Name = "callOrPut")]
        public string CallOrPut { get; set; }

        [DataMember(Name = "contractMultiplierUnit")]
        public long ContractMultiplierUnit { get; set; }

        [DataMember(Name = "exercisePrice")]
        public double ExercisePrice { get; set; }

        [DataMember(Name = "startDate")]
        public int StartDate { get; set; }

        [DataMember(Name = "endDate")]
        public int EndDate { get; set; }

        [DataMember(Name = "exerciseDate")]
        public int ExerciseDate { get; set; }

        [DataMember(Name = "deliveryDate")]
        public int DeliveryDate { get; set; }

        [DataMember(Name = "expireDate")]
        public int ExpireDate { get; set; }

        [DataMember(Name = "updateVersion")]
        public string UpdateVersion { get; set; }

        [DataMember(Name = "totalLongPosition")]
        public long TotalLongPosition { get; set; }

        [DataMember(Name = "securityClosePx")]
        public double SecurityClosePx { get; set; }

        [DataMember(Name = "settlePrice")]
        public double SettlePrice { get; set; }

        [DataMember(Name = "underlyingClosePx")]
        public double UnderlyingClosePx { get; set; }

        [DataMember(Name = "priceLimitType")]
        public string PriceLimitType { get; set; }

        [DataMember(Name = "dailyPriceUpLimit")]
        public double DailyPriceUpLimit { get; set; }

        [DataMember(Name = "dailyPriceDownLimit")]
        public double DailyPriceDownLimit { get; set; }

        [DataMember(Name = "marginUnit")]
        public double MarginUnit { get; set; }

        [DataMember(Name = "marginRatioParam1")]
        public double MarginRatioParam1 { get; set; }

        [DataMember(Name = "marginRatioParam2")]
        public double MarginRatioParam2 { get; set; }

        [DataMember(Name = "roundLot")]
        public long RoundLot { get; set; }

        [DataMember(Name = "limitOrderMinFloor")]
        public long LimitOrderMinFloor { get; set; }

        [DataMember(Name = "limitOrderMaxFloor")]
        public long LimitOrderMaxFloor { get; set; }

        [DataMember(Name = "marketOrderMinFloor")]
        public long MarketOrderMinFloor { get; set; }

        [DataMember(Name = "marketOrderMaxFloor")]
        public long MarketOrderMaxFloor { get; set; }

        [DataMember(Name = "tickSize")]
        public double TickSize { get; set; }

        [DataMember(Name = "securityStatusFlag")]
        public string SecurityStatusFlag { get; set; }
    }
}

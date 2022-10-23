using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System.ComponentModel;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Services;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class InstrumentInfo : InfoWithSecurityInfo
    {
        private ePositionInfoMsgType msgType;
        [DataMember(Name = "msgType")]
        public ePositionInfoMsgType MsgType
        {
            get { return msgType; }
            set
            {
                SetProperty(ref msgType, value);
            }
        }

        private string futureKindName;
        [DataMember(Name = "futureKindName")]
        public string FutureKindName
        {
            get { return futureKindName; }
            set
            {
                SetProperty(ref futureKindName, value);
            }
        }

        private eMarketType targetMarketNo;
        [DataMember(Name = "targetMarketNo")]
        public eMarketType TargetMarketNo
        {
            get { return targetMarketNo; }
            set
            {
                SetProperty(ref targetMarketNo, value);
            }
        }

        private string targetStockCode;
        [DataMember(Name = "targetStockCode")]
        public string TargetStockCode
        {
            get { return targetStockCode; }
            set
            {
                SetProperty(ref targetStockCode, value);
            }
        }

        private string settlementMonth;
        [DataMember(Name = "settlementMonth")]
        public string SettlementMonth
        {
            get { return settlementMonth; }
            set
            {
                SetProperty(ref settlementMonth, value);
            }
        }

        private int multiple;
        [DataMember(Name = "multiple")]
        public int Multiple
        {
            get { return multiple; }
            set
            {
                SetProperty(ref multiple, value);
            }
        }

        private string lastTradeDate;
        [DataMember(Name = "lastTradeDate")]
        public string LastTradeDate
        {
            get { return lastTradeDate; }
            set
            {
                SetProperty(ref lastTradeDate, value);
            }
        }


        private string lastTradeTime;
        [DataMember(Name = "lastTradeTime")]
        public string LastTradeTime
        {
            get { return lastTradeTime; }
            set
            {
                SetProperty(ref lastTradeTime, value);
            }
        }

        private string settlementDate;
        [DataMember(Name = "settlementDate")]
        public string SettlementDate
        {
            get { return settlementDate; }
            set
            {
                SetProperty(ref settlementDate, value);
            }
        }

        private double settlementPrice;
        [DataMember(Name = "settlementPrice")]
        public double SettlementPrice
        {
            get { return settlementPrice; }
            set
            {
                SetProperty(ref settlementPrice, value);
            }
        }

        private double preSettlementPrice;
        [DataMember(Name = "preSettlementPrice")]
        public double PreSettlementPrice
        {
            get { return preSettlementPrice; }
            set
            {
                SetProperty(ref preSettlementPrice, value);
            }
        }

        private double marketPosition;
        [DataMember(Name = "marketPosition")]
        public double MarketPosition
        {
            get { return marketPosition; }
            set
            {
                SetProperty(ref marketPosition, value);
            }
        }

        private double preMarketPosition;
        [DataMember(Name = "preMarketPosition")]
        public double PreMarketPosition
        {
            get { return preMarketPosition; }
            set
            {
                SetProperty(ref preMarketPosition, value);
            }
        }

        private string marketPricePermit;
        [DataMember(Name = "marketPricePermit")]
        public string MarketPricePermit
        {
            get { return marketPricePermit; }
            set
            {
                SetProperty(ref marketPricePermit, value);
            }
        }

        private double uplimitedPrice;
        [DataMember(Name = "uplimitedPrice")]
        public double UplimitedPrice
        {
            get { return uplimitedPrice; }
            set
            {
                SetProperty(ref uplimitedPrice, value);
            }
        }

        private double downlimitedPrice;
        [DataMember(Name = "downlimitedPrice")]
        public double DownlimitedPrice
        {
            get { return downlimitedPrice; }
            set
            {
                SetProperty(ref downlimitedPrice, value);
            }
        }
    }

    public class InstrumentInfoNotifyEvent : PubSubEvent<InstrumentInfo>
    {

    }
}
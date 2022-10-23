using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class TradeResultInfo : InfoWithCombiNoAndSecurityInfo, IExport
    {
        private eTradeResultInfoMsgType _msgType;
        [DataMember(Name = "msgType")]
        public eTradeResultInfoMsgType MsgType
        {
            get { return _msgType; }
            set
            {
                SetProperty(ref _msgType, value);
            }
        }

        private string _dealDate;
        [DataMember(Name = "dealDate")]
        public string DealDate
        {
            get { return _dealDate; }
            set
            {
                SetProperty(ref _dealDate, value);
            }
        }

        private double _dealBalance;
        [DataMember(Name = "dealBalance")]
        public double DealBalance
        {
            get { return _dealBalance; }
            set
            {
                SetProperty(ref _dealBalance, value);
            }
        }

        private double _totalFee;
        [DataMember(Name = "totalFee")]
        public double TotalFee
        {
            get { return _totalFee; }
            set
            {
                SetProperty(ref _totalFee, value);
            }
        }

        private string _systemNumber;
        [DataMember(Name = "systemNumber")]
        public string SystemNumber
        {
            get { return _systemNumber; }
            set
            {
                SetProperty(ref _systemNumber, value);
            }
        }

        private int _entrustNo;
        [DataMember(Name = "entrustNo")]
        public int EntrustNo
        {
            get { return _entrustNo; }
            set
            {
                SetProperty(ref _entrustNo, value);
            }
        }

        private string _dealTime;
        [DataMember(Name = "dealTime")]
        public string DealTime
        {
            get { return _dealTime; }
            set
            {
                SetProperty(ref _dealTime, value);
            }
        }

        private eEntrustDirection _entrustDirection;
        [DataMember(Name = "entrustDirection")]
        public eEntrustDirection EntrustDirection
        {
            get { return _entrustDirection; }
            set
            {
                SetProperty(ref _entrustDirection, value);
            }
        }

        private double _dealPrice;
        [DataMember(Name = "dealPrice")]
        public double DealPrice
        {
            get { return _dealPrice; }
            set
            {
                SetProperty(ref _dealPrice, value);
            }
        }

        private int _dealAmount;
        [DataMember(Name = "dealAmount")]
        public int DealAmount
        {
            get { return _dealAmount; }
            set
            {
                SetProperty(ref _dealAmount, value);
            }
        }

        private string _dealNo;
        [DataMember(Name = "dealNo")]
        public string DealNo
        {
            get { return _dealNo; }
            set
            {
                SetProperty(ref _dealNo, value);
            }
        }

        private eFuturesDirection _futuresDirection;
        [DataMember(Name = "futuresDirection")]
        public eFuturesDirection FuturesDirection
        {
            get { return _futuresDirection; }
            set
            {
                SetProperty(ref _futuresDirection, value);
            }
        }

        public string GetTitle()
        {
            return "交易市场,证券代码,证券名称,组合编号,股东代码,成交日期,委托编号,成交时间,委托方向,成交价格,成交数量,成交编号,开平方向";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", MarketType, SecurityID, String.IsNullOrEmpty(SecurityName) ? " " : SecurityName, CombiNo, StockholderId, DealDate, EntrustNo, DealTime, EntrustDirection, DealPrice, DealAmount, DealNo, FuturesDirection);
        }

        public override string ToString()
        {
            return Export();
        }
    }

    [DataContract]
    public class HistoricalTradeResultInfo : TradeResultInfo
    {

    }

    [DataContract]
    public class FutureTradeResultInfo : TradeResultInfo
    {

    }


    [DataContract]
    public class OptionTradeResultInfo : TradeResultInfo
    {

    }

    [DataContract]
    public class FundTradeResultInfo : TradeResultInfo
    {

    }

    public class TradeResultNotifyEvent : PubSubEvent<TradeResultInfo>
    {

    }

    public class FutureTradeResultNotifyEvent : PubSubEvent<FutureTradeResultInfo>
    {

    }


    public class OptionTradeResultNotifyEvent : PubSubEvent<OptionTradeResultInfo>
    {

    }

    public class FundTradeResultNotifyEvent : PubSubEvent<FundTradeResultInfo>
    {

    }
}
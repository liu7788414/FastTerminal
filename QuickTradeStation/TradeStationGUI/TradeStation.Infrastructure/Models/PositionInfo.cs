using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models.Local;


namespace TradeStation.Infrastructure.Models
{

    [DataContract]
    public class PositionInfoBase : InfoWithCombiNoAndSecurityInfo,IExport
    {
        public void Copy(PositionInfoBase source)
        {
            _msgType = source.MsgType;
            StockholderId = source.StockholderId;
            _holdSeat = source.HoldSeat;
            _investType = source.InvestType;
            _currentAmount = source.CurrentAmount;
            _enableAmount = source.EnableAmount;
            _dealAmount = source.DealAmount;
            _positionFlag = source.PositionFlag;
            _todayAmount = source.TodayAmount;
            _lastdayAmount = source.LastdayAmount;
            _todayEnableAmount = source.TodayEnableAmount;
            _lastdayEnableAmount = source.LastdayEnableAmount;
            _optionType = source.OptionType;
            CombiNo = source.CombiNo;
            _futuresDirection = source.FuturesDirection;
            _entrustDirection = source.EntrustDirection;
            DynamicProfit = source.DynamicProfit;
            CloseProfit = source.CloseProfit;
        }

        private double _dynamicProfit;
        [DataMember(Name = "dynamicProfit")]
        public double DynamicProfit
        {
            get { return _dynamicProfit; }
            set
            {
                SetProperty(ref _dynamicProfit, value);
                TotalProfit = DynamicProfit + CloseProfit;
            }
        }

        private double _closeProfit;
        [DataMember(Name = "closeProfit")]
        public double CloseProfit
        {
            get { return _closeProfit; }
            set
            {
                SetProperty(ref _closeProfit, value);
                TotalProfit = DynamicProfit + CloseProfit;
            }
        }

        private double _totalProfit;

        [DataMember(Name = "totalProfit")]
        public double TotalProfit
        {
            get { return _totalProfit; }
            set { SetProperty(ref _totalProfit, value); }
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

        private ePositionInfoMsgType _msgType;
        [DataMember(Name = "msgType")]
        public ePositionInfoMsgType MsgType
        {
            get { return _msgType; }
            set
            {
                SetProperty(ref _msgType, value);
            }
        }

        private string _holdSeat;
        [DataMember(Name = "holdSeat")]
        public string HoldSeat
        {
            get { return _holdSeat; }
            set
            {
                SetProperty(ref _holdSeat, value);
            }
        }

        private eInvestType _investType;
        [DataMember(Name = "investType")]
        public eInvestType InvestType
        {
            get { return _investType; }
            set
            {
                SetProperty(ref _investType, value);
            }
        }

        private int _currentAmount;
        [DataMember(Name = "currentAmount")]
        public int CurrentAmount
        {
            get { return _currentAmount; }
            set
            {
                SetProperty(ref _currentAmount, value);
            }
        }

        private int _enableAmount;
        [DataMember(Name = "enableAmount")]
        public int EnableAmount
        {
            get { return _enableAmount; }
            set
            {
                SetProperty(ref _enableAmount, value);
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

        private ePositionFlag _positionFlag;
        [DataMember(Name = "positionFlag")]
        public ePositionFlag PositionFlag
        {
            get { return _positionFlag; }
            set
            {
                SetProperty(ref _positionFlag, value);
            }
        }



        private int _todayAmount;
        [DataMember(Name = "todayAmount")]
        public int TodayAmount
        {
            get { return _todayAmount; }
            set
            {
                SetProperty(ref _todayAmount, value);
            }
        }

        private int _lastdayAmount;
        [DataMember(Name = "lastdayAmount")]
        public int LastdayAmount
        {
            get { return _lastdayAmount; }
            set
            {
                SetProperty(ref _lastdayAmount, value);
            }
        }

        private int _todayEnableAmount;
        [DataMember(Name = "todayEnableAmount")]
        public int TodayEnableAmount
        {
            get { return _todayEnableAmount; }
            set
            {
                SetProperty(ref _todayEnableAmount, value);
            }
        }

        private int _lastdayEnableAmount;
        [DataMember(Name = "lastdayEnableAmount")]
        public int LastdayEnableAmount
        {
            get { return _lastdayEnableAmount; }
            set
            {
                SetProperty(ref _lastdayEnableAmount, value);
            }
        }

        private eOptionType _optionType;
        [DataMember(Name = "optionType")]
        public eOptionType OptionType
        {
            get { return _optionType; }
            set
            {
                SetProperty(ref _optionType, value);
            }
        }

        private double _beginCost;
        [DataMember(Name = "beginCost")]
        public double BeginCost
        {
            get { return _beginCost; }
            set
            {
                SetProperty(ref _beginCost, value);
            }
        }

        private double _currentCost;
        [DataMember(Name = "currentCost")]
        public double CurrentCost
        {
            get { return _currentCost; }
            set
            {
                SetProperty(ref _currentCost, value);
            }
        }

        private double _currentCostPrice;
        [DataMember(Name = "currentCostPrice")]
        public double CurrentCostPrice
        {
            get { return _currentCostPrice; }
            set
            {
                SetProperty(ref _currentCostPrice, value);
            }
        }

        private int _preBuyAmount;
        [DataMember(Name = "preBuyAmount")]
        public int PreBuyAmount
        {
            get { return _preBuyAmount; }
            set
            {
                SetProperty(ref _preBuyAmount, value);
            }
        }

        private int _preSellAmount;
        [DataMember(Name = "preSellAmount")]
        public int PreSellAmount
        {
            get { return _preSellAmount; }
            set
            {
                SetProperty(ref _preSellAmount, value);
            }
        }

        private double _preBuyBalance;
        [DataMember(Name = "preBuyBalance")]
        public double PreBuyBalance
        {
            get { return _preBuyBalance; }
            set
            {
                SetProperty(ref _preBuyBalance, value);
            }
        }

        private double _preSellBalance;
        [DataMember(Name = "preSellBalance")]
        public double PreSellBalance
        {
            get { return _preSellBalance; }
            set
            {
                SetProperty(ref _preSellBalance, value);
            }
        }

        private double _todayBuyAmount;
        [DataMember(Name = "todayBuyAmount")]
        public double TodayBuyAmount
        {
            get { return _todayBuyAmount; }
            set
            {
                SetProperty(ref _todayBuyAmount, value);
            }
        }

        private int _todaySellAmount;
        [DataMember(Name = "todaySellAmount")]
        public int TodaySellAmount
        {
            get { return _todaySellAmount; }
            set
            {
                SetProperty(ref _todaySellAmount, value);
            }
        }

        private double _todayBuyBalance;
        [DataMember(Name = "todayBuyBalance")]
        public double TodayBuyBalance
        {
            get { return _todayBuyBalance; }
            set
            {
                SetProperty(ref _todayBuyBalance, value);
            }
        }

        private double _todaySellBalance;
        [DataMember(Name = "todaySellBalance")]
        public double TodaySellBalance
        {
            get { return _todaySellBalance; }
            set
            {
                SetProperty(ref _todaySellBalance, value);
            }
        }

        private double _todayBuyFee;
        [DataMember(Name = "todayBuyFee")]
        public double TodayBuyFee
        {
            get { return _todayBuyFee; }
            set
            {
                SetProperty(ref _todayBuyFee, value);
            }
        }

        private double _todaySellFee;
        [DataMember(Name = "todaySellFee")]
        public double TodaySellFee
        {
            get { return _todaySellFee; }
            set
            {
                SetProperty(ref _todaySellFee, value);
            }
        }


        public string GetTitle()
        {
            return "交易市场,证券代码,证券名称,组合编号,多空标志,持仓席位,股东代码,投资类型,当前数量,今日数量,昨仓数量,可用数量,今日可用数量,昨仓可用数量,当前成本价,当日开仓数量,当日平仓数量,当日开仓金额,当日平仓金额";
        }

        public string Export()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", MarketType,
                SecurityID, String.IsNullOrEmpty(SecurityName) ? " " : SecurityName, CombiNo, PositionFlag, HoldSeat,
                StockholderId, InvestType, CurrentAmount, TodayAmount, LastdayAmount, EnableAmount, TodayEnableAmount,
                LastdayEnableAmount, CurrentCostPrice, TodayBuyAmount, TodaySellAmount, TodayBuyBalance,
                TodaySellBalance);
        }
    }

    [DataContract]
    public class PositionInfo : PositionInfoBase
    {
    }

    public class FuturePositionInfo : PositionInfoBase
    {

    }


    public class OptionPositionInfo : FuturePositionInfo
    {
        private OptionInfoModel _optionInstrumentInfo;
        public OptionInfoModel OptionInstrumentInfo
        {
            get { return _optionInstrumentInfo; }
            set
            {
                SetProperty(ref _optionInstrumentInfo, value);
            }
        }
    }

    public class PositionInfoNotifyEvent : PubSubEvent<PositionInfo>
    {

    }

    public class RelatedPositionItemChangedNotifyEvent : PubSubEvent<PositionInfo>
    {

    }

    public class FuturePositionInfoNotifyEvent : PubSubEvent<FuturePositionInfo>
    {

    }

    public class RelatedFuturePositionItemChangedNotifyEvent : PubSubEvent<FuturePositionInfo>
    {

    }

    public class OptionPositionInfoNotifyEvent : PubSubEvent<OptionPositionInfo>
    {

    }

    public class RelatedOptionPositionItemChangedNotifyEvent : PubSubEvent<OptionPositionInfo>
    {

    }


    public class ClosePositionInfoNotifyEvent : PubSubEvent<PositionInfo>
    {

    }

    public class CloseFuturePositionInfoNotifyEvent : PubSubEvent<FuturePositionInfo>
    {

    }

    public class CloseOptionPositionInfoNotifyEvent : PubSubEvent<OptionPositionInfo>
    {

    }
}
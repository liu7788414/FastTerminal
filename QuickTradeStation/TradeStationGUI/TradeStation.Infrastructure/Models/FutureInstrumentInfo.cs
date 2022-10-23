using System;
using System.Runtime.Serialization;
using Microsoft.Practices.Prism.PubSubEvents;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class FutureInstrumentInfo : InfoWithSecurityInfo, IExport
    {
        private ePositionInfoMsgType _msgType;

        [DataMember(Name = "msgType")]
        public ePositionInfoMsgType MsgType
        {
            get { return _msgType; }
            set { SetProperty(ref _msgType, value); }
        }

        private string _futureKindName;

        [DataMember(Name = "futureKindName")]
        public string FutureKindName
        {
            get { return _futureKindName; }
            set { SetProperty(ref _futureKindName, value); }
        }

        private eMarketType _targetMarketNo;

        [DataMember(Name = "targetMarketNo")]
        public eMarketType TargetMarketNo
        {
            get { return _targetMarketNo; }
            set { SetProperty(ref _targetMarketNo, value); }
        }

        private string _targetStockCode;

        [DataMember(Name = "targetStockCode")]
        public string TargetStockCode
        {
            get { return _targetStockCode; }
            set { SetProperty(ref _targetStockCode, value); }
        }

        private string _settlementMonth;

        [DataMember(Name = "settlementMonth")]
        public string SettlementMonth
        {
            get { return _settlementMonth; }
            set { SetProperty(ref _settlementMonth, value); }
        }

        private int _multiple;

        [DataMember(Name = "multiple")]
        public int Multiple
        {
            get { return _multiple; }
            set { SetProperty(ref _multiple, value); }
        }

        private string _lastTradeDate;

        [DataMember(Name = "lastTradeDate")]
        public string LastTradeDate
        {
            get { return _lastTradeDate; }
            set { SetProperty(ref _lastTradeDate, value); }
        }


        private string _lastTradeTime;

        [DataMember(Name = "lastTradeTime")]
        public string LastTradeTime
        {
            get { return _lastTradeTime; }
            set { SetProperty(ref _lastTradeTime, value); }
        }

        private double _settlementPrice;

        [DataMember(Name = "settlementPrice")]
        public double SettlementPrice
        {
            get { return _settlementPrice; }
            set { SetProperty(ref _settlementPrice, value); }
        }

        private double _preSettlementPrice;

        [DataMember(Name = "preSettlementPrice")]
        public double PreSettlementPrice
        {
            get { return _preSettlementPrice; }
            set { SetProperty(ref _preSettlementPrice, value); }
        }

        private double _marketPosition;

        [DataMember(Name = "marketPosition")]
        public double MarketPosition
        {
            get { return _marketPosition; }
            set { SetProperty(ref _marketPosition, value); }
        }

        private double _preMarketPosition;

        [DataMember(Name = "preMarketPosition")]
        public double PreMarketPosition
        {
            get { return _preMarketPosition; }
            set { SetProperty(ref _preMarketPosition, value); }
        }

        private string _marketPricePermit;

        [DataMember(Name = "marketPricePermit")]
        public string MarketPricePermit
        {
            get { return _marketPricePermit; }
            set { SetProperty(ref _marketPricePermit, value); }
        }

        private double _uplimitedPrice;

        [DataMember(Name = "uplimitedPrice")]
        public double UplimitedPrice
        {
            get { return _uplimitedPrice; }
            set { SetProperty(ref _uplimitedPrice, value); }
        }

        private double _downlimitedPrice;

        [DataMember(Name = "downlimitedPrice")]
        public double DownlimitedPrice
        {
            get { return _downlimitedPrice; }
            set { SetProperty(ref _downlimitedPrice, value); }
        }

        public string GetTitle()
        {
            return "交易市场,合约代码,合约名称,期货品种名称,合约月份,合约乘数,最后交易日,最后交易时间,交割日,结算价,前结算价,市场持仓量,前市场持仓量,市场申报许可,涨停板价格,跌停板价格";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", MarketType,
                SecurityID, String.IsNullOrEmpty(SecurityName) ? " " : SecurityName, FutureKindName, SettlementMonth,
                Multiple, LastTradeDate, LastTradeTime, SettlementDate, SettlementPrice, PreSettlementPrice,
                MarketPosition, PreMarketPosition, MarketPricePermit, UplimitedPrice, DownlimitedPrice);
        }
    }


    [DataContract]
    public class EtfBaseInfo : InfoWithSecurityInfo, IExport
    {
        private string _businessDate;

        [DataMember(Name = "businessDate")]
        public string BusinessDate
        {
            get { return _businessDate; }
            set { SetProperty(ref _businessDate, value); }
        }

        private string _etfCode;

        [DataMember(Name = "etfCode")]
        public string EtfCode
        {
            get { return _etfCode; }
            set { SetProperty(ref _etfCode, value); }
        }

        private int _stockNum;

        [DataMember(Name = "stockNum")]
        public int StockNum
        {
            get { return _stockNum; }
            set { SetProperty(ref _stockNum, value); }
        }

        private eCreationRedeemType _creationRedeemType;

        [DataMember(Name = "creationRedeemType")]
        public eCreationRedeemType CreationRedeemType
        {
            get { return _creationRedeemType; }
            set { SetProperty(ref _creationRedeemType, value); }
        }


        private eEtfMarketType _etfMarketType;

        [DataMember(Name = "etfMarketType")]
        public eEtfMarketType EtfMarketType
        {
            get { return _etfMarketType; }
            set { SetProperty(ref _etfMarketType, value); }
        }

        private string _rivalMarket;

        [DataMember(Name = "rivalMarket")]
        public string RivalMarket
        {
            get { return _rivalMarket; }
            set { SetProperty(ref _rivalMarket, value); }
        }

        private eEtfType _etfType;

        [DataMember(Name = "etfType")]
        public eEtfType EtfType
        {
            get { return _etfType; }
            set { SetProperty(ref _etfType, value); }
        }

        private double _maxCashRatio;

        [DataMember(Name = "maxCashRatio")]
        public double MaxCashRatio
        {
            get { return _maxCashRatio; }
            set { SetProperty(ref _maxCashRatio, value); }
        }

        private double _reportUnit;

        [DataMember(Name = "reportUnit")]
        public double ReportUnit
        {
            get { return _reportUnit; }
            set { SetProperty(ref _reportUnit, value); }
        }

        private double _yesterdayCash;

        [DataMember(Name = "yesterdayCash")]
        public double YesterdayCash
        {
            get { return _yesterdayCash; }
            set { SetProperty(ref _yesterdayCash, value); }
        }

        private double _yesterdayNav;

        [DataMember(Name = "yesterdayNav")]
        public double YesterdayNav
        {
            get { return _yesterdayNav; }
            set { SetProperty(ref _yesterdayNav, value); }
        }

        private double _estimateCash;

        [DataMember(Name = "estimateCash")]
        public double EstimateCash
        {
            get { return _estimateCash; }
            set { SetProperty(ref _estimateCash, value); }
        }

        private string _underlyingIndex;

        [DataMember(Name = "underlyingIndex")]
        public string UnderlyingIndex
        {
            get { return _underlyingIndex; }
            set { SetProperty(ref _underlyingIndex, value); }
        }

        public string GetTitle()
        {
            return
                "业务日期,交易市场,ETF代码,证券名称,证券代码,成份股数量,当天状态,ETF市场类型,对方市场,ETF分类,现金替代比例上限,申报单位,昨日现金余额,昨日单位净值,预估现金差额,拟合指数代码";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", BusinessDate,
                MarketType, EtfCode, SecurityName, SecurityID,
                StockNum, CreationRedeemType, EtfMarketType, RivalMarket, EtfType, MaxCashRatio,
                ReportUnit, YesterdayCash, YesterdayNav, EstimateCash, UnderlyingIndex);
        }
    }

    [DataContract]
    public class EtfStock : InfoWithSecurityInfo, IExport
    {
        private string _businessDate;

        [DataMember(Name = "businessDate")]
        public string BusinessDate
        {
            get { return _businessDate; }
            set { SetProperty(ref _businessDate, value); }
        }

        private int _stockAmount;

        [DataMember(Name = "stockAmount")]
        public int StockAmount
        {
            get { return _stockAmount; }
            set { SetProperty(ref _stockAmount, value); }
        }

        private eReplaceFlag _replaceFlag;

        [DataMember(Name = "replaceFlag")]
        public eReplaceFlag ReplaceFlag
        {
            get { return _replaceFlag; }
            set { SetProperty(ref _replaceFlag, value); }
        }

        private double _replaceRatio;

        [DataMember(Name = "replaceRatio")]
        public double ReplaceRatio
        {
            get { return _replaceRatio; }
            set { SetProperty(ref _replaceRatio, value); }
        }

        private double _replaceBalance;

        [DataMember(Name = "replaceBalance")]
        public double ReplaceBalance
        {
            get { return _replaceBalance; }
            set { SetProperty(ref _replaceBalance, value); }
        }

        private double _redeemReplaceBalance;

        [DataMember(Name = "redeemReplaceBalance")]
        public double RedeemReplaceBalance
        {
            get { return _redeemReplaceBalance; }
            set { SetProperty(ref _redeemReplaceBalance, value); }
        }

        public string GetTitle()
        {
            return "业务日期,交易市场,证券名称,证券数量,现金替代标志,溢价比率,替代金额,赎回替代金额";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", BusinessDate, MarketType, SecurityID, StockAmount,
                ReplaceFlag, ReplaceRatio, ReplaceBalance, RedeemReplaceBalance);
        }
    }

    public class EtfBaseInfoNotifyEvent : PubSubEvent<EtfBaseInfo>
    {

    }
}
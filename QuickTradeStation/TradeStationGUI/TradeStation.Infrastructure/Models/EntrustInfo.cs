using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class EntrustInfo : InfoWithCombiNoAndSecurityInfo, IExport, IImport
    {
        private bool _canChasedOrder;
        [DataMember(Name = "canChasedOrder")]
        public bool CanChasedOrder
        {
            get { return _canChasedOrder; }
            set
            {
                SetProperty(ref _canChasedOrder, value);
            }
        }

        private bool _isChasingOrder = false;
        [DataMember(Name = "isChasingOrder")]
        public bool IsChasingOrder
        {
            get { return _isChasingOrder; }
            set
            {
                SetProperty(ref _isChasingOrder, value);
            }
        }

        private eChaseOrderType _chaseOrderType;
        [DataMember(Name = "chaseOrderType")]
        public eChaseOrderType ChaseOrderType
        {
            get { return _chaseOrderType; }
            set
            {
                SetProperty(ref _chaseOrderType, value);
            }
        }

        private string _instanceNo;
        [DataMember(Name = "instanceNo")]
        public string InstanceNo
        {
            get { return _instanceNo; }
            set
            {
                SetProperty(ref _instanceNo, value);
            }
        }

        private string _operatorNo;
        [DataMember(Name = "operatorNo")]
        public string OperatorNo
        {
            get { return _operatorNo; }
            set
            {
                SetProperty(ref _operatorNo, value);
            }
        }

        private string _firstDealTime;
        [DataMember(Name = "firstDealTime")]
        public string FirstDealTime
        {
            get { return _firstDealTime; }
            set
            {
                SetProperty(ref _firstDealTime, value);
            }
        }

        private string _confirmNo;
        [DataMember(Name = "confirmNo")]
        public string ConfirmNo
        {
            get { return _confirmNo; }
            set
            {
                SetProperty(ref _confirmNo, value);
            }
        }

        private int _withdrawAmount;
        [DataMember(Name = "withdrawAmount")]
        public int WithdrawAmount
        {
            get { return _withdrawAmount; }
            set
            {
                SetProperty(ref _withdrawAmount, value);
            }
        }

        private int _dealTimes;
        [DataMember(Name = "dealTimes")]
        public int DealTimes
        {
            get { return _dealTimes; }
            set
            {
                SetProperty(ref _dealTimes, value);
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

        private double _dealFee;
        [DataMember(Name = "dealFee")]
        public double DealFee
        {
            get { return _dealFee; }
            set
            {
                SetProperty(ref _dealFee, value);
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

        private double _preBuyFrozenBalance;
        [DataMember(Name = "preBuyFrozenBalance")]
        public double PreBuyFrozenBalance
        {
            get { return _preBuyFrozenBalance; }
            set
            {
                SetProperty(ref _preBuyFrozenBalance, value);
            }
        }

        private string _extsystemId;
        [DataMember(Name = "extsystemId")]
        public string ExtsystemId
        {
            get { return _extsystemId; }
            set
            {
                SetProperty(ref _extsystemId, value);
            }
        }

        private string _thirdReff;
        [DataMember(Name = "thirdReff")]
        public string ThirdReff
        {
            get { return _thirdReff; }
            set
            {
                SetProperty(ref _thirdReff, value);
            }
        }

        private string _revokeCause;
        [DataMember(Name = "revokeCause")]
        public string RevokeCause
        {
            get { return _revokeCause; }
            set
            {
                SetProperty(ref _revokeCause, value);
            }
        }

        private bool _selected;
        [DataMember(Name = "selected")]
        public bool Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value);
            }
        }

        private eMessageType _msgType;
        [DataMember(Name = "msgType")]
        public eMessageType MsgType
        {
            get { return _msgType; }
            set
            {
                SetProperty(ref _msgType, value);
            }
        }

        private string _entrustDate;
        [DataMember(Name = "entrustDate")]
        public string EntrustDate
        {
            get { return _entrustDate; }
            set
            {
                SetProperty(ref _entrustDate, value);
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

        private int _batchNo;
        [DataMember(Name = "batchNo")]
        public int BatchNo
        {
            get { return _batchNo; }
            set
            {
                SetProperty(ref _batchNo, value);
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

        private int _detailEntrustNo;
        [DataMember(Name = "detailEntrustNo")]
        public int DetailEntrustNo
        {
            get { return _detailEntrustNo; }
            set
            {
                SetProperty(ref _detailEntrustNo, value);
            }
        }

        private int _cancelEntrustNo;
        [DataMember(Name = "cancelEntrustNo")]
        public int CancelEntrustNo
        {
            get { return _cancelEntrustNo; }
            set
            {
                SetProperty(ref _cancelEntrustNo, value);
            }
        }

        private string _entrustBatchNo;
        [DataMember(Name = "entrustBatchNo")]
        public string EntrustBatchNo
        {
            get { return _entrustBatchNo; }
            set
            {
                SetProperty(ref _entrustBatchNo, value);
            }
        }

        private string _entrustTime;
        [DataMember(Name = "entrustTime")]
        public string EntrustTime
        {
            get { return _entrustTime; }
            set
            {
                SetProperty(ref _entrustTime, value);
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

        private eEntrustPriceType _entrustPriceType;
        [DataMember(Name = "entrustPriceType")]
        public eEntrustPriceType EntrustPriceType
        {
            get { return _entrustPriceType; }
            set
            {
                SetProperty(ref _entrustPriceType, value);
            }
        }

        private double _entrustPrice;
        [DataMember(Name = "entrustPrice")]
        public double EntrustPrice
        {
            get { return _entrustPrice; }
            set
            {
                SetProperty(ref _entrustPrice, value);
            }
        }


        private int _entrustAmount;
        [DataMember(Name = "entrustAmount")]
        public int EntrustAmount
        {
            get { return _entrustAmount; }
            set
            {
                SetProperty(ref _entrustAmount, value);
            }
        }

        private eEntrustState _entrustState;
        [DataMember(Name = "entrustState")]
        public eEntrustState EntrustState
        {
            get { return _entrustState; }
            set
            {
                SetProperty(ref _entrustState, value);
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

        private string _dealProgress;
        [DataMember(Name = "dealProgress")]
        public string DealProgress
        {
            get
            {
                double p = ((double)_totalDealAmount) / _entrustAmount;
                return string.Format("{0:0%}", p);
            }
            set
            {
                SetProperty(ref _dealProgress, value);
            }
        }

        private int _dealprogressWidth;
        [DataMember(Name = "dealprogressWidth")]
        public int DealprogressWidth
        {
            get
            {
                var p = (int)((((double)_totalDealAmount) / _entrustAmount) * 100);
                return p;
            }
            set
            {
                SetProperty(ref _dealprogressWidth, value);
            }
        }

        private int _totalDealAmount;
        [DataMember(Name = "totalDealAmount")]
        public int TotalDealAmount
        {
            get { return _totalDealAmount; }
            set
            {
                SetProperty(ref _totalDealAmount, value);
            }
        }

        private double _totalDealBalance;
        [DataMember(Name = "totalDealBalance")]
        public double TotalDealBalance
        {
            get { return _totalDealBalance; }
            set
            {
                SetProperty(ref _totalDealBalance, value);
            }
        }

        private int _cancelAmount;
        [DataMember(Name = "cancelAmount")]
        public int CancelAmount
        {
            get { return _cancelAmount; }
            set
            {
                SetProperty(ref _cancelAmount, value);
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

        private double _entrustBalance;
        [DataMember(Name = "entrustBalance")]
        public double EntrustBalance
        {
            get { return _entrustBalance; }
            set
            {
                SetProperty(ref _entrustBalance, value);
            }
        }


        private ePurchaseWay _purchaseWay;
        [DataMember(Name = "purchaseWay")]
        public ePurchaseWay PurchaseWay
        {
            get { return _purchaseWay; }
            set
            {
                SetProperty(ref _purchaseWay, value);
            }
        }

        // Only for view to display. It is not a data member.
        // Consider to move to another viewmodel.
        private SecurityQuotation _securityQuotation;
        public SecurityQuotation SecurityQuotation
        {
            get { return _securityQuotation; }
            set
            {
                SetProperty(ref _securityQuotation, value);
            }
        }
        
        public string GetTitle()
        {
            return "证券种类,交易市场,证券代码,证券名称,组合编号,委托方向,委托价格类型,委托价格,委托数量,开平方向,投资类型,成交数量,委托时间,委托编号,委托状态,备注";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", Category, MarketType, SecurityID, string.IsNullOrEmpty(SecurityName) ? " " : SecurityName, CombiNo, EntrustDirection, EntrustPriceType, EntrustPrice, EntrustAmount, FuturesDirection, InvestType, DealAmount, EntrustTime, EntrustNo > 0 ? EntrustNo : 0, EntrustState, RevokeCause);
        }

        public void Import(string[] s)
        {
            Category = (eCategory)Enum.Parse(typeof(eCategory), s[0]);
            MarketType = (eMarketType)Enum.Parse(typeof(eMarketType), s[1]);
            SecurityID = s[2];
            SecurityName = s[3];
            CombiNo = s[4];
            EntrustDirection = (eEntrustDirection)Enum.Parse(typeof(eEntrustDirection), s[5]);
            EntrustPriceType = (eEntrustPriceType)Enum.Parse(typeof(eEntrustPriceType), s[6]);
            EntrustPrice = Convert.ToDouble(s[7]);
            EntrustAmount = Convert.ToInt32(s[8]);
            FuturesDirection = (eFuturesDirection)Enum.Parse(typeof(eFuturesDirection), s[9]);
            InvestType = (eInvestType)Enum.Parse(typeof(eInvestType), s[10]);
            DealAmount = Convert.ToInt32(s[11]);
            EntrustTime = s[12];

            if (!string.IsNullOrEmpty(s[13]))
            {
                EntrustNo = Convert.ToInt32(s[13]);
            }

            EntrustState = (eEntrustState)Enum.Parse(typeof(eEntrustState), s[14]);

            if(s.Length == 16)
            {
                RevokeCause = s[15];
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "品种:{0},市场:{1},代码:{2},名称:{3},组合:{4},方向:{5},价格类型:{6},委价:{7},数量:{8},开平仓:{9},投资类型:{10},成交数量:{11},委托时间:{12},委托编号:{13},状态:{14},第三方编号:{15},原因:{16},撤单号:{17}",
                    Category, MarketType, SecurityID, string.IsNullOrEmpty(SecurityName) ? " " : SecurityName, CombiNo,
                    EntrustDirection, EntrustPriceType, EntrustPrice, EntrustAmount, FuturesDirection, InvestType,
                    DealAmount, EntrustTime, EntrustNo > 0 ? EntrustNo : 0, EntrustState, ExtsystemId, RevokeCause,
                    CancelEntrustNo
                    );
        }
    }

    [DataContract]
    public class HistoricalEntrustInfo : EntrustInfo
    {

    }

    [DataContract]
    public class FutureEntrustInfo : EntrustInfo
    {

    }

    [DataContract]
    public abstract class HistoricalFutureEntrustInfo : HistoricalEntrustInfo
    {

    }

    [DataContract]
    public class FundEntrustInfo : EntrustInfo
    {

    }

    [DataContract]
    public class HistoricalFundEntrustInfo : HistoricalEntrustInfo
    {

    }

    [DataContract]
    public class OptionEntrustInfo : EntrustInfo
    {

    }

    [DataContract]
    public class HistoricalOptionEntrustInfo : HistoricalEntrustInfo
    {

    }

    public class EntrustInfoNotifyEvent : PubSubEvent<EntrustInfo>
    {

    }

    public class HistoricalEntrustInfoNotifyEvent : PubSubEvent<HistoricalEntrustInfo>
    {

    }

    public class FutureEntrustInfoNotifyEvent : PubSubEvent<FutureEntrustInfo>
    {

    }

    public class HistoricalFutureEntrustInfoNotifyEvent : PubSubEvent<HistoricalFutureEntrustInfo>
    {

    }

    public class FundEntrustInfoNotifyEvent : PubSubEvent<FundEntrustInfo>
    {

    }

    public class HistoricalFundEntrustInfoNotifyEvent : PubSubEvent<HistoricalFundEntrustInfo>
    {

    }

    public class OptionEntrustInfoNotifyEvent : PubSubEvent<OptionEntrustInfo>
    {

    }

    public class NewOptionEntrustNotifyEvent : PubSubEvent<OptionEntrustInfo>
    {

    }

    public class NewStockEntrustNotifyEvent : PubSubEvent<EntrustInfo>
    {

    }

    public class NewFutureEntrustNotifyEvent : PubSubEvent<FutureEntrustInfo>
    {

    }

    public class HistoricalOptionEntrustInfoNotifyEvent : PubSubEvent<HistoricalOptionEntrustInfo>
    {

    }

    public class EntrustDealCallBackEvent : PubSubEvent<EntrustInfo>
    {

    }
}
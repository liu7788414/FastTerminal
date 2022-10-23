using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Practices.Prism.Mvvm;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Services;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class ArbitrageItem : BindableBase
    {
        private bool _isChecked;
        [DataMember(Name = "isChecked")]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        private SecurityInfo _securityInfo;
        [DataMember(Name = "securityInfo")]
        public SecurityInfo SecurityInfo
        {
            get { return _securityInfo; }
            set
            {
                SetProperty(ref _securityInfo, value);
            }
        }

        private int _amount;
        [DataMember(Name = "amount")]
        public int Amount
        {
            get { return _amount; }
            set
            {
                SetProperty(ref _amount, value);
            }
        }

        private int _enabledAmount;
        [DataMember(Name = "enabledAmount")]
        public int EnabledAmount
        {
            get { return _enabledAmount; }
            set { SetProperty(ref _enabledAmount, value); }
        }

        private double _lastPrice;
        [DataMember(Name = "lastPrice")]
        public double LastPrice
        {
            get { return _lastPrice; }
            set
            {
                SetProperty(ref _lastPrice, value);
            }
        }

        public static HsStock Trader;
    }


    [DataContract]
    public class ArbitrageBase : BindableBase
    {
        private bool _isChecked;
        [DataMember(Name = "isChecked")]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        private ArbitrageStatus _arbitrageStatus = ArbitrageStatus.正在监控;
        [DataMember(Name = "arbitrageStatus")]
        public ArbitrageStatus ArbitrageStatus
        {
            get { return _arbitrageStatus; }
            set
            {
                SetProperty(ref _arbitrageStatus, value);
            }
        }

        private SecurityInfo _displaySecurityInfoLeg1;

        public SecurityInfo DisplaySecurityInfoLeg1
        {
            get { return _displaySecurityInfoLeg1; }
            set { SetProperty(ref _displaySecurityInfoLeg1, value); }
        }

        private SecurityInfo _displaySecurityInfoLeg2;

        public SecurityInfo DisplaySecurityInfoLeg2
        {
            get { return _displaySecurityInfoLeg2; }
            set { SetProperty(ref _displaySecurityInfoLeg2, value); }
        }

        private SecurityInfo _selectedSecurityInfoLeg1;

        public SecurityInfo SelectedSecurityInfoLeg1
        {
            get { return _selectedSecurityInfoLeg1; }
            set { SetProperty(ref _selectedSecurityInfoLeg1, value); }
        }

        private SecurityInfo _selectedSecurityInfoLeg2;

        public SecurityInfo SelectedSecurityInfoLeg2
        {
            get { return _selectedSecurityInfoLeg2; }
            set { SetProperty(ref _selectedSecurityInfoLeg2, value); }
        }

        private int _entrustAmountLeg1;

        public int EntrustAmountLeg1
        {
            get { return _entrustAmountLeg1; }
            set { SetProperty(ref _entrustAmountLeg1, value); }
        }

        private int _entrustAmountLeg2;

        public int EntrustAmountLeg2
        {
            get { return _entrustAmountLeg2; }
            set { SetProperty(ref _entrustAmountLeg2, value); }
        }

        //private string _leg1Number;

        //public string Leg1Number
        //{
        //    get { return _leg1Number; }
        //    set { SetProperty(ref _leg1Number, value); }
        //}

        //private string _leg2Number;

        //public string Leg2Number
        //{
        //    get { return _leg2Number; }
        //    set { SetProperty(ref _leg2Number, value); }
        //}

        private string _selectedCombiNoLeg1;
        public string SelectedCombiNoLeg1
        {
            get { return _selectedCombiNoLeg1; }
            set
            {
                SetProperty(ref _selectedCombiNoLeg1, value);
            }
        }

        private string _selectedCombiNoLeg2;
        public string SelectedCombiNoLeg2
        {
            get { return _selectedCombiNoLeg2; }
            set
            {
                SetProperty(ref _selectedCombiNoLeg2, value);
            }
        }

        private eEntrustDirection _entrustDirectionLeg1 = eEntrustDirection.买入;

        public eEntrustDirection EntrustDirectionLeg1
        {
            get { return _entrustDirectionLeg1; }
            set { SetProperty(ref _entrustDirectionLeg1, value); }
        }

        private eEntrustDirection _entrustDirectionLeg2 = eEntrustDirection.买入;

        public eEntrustDirection EntrustDirectionLeg2
        {
            get { return _entrustDirectionLeg2; }
            set { SetProperty(ref _entrustDirectionLeg2, value); }
        }

        private string _nameLeg1;
        [DataMember(Name = "nameLeg1")]
        public string NameLeg1
        {
            get { return _nameLeg1; }
            set
            {
                SetProperty(ref _nameLeg1, value);
            }
        }

        private string _nameLeg2;
        [DataMember(Name = "nameLeg2")]
        public string NameLeg2
        {
            get { return _nameLeg2; }
            set
            {
                SetProperty(ref _nameLeg2, value);
            }
        }

        private string _limitEntrustRatioLeg1 = "100%";
        [DataMember(Name = "limitEntrustRatioLeg1")]
        public string LimitEntrustRatioLeg1
        {
            get { return _limitEntrustRatioLeg1; }
            set
            {
                SetProperty(ref _limitEntrustRatioLeg1, value);
            }
        }

        private string _limitEntrustRatioLeg2 = "100%";
        [DataMember(Name = "limitEntrustRatioLeg2")]
        public string LimitEntrustRatioLeg2
        {
            get { return _limitEntrustRatioLeg2; }
            set
            {
                SetProperty(ref _limitEntrustRatioLeg2, value);
            }
        }

        private int _executionTimes = 1;
        [DataMember(Name = "executionTimes")]
        public int ExecutionTimes
        {
            get { return _executionTimes; }
            set
            {
                SetProperty(ref _executionTimes, value);
            }
        }

        private int _currentTime;
        [DataMember(Name = "currentTime")]
        public int CurrentTime
        {
            get { return _currentTime; }
            set
            {
                SetProperty(ref _currentTime, value);
            }
        }

        private PriceDifferenceType _priceDifferenceType = PriceDifferenceType.点数;
        [DataMember(Name = "priceDifferenceType")]
        public PriceDifferenceType PriceDifferenceType
        {
            get { return _priceDifferenceType; }
            set
            {
                SetProperty(ref _priceDifferenceType, value);
            }
        }

        private ArbitrageTradeMethod _basketTradeMethod = ArbitrageTradeMethod.两腿同时;
        [DataMember(Name = "basketTradeMethod")]
        public ArbitrageTradeMethod BasketTradeMethod
        {
            get { return _basketTradeMethod; }
            set
            {
                SetProperty(ref _basketTradeMethod, value);
            }
        }

        private bool _issueNow;
        [DataMember(Name = "issueNow")]
        public bool IssueNow
        {
            get { return _issueNow; }
            set { SetProperty(ref _issueNow, value); }
        }

        private CompareSymbol _compareSymbol = CompareSymbol.大于;
        [DataMember(Name = "compareSymbol")]
        public CompareSymbol CompareSymbol
        {
            get { return _compareSymbol; }
            set { SetProperty(ref _compareSymbol, value); }
        }
        
        private double _priceDifference;
        [DataMember(Name = "priceDifference")]
        public double PriceDifference
        {
            get { return _priceDifference; }
            set
            {
                SetProperty(ref _priceDifference, value);
            }
        }

        private eFuturesDirection _futuresDirection = eFuturesDirection.开仓;
        [DataMember(Name = "futuresDirection")]
        public eFuturesDirection FuturesDirection 
        {
            get { return _futuresDirection; }
            set { SetProperty(ref _futuresDirection, value); }
        }

        private double _currentPriceDifference;
        [DataMember(Name = "currentPriceDifference")]
        public double CurrentPriceDifference
        {
            get { return _currentPriceDifference; }
            set
            {
                SetProperty(ref _currentPriceDifference, value);
            }
        }

        private ObservableCollection<ArbitrageItem> _arbitrageItemListLeg1 = new ObservableCollection<ArbitrageItem>();
        public ObservableCollection<ArbitrageItem> ArbitrageItemListLeg1
        {
            get { return _arbitrageItemListLeg1; }
            set
            {
                SetProperty(ref _arbitrageItemListLeg1, value);
            }
        }

        private ObservableCollection<ArbitrageItem> _arbitrageItemListLeg2 = new ObservableCollection<ArbitrageItem>();
        public ObservableCollection<ArbitrageItem> ArbitrageItemListLeg2
        {
            get { return _arbitrageItemListLeg2; }
            set
            {
                SetProperty(ref _arbitrageItemListLeg2, value);
            }
        }
    }

    [DataContract]
    public class Arbitrage : ArbitrageBase
    {

    }

    public class AddEtfStocksNotifyEvent : PubSubEvent<string>
    {

    }

    public class LegInfo
    {
        public int Number;      
        public double TotalValue;
        public string StoppedSecurity;
        public string CancelledSecurity;
        public string StockUpperLimitSecurity;
        public string StockLowerLimitSecurity;
        public string FuturesUpperLimitSecurity;
        public string FuturesLowerLimitSecurity;
    }
}
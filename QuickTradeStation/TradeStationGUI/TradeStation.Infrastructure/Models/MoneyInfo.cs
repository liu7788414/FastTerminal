using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class MoneyInfoBase : BindableBase, IExport
    {
        private eMoneyInfoMsgType _msgType;
        [DataMember(Name = "msgType")]
        public eMoneyInfoMsgType MsgType
        {
            get { return _msgType; }
            set
            {
                SetProperty(ref _msgType, value);
            }
        }

        private eMoneyType _moneyType;
        [DataMember(Name = "moneyType")]
        public eMoneyType MoneyType
        {
            get { return _moneyType; }
            set
            {
                SetProperty(ref _moneyType, value);
            }
        }

        private string _accountCode;
        [DataMember(Name = "accountCode")]
        public string AccountCode
        {
            get { return _accountCode; }
            set
            {
                SetProperty(ref _accountCode, value);
            }
        }

        private string _assetNo;
        [DataMember(Name = "assetNo")]
        public string AssetNo
        {
            get { return _assetNo; }
            set
            {
                SetProperty(ref _assetNo, value);
            }
        }

        private double _enabledMoney;
        [DataMember(Name = "enabledMoney")]
        public double EnabledMoney
        {
            get { return _enabledMoney; }
            set
            {
                SetProperty(ref _enabledMoney, value);
            }
        }

        private double _totalMoney;
        [DataMember(Name = "totalMoney")]
        public double TotalMoney
        {
            get { return _totalMoney; }
            set
            {
                SetProperty(ref _totalMoney, value);
            }
        }

        private double _occupyDepositBalance;
        [DataMember(Name = "occupyDepositBalance")]
        public double OccupyDepositBalance
        {
            get { return _occupyDepositBalance; }
            set
            {
                SetProperty(ref _occupyDepositBalance, value);
            }
        }

        private double _enableDepositBalance;
        [DataMember(Name = "enableDepositBalance")]
        public double EnableDepositBalance
        {
            get { return _enableDepositBalance; }
            set
            {
                SetProperty(ref _enableDepositBalance, value);
            }
        }

        public string GetTitle()
        {
            return "币种,账户编号,资产单元编号,T+0可用资金,T+1可用资金,期货占用保证金,期货可用保证金";
        }

        public string Export()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", MoneyType, AccountCode, AssetNo, EnabledMoney, TotalMoney, OccupyDepositBalance, EnableDepositBalance);
        }
    }

    [DataContract]
    public class MoneyInfo : MoneyInfoBase
    {

    }

    [DataContract]
    public class FutureMarginInfo : MoneyInfoBase
    {

    }

    [DataContract]
    public class OptionMarginInfo : MoneyInfoBase
    {

    }

    public class MoneyInfoNotifyEvent : PubSubEvent<MoneyInfo>
    {

    }

    public class FutureMarginInfoNotifyEvent : PubSubEvent<FutureMarginInfo>
    {

    }

    public class OptionMarginInfoNotifyEvent : PubSubEvent<OptionMarginInfo>
    {

    }
}
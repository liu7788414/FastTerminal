using Microsoft.Practices.Prism.Mvvm;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    [Export(typeof(UserSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [XmlRoot("UserSettings")]
    public class UserSettings : BindableBase
    {
        [ImportingConstructor]
        public UserSettings()
        {

        }

        private int _fastTradeAmountStock;
        [DataMember(Name = "fastTradeAmountStock")]
        [DisplayName("股票快速交易默认股数（单位：股）")]
        [Category("快速交易")]
        [Description("在进行股票快速交易时，默认股票数量。")]
        [DefaultValue("100")]
        [XmlElement("FastTradeAmountStock")]
        public int FastTradeAmountStock
        {
            get { return _fastTradeAmountStock; }
            set
            {
                SetProperty(ref _fastTradeAmountStock, value);
            }
        }

        private int _fastTradeAmountFuture;
        [DataMember(Name = "fastTradeAmountFuture")]
        [DisplayName("期货快速交易默认手数（单位：手）")]
        [Category("快速交易")]
        [Description("在进行期货快速交易时，默认合约手数。")]
        [DefaultValue("1")]
        [XmlElement("FastTradeAmountFuture")]
        public int FastTradeAmountFuture
        {
            get { return _fastTradeAmountFuture; }
            set
            {
                SetProperty(ref _fastTradeAmountFuture, value);
            }
        }

        private int _fastTradeAmountOption;
        [DataMember(Name = "fastTradeAmountOption")]
        [DisplayName("期权快速交易默认手数（单位：张）")]
        [Category("快速交易")]
        [Description("在进行期权快速交易时，默认合约张数。")]
        [DefaultValue("1")]
        [XmlElement("FastTradeAmountOption")]
        public int FastTradeAmountOption
        {
            get { return _fastTradeAmountOption; }
            set
            {
                SetProperty(ref _fastTradeAmountOption, value);
            }
        }


        private double _moneyLimitPerTrade;
        [DataMember(Name = "moneyLimitPerTrade")]
        [DisplayName("单笔委托金额上限（单位：元）")]
        [Category("风险控制")]
        [Description("单笔委托时，单价*数量不得大于该数值。")]
        [DefaultValue("1000000")]
        [XmlElement("moneyLimitPerTrade")]
        public double MoneyLimitPerTrade
        {
            get { return _moneyLimitPerTrade; }
            set
            {
                SetProperty(ref _moneyLimitPerTrade, value);
            }
        }

        private int _amountLimitPerTradeStock;
        [DataMember(Name = "amountLimitPerTradeStock")]
        [DisplayName("股票交易单笔委托数量上限（单位：股）")]
        [Category("风险控制")]
        [Description("在进行股票交易时，单笔委托数量不得大于该数值。")]
        [DefaultValue("10000")]
        [XmlElement("amountLimitPerTradeStock")]
        public int AmountLimitPerTradeStock
        {
            get { return _amountLimitPerTradeStock; }
            set
            {
                SetProperty(ref _amountLimitPerTradeStock, value);
            }
        }

        private int _amountLimitPerTradeFuture;
        [DataMember(Name = "amountLimitPerTradeFuture")]
        [DisplayName("期货交易单笔委托数量上限（单位：手）")]
        [Category("风险控制")]
        [Description("在进行期货交易时，单笔委托数量不得大于该数值。")]
        [DefaultValue("10")]
        [XmlElement("amountLimitPerTradeFuture")]
        public int AmountLimitPerTradeFuture
        {
            get { return _amountLimitPerTradeFuture; }
            set
            {
                SetProperty(ref _amountLimitPerTradeFuture, value);
            }
        }

        private int _amountLimitPerTradeOption;
        [DataMember(Name = "amountLimitPerTradeOption")]
        [DisplayName("期权交易单笔委托数量上限（单位：张）")]
        [Category("风险控制")]
        [Description("在进行期权交易时，单笔委托数量不得大于该数值。")]
        [DefaultValue("10")]
        [XmlElement("amountLimitPerTradeOption")]
        public int AmountLimitPerTradeOption
        {
            get { return _amountLimitPerTradeOption; }
            set
            {
                SetProperty(ref _amountLimitPerTradeOption, value);
            }
        }

        private bool _lockContentPane;
        [DataMember(Name = "lockContentPane")]
        [DisplayName("是否允许移动面板")]
        [Category("界面设置")]
        [Description("勾选即可移动面板。")]
        [DefaultValue("True")]
        [XmlElement("lockContentPane")]
        [Browsable(false)]
        public bool LockContentPane
        {
            get { return _lockContentPane; }
            set
            {
                SetProperty(ref _lockContentPane, value);
            }
        }


        private bool _isPromptEnabled;
        [DataMember(Name = "isPromptEnabled")]
        [DisplayName("委托时是否弹出确认提示框")]
        [Category("快速交易")]
        [Description("发送委托时，是否弹出确认提示框。")]
        [DefaultValue(true)]
        [XmlElement("IsPromptEnabled")]
        public bool IsPromptEnabled
        {
            get { return _isPromptEnabled; }
            set
            {
                SetProperty(ref _isPromptEnabled, value);
            }
        }


        private string _version;
        [DataMember(Name = "version")]
        [DisplayName("版本号")]
        [Category("软件信息")]
        [Description("软件版本信息")]
        [DefaultValue("1.0.0.0")]
        [XmlElement("Version")]
        [ReadOnly(true)]
        public string Version
        {
            get { return _version; }
            set
            {
                SetProperty(ref _version, value);
            }
        }

        private string _softwareName;
        [DataMember(Name = "softwareName")]
        [DisplayName("软件名称")]
        [Category("软件信息")]
        [Description("软件名称")]
        [DefaultValue("天风证券傲速交易终端")]
        [XmlElement("SoftwareName")]
        [ReadOnly(true)]
        public string SoftwareName
        {
            get { return _softwareName; }
            set
            {
                SetProperty(ref _softwareName, value);
            }
        }

        private bool _isStockVisible;
        [DataMember(Name = "isStockVisible")]
        [DisplayName("显示股票面板")]
        [Category("面板可见性")]
        [Description("设置股票面板的可见性")]
        [DefaultValue(true)]
        [XmlElement("IsStockVisible")]
        [Browsable(false)]
        public bool IsStockVisible
        {
            get { return _isStockVisible; }
            set
            {
                SetProperty(ref _isStockVisible, value);
            }
        }

        private bool _isFutureVisible;
        [DataMember(Name = "isFutureVisible")]
        [DisplayName("显示期货面板")]
        [Category("面板可见性")]
        [Description("设置期货面板的可见性")]
        [DefaultValue(true)]
        [XmlElement("IsFutureVisible")]
        [Browsable(false)]
        public bool IsFutureVisible
        {
            get { return _isFutureVisible; }
            set
            {
                SetProperty(ref _isFutureVisible, value);
            }
        }

        private bool _isOptionVisible;
        [DataMember(Name = "isOptionVisible")]
        [DisplayName("显示期权面板")]
        [Category("面板可见性")]
        [Description("设置期权面板的可见性")]
        [DefaultValue(true)]
        [XmlElement("IsOptionVisible")]
        [Browsable(false)]
        public bool IsOptionVisible
        {
            get { return _isOptionVisible; }
            set
            {
                SetProperty(ref _isOptionVisible, value);
            }
        }

        private bool _isFundVisible;
        [DataMember(Name = "isFundVisible")]
        [DisplayName("显示基金特殊业务面板")]
        [Category("面板可见性")]
        [Description("设置基金特殊业务面板的可见性")]
        [DefaultValue(true)]
        [XmlElement("IsFundVisible")]
        [Browsable(false)]
        public bool IsFundVisible
        {
            get { return _isFundVisible; }
            set
            {
                SetProperty(ref _isFundVisible, value);
            }
        }

        private bool _isBasketVisible;
        [DataMember(Name = "isBasketVisible")]
        [DisplayName("显示篮子交易面板")]
        [Category("面板可见性")]
        [Description("设置篮子交易面板的可见性")]
        [DefaultValue(true)]
        [XmlElement("IsBasketVisible")]
        [Browsable(false)]
        public bool IsBasketVisible
        {
            get { return _isBasketVisible; }
            set
            {
                SetProperty(ref _isBasketVisible, value);
            }
        }

        private bool _isQuotesOnly;

        [DataMember(Name = "isQuotesOnly")]
        [DisplayName("是否仅用于浏览行情")]
        [Category("运行方式")]
        [Description("设置是否仅用于浏览行情")]
        [DefaultValue(false)]
        [XmlElement("IsQuotesOnly")]
        [Browsable(false)]
        public bool IsQuotesOnly
        {
            get { return _isQuotesOnly; }
            set
            {
                SetProperty(ref _isQuotesOnly, value);              
            }
        }
    }
}
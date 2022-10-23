using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Engines;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStationShell
{
    [Export]
    public class LoginViewModel : BindableBase, IPartImportsSatisfiedNotification, IDataErrorInfo
    {

        [Import]
        private MarketDataService MarketDataService { get; set; }

        [Import]
        private MarketDataMulticastEngine MarketDataMulticastEngine { get; set; }

        [Import]
        private MarketDataSubscribeEngine MarketDataSubscribeEngine { get; set; }

        [Import]
        private TradeService TradeService { get; set; }

        [Import]
        public UserSettings UserSettings { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public LoginWindow TheWindow = null;

        #region Properties

        private string _operatorName;
        public string OperatorName
        {
            get { return _operatorName; }
            set
            {
                SetProperty(ref _operatorName, value);
            }
        }

        private string _operatorPassword;
        public string OperatorPassword
        {
            get { return _operatorPassword; }
            set
            {
                SetProperty(ref _operatorPassword, value);
            }
        }

        private bool _isMulticastPrice;
        public bool IsMulticastPrice
        {
            get { return _isMulticastPrice; }
            set
            {
                SetProperty(ref _isMulticastPrice, value);
            }
        }

        #region O32 related

        private string _tradeServerAddress;
        public string TradeServerAddress
        {
            get { return _tradeServerAddress; }
            set
            {
                SetProperty(ref _tradeServerAddress, value);
            }
        }

        private int _tradeServerPort;
        public int TradeServerPort
        {
            get { return _tradeServerPort; }
            set
            {
                SetProperty(ref _tradeServerPort, value);
            }
        }

        private string _subscribeServerAddress;
        public string SubscribeServerAddress
        {
            get { return _subscribeServerAddress; }
            set
            {
                SetProperty(ref _subscribeServerAddress, value);
            }
        }

        private int _subscribeServerPort;
        public int SubscribeServerPort
        {
            get { return _subscribeServerPort; }
            set
            {
                SetProperty(ref _subscribeServerPort, value);
            }
        }

        #endregion

        #region Json service Related

        private string _refServiceAddress;
        public string RefServiceAddress
        {
            get { return _refServiceAddress; }
            set
            {
                SetProperty(ref _refServiceAddress, value);
            }
        }

        private int _refServicePort;
        public int RefServicePort
        {
            get { return _refServicePort; }
            set
            {
                SetProperty(ref _refServicePort, value);
            }
        }

        private string _kLineServiceAddress;
        public string KLineServiceAddress
        {
            get { return _kLineServiceAddress; }
            set
            {
                SetProperty(ref _kLineServiceAddress, value);
            }
        }

        private int _kLineServicePort;
        public int KLineServicePort
        {
            get { return _kLineServicePort; }
            set
            {
                SetProperty(ref _kLineServicePort, value);
            }
        }

        #endregion

        #region Price Multicast Related

        private string _stockQuotMulticastAddress;
        public string StockQuotMulticastAddress
        {
            get { return _stockQuotMulticastAddress; }
            set
            {
                SetProperty(ref _stockQuotMulticastAddress, value);
            }
        }

        private int _stockQuotMulticastPort;
        public int StockQuotMulticastPort
        {
            get { return _stockQuotMulticastPort; }
            set
            {
                SetProperty(ref _stockQuotMulticastPort, value);
            }
        }

        private string _indexQuotMulticastAddress;
        public string IndexQuotMulticastAddress
        {
            get { return _indexQuotMulticastAddress; }
            set
            {
                SetProperty(ref _indexQuotMulticastAddress, value);
            }
        }

        private int _indexQuotMulticastPort;
        public int IndexQuotMulticastPort
        {
            get { return _indexQuotMulticastPort; }
            set
            {
                SetProperty(ref _indexQuotMulticastPort, value);
            }
        }

        private string _futureQuotMulticastAddress;
        public string FutureQuotMulticastAddress
        {
            get { return _futureQuotMulticastAddress; }
            set
            {
                SetProperty(ref _futureQuotMulticastAddress, value);
            }
        }

        private int _futureQuotMulticastPort;
        public int FutureQuotMulticastPort
        {
            get { return _futureQuotMulticastPort; }
            set
            {
                SetProperty(ref _futureQuotMulticastPort, value);
            }
        }

        private string _optionQuotMulticastAddress;
        public string OptionQuotMulticastAddress
        {
            get { return _optionQuotMulticastAddress; }
            set
            {
                SetProperty(ref _optionQuotMulticastAddress, value);
            }
        }

        private int _optionQuotMulticastPort;
        public int OptionQuotMulticastPort
        {
            get { return _optionQuotMulticastPort; }
            set
            {
                SetProperty(ref _optionQuotMulticastPort, value);
            }
        }

        #endregion

        #region Price Subscribe Related

        private string _stockQuotSubscribeAddress;
        public string StockQuotSubscribeAddress
        {
            get { return _stockQuotSubscribeAddress; }
            set
            {
                SetProperty(ref _stockQuotSubscribeAddress, value);
            }
        }

        private int _stockQuotSubscribePort;
        public int StockQuotSubscribePort
        {
            get { return _stockQuotSubscribePort; }
            set
            {
                SetProperty(ref _stockQuotSubscribePort, value);
            }
        }

        #endregion

        private string _errMsg;
        public string ErrMsg
        {
            get { return _errMsg; }
            set
            {
                SetProperty(ref _errMsg, value);
            }
        }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public LoginViewModel(IEventAggregator eventAggr)
        {
            EventAggregator = eventAggr;

            SubscribeEvents();

            LoginServerCommand = new DelegateCommand(OnLoginServer);
            ExitApplicationCommand = new DelegateCommand(OnExitApplication);

            ErrorMsgDic = new Dictionary<string, string>();
        }

        #endregion

        #region Commands

        public ICommand LoginServerCommand { get; private set; }
        public ICommand ExitApplicationCommand { get; private set; }

        #endregion

        #region Private Methods

        private void SubscribeEvents()
        {
            EventAggregator.GetEvent<TraderLoginStatusNotifyEvent>().Subscribe(OnTraderLogin);
        }

        private void OnTraderLogin(TraderLoginStatusEntity loginStatus)
        {
            DispatcherService.Invoke(() =>
            {
                if (TheWindow != null && !TheWindow.IsAuth && loginStatus.LoginStatus)
                {
                    // Connect TFMkdtAPI
                    if (AppConfigService.IsMulticastPrice)
                    {
                        MarketDataService.Initialize(MarketDataMulticastEngine);
                    }
                    else
                    {
                        MarketDataService.Initialize(MarketDataSubscribeEngine);
                    }

                    // Verify correctly. Close the login window.
                    TheWindow.IsAuth = true;
                    TheWindow.DialogResult = true;
                    TheWindow.Close();
                }
            });

        }

        private void OnLoginServer()
        {
            if (!this.HasError)
            {
                TheWindow.Topmost = false;
                ErrMsg = "";
                SaveSettingToConfigFile();

                if (UserSettings.IsQuotesOnly)
                {
                    EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("已登录");
                    EventAggregator.GetEvent<TraderLoginStatusNotifyEvent>().Publish(new TraderLoginStatusEntity
                    {
                        TraderID = "test123",
                        LoginStatus = true,
                        Message = "登录成功"
                    });
                }
                else
                {
                    TradeService.ConnectServer(_operatorName, _operatorPassword);
                }
            }
        }

        private void OnExitApplication()
        {
            Environment.Exit(0);
        }

        private void SaveSettingToConfigFile()
        {
            AppConfigService.OperatorName = OperatorName;
            AppConfigService.OperatorPassword = OperatorPassword;
            AppConfigService.TradeServerIp = TradeServerAddress;
            AppConfigService.TradeServerPort = TradeServerPort;
            AppConfigService.SubscribeServerIp = SubscribeServerAddress;
            AppConfigService.SubscribeServerPort = SubscribeServerPort;
            AppConfigService.KLineHttpServiceIp = KLineServiceAddress;
            AppConfigService.KLineHttpServicePort = KLineServicePort;

            AppConfigService.IsMulticastPrice = IsMulticastPrice;

            AppConfigService.RefHttpServiceIp = RefServiceAddress;
            AppConfigService.RefHttpServicePort = RefServicePort;

            AppConfigService.StockMulticastIp = StockQuotMulticastAddress;
            AppConfigService.StockMulticastPort = StockQuotMulticastPort;
            AppConfigService.FutureMulticastIp = FutureQuotMulticastAddress;
            AppConfigService.FutureMulticastPort = FutureQuotMulticastPort;
            AppConfigService.IndexMulticastIp = IndexQuotMulticastAddress;
            AppConfigService.IndexMulticastPort = IndexQuotMulticastPort;
            AppConfigService.OptionMulticastIp = OptionQuotMulticastAddress;
            AppConfigService.OptionMulticastPort = OptionQuotMulticastPort;

            AppConfigService.StockSubscribeIp = StockQuotSubscribeAddress;
            AppConfigService.StockSubscribePort = StockQuotSubscribePort;
        }

        private void LoadSettingFromConfigFile()
        {
            OperatorName = AppConfigService.OperatorName;
            OperatorPassword = AppConfigService.OperatorPassword;
            TradeServerAddress = AppConfigService.TradeServerIp;
            TradeServerPort = AppConfigService.TradeServerPort;
            SubscribeServerAddress = AppConfigService.SubscribeServerIp;
            SubscribeServerPort = AppConfigService.SubscribeServerPort;
            KLineServiceAddress = AppConfigService.KLineHttpServiceIp;
            KLineServicePort = AppConfigService.KLineHttpServicePort;

            IsMulticastPrice = AppConfigService.IsMulticastPrice;

            RefServiceAddress = AppConfigService.RefHttpServiceIp;
            RefServicePort = AppConfigService.RefHttpServicePort;

            StockQuotMulticastAddress = AppConfigService.StockMulticastIp;
            StockQuotMulticastPort = AppConfigService.StockMulticastPort;
            FutureQuotMulticastAddress = AppConfigService.FutureMulticastIp;
            FutureQuotMulticastPort = AppConfigService.FutureMulticastPort;
            IndexQuotMulticastAddress = AppConfigService.IndexMulticastIp;
            IndexQuotMulticastPort = AppConfigService.IndexMulticastPort;
            OptionQuotMulticastAddress = AppConfigService.OptionMulticastIp;
            OptionQuotMulticastPort = AppConfigService.OptionMulticastPort;

            StockQuotSubscribeAddress = AppConfigService.StockSubscribeIp;
            StockQuotSubscribePort = AppConfigService.StockSubscribePort;
        }

        #endregion

        #region Error Message Related

        public bool HasError
        {
            get
            {
                return ErrorMsgDic.Values.Any(x => !string.IsNullOrEmpty(x));
            }
        }

        public Dictionary<string, string> ErrorMsgDic { get; set; }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "OperatorName")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.OperatorName))
                    {
                        errorMsg = "必填项";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "TradeServerAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.TradeServerAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.TradeServerAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "SubscribeServerAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.SubscribeServerAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.SubscribeServerAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "RefServiceAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.RefServiceAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.RefServiceAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "KLineServiceAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.KLineServiceAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.KLineServiceAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "StockQuotMulticastAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.StockQuotMulticastAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.StockQuotMulticastAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "FutureQuotMulticastAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.FutureQuotMulticastAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.FutureQuotMulticastAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "OptionQuotMulticastAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.OptionQuotMulticastAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.OptionQuotMulticastAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "IndexQuotMulticastAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.IndexQuotMulticastAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.IndexQuotMulticastAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                if (columnName == "StockQuotSubscribeAddress")
                {
                    string errorMsg = null;

                    if (string.IsNullOrEmpty(this.StockQuotSubscribeAddress))
                    {
                        errorMsg = "必填项";
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(this.StockQuotSubscribeAddress, CommonUtil.IP_REGEX_PATTERN))
                    {
                        errorMsg = "IP地址格式错误";
                    }

                    ErrorMsgDic[columnName] = errorMsg;

                    return errorMsg;
                }

                return null;
            }
        }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            CommonUtil.LoadUserSettings(UserSettings);
            LoadSettingFromConfigFile();
        }

        #endregion
    }
}

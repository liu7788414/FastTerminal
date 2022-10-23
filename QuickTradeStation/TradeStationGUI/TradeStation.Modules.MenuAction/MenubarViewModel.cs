using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using MoonPdf;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.MenuAction;

using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace TradeStation.Modules.Trade.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MenubarViewModel : BindableBase, IPartImportsSatisfiedNotification
    {
        [Import]
        public UserSettings UserSettings { get; set; }

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public HsStock Trader { get; set; }

        [Import]
        private SaveLoadDataBufferService _saveLoadDataBufferHelper = null;

        [Import]
        private SecurityInfoMetadata SecurityInfoMetadata { get; set; }

        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public MenubarViewModel(IEventAggregator eventAgg)
        {
            _eventAggregator = eventAgg;
            InitCommands();
            SubscribeEvents();
        }

        private void InitCommands()
        {
            UserSettingsCommand = new DelegateCommand(OnUserSettings);
            AboutCommand = new DelegateCommand(OnAbout);
            HelpDocCommand = new DelegateCommand(OnHelpDoc);
            QryHistoricalEntrustCommand = new DelegateCommand(OnQryHistoricalEntrust);
            QryHistoricalTradeResultCommand = new DelegateCommand(OnQryHistoricalTradeResult);
            DeleteHistoricalLogsCommand = new DelegateCommand(OnDeleteHistoricalLogs);
            ClearSecurityInfoBufferCommand = new DelegateCommand(OnClearSecurityInfoBuffer);
        }

        private void OnUserSettings()
        {
            var win = new WinUserSettings(UserSettings);
            win.ShowDialog();
        }

        private void OnAbout()
        {
            var win = new WinAbout();
            win.ShowDialog();
        }

        private void OnHelpDoc()
        {
            string s = CommonUtil.AssemblyPath + "天风证券傲速交易终端使用手册.pdf";
            var win = new PdfWindow(s);
            win.ShowDialog();
        }

        private void OnDeleteHistoricalLogs()
        {
            if (
                MessageBox.Show("确定要删除以往的日志文件吗？(不包含今日)", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning,
                    MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Logger.Info("删除以往的日志...");
                string[] files = Directory.GetFiles(CommonUtil.AssemblyPath + "logs", "*txt*");

                string today = DateTime.Now.ToString("yyyy-MM-dd");

                foreach (var s1 in files.Where(s => !s.Contains(today)))
                {
                    try
                    {
                        File.Delete(s1);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + ex.Source + ex.StackTrace);
                    }
                }
            }
        }

        private void OnQryHistoricalEntrust()
        {
            _eventAggregator.GetEvent<QryHistoricalEntrustNotifyEvent>().Publish("");
        }

        private void OnQryHistoricalTradeResult()
        {
            _eventAggregator.GetEvent<QryHistoricalTradeResultNotifyEvent>().Publish("");
        }

        private void OnClearSecurityInfoBuffer()
        {
            if (MessageBox.Show("确定要清除所有的证券信息缓存吗？清除后重启终端将重新读取这些信息。(包含证券代码表、复权因子信息、证券交易时段信息)", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Logger.Info("删除证券信息缓存...");

                _saveLoadDataBufferHelper.ClearAllBufferFile();
            }
        }

        public ICommand UserSettingsCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand HelpDocCommand { get; set; }
        public ICommand QryHistoricalEntrustCommand { get; set; }

        public ICommand QryHistoricalTradeResultCommand { get; set; }

        public ICommand DeleteHistoricalLogsCommand { get; set; }
        public ICommand ClearSecurityInfoBufferCommand { get; set; }

        public void OnImportsSatisfied()
        {
            _stockMarkets.Add(eMarketType.上交所);
            _stockMarkets.Add(eMarketType.深交所);

            _futureMarkets.Add(eMarketType.上期所);
            _futureMarkets.Add(eMarketType.郑商所);
            _futureMarkets.Add(eMarketType.中金所);
            _futureMarkets.Add(eMarketType.大商所);

            _optionMarkets.Add(eMarketType.上交所);
            _optionMarkets.Add(eMarketType.深交所);
            _optionMarkets.Add(eMarketType.大商所);
            _optionMarkets.Add(eMarketType.郑商所);

            _stockEntrustDirection.Add(eEntrustDirection.买入);
            _stockEntrustDirection.Add(eEntrustDirection.卖出);

            _futureEntrustDirection.Add(eEntrustDirection.买入);
            _futureEntrustDirection.Add(eEntrustDirection.卖出);

            _optionEntrustDirection.Add(eEntrustDirection.买入);
            _optionEntrustDirection.Add(eEntrustDirection.卖出);

            _basketEntrustDirection.Add(eEntrustDirection.买入);
            _basketEntrustDirection.Add(eEntrustDirection.卖出);

            _fundEntrustDirectionEtf.Add(eEntrustDirection.ETF申购);
            _fundEntrustDirectionEtf.Add(eEntrustDirection.ETF赎回);
            _fundEntrustDirection.Add(eEntrustDirection.基金分拆);
            _fundEntrustDirection.Add(eEntrustDirection.基金合并);
            _fundEntrustDirection.Add(eEntrustDirection.基金认购);

            _bondsEntrustDirection.Add(eEntrustDirection.融资回购);
            _bondsEntrustDirection.Add(eEntrustDirection.融券回购);

            OptionTargets.Add("50ETF");
            OptionTargets.Add("180ETF");
            OptionTargets.Add("300ETF");

            SelectedOptionTarget = "50ETF";
        }

        private void QryAllInfoForOneCombiNo(string combiNo)
        {
            Trader.QryAccountMoney(combiNo);
            Trader.QryFutureMargin(combiNo);
            Trader.QryOptionMargin(combiNo, CommonUtil.market_no_上交所);

            Trader.QryHoldingPosition(combiNo);
            Trader.QryHoldingPosition(combiNo, eCategory.期货);
            Trader.QryHoldingPosition(combiNo, eCategory.期权);

            Trader.QryTradeResult(combiNo);
            Trader.QryTradeResult(combiNo, eCategory.期货);
            Trader.QryTradeResult(combiNo, eCategory.期权);
            Trader.QryTradeResult(combiNo, eCategory.基金分拆合并);

            Trader.QryEntrust(combiNo);
            Trader.QryEntrust(combiNo, eCategory.期货);
            Trader.QryEntrust(combiNo, eCategory.期权);
            Trader.QryEntrust(combiNo, eCategory.基金分拆合并);
        }

        private List<eMarketType> _stockMarkets = new List<eMarketType>();
        public List<eMarketType> StockMarkets
        {
            get { return _stockMarkets; }
            set
            {
                SetProperty(ref _stockMarkets, value);
            }
        }

        private List<eMarketType> _futureMarkets = new List<eMarketType>();
        public List<eMarketType> FutureMarkets
        {
            get { return _futureMarkets; }
            set
            {
                SetProperty(ref _futureMarkets, value);
            }
        }

        private List<eMarketType> _optionMarkets = new List<eMarketType>();
        public List<eMarketType> OptionMarkets
        {
            get { return _optionMarkets; }
            set
            {
                SetProperty(ref _optionMarkets, value);
            }
        }

        private List<eEntrustDirection> _stockEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> StockEntrustDirection
        {
            get { return _stockEntrustDirection; }
            set
            {
                SetProperty(ref _stockEntrustDirection, value);
            }
        }

        private List<eEntrustDirection> _futureEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> FutureEntrustDirection
        {
            get { return _futureEntrustDirection; }
            set
            {
                SetProperty(ref _futureEntrustDirection, value);
            }
        }

        private List<eEntrustDirection> _optionEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> OptionEntrustDirection
        {
            get { return _optionEntrustDirection; }
            set
            {
                SetProperty(ref _optionEntrustDirection, value);
            }
        }

        private List<eEntrustDirection> _fundEntrustDirectionEtf = new List<eEntrustDirection>();
        public List<eEntrustDirection> FundEntrustDirectionEtf
        {
            get { return _fundEntrustDirectionEtf; }
            set
            {
                SetProperty(ref _fundEntrustDirectionEtf, value);
            }
        }

        private List<eEntrustDirection> _fundEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> FundEntrustDirection
        {
            get { return _fundEntrustDirection; }
            set
            {
                SetProperty(ref _fundEntrustDirection, value);
            }
        }

        private List<eEntrustDirection> _bondsEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> BondsEntrustDirection
        {
            get { return _bondsEntrustDirection; }
            set
            {
                SetProperty(ref _bondsEntrustDirection, value);
            }
        }

        private List<eEntrustDirection> _basketEntrustDirection = new List<eEntrustDirection>();
        public List<eEntrustDirection> BasketEntrustDirection
        {
            get { return _basketEntrustDirection; }
            set
            {
                SetProperty(ref _basketEntrustDirection, value);
            }
        }

        private List<string> _optionTargets = new List<string>();
        public List<string> OptionTargets
        {
            get { return _optionTargets; }
            set
            {
                SetProperty(ref _optionTargets, value);
            }
        }

        private string _selectedOptionTarget;
        public string SelectedOptionTarget
        {
            get { return _selectedOptionTarget; }
            set
            {
                SetProperty(ref _selectedOptionTarget, value);
            }
        }

        private List<string> _combiNos = new List<string>();
        public List<string> CombiNos
        {
            get { return _combiNos; }
            set
            {
                SetProperty(ref _combiNos, value);
            }
        }

        private string _selectedCombiNo;
        public string SelectedCombiNo
        {
            get { return _selectedCombiNo; }
            set
            {
                SetProperty(ref _selectedCombiNo, value);
            }
        }

        private void SubscribeEvents()
        {
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<CombiNoInfoArrivedNotifyEvent>().Subscribe(OnCombiNoInfoArrived, ThreadOption.UIThread);
            }
        }

        private void OnCombiNoInfoArrived(InfoWithCombiNoAndSecurityInfo combiNoInfo)
        {
            Logger.Debug("onCombiNoInfoArrived");
            DispatcherService.Invoke(() =>
            {
                if (!_combiNos.Contains(combiNoInfo.CombiNo))
                {
                    _combiNos.Add(combiNoInfo.CombiNo);

                    QryAllInfoForOneCombiNo(combiNoInfo.CombiNo);
                }
            });
        }
    }
}

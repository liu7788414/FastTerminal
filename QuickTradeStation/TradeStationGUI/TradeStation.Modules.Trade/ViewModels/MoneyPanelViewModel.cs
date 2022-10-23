using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;

namespace TradeStation.Modules.Trade.ViewModels
{

    public abstract class MoneyPanelViewModelBase : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        protected MoneyPanelViewModelBase(IEventAggregator eventAggr,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }

        private ObservableCollection<MoneyInfoBase> _moneyInfoList = new ObservableCollection<MoneyInfoBase>();
        public ObservableCollection<MoneyInfoBase> MoneyInfoList
        {
            get { return _moneyInfoList; }
            set
            {
                SetProperty(ref _moneyInfoList, value);
            }
        }

        protected void OnReturnMoneyInfo(MoneyInfoBase moneyInfo)
        {
            Logger.Debug("onReturnMoneyInfo");

            DispatcherService.Invoke(() =>
            {
                switch (moneyInfo.MsgType)
                {
                    case eMoneyInfoMsgType.查询:
                        {
                            _moneyInfoList.Add(moneyInfo);
                            break;
                        }
                    case eMoneyInfoMsgType.清空:
                        {
                            _moneyInfoList.Clear();
                            break;
                        }
                }

            });
        }

        public void DailyReInitialize()
        {
            DispatcherService.Invoke(() =>
            {
                OnRefresh();
            });
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MoneyPanelViewModel : MoneyPanelViewModelBase
    {
        [ImportingConstructor]
        public MoneyPanelViewModel(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<MoneyInfoNotifyEvent>().Subscribe(OnReturnMoneyInfo);
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("QryMoneyInfo");
            MoneyInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryAccountMoney(combiNo);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出股票资金列表...", MoneyInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MoneyPanelViewModelFuture : MoneyPanelViewModelBase
    {

        [ImportingConstructor]
        public MoneyPanelViewModelFuture(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FutureMarginInfoNotifyEvent>().Subscribe(OnReturnMoneyInfo);
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("QryMoneyInfo");
            MoneyInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryFutureMargin(combiNo);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期货保证金列表...", MoneyInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MoneyPanelViewModelOption : MoneyPanelViewModelBase
    {

        [ImportingConstructor]
        public MoneyPanelViewModelOption(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }


        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionMarginInfoNotifyEvent>().Subscribe(OnReturnMoneyInfo);
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("QryMoneyInfo");
            MoneyInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryOptionMargin(combiNo, CommonUtil.market_no_上交所);
                    //trader.QryOptionMargin(combiNo, CommonUtil.market_no_深交所);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期权保证金列表...", MoneyInfoList);
        }
    }
}

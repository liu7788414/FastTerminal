using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.ServiceLocation;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.Views;

namespace TradeStation.Modules.Trade.ViewModels
{
    public abstract class TradeResultPanelViewModelBase : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelBase(IEventAggregator eventAggr,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }
        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
            AdvancedQueryCommand = new DelegateCommand(OnAdvancedQuery);
        }

        protected void OnReturnTradeResult(ObservableCollection<TradeResultInfo> tradeResultInfoList, TradeResultInfo tradeResultInfo)
        {
            DispatcherService.Invoke(() =>
            {
                switch (tradeResultInfo.MsgType)
                {
                    case eTradeResultInfoMsgType.查询:
                        {
                            tradeResultInfoList.Add(tradeResultInfo);
                            break;
                        }
                    case eTradeResultInfoMsgType.清空:
                        {
                            tradeResultInfoList.Clear();
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
    public class TradeResultPanelViewModel : TradeResultPanelViewModelBase
    {
        [ImportingConstructor]
        public TradeResultPanelViewModel(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }


        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<TradeResultNotifyEvent>().Subscribe(OnReturnStockTradeResult);
        }

        private void OnReturnStockTradeResult(TradeResultInfo tradeResultInfo)
        {
            Logger.Debug(string.Format("OnReturnStockTradeResult: {0}", tradeResultInfo));
            OnReturnTradeResult(Trader.TradeResultInfoCollection.StockTradeResultInfoList, tradeResultInfo);
        }

        protected override void OnAdvancedQuery()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");
            Trader.TradeResultInfoCollection.StockTradeResultInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryTradeResult(combiNo);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出股票成交列表...", Trader.TradeResultInfoCollection.StockTradeResultInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeResultPanelViewModelFuture : TradeResultPanelViewModelBase
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelFuture(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FutureTradeResultNotifyEvent>().Subscribe(OnReturnFutureTradeResult);
        }

        private void OnReturnFutureTradeResult(FutureTradeResultInfo tradeResultInfo)
        {
            Logger.Debug(string.Format("OnReturnFutureTradeResult: {0}", tradeResultInfo));
            OnReturnTradeResult(Trader.TradeResultInfoCollection.FutureTradeResultInfoList, tradeResultInfo);
        }

        protected override void OnAdvancedQuery()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");
            Trader.TradeResultInfoCollection.FutureTradeResultInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryTradeResult(combiNo, eCategory.期货);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期货成交列表...", Trader.TradeResultInfoCollection.FutureTradeResultInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeResultPanelViewModelOption : TradeResultPanelViewModelBase
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelOption(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionTradeResultNotifyEvent>().Subscribe(OnReturnOptionTradeResult);
        }

        private void OnReturnOptionTradeResult(OptionTradeResultInfo tradeResultInfo)
        {
            Logger.Debug(string.Format("OnReturnOptionTradeResult：{0}", tradeResultInfo));
            OnReturnTradeResult(Trader.TradeResultInfoCollection.OptionTradeResultInfoList, tradeResultInfo);
        }

        protected override void OnAdvancedQuery()
        {
            var win = ServiceLocator.Current.GetInstance<AdvancedQueryPanelOptionTradeResult>();
            win.Show();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");
            Trader.TradeResultInfoCollection.OptionTradeResultInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryTradeResult(combiNo, eCategory.期权);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期权成交列表...", Trader.TradeResultInfoCollection.OptionTradeResultInfoList);
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeResultPanelViewModelFund : TradeResultPanelViewModelBase
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelFund(IEventAggregator eventAggr,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {

        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FundTradeResultNotifyEvent>().Subscribe(OnReturnFundTradeResult);
        }

        private void OnReturnFundTradeResult(FundTradeResultInfo tradeResultInfo)
        {
            Logger.Debug(string.Format("OnReturnFundTradeResult: {0}", tradeResultInfo));
            OnReturnTradeResult(Trader.TradeResultInfoCollection.FundTradeResultInfoList, tradeResultInfo);
        }

        protected override void OnAdvancedQuery()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");
            Trader.TradeResultInfoCollection.FundTradeResultInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryTradeResult(combiNo, eCategory.基金分拆合并);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出基金特殊业务成交列表...", Trader.TradeResultInfoCollection.FundTradeResultInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeResultPanelViewModelForBasket : TradeResultPanelViewModel
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelForBasket(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            SubscribeEvents();
        }


        protected sealed override void SubscribeEvents()
        { }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeResultPanelViewModelFutureForBasket : TradeResultPanelViewModelFuture
    {
        [ImportingConstructor]
        public TradeResultPanelViewModelFutureForBasket(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            SubscribeEvents();
        }


        protected sealed override void SubscribeEvents()
        { }
    }
}

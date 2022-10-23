using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;

namespace TradeStation.Modules.Trade.ViewModels
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InstrumentPanelViewModel : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        public InstrumentPanelViewModel(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }

        protected override void SubscribeEvents()
        {
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
            Trader.InstrumentInfoCollection.InstrumentInfoList.Clear();
            Trader.QryInstrument();
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出合约信息列表...", Trader.InstrumentInfoCollection.InstrumentInfoList);
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
    public class EtfBaseInfoPanelViewModel : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        public EtfBaseInfoPanelViewModel(IEventAggregator eventAggr,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }

        private EtfBaseInfo _selectedEtfBaseInfo;

        public EtfBaseInfo SelectedEtfBaseInfo
        {
            get { return _selectedEtfBaseInfo; }
            set { SetProperty(ref _selectedEtfBaseInfo, value); }
        }


        public ICommand FillEtfInfoCommand { get; set; }

        protected override void SubscribeEvents()
        {
        }

        protected override void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
            FillEtfInfoCommand = new DelegateCommand(OnFillEtfInfo);
        }


        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }
        protected void OnFillEtfInfo()
        {
            EventAggregator.GetEvent<EtfBaseInfoNotifyEvent>().Publish(SelectedEtfBaseInfo);
        }

        protected override void OnRefresh()
        {
            Trader.EtfBaseInfoCollection.EtfBaseInfoList.Clear();
            Trader.QryEtfBaseInfo();
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出ETF基础信息列表...", Trader.EtfBaseInfoCollection.EtfBaseInfoList);
        }

        public void DailyReInitialize()
        {
            DispatcherService.Invoke(() =>
            {
                OnRefresh();
            });
        }
    }
}

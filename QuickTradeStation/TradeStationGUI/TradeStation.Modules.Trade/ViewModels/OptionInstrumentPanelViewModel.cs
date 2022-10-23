using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.ComponentModel.Composition;
using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;

namespace TradeStation.Modules.Trade.ViewModels
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OptionInstrumentPanelViewModel : TradeViewModelBase, IReInitializable
    {
        private SecurityRefHttpService _securityRefHttpService;
        public SecurityInfoMetadata SecurityInfoMetadata { get; set; }

        [ImportingConstructor]
        public OptionInstrumentPanelViewModel(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider,
            SecurityInfoMetadata securityInfoMetadata,
            SecurityRefHttpService securityRefHttpService)
            : base(eventAggr, securitySearchProvider)
        {
            SecurityInfoMetadata = securityInfoMetadata;
            _securityRefHttpService = securityRefHttpService;

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
            SecurityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Clear();
            _securityRefHttpService.GetOptionInformation();
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期权合约信息列表...", SecurityInfoMetadata.OptionInfoModelCollection.OptionInfoList);
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

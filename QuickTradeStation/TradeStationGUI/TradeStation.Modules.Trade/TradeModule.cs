using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;

using TradeStation.Infrastructure.Models;
using TradeStation.Modules.Trade.Views;

namespace TradeStation.Modules.Trade
{
    [ModuleExport(typeof(TradeModule))]
    public class TradeModule : IModule
    {
        private IEventAggregator _eventAggregator;
        private IServiceLocator _serviceLocator;

        [ImportingConstructor]
        public TradeModule(IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
        {
            _eventAggregator = eventAggregator;
            _serviceLocator = serviceLocator;
        }

        public void Initialize()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<QryHistoricalEntrustNotifyEvent>().Subscribe(OnQryHistoricalEntrust);
            _eventAggregator.GetEvent<QryHistoricalTradeResultNotifyEvent>().Subscribe(OnQryHistoricalTradeResult);
        }

        private void OnQryHistoricalEntrust(string loginStatus)
        {
            var p = _serviceLocator.GetInstance<QryPanel>();
            p.Show();
        }

        private void OnQryHistoricalTradeResult(string loginStatus)
        {
            var p = _serviceLocator.GetInstance<QryHistoricalTradeResultPanel>();
            p.Show();
        }
    }
}

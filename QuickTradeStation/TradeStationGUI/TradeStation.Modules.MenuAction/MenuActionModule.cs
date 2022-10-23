using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Infrastructure.Services;

namespace TradeStation.Modules.MenuAction
{
    [ModuleExport(typeof(MenuActionModule))]
    public class MenuActionModule : IModule
    {
        [Import]
        public HsStock Trader { get; set; }

        public void Initialize()
        {
            Trader.QryAccount();
            Trader.QryCombiNo();
            Trader.QryInstrument();
            Trader.QryEtfBaseInfo();
        }
    }
}

using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Infrastructure.Managers;

namespace TradeStation.Modules.RealTimePrice
{
    [ModuleExport(typeof(RealTimePriceModule))]
    public class RealTimePriceModule : IModule
    {
        public void Initialize()
        {
        }
    }
}

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;

namespace TradeStation.Modules.LogMessage
{
    [ModuleExport(typeof(LogMessageModule))]
    public class LogMessageModule : IModule
    {
        public void Initialize()
        {
        }
    }
}

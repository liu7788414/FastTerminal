using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Engines;
using TradeStation.Infrastructure.Services;
using TradeStation.Infrastructure.Managers;

namespace TradeStation.Infrastructure
{
    [ModuleExport(CommonUtil.INFRASTRUCTURE_MODULE_NAME, typeof(InfrastructureModule))]
    public class InfrastructureModule : IModule
    {
        private ReInitializationEngine _reInitializableList;
        private SecurityRefHttpService _securityRefHttpService;

        [ImportingConstructor]
        public InfrastructureModule(SecurityRefHttpService securityRefHttpService,
            ReInitializationEngine reInitializableList)
        {
            _securityRefHttpService = securityRefHttpService;
            _reInitializableList = reInitializableList;
        }

        public void Initialize()
        {
            _securityRefHttpService.GetInitData();
            _reInitializableList.Initialize();
        }
    }
}

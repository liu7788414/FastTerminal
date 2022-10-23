using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Future.Controllers;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Managers;

namespace TradeStation.Future
{
    [ModuleExport(typeof(FutureModule), DependsOnModuleNames = new string[] { CommonUtil.INFRASTRUCTURE_MODULE_NAME })]
    public class FutureModule : IModule
    {
        private FutureQuotPanelController _futureQuotPanelController;

        [ImportingConstructor]
        public FutureModule(FutureQuotPanelController futureRealTimeController)
        {
            _futureQuotPanelController = futureRealTimeController;
        }

        public void Initialize()
        {
            TFSplashScreenManager.Instance.Message = "正在初始化期货模块...";

            _futureQuotPanelController.Initialize();
        }
    }
}

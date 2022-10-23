using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Managers;
using TradeStation.Option.Controllers;

namespace TradeStation.Option
{
    [ModuleExport(typeof(OptionModule), DependsOnModuleNames = new string[] { CommonUtil.INFRASTRUCTURE_MODULE_NAME })]
    public class OptionModule : IModule
    {
        private OptionQuotPanelController _optionQuotPanelController;

        [ImportingConstructor]
        public OptionModule(OptionQuotPanelController optionRealTimePriceController)
        {
            _optionQuotPanelController = optionRealTimePriceController;
        }

        public void Initialize()
        {
            TFSplashScreenManager.Instance.Message = "正在初始化期权模块...";

            _optionQuotPanelController.Initialize();
        }
    }
}

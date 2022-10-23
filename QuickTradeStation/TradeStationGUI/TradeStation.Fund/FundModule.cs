using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Fund.Controllers;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Managers;

namespace TradeStation.Fund
{
    [ModuleExport(typeof(FundModule), DependsOnModuleNames = new[] { CommonUtil.INFRASTRUCTURE_MODULE_NAME })]
    public class FundModule : IModule
    {
        [Import]
        public FundRealTimePriceController FundRealTimePriceController;

        public void Initialize()
        {
            TFSplashScreenManager.Instance.Message = "正在初始化基金模块...";
            FundRealTimePriceController.Initialize();
        }
    }
}

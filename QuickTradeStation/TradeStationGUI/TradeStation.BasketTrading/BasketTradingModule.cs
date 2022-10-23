using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.BasketTrading.Controllers;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Managers;

namespace TradeStation.BasketTrading
{
    [ModuleExport(typeof(BasketTradingModule), DependsOnModuleNames = new string[] { CommonUtil.INFRASTRUCTURE_MODULE_NAME })]
    public class BasketTradingModule : IModule
    {
        [Import]
        public BasketTradingController _basketTradingController;

        public void Initialize()
        {
            TFSplashScreenManager.Instance.Message = "正在初始化篮子交易模块...";

            _basketTradingController.Initialize();
        }
    }
}

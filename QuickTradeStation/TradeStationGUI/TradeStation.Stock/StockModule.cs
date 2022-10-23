using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Managers;
using TradeStation.Stock.Controllers;

namespace TradeStation.Stock
{
    [ModuleExport(typeof(StockModule), DependsOnModuleNames = new string[] { CommonUtil.INFRASTRUCTURE_MODULE_NAME })]
    public class StockModule : IModule
    {
        private StockQuotPanelController _stockQuotPanelController;

        [ImportingConstructor]
        public StockModule(StockQuotPanelController realTimePriceController)
        {
            _stockQuotPanelController = realTimePriceController;
        }

        public void Initialize()
        {
            TFSplashScreenManager.Instance.Message = "正在初始化股票模块...";

            _stockQuotPanelController.Initialize();
        }

        // TODO: Implement this method.
        public void LoadDataTemplate()
        {

        }
    }
}

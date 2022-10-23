using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Controllers;
using TradeStation.Future.ViewModels;

namespace TradeStation.BasketTrading.Controllers
{
    [Export]
    public class BasketTradingController : ControllerBase
    {
        public BasketTradingMainViewModel BasketTradingMainViewModel { get; set; }

        [ImportingConstructor]
        public BasketTradingController(BasketTradingMainViewModel basketTradingMainViewModel)
        {
            BasketTradingMainViewModel = basketTradingMainViewModel;
        }

        public override void Initialize()
        {
            base.Initialize();

            AttachView(BasketTradingMainViewModel.View, RegionNames.MainTabPanelRegion);
        }

        protected override void SubscribeEvents()
        {
        }

        protected override void InitializeCommands()
        {
        }
    }
}

using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Regions;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Controllers;
using TradeStation.Future.ViewModels;

namespace TradeStation.Fund.Controllers
{
    [Export]
    public class FundRealTimePriceController : ControllerBase
    {
        public FundMainViewModel FundMainViewModel { get; set; }

        [ImportingConstructor]
        public FundRealTimePriceController(FundMainViewModel fundMainViewModel,
            IRegionManager regionManager)
        {
            FundMainViewModel = fundMainViewModel;
        }

        public override void Initialize()
        {
            base.Initialize();

            AttachView(FundMainViewModel.View, RegionNames.MainTabPanelRegion);
        }

        protected override void SubscribeEvents()
        {
        }

        protected override void InitializeCommands()
        {
        }
    }
}

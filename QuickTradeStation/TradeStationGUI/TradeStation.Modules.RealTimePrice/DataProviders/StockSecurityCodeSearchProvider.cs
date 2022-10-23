using System.ComponentModel.Composition;

using TradeStation.Infrastructure;

namespace TradeStation.Modules.RealTimePrice.DataProviders
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockSecurityCodeSearchProvider : SecurityCodeSearchProvider
    {
        public override void SetFilteredTypes()
        {
            SetFilteredTypes(true, true, false, false, true, true);
        }
    }
}

using System.ComponentModel.Composition;

using TradeStation.Infrastructure;

namespace TradeStation.Modules.RealTimePrice.DataProviders
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FundSecurityCodeSearchProvider : SecurityCodeSearchProvider
    {
        public override void SetFilteredTypes()
        {
            SetFilteredTypes(false, true, false, false);
        }
    }
}

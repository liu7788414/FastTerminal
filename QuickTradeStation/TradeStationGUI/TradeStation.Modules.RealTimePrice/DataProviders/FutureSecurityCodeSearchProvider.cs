using System.ComponentModel.Composition;

using TradeStation.Infrastructure;

namespace TradeStation.Modules.RealTimePrice.DataProviders
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FutureSecurityCodeSearchProvider : SecurityCodeSearchProvider
    {
        public override void SetFilteredTypes()
        {
            SetFilteredTypes(false, false, true, false);
        }
    }
}

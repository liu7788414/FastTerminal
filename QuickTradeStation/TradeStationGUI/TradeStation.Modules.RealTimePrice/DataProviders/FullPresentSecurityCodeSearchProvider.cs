using System.ComponentModel.Composition;

using TradeStation.Infrastructure;

namespace TradeStation.Modules.RealTimePrice.DataProviders
{
    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FullPresentSecurityCodeSearchProvider : SecurityCodeSearchProvider
    {
        public override void SetFilteredTypes()
        {
            SetFilteredTypes(true, true, true, true, true, true, false);
        }
    }
}

using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.ViewModels;
using TradeStation.Stock.Views;

namespace TradeStation.Stock.ViewModels
{
    [Export]
    public class StockMainViewModel : ViewModelBase<StockMainView>
    {
        public UserSettings UserSettings { get; set; }

        [ImportingConstructor]
        public StockMainViewModel(UserSettings userSettings,
            StockMainView view)
            : base(view)
        {
            UserSettings = userSettings;

            View = view;
            View.DataContext = this;
        }
    }
}

using System.ComponentModel.Composition;

using TradeStation.BasketTrading.Views;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Future.ViewModels
{
    [Export]
    public class BasketTradingMainViewModel : ViewModelBase<BasketTradingMainView>
    {
        public UserSettings UserSettings { get; set; }

        [ImportingConstructor]
        public BasketTradingMainViewModel(UserSettings userSettings,
            BasketTradingMainView view)
            : base(view)
        {
            UserSettings = userSettings;

            View = view;
            View.DataContext = this;
        }
    }
}

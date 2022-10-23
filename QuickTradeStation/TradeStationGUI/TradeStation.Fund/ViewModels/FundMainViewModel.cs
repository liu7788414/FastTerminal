using System.ComponentModel.Composition;

using TradeStation.Fund.Views;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Future.ViewModels
{
    [Export]
    public class FundMainViewModel : ViewModelBase<FundMainView>
    {
        public UserSettings UserSettings { get; set; }

        [ImportingConstructor]
        public FundMainViewModel(UserSettings userSettings,
            FundMainView view)
            : base(view)
        {
            UserSettings = userSettings;

            View = view;
            View.DataContext = this;
        }
    }
}

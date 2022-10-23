using System.ComponentModel.Composition;

using TradeStation.Future.Views;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Future.ViewModels
{
    [Export]
    public class FutureMainViewModel : ViewModelBase<FutureMainView>
    {
        public UserSettings UserSettings { get; set; }

        [ImportingConstructor]
        public FutureMainViewModel(UserSettings userSettings,
            FutureMainView view)
            : base(view)
        {
            UserSettings = userSettings;

            View = view;
            View.DataContext = this;
        }
    }
}

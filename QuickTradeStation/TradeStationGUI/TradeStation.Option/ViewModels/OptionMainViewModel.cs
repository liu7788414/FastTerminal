using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.ViewModels;
using TradeStation.Option.Views;

namespace TradeStation.Future.ViewModels
{
    [Export]
    public class OptionMainViewModel : ViewModelBase<OptionMainView>
    {
        public UserSettings UserSettings { get; set; }

        [ImportingConstructor]
        public OptionMainViewModel(UserSettings userSettings,
            OptionMainView view)
            : base(view)
        {
            UserSettings = userSettings;

            View = view;
            View.DataContext = this;
        }
    }
}

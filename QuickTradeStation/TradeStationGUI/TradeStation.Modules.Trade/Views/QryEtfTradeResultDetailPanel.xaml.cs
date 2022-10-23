using System.ComponentModel.Composition;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [ViewExport(RegionName = RegionNames.EtfTradeResultDetailPanelRegion)]
    public partial class QryEtfTradeResultDetailPanel
    {
        public QryEtfTradeResultDetailPanel()
        {
            InitializeComponent();
        }

        [Import]
        public QryEtfTradeResultDetailViewModel Model
        {
            get
            {
                return DataContext as QryEtfTradeResultDetailViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}

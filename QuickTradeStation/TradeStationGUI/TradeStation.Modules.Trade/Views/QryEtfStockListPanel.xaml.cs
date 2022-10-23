using System.ComponentModel.Composition;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [ViewExport(RegionName = RegionNames.EtfStockListPanelRegion)]
    public partial class QryEtfStockListPanel
    {
        public QryEtfStockListPanel()
        {
            InitializeComponent();
        }

        [Import]
        public QryEtfStockListViewModel Model
        {
            get
            {
                return DataContext as QryEtfStockListViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}

using System.ComponentModel.Composition;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [ViewExport(RegionName = RegionNames.EtfEntrustDetailPanelRegion)]
    public partial class QryEtfEntrustDetailPanel
    {
        public QryEtfEntrustDetailPanel()
        {
            InitializeComponent();
        }

        [Import]
        public QryEtfEntrustDetailViewModel Model
        {
            get
            {
                return DataContext as QryEtfEntrustDetailViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}

using System.ComponentModel.Composition;

using Infragistics.Windows.DockManager;

namespace TradeStation.Stock.Views
{
    /// <summary>
    /// StockQuotPanel.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class StockQuotPanel : ContentPane
    {
        public StockQuotPanel()
        {
            InitializeComponent();
        }
    }
}

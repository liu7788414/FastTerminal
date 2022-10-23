using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;

using Infragistics.Controls;
using Infragistics.Controls.Charts;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;

namespace TradeStation.Modules.RealTimePrice.Views
{
    /// <summary>
    /// RealTimePriceDiagramPanel.xaml 的交互逻辑
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RealTimePriceDiagramPanel : ContentPane
    {
        public RealTimePriceDiagramPanel()
        {
            InitializeComponent();

            this.KLineDataChart.HorizontalZoombar.Range = new Range() { Maximum = 1, Minimum = 0.7 };
        }

        private void ContentPane_OptionsMenuOpening(object sender, PaneOptionsMenuOpeningEventArgs e)
        {
            e.Items.Clear();
        }

        private void QuickEntrustAmountButton_Click(object sender, RoutedEventArgs e)
        {
            this.EntrustAmountInput.Focus();
        }
    }
}

using System.ComponentModel.Composition;

using Infragistics.Windows.DockManager;

namespace TradeStation.Option.ViewModels
{
    /// <summary>
    /// OptionQuotPanel.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class OptionQuotPanel : ContentPane
    {
        public OptionQuotPanel()
        {
            InitializeComponent();
        }
    }
}

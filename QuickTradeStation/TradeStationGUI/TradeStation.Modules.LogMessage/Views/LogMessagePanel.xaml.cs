using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.LogMessage.ViewModels;
using System.Timers;
using Infragistics.Controls.Grids;

namespace TradeStation.Modules.LogMessage.Views
{
    /// <summary>
    /// LogMessagePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.LogMessageRegion)]
    public partial class LogMessagePanel
    {
        public LogMessagePanel()
        {
            InitializeComponent();

            var timer = new Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (cbAutoScroll.IsChecked == null || !cbAutoScroll.IsChecked.Value) return;
                    var rc = logList.Rows;

                    if (rc.Count <= 0) return;
                    var r = logList.Rows[rc.Count - 1];
                    var cc = r.Cells;
                    var c = cc[0];
                    logList.ScrollCellIntoView(c);
                });
            }
            catch (System.Exception)
            {
                // ignored
            }
        }

        [Import]
        public LogMessageViewModel Model
        {
            get
            {
                return DataContext as LogMessageViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void XamGrid_RowAdded(object sender, RowEventArgs e)
        {
            logList.ScrollCellIntoView(e.Row.Cells["日志"]);
        }
    }
}

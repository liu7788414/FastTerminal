using Infragistics.Documents.RichText;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.MenuAction
{
    /// <summary>
    /// winUserSettings.xaml 的交互逻辑
    /// </summary>
    public partial class winHelpDoc : Window
    {
        public winHelpDoc()
        {
            InitializeComponent();

            string s = CommonUtil.AssemblyPath + "天风证券快速交易终端使用手册.pdf";

            try
            {
                pdfViewer.LoadFile(s);
            }
            catch (Exception ex)
            {

            }
        }
    }
}

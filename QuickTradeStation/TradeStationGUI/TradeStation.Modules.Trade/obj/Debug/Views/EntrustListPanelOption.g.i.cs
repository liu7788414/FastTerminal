#pragma checksum "..\..\..\Views\EntrustListPanelOption.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EC1C2FF307FF60F2F82834E028D85B44"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using FeserWard.Controls;
using Infragistics;
using Infragistics.Controls;
using Infragistics.Controls.Charts;
using Infragistics.Controls.Editors;
using Infragistics.Controls.Grids;
using Infragistics.Controls.Interactions;
using Infragistics.Themes;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TradeStation.Infrastructure.Converters;
using TradeStation.Modules.Trade.Views;


namespace TradeStation.Modules.Trade.Views {
    
    
    /// <summary>
    /// EntrustListPanelOption
    /// </summary>
    public partial class EntrustListPanelOption : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Infragistics.Controls.Grids.XamGrid xamGrid;
        
        #line default
        #line hidden
        
        
        #line 172 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbTraded;
        
        #line default
        #line hidden
        
        
        #line 179 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbCancelled;
        
        #line default
        #line hidden
        
        
        #line 186 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbObsolete;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbFilter;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\..\Views\EntrustListPanelOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FeserWard.Controls.Intellibox intellibox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TradeStation.Modules.Trade;component/views/entrustlistpaneloption.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\EntrustListPanelOption.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.xamGrid = ((Infragistics.Controls.Grids.XamGrid)(target));
            return;
            case 2:
            this.cbTraded = ((System.Windows.Controls.CheckBox)(target));
            
            #line 174 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbTraded.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 178 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbTraded.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cbCancelled = ((System.Windows.Controls.CheckBox)(target));
            
            #line 181 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbCancelled.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 185 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbCancelled.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cbObsolete = ((System.Windows.Controls.CheckBox)(target));
            
            #line 188 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbObsolete.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 191 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbObsolete.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cbFilter = ((System.Windows.Controls.CheckBox)(target));
            
            #line 194 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbFilter.Checked += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            
            #line 196 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.cbFilter.Unchecked += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.intellibox = ((FeserWard.Controls.Intellibox)(target));
            
            #line 211 "..\..\..\Views\EntrustListPanelOption.xaml"
            this.intellibox.SearchBeginning += new System.Action<string, int, object>(this.intellibox_SearchBeginning);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


#pragma checksum "..\..\..\Views\EntrustPanelFuture - 复制.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B2ED62733B4810ABC04029F0891AE033"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace TradeStation.Modules.Trade.Views {
    
    
    /// <summary>
    /// EntrustPanelFuture
    /// </summary>
    public partial class EntrustPanelFuture : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Infragistics.Controls.Grids.XamGrid xamGrid;
        
        #line default
        #line hidden
        
        
        #line 163 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbNotTraded;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbTraded;
        
        #line default
        #line hidden
        
        
        #line 176 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbAll;
        
        #line default
        #line hidden
        
        
        #line 184 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbCancelled;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbObsolete;
        
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
            System.Uri resourceLocater = new System.Uri("/TradeStation.Modules.Trade;component/views/entrustpanelfuture%20-%20%e5%a4%8d%e5" +
                    "%88%b6.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            this.cbNotTraded = ((System.Windows.Controls.CheckBox)(target));
            
            #line 164 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbNotTraded.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 168 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbNotTraded.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cbTraded = ((System.Windows.Controls.CheckBox)(target));
            
            #line 171 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbTraded.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 175 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbTraded.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cbAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 178 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbAll.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 182 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbAll.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cbCancelled = ((System.Windows.Controls.CheckBox)(target));
            
            #line 186 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbCancelled.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 190 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbCancelled.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cbObsolete = ((System.Windows.Controls.CheckBox)(target));
            
            #line 194 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbObsolete.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 197 "..\..\..\Views\EntrustPanelFuture - 复制.xaml"
            this.cbObsolete.Unchecked += new System.Windows.RoutedEventHandler(this.cbNotTraded_Unchecked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


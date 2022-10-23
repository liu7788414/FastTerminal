using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using Infragistics.Controls.Editors;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Modules.Trade.Converters;
using TradeStation.Modules.Trade.ViewModels;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.OptionMatrixRegion)]
    public partial class OptionMatrixPanel
    {
        public OptionMatrixPanel()
        {
            InitializeComponent();
        }

        [Import]
        public OptionMatrixPanelViewModel Model
        {
            get { return DataContext as OptionMatrixPanelViewModel; }
            set { DataContext = value; }
        }

        [Import]
        public LogUtils Logger { get; set; }

        private void FillControls(Grid grid, List<List<OptionInfoModel>> arrayOptions50Etf,
            Action<Control, List<List<OptionInfoModel>>, int, int> action)
        {
            //填充按钮
            for (var i = 0; i < arrayOptions50Etf.Count; i++)
            {
                for (var j = 0; j < arrayOptions50Etf[i].Count; j++)
                {
                    var tb0 = new MyControl
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0),
                        FontSize = 11,
                        Foreground = Brushes.DarkRed,
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.White
                    };

                    if (arrayOptions50Etf[i][0].PlainText.Contains("卖"))
                    {
                        tb0.Background = Brushes.AntiqueWhite;
                    }

                    grid.Children.Add(tb0); //添加到Grid控件
                    tb0.SetValue(Grid.RowProperty, i); //设置按钮所在Grid控件的行
                    tb0.SetValue(Grid.ColumnProperty, j); //设置按钮所在Grid控件的列

                    switch (arrayOptions50Etf[i][j].NodeType)
                    {
                        case eNodeType.ExercisePrice:
                        {
                            tb0.Foreground = Brushes.Blue;
                            tb0.Background = Brushes.LightBlue;
                            tb0.Content = arrayOptions50Etf[i][j].PlainText;
                            break;
                        }
                        case eNodeType.BuyCall:
                        case eNodeType.SellCall:
                        case eNodeType.BuyPut:
                        case eNodeType.SellPut:
                        {
                            tb0.Foreground = Brushes.Black;
                            tb0.Content = arrayOptions50Etf[i][j].PlainText;
                            break;
                        }
                        case eNodeType.ExerciseDate:
                        {
                            tb0.Foreground = Brushes.Red;
                            tb0.Background = Brushes.LightBlue;
                            tb0.Content = arrayOptions50Etf[i][j].PlainText;
                            break;
                        }
                        case eNodeType.OptionInfoModelOriginal:
                        {
                            if (arrayOptions50Etf[i][j].ExID != null)
                            {
                                tb0.Model = arrayOptions50Etf[i][j];

                                switch (arrayOptions50Etf[i][0].NodeType)
                                {
                                    case eNodeType.BuyCall:
                                    {
                                        tb0.NodeType = eNodeType.OptionInfoModelBuyCall;
                                        break;
                                    }
                                    case eNodeType.BuyPut:
                                    {
                                        tb0.NodeType = eNodeType.OptionInfoModelBuyPut;
                                        break;
                                    }
                                    case eNodeType.SellCall:
                                    {
                                        tb0.NodeType = eNodeType.OptionInfoModelSellCall;
                                        break;
                                    }
                                    case eNodeType.SellPut:
                                    {
                                        tb0.NodeType = eNodeType.OptionInfoModelSellPut;
                                        break;
                                    }
                                }

                                tb0.MouseDoubleClick += tb0_MouseDoubleClick;
                                tb0.MouseEnter += tb0_MouseEnter;
                                tb0.MouseLeave += tb0_MouseLeave;
                                tb0.MouseDown += tb0_MouseDown;
                                action(tb0, arrayOptions50Etf, i, j);
                            }
                            else
                            {
                                tb0.IsEnabled = false;
                            }
                            break;
                        }
                    }

                    tb0.Clicked = false;
                    tb0.OriginalColor = tb0.Background;
                }
            }
        }

        private void SetBackground(Grid grid, object sender)
        {
            foreach (var child in grid.Children)
            {
                var control = (MyControl) child;

                if (!control.Equals(sender))
                {
                    control.Clicked = false;
                    control.Background = control.OriginalColor;
                }
            }
        }

        private void tb0_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = ((MyControl) sender);

            control.Background = Brushes.OrangeRed;
            control.Clicked = true;

            SetBackground(gridTradingPrice, sender);
            SetBackground(gridImpliedVolatility, sender);
            SetBackground(gridQuotation, sender);

            var model = control.Model;
            Model.SelectedOptionInfoModel = model;

            var quote = Model.MarketDataService.GetAndSubscribeSecurityQuote(model.ExID, model.SecurityID);

            switch (control.NodeType)
            {
                case eNodeType.OptionInfoModelBuyPut:
                case eNodeType.OptionInfoModelBuyCall:
                {
                    Model.EntrustDirection = eEntrustDirection.买入;
                    Model.Price = quote.BidPx1;
                    break;
                }
                case eNodeType.OptionInfoModelSellPut:
                case eNodeType.OptionInfoModelSellCall:
                {
                    Model.EntrustDirection = eEntrustDirection.卖出;
                    Model.Price = quote.AskPx1;
                    break;
                }
            }
        }

        private void tb0_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var control = (MyControl) sender;
            if (!control.Clicked)
            {
                control.Background = control.OriginalColor;
            }
        }

        private void tb0_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Control)sender).Background = Brushes.LightBlue;
        }

        private void tb0_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var myControl = (MyControl) sender;

            Model.EventAggregator.GetEvent<OptionInfoModelSelectedEvent>().Publish(myControl.Model);
        }

        private void LastPrice(Control tb0, List<List<OptionInfoModel>> arrayOptions50Etf, int i,
            int j)
        {
            //只有买的情况才显示最新价
            if (arrayOptions50Etf[i][0].NodeType == eNodeType.BuyCall ||
                arrayOptions50Etf[i][0].NodeType == eNodeType.BuyPut)
            {
                var binding = new Binding
                {
                    Source = arrayOptions50Etf[i][j].Quotation,
                    Path = new PropertyPath("LastPx"),
                    Mode = BindingMode.TwoWay
                };

                tb0.SetBinding(ContentProperty, binding);
            }
        }

        private void MarketPrice(FrameworkElement tb0, List<List<OptionInfoModel>> arrayOptions50Etf, int i, int j)
        {
            var bindingString = "AskPx1";

            switch (arrayOptions50Etf[i][0].NodeType)
            {
                case eNodeType.BuyCall:
                case eNodeType.BuyPut:
                {
                    bindingString = "BidPx1";
                    break;
                }
                case eNodeType.SellCall:
                case eNodeType.SellPut:
                {
                    bindingString = "AskPx1";
                    break;
                }
            }

            var binding = new Binding
            {
                Source = arrayOptions50Etf[i][j].Quotation,
                Path = new PropertyPath(bindingString),
                Mode = BindingMode.TwoWay
            };

            tb0.SetBinding(ContentProperty, binding);
        }

        private void ImpliedVolatility(FrameworkElement tb0, List<List<OptionInfoModel>> arrayOptions50Etf, int i, int j)
        {
            var bindingString = "AskPriceImpV1";

            switch (arrayOptions50Etf[i][0].NodeType)
            {
                case eNodeType.BuyCall:
                case eNodeType.BuyPut:
                    {
                        bindingString = "BidPriceImpV1";
                        break;
                    }
                case eNodeType.SellCall:
                case eNodeType.SellPut:
                    {
                        bindingString = "AskPriceImpV1";
                        break;
                    }
            }

            var valueBinding = new Binding
            {
                Source = arrayOptions50Etf[i][j].Quotation,
                Path = new PropertyPath(bindingString),
                Mode = BindingMode.TwoWay,

            };
            var digitalBinding = new Binding
            {
                Source = 2,
            };
            var multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(valueBinding);
            multiBinding.Bindings.Add(digitalBinding);
            multiBinding.Converter = new ImpVDigitFormatStringConverter();

            tb0.SetBinding(ContentProperty, multiBinding);
        }

        private void ReLoad(Grid grid, Action<Control, List<List<OptionInfoModel>>, int, int> action)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            var arrayOptions50Etf = Model.ArrayOptions50ETF;

            int i;

            var gridHeight = 25;
            var gridWidth = 45;

            //添加Grid的行
            for (i = 0; i < arrayOptions50Etf.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(gridHeight)});
            }

            if (arrayOptions50Etf.Count > 0)
            {
                //添加Grid的列
                for (i = 0; i < arrayOptions50Etf[0].Count; i++)
                {
                    if (i == 0)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(gridWidth) });
                    }

                }

                FillControls(grid, arrayOptions50Etf, action);
            }
        }

        private void ReLoad()
        {
            ReLoad(gridTradingPrice, LastPrice);
            ReLoad(gridImpliedVolatility, ImpliedVolatility);
            ReLoad(gridQuotation, MarketPrice);

        }

        private void OptionMatrixPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            //comboCombiNos.SelectedIndex = 0;
            if (xamComboEditor.Items.Count > 0)
            {
                xamComboEditor.SelectedIndex = 0;
            }
        }

        private void XamComboEditor_SelectionChanged(object sender,
            Infragistics.Controls.Editors.SelectionChangedEventArgs e)
        {
            var v = (XamComboEditor) sender;
            Model.LoadOptionInfoModels((string) v.SelectedItem);

            if (tabTradingPrice.IsSelected)
            {
                ReLoad(gridTradingPrice, LastPrice);
            }

            if (tabImpliedVolatility.IsSelected)
            {
                ReLoad(gridImpliedVolatility, ImpliedVolatility);
            }

            if (tabQuotation.IsSelected)
            {
                ReLoad(gridQuotation, MarketPrice);
            }
        }

        private void tabControlMatrix_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tabTradingPrice.IsSelected)
            {
                ReLoad(gridTradingPrice, LastPrice);
            }

            if (tabImpliedVolatility.IsSelected)
            {
                ReLoad(gridImpliedVolatility, ImpliedVolatility);
            }

            if (tabQuotation.IsSelected)
            {
                ReLoad(gridQuotation, MarketPrice);
            }
        }
    }
}

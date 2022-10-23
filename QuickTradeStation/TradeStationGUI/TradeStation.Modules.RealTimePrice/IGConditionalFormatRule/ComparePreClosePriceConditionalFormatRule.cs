using System;
using System.Windows;

using Infragistics.Controls.Grids;

using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.RealTimePrice.IGConditionalFormatRule
{
    public class ComparePreClosePriceConditionalFormatRule : ConditionalFormattingRuleBase
    {
        #region Greater Than Style

        public static readonly DependencyProperty GreaterThanStyleProperty = DependencyProperty.Register("GreaterThanStyle", typeof(Style), typeof(ComparePreClosePriceConditionalFormatRule), new PropertyMetadata(new PropertyChangedCallback(ComparePreClosePriceConditionalFormatRule.GreaterThanStyleChanged)));

        private static void GreaterThanStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ComparePreClosePriceConditionalFormatRule)obj).OnPropertyChanged("GreaterThanStyle");
        }

        public Style GreaterThanStyle
        {
            get
            {
                return (Style) base.GetValue(GreaterThanStyleProperty);
            }
            set
            {
                base.SetValue(GreaterThanStyleProperty, value);
            }
        }

        #endregion

        #region Less Than Style

        public static readonly DependencyProperty LessThanStyleProperty = DependencyProperty.Register("LessThanStyle", typeof(Style), typeof(ComparePreClosePriceConditionalFormatRule), new PropertyMetadata(new PropertyChangedCallback(ComparePreClosePriceConditionalFormatRule.LessThanStyleChanged)));

        private static void LessThanStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ComparePreClosePriceConditionalFormatRule)obj).OnPropertyChanged("LessThanStyle");
        }

        public Style LessThanStyle
        {
            get
            {
                return (Style)base.GetValue(LessThanStyleProperty);
            }
            set
            {
                base.SetValue(LessThanStyleProperty, value);
            }
        }

        #endregion

        protected override IConditionalFormattingRuleProxy CreateProxy()
        {
            return new ComparePreClosePriceConditionalFormatRuleProxy();
        }
    }

    public class  ComparePreClosePriceConditionalFormatRuleProxy : ConditionalFormattingRuleBaseProxy
    {
        public ComparePreClosePriceConditionalFormatRuleProxy()
        {
        }

        protected override Style EvaluateCondition(object sourceDataObject, object sourceDataValue)
        {
            if (sourceDataObject is SecurityInfo)
            {
                var securityInfo = sourceDataObject as SecurityInfo;

                IComparable currentValue = sourceDataValue as IComparable;
                IComparable preCloseValue = securityInfo.LastCpx as IComparable;

                if ((currentValue != null) && (preCloseValue != null) && currentValue.CompareTo(0.0) != 0)
                {
                    if (currentValue.CompareTo(preCloseValue) > 0)
                    {
                        return ((ComparePreClosePriceConditionalFormatRule)base.Parent).GreaterThanStyle;
                    }
                    else if (currentValue.CompareTo(preCloseValue) < 0)
                    {
                        return ((ComparePreClosePriceConditionalFormatRule)base.Parent).LessThanStyle;
                    }
                    
                }
            }

            return null;
        }
    }
}

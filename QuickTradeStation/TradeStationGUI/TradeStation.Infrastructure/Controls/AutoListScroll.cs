using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;
namespace TradeStation.Infrastructure.Controls
{
    class WpfTreeHelper
    {
        public static T FindChild<T>(DependencyObject node)
            where T : DependencyObject
        {
            if (node == null)
                return null;

            T found = null;
            var childlen = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < childlen; i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                var target = child as T;
                if (target == null)
                {
                    found = FindChild<T>(child);
                    if (found != null)
                        break;
                }
                else
                {
                    found = (T)child;
                    break;
                }
            }

            return found;
        }
    }

    public class AutoListScroll
    {
        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(AutoListScroll),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsEnabledChanged)));

        public static bool GetIsEnabled(ItemsControl d)
        {
            return (bool)d.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(ItemsControl d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool oldIsEnabled = (bool)e.OldValue;
            bool newIsEnabled = (bool)d.GetValue(IsEnabledProperty);

            var itemsControl = d as ItemsControl;
            if (itemsControl == null)
                return;

            if (newIsEnabled)
            {
                itemsControl.Loaded += (ss, ee) =>
                {
                    ScrollViewer scrollviewer = WpfTreeHelper.FindChild<ScrollViewer>(itemsControl);
                    if (scrollviewer != null)
                    {
                        ((ICollectionView)itemsControl.Items).CollectionChanged += (sss, eee) =>
                        {
                            scrollviewer.ScrollToEnd();
                        };
                    }
                };
            }
        }

        #endregion

    }
}

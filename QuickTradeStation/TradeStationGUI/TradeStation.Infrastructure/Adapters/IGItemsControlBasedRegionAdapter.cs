using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;

using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Infrastructure.Adapters
{
    [Export]
    public class IGItemsControlBasedRegionAdapter : RegionAdapterBase<ItemsControl>
    {
        [ImportingConstructor]
        public IGItemsControlBasedRegionAdapter(IRegionBehaviorFactory behaviorFactory)
            : base(behaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, ItemsControl regionTarget)
        {
            region.Views.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        // Insert view to the ItemsControl follow the rule:
                        // 1. If it is a ISequencableView, insert it as its sequence.
                        // 2. If it is not a ISequencableView, just insert it to the end of list.
                        foreach (FrameworkElement element in e.NewItems)
                        {
                            if (!(element is ISequencableView) || regionTarget.Items.Count == 0)
                            {
                                // Add the non-ISequencableView directly.
                                regionTarget.Items.Add(element);
                            }
                            else
                            {
                                var targetOrder = (element as ISequencableView).Order;

                                // Loop to find right position to insert.
                                for (int ix = 0; ix < regionTarget.Items.Count; ix++)
                                {
                                    if (!(regionTarget.Items[ix] is ISequencableView)
                                        || ix == regionTarget.Items.Count - 1
                                        || targetOrder < (regionTarget.Items[ix] as ISequencableView).Order)
                                    {
                                        regionTarget.Items.Insert(ix, element);
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (UIElement elementLoopVariable in e.OldItems)
                        {
                            var element = elementLoopVariable;
                            if (regionTarget.Items.Contains(element))
                            {
                                regionTarget.Items.Remove(element);
                            }
                        }
                        break;
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}

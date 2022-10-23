using System.Windows.Controls;

using Microsoft.Practices.Prism.Mvvm;

namespace TradeStation.Infrastructure.ViewModels
{
    public abstract class ViewModelBase<TView> : BindableBase
        where TView : ContentControl
    {
        public ViewModelBase(TView view)
        {
            View = view;
        }

        private TView _view;
        public TView View
        {
            get { return _view; }
            set
            {
                SetProperty(ref _view, value);
            }
        }
    }
}

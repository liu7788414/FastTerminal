using System;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel.Composition;

namespace TradeStation.Infrastructure.Services
{
    [Export(typeof(DispatcherService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public static class DispatcherService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }
}

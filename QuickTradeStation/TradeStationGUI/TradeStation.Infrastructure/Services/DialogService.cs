using System.Windows;
using System.ComponentModel.Composition;

namespace TradeStation.Infrastructure.Services
{
    [Export(typeof(DialogService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogService
    {
        public void ShowMessage(string title, string msg)
        {
            MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string title, string msg)
        {
            bool result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            return result;
        }
    }
}

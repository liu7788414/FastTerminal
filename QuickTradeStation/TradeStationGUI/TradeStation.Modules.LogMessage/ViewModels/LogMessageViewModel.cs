using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.LogMessage.ViewModels
{
    [Export]
    public class LogMessageViewModel : BindableBase
    {
        private ObservableCollection<LogItem> _logMessageList = new ObservableCollection<LogItem>();
        public ObservableCollection<LogItem> LogMessageList
        {
            get { return _logMessageList; }
            set
            {
                SetProperty(ref _logMessageList, value);
            }
        }

        private IEventAggregator _eventAggregator;
        
        [ImportingConstructor]
        public LogMessageViewModel(IEventAggregator eventAgg)
        {
            _eventAggregator = eventAgg;

            //init commands
            ResetLogWindowCommand = new DelegateCommand<object>(OnResetLogWindow);

            //subscribe event
            _eventAggregator.GetEvent<LogMessageNotifyEvent>().Subscribe(UpdateMessageLog, ThreadOption.UIThread);

        }

        [Import]
        public LogUtils Logger { get; set; }

        private void UpdateMessageLog(LogMessageEntity entity)
        {
            LogMessageList.Add(new LogItem(entity.Message));
        }

        public ICommand ResetLogWindowCommand { get; private set; }
        private void OnResetLogWindow(object arg)
        {
            LogMessageList.Clear();

        }

    }
}

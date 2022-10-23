using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;

using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStationShell
{
    [Export]
    public class ShellViewModel : BindableBase, IPartImportsSatisfiedNotification
    {
        #region Private Fields

        private IEventAggregator _eventAggregator;

        private LoginWindow _loginWindow;

        private DialogService _dialogService;

        #endregion

        #region Properties

        public string OperatorName
        {
            get
            {
                if (_loginWindow != null)
                {
                    return _loginWindow.Model.OperatorName;
                }

                return null;
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public UserSettings UserSetting { get; set; }

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ShellViewModel(LoginWindow loginWindow,
            DialogService dialogService,
            UserSettings userSetting,
            IEventAggregator eventAggregator)
        {
            _loginWindow = loginWindow;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            UserSetting = userSetting;

            SubscribeEvents();
        }

        #endregion

        #region Private Methods

        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<StatusChangedNotifyEvent>().Subscribe(OnStatusChanged);
        }

        private void OnStatusChanged(string status)
        {
            Status = status;
        }

        private void Authentication()
        {
            //dialogService.ShowMessage("创建完成", "创建完成消息");
            var result = _loginWindow.ShowDialog();
            if (result == true)
            {
                //show main window
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        #endregion

        public void OnImportsSatisfied()
        {
            Authentication();
        }
    }
}

using Microsoft.Practices.Prism.Mvvm;
using System;
using TradeStation.Infrastructure.Views;

namespace TradeStation.Infrastructure.Managers
{
    public class TFSplashScreenManager : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value);
            }
        }

        private static TFSplashScreenManager _instance = null;
        public static TFSplashScreenManager Instance
        {
            get
            {
                if (null == TFSplashScreenManager._instance)
                {
                    TFSplashScreenManager._instance = new TFSplashScreenManager();
                }
                return TFSplashScreenManager._instance;
            }
        }
    }
}

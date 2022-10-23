using System.Timers;
using System.ComponentModel.Composition;
using hundsun.t2sdk;

namespace TradeStation.Infrastructure.Services
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TradeService:IPartImportsSatisfiedNotification
    {
        private Timer _heartBeatTimer;
        private const int HeartBeatInterval = 10000;

        [Import]
        public HsStock Trader { get; set; }

        public void ConnectServer(string sOperator, string sPassword)
        {
            Subscribe param;
            param.FundAccount = sOperator;
            param.FundAccountPassword = sPassword;
            if (!Trader.ConnectSubcribeService(param))
            {
                return;
            }

            var config = new CT2Configinterface();
            config.SetString("t2sdk", "servers", string.Format("{0}:{1}", AppConfigService.TradeServerIp, AppConfigService.TradeServerPort));
            config.SetString("t2sdk", "license_file", AppConfigService.LicenseFile);
            if (!Trader.ConnectTradeService(config, sOperator, sPassword))
            {
                return;
            }
        }

        // 摘要: 
        //     在满足部件的导入并可安全使用时调用。
        public void OnImportsSatisfied()
        {
            _heartBeatTimer = new Timer(HeartBeatInterval);
            _heartBeatTimer.Elapsed += heartBeatTimer_Elapsed;
            _heartBeatTimer.Start();
        }

        private void heartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(!string.IsNullOrEmpty(Trader.UserToken))
            {
                Trader.SendHeartBeat();
            }
        }
    }
}

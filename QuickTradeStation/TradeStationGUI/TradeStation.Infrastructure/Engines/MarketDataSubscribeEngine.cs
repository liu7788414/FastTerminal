using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Commands;

using TFMkdtCS;
using TFMkdtSubAPI;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Engines
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MarketDataSubscribeEngine : CSSubscribeBaseAPI, IMarketDataEngine
    {
        private MarketDataService _marketDataService;
        private RealTimeDataProcessor _realTimeDataProcessor;
        private KLineProcessor _kLineProcessor;
        private OptionFinanceCalculator _optionFinanceCalculator;

        public LogUtils Logger { get; set; }

        [ImportingConstructor]
        public MarketDataSubscribeEngine(MarketDataService marketDataService,
            RealTimeDataProcessor realTimeDataProcessor,
            KLineProcessor kLineProcessor,
            OptionFinanceCalculator optionFinanceCalculator,
            LogUtils logger)
        {
            _marketDataService = marketDataService;
            _realTimeDataProcessor = realTimeDataProcessor;
            _kLineProcessor = kLineProcessor;
            _optionFinanceCalculator = optionFinanceCalculator;

            Logger = logger;
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(OnShutdownCommand));
        }

        #region Public Methods

        public void RunEngine()
        {
            this.Init();
            this.Connect();
        }

        public void CloseEngine()
        {
            this.LogOut();
            this.UnInit();
        }

        public void SubscribeSecurity(string exID, string securityID, eCategory securityType)
        {
            MDS_QuotsubItem item;
            item.exID = exID;
            item.secID = securityID;

            switch (securityType)
            {
                case eCategory.指数:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_IndexTick;
                    break;
                case eCategory.期权:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_OptionTick;
                    break;
                case eCategory.期货:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_FuturesTick;
                    break;
                case eCategory.债券:
                case eCategory.债券回购:
                case eCategory.基金:
                case eCategory.基金分拆合并:
                case eCategory.股票:
                default:
                    item.dataType = (Int32)MDS_QuotsubMsgType.QuotSub_StockTick;
                    break;
            }

            try
            {
                Logger.Debug(string.Format("Subscribing '{0}' of {1} {2}.", item.dataType.ToString(), exID, securityID));
                Subscribe(ref item);
            }
            catch (Exception)
            {
                Logger.Error(string.Format("Subscribing '{0}' of {1} {2}.", item.dataType.ToString(), exID, securityID));

                // May need popup a dialog to inform user.
            }
        }

        public void UnSubscribeSecurity(string exID, string securityID, eCategory securityType)
        {
            MDS_QuotsubItem item;
            item.exID = exID;
            item.secID = securityID;

            switch (securityType)
            {
                case eCategory.指数:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_IndexTick;
                    break;
                case eCategory.期权:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_OptionTick;
                    break;
                case eCategory.期货:
                    item.dataType = (Int32)MDS_QuotsubMsgType.Quotsub_FuturesTick;
                    break;
                case eCategory.债券:
                case eCategory.债券回购:
                case eCategory.基金:
                case eCategory.基金分拆合并:
                case eCategory.股票:
                default:
                    item.dataType = (Int32)MDS_QuotsubMsgType.QuotSub_StockTick;
                    break;
            }

            try
            {
                Logger.Debug(string.Format("Unsubscribing {0} {1}.", exID, securityID));
                UnSubscribe(ref item);
            }
            catch (Exception)
            {
                Logger.Error(string.Format("Unsubscribe {0} {1} Failed.", exID, securityID));

                // May need popup a dialog to inform user.
            }
        }

        #endregion

        #region CSSubscribeBaseAPI members

        public override void OnConnect(int statusCode)
        {
            if ((int)MDS_QuotsubRspType.Normal_type != statusCode)
            {
                Logger.Error(string.Format("Connect to subscription server failed! statusCode:" + statusCode));
            }
            else
            {
                Logger.Debug("Connect to subscription server successfully.");

                this.Login();
            }
        }

        public override void OnRspLogin(int statusCode)
        {
            if ((int)MDS_QuotsubRspType.Normal_type != statusCode)
            {
                Logger.Error(string.Format("Login to subscription server failed! statusCode:" + statusCode));
            }
            else
            {
                Logger.Debug("Login to subscription server successfully.");
            }
        }

        public override void OnRspLogout(int statusCode)
        {
            if ((int)MDS_QuotsubRspType.Normal_type != statusCode)
            {
                Logger.Error(string.Format("Logout from subscription server failed! statusCode:" + statusCode));
            }
            else
            {
                Logger.Debug("Logout from subscription server successfully.");
            }
        }

        public override void OnRspSubMarketData(int statusCode, ref MDS_QuotsubItem quote)
        {
            if ((int)MDS_QuotsubRspType.Normal_type == statusCode)
            {
                Logger.Debug("subscribe success!");

                Logger.Debug("statusCode" + statusCode);
                Logger.Debug("--------------------------");
            }
            else
            {
                Logger.Error("subscribe failed!");

                Logger.Error("dataType:" + quote.dataType);
                Logger.Error("exID:" + quote.exID);
                Logger.Error("secID" + quote.secID);
                Logger.Error("statusCode" + statusCode);
                Logger.Error("--------------------------");
            }
        }

        public override void OnRspUnSubMarketData(int statusCode, ref MDS_QuotsubItem quote)
        {
            if ((int)MDS_QuotsubRspType.Normal_type == statusCode)
            {
                Logger.Debug("unsubscribe success!");

                Logger.Debug("statusCode:" + statusCode);
                Logger.Debug("--------------------------");
            }
            else
            {
                Logger.Error("unsubscribe failed!");

                Logger.Error("dataType:" + quote.dataType);
                Logger.Error("exID:" + quote.exID);
                Logger.Error("secID:" + quote.secID);
                Logger.Error("statusCode:" + statusCode);
                Logger.Error("--------------------------");
            }
        }

        public override void OnRtnTickData(ref StockTickData data)
        {
            ExSecID exSecId = new ExSecID(data.ExID, data.SecID);
            if (_marketDataService.SubscribeSecurities.Contains(exSecId))
            {
                var model = data.ToStockTickDataModel();

                DispatcherService.Invoke(() =>
                {
                    _marketDataService.SecurityQuotMap[exSecId].UpdateQuote(model);

                    // 更新分时图行情
                    if (_marketDataService.RealTimeDataMap.ContainsKey(exSecId))
                    {
                    	_realTimeDataProcessor.UpdateRealTimeDataByTickData(_marketDataService.RealTimeDataMap[exSecId], model);
                    }

                    // 只用实时行情快照，来更新K线图
                    if (model.Type % 100 == 0)
                    {
                        // 更新K线图行情
                        _kLineProcessor.ProcessTickData(model);
                    }
                });
            }
        }

        public override void OnRtnFuturesTickData(ref FuturesTickData data)
        {
            ExSecID exSecId = new ExSecID(data.ExID, data.SecID);
            if (_marketDataService.SubscribeSecurities.Contains(exSecId))
            {
                var model = data.ToFutureDataModel();

                DispatcherService.Invoke(() =>
                {
                    _marketDataService.SecurityQuotMap[exSecId].UpdateQuote(model);

                    // 更新分时图行情
                    if (_marketDataService.RealTimeDataMap.ContainsKey(exSecId))
                    {
                    _realTimeDataProcessor.UpdateRealTimeDataByTickData(_marketDataService.RealTimeDataMap[exSecId], model);
                    }

                    // 只用实时行情快照，来更新K线图
                    if (model.Type % 500 == 0)
                    {
                        // 更新K线图行情
                        _kLineProcessor.ProcessTickData(model);
                    }
                });
            }
        }

        public override void OnRtnOptionTickData(ref OptionData data)
        {
            ExSecID exSecId = new ExSecID(data.ExID, data.SecID);
            if (_marketDataService.SubscribeSecurities.Contains(exSecId))
            {
                var model = data.ToOptionDataModel();

                DispatcherService.Invoke(() =>
                {
                    _optionFinanceCalculator.CalcImpliedVolatility(model, ImpliedVolatilityMethod.Bisections);

                    _marketDataService.SecurityQuotMap[exSecId].UpdateQuote(model);
                });
            }
        }

        public override void OnRtnIndexTickData(ref IndexData data)
        {
            // Do nothing.
        }

        public override void OnRtnTransactionData(ref TransactionData data)
        {
            // Do nothing.
            // Currently. Transaction data will not be handled in this way.
        }

        public override void OnRtnOrderData(ref OrderData data)
        {
            // Do nothing.
        }

        public override void OnRtnOrderQueueData(ref OrderQueueDataHead data, IntPtr orderQueue, int orderCount)
        {
            // Do nothing.
        }

        #endregion

        private void OnShutdownCommand()
        {
            if (!AppConfigService.IsMulticastPrice)
            {
                CloseEngine();
            }
        }
    }
}

using System;
using System.ComponentModel.Composition;

using Microsoft.Practices.Prism.Commands;

using TFMkdtCS;
using TFMkdtMultiAPI;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Engines
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MarketDataMulticastEngine : CSMulticastBaseAPI, IMarketDataEngine
    {
        private MarketDataService _marketDataService;
        private RealTimeDataProcessor _realTimeDataProcessor;
        private KLineProcessor _kLineProcessor;
        private OptionFinanceCalculator _optionFinanceCalculator;

        [ImportingConstructor]
        public MarketDataMulticastEngine(
            MarketDataService marketDataService,
            RealTimeDataProcessor realTimeDataProcessor,
            KLineProcessor kLineProcessor,
            OptionFinanceCalculator optionFinanceCalculator)
        {
            _marketDataService = marketDataService;
            _realTimeDataProcessor = realTimeDataProcessor;
            _kLineProcessor = kLineProcessor;
            _optionFinanceCalculator = optionFinanceCalculator;

            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(OnShutdownCommand));
        }

        #region Public Methods

        public void RunEngine()
        {
            this.Init();
            this.Start();
        }

        public void CloseEngine()
        {
            this.Stop();
        }

        public void SubscribeSecurity(string exID, string securityID, eCategory securityType)
        {
            // Do nothing.
            // No need to subscribe.
        }

        public void UnSubscribeSecurity(string exID, string securityID, eCategory securityType)
        {
            // Do nothing.
            // No need to subscribe.
        }

        #endregion

        #region CSMulticastBaseAPI members

        // TODO: 目前商品期权也走此通道，之后代码更改需要考虑。目前期权tick走此段代码逻辑与FunRecvOption一致
        // 因为 1. 目前期权不存在分时与K线的计算
        //      2. O32目前不支持商品期权，故序列期权与合成期货下单目前不涉及商品期权，所以可忽略波动率的计算
        public override void FunRecvFuture(ref FuturesTickData data)
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

        public override void FunRecvIndex(ref IndexData data)
        {
            // Do nothing.
            // Currently. Index tick will not be handled in this way.
        }

        public override void FunRecvOption(ref OptionData data)
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

        public override void FunRecvStock(ref StockTickData data)
        {
            if (AppConfigService.IsMulticastPrice)
            {
                ExSecID exSecId = new ExSecID(data.ExID, data.SecID);
                if (_marketDataService.SubscribeSecurities.Contains(exSecId))
                {
                    var model = data.ToStockTickDataModel();

                    DispatcherService.Invoke(() =>
                    {
                        // 对集合竞价的Tick做处理
                        if (model.Type == 101)
                        {
                            DateTime lastCallAuctionLastUpdatedTime;

                            // 对于比缓存的最后一次集合竞价时间早的Tick不做处理，直接返回，否则用其更新报价
                            if (_marketDataService.CallAuctionLastUpdatedTime.TryGetValue(exSecId, out lastCallAuctionLastUpdatedTime)
                                && model.ExchangeTime <= lastCallAuctionLastUpdatedTime)
                            {
                                return;
                            }

                            _marketDataService.CallAuctionLastUpdatedTime[exSecId] = model.ExchangeTime;
                        }

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
        }

        #endregion

        private void OnShutdownCommand()
        {
            if (AppConfigService.IsMulticastPrice)
            {
                CloseEngine();
            }
        }
    }
}

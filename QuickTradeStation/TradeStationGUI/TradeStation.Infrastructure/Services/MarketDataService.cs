using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Engines;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Services
{
    [Export(typeof(MarketDataService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MarketDataService
    {
        [Import]
        private SecurityInfoMetadata secInfoMetadata { get; set; }

        private IEventAggregator EventAggregator { get; set; }
        private IMarketDataEngine marketDataEngine { get; set; }
        private IList<eKLinePeriodType> allKLineTypes { get; set; }

        public Dictionary<ExSecID, SecurityQuotation> SecurityQuotMap { get; private set; }
        public HashSet<ExSecID> SubscribeSecurities { get; private set; }

        public Dictionary<ExSecID, RealTimeMarketData> RealTimeDataMap { get; private set; }
        public Dictionary<ExSecID, Dictionary<eKLinePeriodType, RealTimeMarketData>> KLineMarketDataMap { get; private set; }

        public Dictionary<ExSecID, DateTime> CallAuctionLastUpdatedTime { get; private set; }

        [ImportingConstructor]
        public MarketDataService(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;

            SecurityQuotMap = new Dictionary<ExSecID, SecurityQuotation>();
            SubscribeSecurities = new HashSet<ExSecID>();
            RealTimeDataMap = new Dictionary<ExSecID, RealTimeMarketData>();
            KLineMarketDataMap = new Dictionary<ExSecID, Dictionary<eKLinePeriodType, RealTimeMarketData>>();
            CallAuctionLastUpdatedTime = new Dictionary<ExSecID, DateTime>();

            allKLineTypes = Enum.GetValues(typeof(eKLinePeriodType)).OfType<eKLinePeriodType>().ToList();
        }

        #region Public Methods

        public void Initialize(IMarketDataEngine engine)
        {
            marketDataEngine = engine;
            marketDataEngine.RunEngine();
        }

        public SecurityQuotation GetAndSubscribeSecurityQuote(string exId, string secId)
        {
            ExSecID exSecId = new ExSecID(exId, secId);
            return GetAndSubscribeSecurityQuote(exSecId);
        }

        public SecurityQuotation GetAndSubscribeSecurityQuote(ExSecID exSecId)
        {
            SubscribeSecQuot(exSecId);

            if (SecurityQuotMap.ContainsKey(exSecId))
                return SecurityQuotMap[exSecId];
            SecurityQuotation newQuot = new SecurityQuotation(exSecId.ExID, exSecId.SecurityID);
            SecurityQuotMap[newQuot.ExSecurityID] = newQuot;
            return newQuot;
        }

        public string GetSecurityName(string exId, string secId)
        {
            Dictionary<ExSecID, SecurityInfo> map = secInfoMetadata.SecurityInfoMap;

            ExSecID exSecId = new ExSecID(exId, secId);

            if (map.ContainsKey(exSecId))
            {
                return map[exSecId].SecurityName;
            }
            else
            {
                return null;
            }
        }

        public SecurityInfo GetSecurityInfo(string exId, string secId)
        {
            var map = secInfoMetadata.SecurityInfoMap;

            ExSecID exSecId = new ExSecID(exId, secId);

            if (map.ContainsKey(exSecId))
            {
                return map[exSecId];
            }
            else
            {
                return null;
            }
        }

        public void AddOrUpdateSecurityQuotationToMap(SecurityInfo securityInfo)
        {
            SecurityQuotMap[securityInfo.Quotation.ExSecurityID] = securityInfo.Quotation;
        }

        public void SubscribeSecQuot(ExSecID exSecId)
        {
            if (!SubscribeSecurities.Contains(exSecId))
            {
                var securityInfo = GetSecurityInfo(exSecId.ExID, exSecId.SecurityID);

                if (null != marketDataEngine && null != securityInfo && !securityInfo.IsExpire)
                {
                    // Subscribe security.
                    marketDataEngine.SubscribeSecurity(exSecId.ExID, exSecId.SecurityID, securityInfo.SecurityType);

                    SubscribeSecurities.Add(exSecId);
                }
            }
        }

        public void UnSubscribeSecQuot(ExSecID exSecId)
        {
            if (SubscribeSecurities.Contains(exSecId))
            {
                var securityInfo = GetSecurityInfo(exSecId.ExID, exSecId.SecurityID);

                if (null != securityInfo)
                {
                    SubscribeSecurities.Remove(exSecId);

                    if (null != marketDataEngine)
                    {
                        marketDataEngine.UnSubscribeSecurity(exSecId.ExID, exSecId.SecurityID, securityInfo.SecurityType);

                        // TODO: If we need set the real time data to null to free the space.
                    }
                }
            }
        }

        #endregion
    }
}

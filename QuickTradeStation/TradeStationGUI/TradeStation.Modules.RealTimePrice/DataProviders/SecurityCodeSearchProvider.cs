using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using FeserWard.Controls;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.RealTimePrice.DataProviders
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class SecurityCodeSearchProvider : IIntelliboxResultsProvider, IReInitializable
    {
        [Import]
        private SecurityInfoMetadata _secInfoMetadata { get; set; }

        [Import]
        private IEventAggregator _eventAggregator;

        protected List<SecurityInfo> _securityInfoList;

        protected List<eCategory> _acceptedSecurityTypes;

        protected bool isGetExpired = false;

        [ImportingConstructor]
        public SecurityCodeSearchProvider()
        {
            SetFilteredTypes();
        }

        public virtual void SetFilteredTypes()
        {
            Initialize();

            // Default to show all types and expired securities in provider.
            SetFilteredTypes(true, true, true, true, true, true, true);
        }

        protected void SetFilteredTypes(bool getStock, bool getFunds, bool getFuture, bool getOption, bool getBondsRepurchase = false, bool getBonds = false, bool getExpired = false)
        {
            Initialize();

            _acceptedSecurityTypes = new List<eCategory>();

            if (getStock)
            {
                _acceptedSecurityTypes.Add(eCategory.股票);
            }
            if (getFunds)
            {
                _acceptedSecurityTypes.Add(eCategory.基金);
            }
            if (getFuture)
            {
                _acceptedSecurityTypes.Add(eCategory.期货);
            }
            if (getOption)
            {
                _acceptedSecurityTypes.Add(eCategory.期权);
            }
            if (getBonds)
            {
                _acceptedSecurityTypes.Add(eCategory.债券);
            }
            if (getBondsRepurchase)
            {
                _acceptedSecurityTypes.Add(eCategory.债券回购);
            }

            isGetExpired = getExpired;
        }

        public IEnumerable DoSearch(string searchTerm, int maxResults, object tag)
        {
            if (_securityInfoList == null || _securityInfoList.Count <= 0)
            {
                _securityInfoList = _secInfoMetadata.GetSecurityList();
            }

            IList<SecurityInfo> securityInfoList;

            if (null != _acceptedSecurityTypes && _acceptedSecurityTypes.Count > 0)
            {
                securityInfoList = _securityInfoList.Where(x => _acceptedSecurityTypes.Contains(x.SecurityType)).ToList();
            }
            else
            {
                securityInfoList = _securityInfoList;
            }

            if (!isGetExpired)
            {
                securityInfoList = securityInfoList.Where(x => !x.IsExpire).ToList();
            }

            int matchCount = 0;
            List<SecurityInfo> resultList = new List<SecurityInfo>();

            foreach (SecurityInfo code in securityInfoList)
            {
                if (matchCount >= maxResults)
                    break;
                if (code.SecurityID.ToUpper().IndexOf(searchTerm.ToUpper()) >= 0)
                {
                    matchCount++;
                    resultList.Add(code);
                }
                else if (code.ChinCharCapitals.IndexOf(searchTerm.ToUpper()) == 0)
                {
                    matchCount++;
                    resultList.Add(code);
                }
            }

            return resultList;
        }

        public void Initialize()
        {
            // Set security info list to null, make it getting security info list again.
            _securityInfoList = null;
        }

        public void DailyReInitialize()
        {
            Initialize();
        }
    }
}

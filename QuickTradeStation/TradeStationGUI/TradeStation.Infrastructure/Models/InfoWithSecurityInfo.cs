using Microsoft.Practices.Prism.Mvvm;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class InfoWithSecurityInfo : BindableBase
    {
        private eMarketType _marketType;
        [DataMember(Name = "marketType")]
        public eMarketType MarketType
        {
            get { return _marketType; }
            set
            {
                SetProperty(ref _marketType, value);
            }
        }

        private string _securityId;
        [DataMember(Name = "securityID")]
        public string SecurityID
        {
            get { return _securityId; }
            set
            {
                SetProperty(ref _securityId, value);
            }
        }

        private string _securityName;
        [DataMember(Name = "securityName")]
        public string SecurityName
        {
            get { return _securityName; }
            set
            {
                SetProperty(ref _securityName, value);
            }
        }

        private string _settlementDate;
        [DataMember(Name = "settlementDate")]
        public string SettlementDate
        {
            get { return _settlementDate; }
            set
            {
                SetProperty(ref _settlementDate, value);
            }
        }
    }
}
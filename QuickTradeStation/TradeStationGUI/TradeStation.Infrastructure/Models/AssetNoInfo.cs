using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System.ComponentModel;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Services;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class AssetNoInfo : BindableBase
    {
        private string assetNo;
        [DataMember(Name = "assetNo")]
        public string AssetNo
        {
            get { return assetNo; }
            set
            {
                SetProperty(ref assetNo, value);
            }
        }
    }

    public class SelectedAssetNoChangedNotifyEvent : PubSubEvent<AssetNoInfo>
    {

    }

    public class AssetNoInfoArrivedNotifyEvent : PubSubEvent<AssetNoInfo>
    {

    }
}
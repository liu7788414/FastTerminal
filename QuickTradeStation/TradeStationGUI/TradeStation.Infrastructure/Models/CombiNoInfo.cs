using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System.ComponentModel;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Services;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class CombiNoInfo : BindableBase
    {
        private string combiNo;
        [DataMember(Name = "combiNo")]
        public string CombiNo
        {
            get { return combiNo; }
            set
            {
                SetProperty(ref combiNo, value);
            }
        }
    }

    public class SelectedCombiNoChangedNotifyEvent : PubSubEvent<CombiNoInfo>
    {

    }

    public class CombiNoInfoArrivedNotifyEvent : PubSubEvent<CombiNoInfo>
    {

    }

    public class QryCombiNoNotifyEvent : PubSubEvent<CombiNoInfo>
    {

    }
}
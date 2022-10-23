using Microsoft.Practices.Prism.PubSubEvents;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;


namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class InfoWithCombiNoAndSecurityInfo : InfoWithSecurityInfo
    {
        private bool _isChecked;
        [DataMember(Name = "isChecked")]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        private string _reportNo;
        [DataMember(Name = "reportNo")]
        public string ReportNo
        {
            get { return _reportNo; }
            set
            {
                SetProperty(ref _reportNo, value);
            }
        }

        private string _accountCode;
        [DataMember(Name = "accountCode")]
        public string AccountCode
        {
            get { return _accountCode; }
            set
            {
                SetProperty(ref _accountCode, value);
            }
        }

        private string _assetNo;
        [DataMember(Name = "assetNo")]
        public string AssetNo
        {
            get { return _assetNo; }
            set
            {
                SetProperty(ref _assetNo, value);
            }
        }

        private string _reportSeat;
        [DataMember(Name = "reportSeat")]
        public string ReportSeat
        {
            get { return _reportSeat; }
            set
            {
                SetProperty(ref _reportSeat, value);
            }
        }

        private string _combiNo;
        [DataMember(Name = "combiNo")]
        public string CombiNo
        {
            get { return _combiNo; }
            set
            {
                SetProperty(ref _combiNo, value);
            }
        }

        private eCategory _category;
        [DataMember(Name = "category")]
        public eCategory Category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value);
            }
        }

        private string _stockholderId;
        [DataMember(Name = "stockholderId")]
        public string StockholderId
        {
            get { return _stockholderId; }
            set
            {
                SetProperty(ref _stockholderId, value);
            }
        }
    }

    public class CombiNoInfoArrivedNotifyEvent : PubSubEvent<InfoWithCombiNoAndSecurityInfo>
    {

    }
}
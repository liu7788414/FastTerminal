using TradeStation.Infrastructure.CommonUtils;
using Microsoft.Practices.Prism.Mvvm;
using System.Runtime.Serialization;
using Microsoft.Practices.Prism.PubSubEvents;

namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class LogItem : BindableBase
    {
        private string _timeStamp;
        [DataMember(Name = "timeStamp")]
        public string TimeStamp 
        {
            get { return _timeStamp; }
            set
            {
                SetProperty(ref _timeStamp, value);
            }
        }

        private string _message;
        [DataMember(Name = "message")]
        public string Messsage
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value);
            }
        }

        public LogItem(string msg)
        {
            TimeStamp = CommonUtil.GetNowTimeStamp();
            Messsage = msg;
        }
    }

    [DataContract]
    public class MessageBoxItem : BindableBase
    {
        private string _title;
        [DataMember(Name = "title")]
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }


        private string _content;
        [DataMember(Name = "content")]
        public string Content
        {
            get { return _content; }
            set
            {
                SetProperty(ref _content, value);
            }
        }

        private System.Windows.Media.Brush _textBrush;
        [DataMember(Name = "brush")]
        public System.Windows.Media.Brush TextBrush
        {
            get { return _textBrush; }
            set
            {
                SetProperty(ref _textBrush, value);
            }
        }

        private bool _isEditable = false;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                SetProperty(ref _isEditable, value);
            }
        }

        public bool IsDataChanged { get; set; }
    }

    public class LogInfoNotifyEvent : PubSubEvent<LogItem>
    {

    }
}

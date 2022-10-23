using System.Drawing;
using System.Windows.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models.Local;


namespace TradeStation.Infrastructure.Models
{
    public class QryHistoricalEntrustNotifyEvent : PubSubEvent<string>
    {

    }

    public class QryHistoricalTradeResultNotifyEvent : PubSubEvent<string>
    {

    }

    public class MyControl : Label
    {
        public OptionInfoModel _model;

        public OptionInfoModel Model
        {
            get { return _model; }
            set
            {
                if (value != null && _model != value)
                {
                    _model = value;
                }
            }
        }

        public System.Windows.Media.Brush _originalColor;

        public System.Windows.Media.Brush OriginalColor
        {
            get { return _originalColor; }
            set { _originalColor = value; }
        }

        public bool _clicked;

        public bool Clicked
        {
            get { return _clicked; }
            set { _clicked = value; }
        }

        public eNodeType _nodeType;

        public eNodeType NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }
    }
}
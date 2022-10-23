using Microsoft.Practices.Prism.Mvvm;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Option.Models
{
    public class DisplayOptionInfo : BindableBase
    {
        private SecurityInfo _callOptionSecurityInfo;
        public SecurityInfo CallOptionSecurityInfo
        {
            get { return _callOptionSecurityInfo; }
            set
            {
                SetProperty(ref _callOptionSecurityInfo, value);
            }
        }

        private SecurityInfo _putOptionSecurityInfo;
        public SecurityInfo PutOptionSecurityInfo
        {
            get { return _putOptionSecurityInfo; }
            set
            {
                SetProperty(ref _putOptionSecurityInfo, value);
            }
        }

        private double _exercisePrice;
        public double ExercisePrice
        {
            get { return _exercisePrice; }
            set
            {
                SetProperty(ref _exercisePrice, value);
            }
        }

        private string _optionExRightSymbol;
        public string OptionExRightSymbol
        {
            get { return _optionExRightSymbol; }
            set
            {
                SetProperty(ref _optionExRightSymbol, value);
            }
        }
    }
}

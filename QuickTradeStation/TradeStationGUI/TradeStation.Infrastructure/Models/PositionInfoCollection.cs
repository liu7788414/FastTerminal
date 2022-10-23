using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;

namespace TradeStation.Infrastructure.Models
{
    [Export]
    public class PositionInfoCollection : BindableBase
    {
        public PositionInfoCollection()
        {
            StockPositionInfoList = new ObservableCollection<PositionInfoBase>();
            FuturePositionInfoList = new ObservableCollection<PositionInfoBase>();
            OptionPositionInfoList = new ObservableCollection<OptionPositionInfo>();
            FundPositionInfoList = new ObservableCollection<PositionInfoBase>();
        }

        private ObservableCollection<PositionInfoBase> _stockPositionInfoList;
        public ObservableCollection<PositionInfoBase> StockPositionInfoList
        {
            get { return _stockPositionInfoList; }
            set
            {
                SetProperty(ref _stockPositionInfoList, value);
            }
        }

        private ObservableCollection<PositionInfoBase> _futurePositionInfoList;
        public ObservableCollection<PositionInfoBase> FuturePositionInfoList
        {
            get { return _futurePositionInfoList; }
            set
            {
                SetProperty(ref _futurePositionInfoList, value);
            }
        }

        private ObservableCollection<OptionPositionInfo> _optionPositionInfoList;
        public ObservableCollection<OptionPositionInfo> OptionPositionInfoList
        {
            get { return _optionPositionInfoList; }
            set
            {
                SetProperty(ref _optionPositionInfoList, value);
            }
        }

        private ObservableCollection<PositionInfoBase> _fundPositionInfoList;
        public ObservableCollection<PositionInfoBase> FundPositionInfoList
        {
            get { return _fundPositionInfoList; }
            set
            {
                SetProperty(ref _fundPositionInfoList, value);
            }
        }
    }

    [Export]
    public class EntrustInfoCollection : BindableBase
    {
        public EntrustInfoCollection()
        {
            StockEntrustInfoList = new ObservableCollection<EntrustInfo>();
            HistoricalStockEntrustInfoList = new ObservableCollection<HistoricalEntrustInfo>();
            FutureEntrustInfoList = new ObservableCollection<EntrustInfo>();
            OptionEntrustInfoList = new ObservableCollection<EntrustInfo>();
            FundEntrustInfoList = new ObservableCollection<EntrustInfo>();
        }

        private ObservableCollection<EntrustInfo> _stockEntrustInfoList;
        public ObservableCollection<EntrustInfo> StockEntrustInfoList
        {
            get { return _stockEntrustInfoList; }
            set
            {
                SetProperty(ref _stockEntrustInfoList, value);
            }
        }

        private List<string> _stockNameList;
        public List<string> StockNameList
        {
            get { return _stockNameList; }
            set
            {
                SetProperty(ref _stockNameList, value);
            }
        }

        private ObservableCollection<HistoricalEntrustInfo> _historicalStockEntrustInfoList;
        public ObservableCollection<HistoricalEntrustInfo> HistoricalStockEntrustInfoList
        {
            get { return _historicalStockEntrustInfoList; }
            set
            {
                SetProperty(ref _historicalStockEntrustInfoList, value);
            }
        }

        private ObservableCollection<EntrustInfo> _futureEntrustInfoList;
        public ObservableCollection<EntrustInfo> FutureEntrustInfoList
        {
            get { return _futureEntrustInfoList; }
            set
            {
                SetProperty(ref _futureEntrustInfoList, value);
            }
        }

        private List<string> _futureNameList;
        public List<string> FutureNameList
        {
            get { return _futureNameList; }
            set
            {
                SetProperty(ref _futureNameList, value);
            }
        }

        private ObservableCollection<EntrustInfo> _optionEntrustInfoList;
        public ObservableCollection<EntrustInfo> OptionEntrustInfoList
        {
            get { return _optionEntrustInfoList; }
            set
            {
                SetProperty(ref _optionEntrustInfoList, value);
            }
        }

        private List<string> _optionNameList;
        public List<string> OptionNameList
        {
            get { return _optionNameList; }
            set
            {
                SetProperty(ref _optionNameList, value);
            }
        }

        private ObservableCollection<EntrustInfo> _fundEntrustInfoList;
        public ObservableCollection<EntrustInfo> FundEntrustInfoList
        {
            get { return _fundEntrustInfoList; }
            set
            {
                SetProperty(ref _fundEntrustInfoList, value);
            }
        }
    }

    [Export]
    public class TradeResultInfoCollection : BindableBase
    {
        public TradeResultInfoCollection()
        {
            StockTradeResultInfoList = new ObservableCollection<TradeResultInfo>();
            FutureTradeResultInfoList = new ObservableCollection<TradeResultInfo>();
            OptionTradeResultInfoList = new ObservableCollection<TradeResultInfo>();
            FundTradeResultInfoList = new ObservableCollection<TradeResultInfo>();
            HistoricalTradeResultInfoList = new ObservableCollection<HistoricalTradeResultInfo>();
        }

        private ObservableCollection<HistoricalTradeResultInfo> _historicalTradeResultInfoList;
        public ObservableCollection<HistoricalTradeResultInfo> HistoricalTradeResultInfoList
        {
            get { return _historicalTradeResultInfoList; }
            set
            {
                SetProperty(ref _historicalTradeResultInfoList, value);
            }
        }

        private ObservableCollection<TradeResultInfo> _stockTradeResultInfoList;
        public ObservableCollection<TradeResultInfo> StockTradeResultInfoList
        {
            get { return _stockTradeResultInfoList; }
            set
            {
                SetProperty(ref _stockTradeResultInfoList, value);
            }
        }

        private ObservableCollection<TradeResultInfo> _futureTradeResultInfoList;
        public ObservableCollection<TradeResultInfo> FutureTradeResultInfoList
        {
            get { return _futureTradeResultInfoList; }
            set
            {
                SetProperty(ref _futureTradeResultInfoList, value);
            }
        }

        private ObservableCollection<TradeResultInfo> _optionTradeResultInfoList;
        public ObservableCollection<TradeResultInfo> OptionTradeResultInfoList
        {
            get { return _optionTradeResultInfoList; }
            set
            {
                SetProperty(ref _optionTradeResultInfoList, value);
            }
        }

        private ObservableCollection<TradeResultInfo> _fundTradeResultInfoList;
        public ObservableCollection<TradeResultInfo> FundTradeResultInfoList
        {
            get { return _fundTradeResultInfoList; }
            set
            {
                SetProperty(ref _fundTradeResultInfoList, value);
            }
        }
    }

    [Export]
    public class InstrumentInfoCollection : BindableBase
    {
        public InstrumentInfoCollection()
        {
            InstrumentInfoList = new ObservableCollection<FutureInstrumentInfo>();
        }

        private ObservableCollection<FutureInstrumentInfo> _instrumentInfoList;
        public ObservableCollection<FutureInstrumentInfo> InstrumentInfoList
        {
            get { return _instrumentInfoList; }
            set
            {
                SetProperty(ref _instrumentInfoList, value);
            }
        }
    }

    [Export]
    public class EtfBaseInfoCollection : BindableBase
    {
        public EtfBaseInfoCollection()
        {
            EtfBaseInfoList = new ObservableCollection<EtfBaseInfo>();
        }

        private ObservableCollection<EtfBaseInfo> _etfBaseInfoList;
        public ObservableCollection<EtfBaseInfo> EtfBaseInfoList
        {
            get { return _etfBaseInfoList; }
            set
            {
                SetProperty(ref _etfBaseInfoList, value);
            }
        }
    }


    [Export]
    public class EtfStockCollection : BindableBase
    {
        public EtfStockCollection()
        {
            EtfStockList = new ObservableCollection<EtfStock>();
        }

        private ObservableCollection<EtfStock> _etfStockList;
        public ObservableCollection<EtfStock> EtfStockList
        {
            get { return _etfStockList; }
            set
            {
                SetProperty(ref _etfStockList, value);
            }
        }
    }
}

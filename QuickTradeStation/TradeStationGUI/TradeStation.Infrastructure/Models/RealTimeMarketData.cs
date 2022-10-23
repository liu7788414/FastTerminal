using System;
using System.Linq;

using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Models
{
    public class RealTimeMarketDataPoint : BindableBase
    {
        #region Constructors

        public RealTimeMarketDataPoint()
            : this(new DateTime(2010, 1, 1), 100, 120, 90, 110, 1000)
        {
            _priceRatio = 1;
            IsTrueDataPoint = false;
        }

        public RealTimeMarketDataPoint(DateTime date, double open, double high, double low, double close, double volume)
        {
            _priceRatio = 1;
            _exchangeTime = date;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = volume;
        }

        #endregion

        #region Properties

        public int LastOneInsert { get; set; }

        public bool IsTrueDataPoint { get; set; }

        private double _priceRatio;
        public double PriceRatio
        {
            get { return _priceRatio; }
            set
            {
                SetProperty(ref _priceRatio, value);

                DisplayedOpen = Open * _priceRatio;
                DisplayedClose = Close * _priceRatio;
                DisplayedHigh = High * _priceRatio;
                DisplayedLow = Low * _priceRatio;
                DisplayedPreClose = PreClose * _priceRatio;
                DisplayedLast = Last * _priceRatio;
            }
        }

        private int _sequence;
        public int Sequence
        {
            get { return _sequence; }
            set
            {
                SetProperty(ref _sequence, value);
            }
        }

        private long _index;
        public long Index
        {
            get { return _index; }
            set
            {
                SetProperty(ref _index, value);
            }
        }

        #region Stored Price

        private double _open;
        public double Open
        {
            get { return _open; }
            set
            {
                SetProperty(ref _open, value);

                DisplayedOpen = Open * _priceRatio;
            }
        }

        private double _close;
        public double Close
        {
            get { return _close; }
            set
            {
                SetProperty(ref _close, value);

                DisplayedClose = Close * _priceRatio;
            }
        }

        private double _high;
        public double High
        {
            get { return _high; }
            set
            {
                SetProperty(ref _high, value);

                DisplayedHigh = High * _priceRatio;
            }
        }

        private double _low;
        public double Low
        {
            get { return _low; }
            set
            {
                SetProperty(ref _low, value);

                DisplayedLow = Low * _priceRatio;
            }

        }

        private double _preClose;
        public double PreClose
        {
            get { return _preClose; }
            set
            {
                SetProperty(ref _preClose, value);

                DisplayedPreClose = PreClose * _priceRatio;
            }
        }

        private double _last;
        public double Last
        {
            get { return _last; }
            set
            {
                SetProperty(ref _last, value);

                DisplayedLast = Last * _priceRatio;
            }
        }

        #endregion

        #region Displayed Price

        private double _displayedOpen;
        public double DisplayedOpen
        {
            get { return _displayedOpen; }
            set
            {
                SetProperty(ref _displayedOpen, value);
            }
        }

        private double _displayedClose;
        public double DisplayedClose
        {
            get { return _displayedClose; }
            set
            {
                SetProperty(ref _displayedClose, value);
            }
        }

        private double _displayedHigh;
        public double DisplayedHigh
        {
            get { return _displayedHigh; }
            set
            {
                SetProperty(ref _displayedHigh, value);
            }
        }

        private double _displayedLow;
        public double DisplayedLow
        {
            get { return _displayedLow; }
            set
            {
                SetProperty(ref _displayedLow, value);
            }

        }

        private double _displayedPreClose;
        public double DisplayedPreClose
        {
            get { return _displayedPreClose; }
            set
            {
                SetProperty(ref _displayedPreClose, value);
            }
        }

        private double _displayedLast;
        public double DisplayedLast
        {
            get { return _displayedLast; }
            set
            {
                SetProperty(ref _displayedLast, value);
            }
        }

        #endregion

        private double _turnover;
        public double Turnover
        {
            get { return _turnover; }
            set
            {
                SetProperty(ref _turnover, value);
            }
        }

        private double _preSumTurnover;
        public double PreSumTurnover
        {
            get { return _preSumTurnover; }
            set
            {
                SetProperty(ref _preSumTurnover, value);
            }
        }

        private double _sumTurnover;
        public double SumTurnover
        {
            get { return _sumTurnover; }
            set
            {
                SetProperty(ref _sumTurnover, value);
            }
        }

        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set
            {
                SetProperty(ref _volume, value);
            }
        }

        private double _preSumVolume;
        public double PreSumVolume
        {
            get { return _preSumVolume; }
            set
            {
                SetProperty(ref _preSumVolume, value);
            }
        }

        private double _sumVolume;
        public double SumVolume
        {
            get { return _sumVolume; }
            set
            {
                SetProperty(ref _sumVolume, value);
            }
        }

        private double _realTimeAverage;
        public double RealTimeAverage
        {
            get { return _realTimeAverage; }
            set
            {
                SetProperty(ref _realTimeAverage, value);
            }
        }

        private double _priceChange;
        public double PriceChange
        {
            get { return _priceChange; }
            set
            {
                SetProperty(ref _priceChange, value);
            }
        }

        private double _priceChangeRate;
        public double PriceChangeRate
        {
            get { return _priceChangeRate; }
            set
            {
                SetProperty(ref _priceChangeRate, value);
            }
        }

        private DateTime _timeFromOpen;
        public DateTime TimeFromOpen
        {
            get { return _timeFromOpen; }
            set
            {
                SetProperty(ref _timeFromOpen, value);
            }
        }

        private DateTime _exchangeTime;
        public DateTime ExchangeTime
        {
            get { return _exchangeTime; }
            set
            {
                SetProperty(ref _exchangeTime, value);
            }
        }

        private DateTime _minTradingTime;
        public DateTime MinTradingTime
        {
            get { return _minTradingTime; }
            set
            {
                SetProperty(ref _minTradingTime, value);
            }
        }

        private DateTime _maxTradingTime;
        public DateTime MaxTradingTime
        {
            get { return _maxTradingTime; }
            set
            {
                SetProperty(ref _maxTradingTime, value);
            }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                SetProperty(ref _label, value);
            }
        }

        #endregion

        public new string ToString()
        {
            return String.Format("Index {0}, Open {1}, High {2}, Low {3}, Close {4}, Change {5}, Volume {6}, Date {7}", Index, Open, High, Low, Close, PriceChange, Volume, ExchangeTime);
        }
    }

    public class RealTimeMarketData : BindableBase
    {
        public RealTimeMarketData(ExSecID exSecID)
        {
            ExSecID = exSecID;
            PreClosePrice = 0.1;
            VolumeMultiplier = 1;
            ExRightType = eDisplayedExRightType.前复权;
            RealTimeMarketDataPointSets = new ObservableCollection<RealTimeMarketDataPoint>();

            IsDataReady = false;
            IsMultiplierReady = false;
        }

        private RealTimeMarketDataPoint _lastTickPoint = null;

        #region Properties

        public ExSecID ExSecID { get; set; }

        public bool IsDataReady { get; set; }

        // 交易量乘数是否已获得
        public bool IsMultiplierReady { get; set; }

        // 交易量乘数
        private double _volumeMultiplier;
        public double VolumeMultiplier
        {
            get { return _volumeMultiplier; }
            set
            {
                SetProperty(ref _volumeMultiplier, value);
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                SetProperty(ref _isOpen, value);
            }
        }

        private eDisplayedExRightType _exRightType;
        public eDisplayedExRightType ExRightType
        {
            get { return _exRightType; }
            set
            {
                SetProperty(ref _exRightType, value);
            }
        }

        private double _upLimitPrice;
        public double UpLimitPrice
        {
            get { return _upLimitPrice; }
            set
            {
                SetProperty(ref _upLimitPrice, value);
            }
        }

        private double _downLimitPrice;
        public double DownLimitPrice
        {
            get { return _downLimitPrice; }
            set
            {
                SetProperty(ref _downLimitPrice, value);
            }
        }

        private double _preClosePrice;
        public double PreClosePrice
        {
            get { return _preClosePrice; }
            set
            {
                SetProperty(ref _preClosePrice, value);
            }
        }

        private double _dayHighPrice;
        public double DayHighPrice
        {
            get { return _dayHighPrice; }
            set
            {
                SetProperty(ref _dayHighPrice, value);
            }
        }

        private double _dayLowPrice;
        public double DayLowPrice
        {
            get { return _dayLowPrice; }
            set
            {
                SetProperty(ref _dayLowPrice, value);
            }
        }

        private double _totalVolume;
        public double TotalVolume
        {
            get { return _totalVolume; }
            set
            {
                SetProperty(ref _totalVolume, value);
            }
        }

        private ObservableCollection<RealTimeMarketDataPoint> _realTimeMarketDataPointSets;
        public ObservableCollection<RealTimeMarketDataPoint> RealTimeMarketDataPointSets
        {
            get { return _realTimeMarketDataPointSets; }
            set
            {
                SetProperty(ref _realTimeMarketDataPointSets, value);
            }
        }

        #region Real Time Chart Drawing Related

        private double _priceInterval;
        public double PriceInterval
        {
            get { return _priceInterval; }
            set
            {
                SetProperty(ref _priceInterval, value);
            }
        }

        private double _visibleHighPrice;
        public double VisibleHighPrice
        {
            get { return _visibleHighPrice; }
            set
            {
                SetProperty(ref _visibleHighPrice, value);
            }
        }

        private double _visibleLowPrice;
        public double VisibleLowPrice
        {
            get { return _visibleLowPrice; }
            set
            {
                SetProperty(ref _visibleLowPrice, value);
            }
        }

        #endregion

        #endregion

        public void InitializeDataForRealTime(IList<MarketPeriodRangeModel> periods)
        {
            RealTimeMarketDataPointSets.Clear();

            var orderedPeriods = periods.OrderBy(x => x.StartTime);
            var maxDate = periods.Max(x => x.EndTime).Date;

            int index = 1;
            foreach (var period in orderedPeriods)
            {
                for (int ix = 1; ix <= period.Interval.TotalMinutes; ix++)
                {
                    RealTimeMarketDataPointSets.Add(new RealTimeMarketDataPoint()
                    {
                        Index = index,
                        TimeFromOpen = maxDate.AddMinutes(index),
                        ExchangeTime = period.StartTime.AddMinutes(ix),
                        Close = double.NaN,
                        RealTimeAverage = double.NaN,
                        Turnover = double.NaN,
                        Volume = double.NaN,
                    });
                    index++;
                }
            }
        }
    }

}

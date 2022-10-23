using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Models
{
    public class ExSecID
    {
        public string ExID;
        public string SecurityID;

        public ExSecID(string exId, string sId)
        {
            ExID = exId;
            SecurityID = sId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExSecID))
                return false;

            ExSecID other = (ExSecID)obj;
            if (ExID.Equals(other.ExID) && SecurityID.Equals(other.SecurityID, StringComparison.InvariantCultureIgnoreCase))
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return (ExID + "#" + SecurityID.ToLower()).GetHashCode();
        }
    }

    [DataContract]
    public class SecurityInfo : INotifyPropertyChanged
    {
        private SecurityStatus _securityStatus;

        [DataMember(Name = "securityStatus")]
        public SecurityStatus SecurityStatus
        {
            get
            {
                if (IsExpire)
                {
                    return SecurityStatus.退市;
                }

                if (isSuspension)
                {
                    return SecurityStatus.停牌;
                }

                return SecurityStatus.正常;
            }
            set
            {
                _securityStatus = value;
                NotifyPropertyChanged("SecurityStatus");
            }
        }

        [DataMember(Name = "exId")]
        public string ExID { get; set; }

        [DataMember(Name = "marketType")]
        public eMarketType MarketType
        {
            get
            {
                return CommonUtil.ExIdToeMarketType(ExID);
            }
            set { MarketType = value; }
        }

        [DataMember(Name = "securityId")]
        public string SecurityID { get; set; }

        private string securityName;
        [DataMember(Name = "securityName")]
        public string SecurityName
        {
            get { return securityName; }
            set
            {
                if (securityName != value)
                {
                    securityName = value;
                    ChinCharCapitals = CommonUtil.GetCapitalLetters(value);
                }
            }
        }

        [DataMember(Name = "securityType")]
        public eCategory SecurityType { get; set; }

        [DataMember(Name = "lastCpx")]
        public double LastCpx { get; set; }

        [DataMember(Name = "mfPrice")]
        public double MinFloatingPrice { get; set; }

        [DataMember(Name = "expire")]
        public bool IsExpire { get; set; }

        public string ChinCharCapitals { get; set; }

        private string _variety;
        public string Variety
        {
            get
            {
                if (string.IsNullOrEmpty(_variety))
                {
                    if (SecurityType == eCategory.期货)
                    {
                        _variety = (ExID + "_" + Regex.Replace(SecurityID, @"\d", "")).ToLower() + "_500";
                    }
                    else if (SecurityType == eCategory.期权)
                    {
                        _variety = (ExID + "_all").ToLower() + "_300";
                    }
                    else
                    {
                        _variety = (ExID + "_all").ToLower() + "_100";
                    }
                }

                return _variety;
            }
        }

        // Temp solution: This is for security to display correct digit of decimal.
        private int priceDigits;
        public int PriceDigits
        {
            get { return priceDigits; }
            set
            {
                priceDigits = value;
                NotifyPropertyChanged("PriceDigits");
            }
        }

        private bool isSuspension;
        public bool IsSuspension
        {
            get { return isSuspension; }
            set
            {
                isSuspension = value;
                NotifyPropertyChanged("IsSuspension");
            }
        }

        private SecurityQuotation quotation;
        public SecurityQuotation Quotation
        {
            get { return quotation; }
            set
            {
                if (quotation != value)
                {
                    quotation = value;
                    NotifyPropertyChanged("Quotation");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class SecurityQuotation : BindableBase
    {
        public SecurityQuotation() { }
        public SecurityQuotation(string exId, string secId)
        {
            ExSecurityID = new ExSecID(exId, secId);
        }
        public ExSecID ExSecurityID { get; private set; }

        public HsStock Trader { get; set; }
        private double lastPx;
        public double LastPx
        {
            get { return lastPx; }
            set
            {
                SetProperty(ref lastPx, value);

                if (PreClosePx > 0 && value > 0)
                {
                    Diff = value - PreClosePx;
                    DiffPercent = Diff / PreClosePx;
                }
                else
                {
                    Diff = 0;
                    DiffPercent = 0;
                }
            }
        }

        private double upLimitPx;
        public double UpLimitPx
        {
            get { return upLimitPx; }
            set
            {
                SetProperty(ref upLimitPx, value);
            }
        }

        private double downLimitPx;
        public double DownLimitPx
        {
            get { return downLimitPx; }
            set
            {
                SetProperty(ref downLimitPx, value);
            }
        }

        private double highPx;
        public double HighPx
        {
            get { return highPx; }
            set
            {
                SetProperty(ref highPx, value);
            }
        }

        private double lowPx;
        public double LowPx
        {
            get { return lowPx; }
            set
            {
                SetProperty(ref lowPx, value);
            }
        }

        private double openPx;
        public double OpenPx
        {
            get { return openPx; }
            set
            {
                SetProperty(ref openPx, value);
            }
        }

        private double preClosePx;
        public double PreClosePx
        {
            get { return preClosePx; }
            set
            {
                SetProperty(ref preClosePx, value);
            }
        }

        private double diff;
        public double Diff
        {
            get { return diff; }
            set
            {
                SetProperty(ref diff, value);
            }
        }

        private double diffPercent;
        public double DiffPercent
        {
            get { return diffPercent; }
            set
            {
                SetProperty(ref diffPercent, value);
            }
        }

        private Int64 tradeVol;
        public Int64 TradeVol
        {
            get { return tradeVol; }
            set
            {
                SetProperty(ref tradeVol, value);
            }
        }

        private double turnover;
        public double Turnover
        {
            get { return turnover; }
            set
            {
                SetProperty(ref turnover, value);
            }
        }

        private int tickTmInt;
        public int TickTmInt
        {
            get { return tickTmInt; }
            set
            {
                SetProperty(ref tickTmInt, value);
            }
        }

        #region Depth level quotes

        // 买一价
        private double bidPx1;
        public double BidPx1
        {
            get { return bidPx1; }
            set
            {
                SetProperty(ref bidPx1, value);
            }
        }

        // 卖一价
        private double askPx1;
        public double AskPx1
        {
            get { return askPx1; }
            set
            {
                SetProperty(ref askPx1, value);
            }
        }

        private long bidVol1;
        public long BidVol1
        {
            get { return bidVol1; }
            set
            {
                SetProperty(ref bidVol1, value);
            }
        }

        private long askVol1;
        public long AskVol1
        {
            get { return askVol1; }
            set
            {
                SetProperty(ref askVol1, value);
            }
        }

        private double bidPx2;
        public double BidPx2
        {
            get { return bidPx2; }
            set
            {
                SetProperty(ref bidPx2, value);
            }
        }

        private double askPx2;
        public double AskPx2
        {
            get { return askPx2; }
            set
            {
                SetProperty(ref askPx2, value);
            }
        }

        private long bidVol2;
        public long BidVol2
        {
            get { return bidVol2; }
            set
            {
                SetProperty(ref bidVol2, value);
            }
        }

        private long askVol2;
        public long AskVol2
        {
            get { return askVol2; }
            set
            {
                SetProperty(ref askVol2, value);
            }
        }

        private double bidPx3;
        public double BidPx3
        {
            get { return bidPx3; }
            set
            {
                SetProperty(ref bidPx3, value);
            }
        }

        private double askPx3;
        public double AskPx3
        {
            get { return askPx3; }
            set
            {
                SetProperty(ref askPx3, value);
            }
        }

        private long bidVol3;
        public long BidVol3
        {
            get { return bidVol3; }
            set
            {
                SetProperty(ref bidVol3, value);
            }
        }

        private long askVol3;
        public long AskVol3
        {
            get { return askVol3; }
            set
            {
                SetProperty(ref askVol3, value);
            }
        }

        private double bidPx4;
        public double BidPx4
        {
            get { return bidPx4; }
            set
            {
                SetProperty(ref bidPx4, value);
            }
        }

        private double askPx4;
        public double AskPx4
        {
            get { return askPx4; }
            set
            {
                SetProperty(ref askPx4, value);
            }
        }

        private long bidVol4;
        public long BidVol4
        {
            get { return bidVol4; }
            set
            {
                SetProperty(ref bidVol4, value);
            }
        }

        private long askVol4;
        public long AskVol4
        {
            get { return askVol4; }
            set
            {
                SetProperty(ref askVol4, value);
            }
        }

        private double bidPx5;
        public double BidPx5
        {
            get { return bidPx5; }
            set
            {
                SetProperty(ref bidPx5, value);
            }
        }

        private double askPx5;
        public double AskPx5
        {
            get { return askPx5; }
            set
            {
                SetProperty(ref askPx5, value);
            }
        }

        private long bidVol5;
        public long BidVol5
        {
            get { return bidVol5; }
            set
            {
                SetProperty(ref bidVol5, value);
            }
        }

        private long askVol5;
        public long AskVol5
        {
            get { return askVol5; }
            set
            {
                SetProperty(ref askVol5, value);
            }
        }

        private double bidPx6;
        public double BidPx6
        {
            get { return bidPx6; }
            set
            {
                SetProperty(ref bidPx6, value);
            }
        }

        private double askPx6;
        public double AskPx6
        {
            get { return askPx6; }
            set
            {
                SetProperty(ref askPx6, value);
            }
        }

        private long bidVol6;
        public long BidVol6
        {
            get { return bidVol6; }
            set
            {
                SetProperty(ref bidVol6, value);
            }
        }

        private long askVol6;
        public long AskVol6
        {
            get { return askVol6; }
            set
            {
                SetProperty(ref askVol6, value);
            }
        }

        private double bidPx7;
        public double BidPx7
        {
            get { return bidPx7; }
            set
            {
                SetProperty(ref bidPx7, value);
            }
        }

        private double askPx7;
        public double AskPx7
        {
            get { return askPx7; }
            set
            {
                SetProperty(ref askPx7, value);
            }
        }

        private long bidVol7;
        public long BidVol7
        {
            get { return bidVol7; }
            set
            {
                SetProperty(ref bidVol7, value);
            }
        }

        private long askVol7;
        public long AskVol7
        {
            get { return askVol7; }
            set
            {
                SetProperty(ref askVol7, value);
            }
        }

        private double bidPx8;
        public double BidPx8
        {
            get { return bidPx8; }
            set
            {
                SetProperty(ref bidPx8, value);
            }
        }

        private double askPx8;
        public double AskPx8
        {
            get { return askPx8; }
            set
            {
                SetProperty(ref askPx8, value);
            }
        }

        private long bidVol8;
        public long BidVol8
        {
            get { return bidVol8; }
            set
            {
                SetProperty(ref bidVol8, value);
            }
        }

        private long askVol8;
        public long AskVol8
        {
            get { return askVol8; }
            set
            {
                SetProperty(ref askVol8, value);
            }
        }

        private double bidPx9;
        public double BidPx9
        {
            get { return bidPx9; }
            set
            {
                SetProperty(ref bidPx9, value);
            }
        }

        private double askPx9;
        public double AskPx9
        {
            get { return askPx9; }
            set
            {
                SetProperty(ref askPx9, value);
            }
        }

        private long bidVol9;
        public long BidVol9
        {
            get { return bidVol9; }
            set
            {
                SetProperty(ref bidVol9, value);
            }
        }

        private long askVol9;
        public long AskVol9
        {
            get { return askVol9; }
            set
            {
                SetProperty(ref askVol9, value);
            }
        }

        private double bidPx10;
        public double BidPx10
        {
            get { return bidPx10; }
            set
            {
                SetProperty(ref bidPx10, value);
            }
        }

        private double askPx10;
        public double AskPx10
        {
            get { return askPx10; }
            set
            {
                SetProperty(ref askPx10, value);
            }
        }

        private long bidVol10;
        public long BidVol10
        {
            get { return bidVol10; }
            set
            {
                SetProperty(ref bidVol10, value);
            }
        }

        private long askVol10;
        public long AskVol10
        {
            get { return askVol10; }
            set
            {
                SetProperty(ref askVol10, value);
            }
        }

        private double auctionPrice;
        public double AuctionPrice
        {
            get { return auctionPrice; }
            set { SetProperty(ref auctionPrice, value); }
        }

        private double _bidPriceImpV1;
        public double BidPriceImpV1
        {
            get { return _bidPriceImpV1; }
            set
            {
                SetProperty(ref _bidPriceImpV1, value);
            }
        }

        private double _askPriceImpV1;
        public double AskPriceImpV1
        {
            get { return _askPriceImpV1; }
            set
            {
                SetProperty(ref _askPriceImpV1, value);
            }
        }

        #endregion

        public void UpdateQuote(StockTickDataModel tickData)
        {
            OpenPx = tickData.OpenPrice;
            PreClosePx = tickData.PreClosePrice;
            UpLimitPx = tickData.UpLimitPx;
            DownLimitPx = tickData.DownLimitPx;
            HighPx = tickData.HighPrice;
            LowPx = tickData.LowPrice;
            LastPx = tickData.ClosePrice;

            Turnover = tickData.Turnover;
            TradeVol = tickData.Volume;

            AskPx1 = tickData.AskPrice[0];
            AskVol1 = tickData.AskVolume[0];
            BidPx1 = tickData.BidPrice[0];
            BidVol1 = tickData.BidVolume[0];

            AskPx2 = tickData.AskPrice[1];
            AskVol2 = tickData.AskVolume[1];
            BidPx2 = tickData.BidPrice[1];
            BidVol2 = tickData.BidVolume[1];

            AskPx3 = tickData.AskPrice[2];
            AskVol3 = tickData.AskVolume[2];
            BidPx3 = tickData.BidPrice[2];
            BidVol3 = tickData.BidVolume[2];

            AskPx4 = tickData.AskPrice[3];
            AskVol4 = tickData.AskVolume[3];
            BidPx4 = tickData.BidPrice[3];
            BidVol4 = tickData.BidVolume[3];

            AskPx5 = tickData.AskPrice[4];
            AskVol5 = tickData.AskVolume[4];
            BidPx5 = tickData.BidPrice[4];
            BidVol5 = tickData.BidVolume[4];

            AskPx6 = tickData.AskPrice[5];
            AskVol6 = tickData.AskVolume[5];
            BidPx6 = tickData.BidPrice[5];
            BidVol6 = tickData.BidVolume[5];

            AskPx7 = tickData.AskPrice[6];
            AskVol7 = tickData.AskVolume[6];
            BidPx7 = tickData.BidPrice[6];
            BidVol7 = tickData.BidVolume[6];

            AskPx8 = tickData.AskPrice[7];
            AskVol8 = tickData.AskVolume[7];
            BidPx8 = tickData.BidPrice[7];
            BidVol8 = tickData.BidVolume[7];

            AskPx9 = tickData.AskPrice[8];
            AskVol9 = tickData.AskVolume[8];
            BidPx9 = tickData.BidPrice[8];
            BidVol9 = tickData.BidVolume[8];

            AskPx10 = tickData.AskPrice[9];
            AskVol10 = tickData.AskVolume[9];
            BidPx10 = tickData.BidPrice[9];
            BidVol10 = tickData.BidVolume[9];

            if (Trader != null)
            {
                var q = from pi in Trader.PositionInfoCollection.StockPositionInfoList
                    where pi.SecurityID.Equals(ExSecurityID.SecurityID)
                    select pi;

                if (q.Count() > 0)
                {
                    foreach (var pi in q)
                    {
                        if (pi.CurrentAmount > 0)
                        {
                            pi.DynamicProfit = (LastPx - pi.CurrentCostPrice)*pi.CurrentAmount;
                        }

                    }
                }
            }
        }

        public void UpdateQuote(OptionDataModel optionData)
        {
            OpenPx = optionData.OpenPrice;
            LastPx = optionData.LastPrice;
            UpLimitPx = optionData.UpLimitPx;
            DownLimitPx = optionData.DownLimitPx;
            HighPx = optionData.HighPrice;
            LowPx = optionData.LowPrice;
            PreClosePx = optionData.PreClosePrice;

            TradeVol = optionData.Volume;
            Turnover = optionData.Turnover;

            AskPx1 = optionData.AskPrice[0];
            AskVol1 = optionData.AskVolume[0];
            BidPx1 = optionData.BidPrice[0];
            BidVol1 = optionData.BidVolume[0];

            AskPx2 = optionData.AskPrice[1];
            AskVol2 = optionData.AskVolume[1];
            BidPx2 = optionData.BidPrice[1];
            BidVol2 = optionData.BidVolume[1];

            AskPx3 = optionData.AskPrice[2];
            AskVol3 = optionData.AskVolume[2];
            BidPx3 = optionData.BidPrice[2];
            BidVol3 = optionData.BidVolume[2];

            AskPx4 = optionData.AskPrice[3];
            AskVol4 = optionData.AskVolume[3];
            BidPx4 = optionData.BidPrice[3];
            BidVol4 = optionData.BidVolume[3];

            AskPx5 = optionData.AskPrice[4];
            AskVol5 = optionData.AskVolume[4];
            BidPx5 = optionData.BidPrice[4];
            BidVol5 = optionData.BidVolume[4];

            AskPx6 = optionData.AskPrice[5];
            AskVol6 = optionData.AskVolume[5];
            BidPx6 = optionData.BidPrice[5];
            BidVol6 = optionData.BidVolume[5];

            AskPx7 = optionData.AskPrice[6];
            AskVol7 = optionData.AskVolume[6];
            BidPx7 = optionData.BidPrice[6];
            BidVol7 = optionData.BidVolume[6];

            AskPx8 = optionData.AskPrice[7];
            AskVol8 = optionData.AskVolume[7];
            BidPx8 = optionData.BidPrice[7];
            BidVol8 = optionData.BidVolume[7];

            AskPx9 = optionData.AskPrice[8];
            AskVol9 = optionData.AskVolume[8];
            BidPx9 = optionData.BidPrice[8];
            BidVol9 = optionData.BidVolume[8];

            AskPx10 = optionData.AskPrice[9];
            AskVol10 = optionData.AskVolume[9];
            BidPx10 = optionData.BidPrice[9];
            BidVol10 = optionData.BidVolume[9];

            AuctionPrice = optionData.AuctionPrice;

            AskPriceImpV1 = optionData.AskPriceImpliedVolatility[0];
            BidPriceImpV1 = optionData.BidPriceImpliedVolatility[0];

            if (Trader != null)
            {
                var q = from pi in Trader.PositionInfoCollection.OptionPositionInfoList
                        where pi.SecurityID.Equals(ExSecurityID.SecurityID)
                        select pi;

                if (q.Count() > 0)
                {
                    foreach (var pi in q)
                    {
                        if (pi.CurrentAmount > 0)
                        {
                            if (pi.PositionFlag == ePositionFlag.多头持仓)
                            {
                                pi.DynamicProfit = (Math.Abs(LastPx) - Math.Abs(pi.CurrentCostPrice)) * pi.CurrentAmount * CommonUtil.OptionAmountPerHand;
                            }
                            else
                            {
                                if (pi.PositionFlag == ePositionFlag.空头持仓)
                                {
                                    pi.DynamicProfit = (Math.Abs(pi.CurrentCostPrice) -Math.Abs(LastPx)) * pi.CurrentAmount * CommonUtil.OptionAmountPerHand;
                                }
                            }
                        }

                    }
                }
            }
        }

        public void UpdateQuote(FutureDataModel FutureData)
        {
            OpenPx = FutureData.OpenPrice;
            LastPx = FutureData.LastPrice;
            UpLimitPx = FutureData.UpLimitPx;
            DownLimitPx = FutureData.DownLimitPx;
            HighPx = FutureData.HighPrice;
            LowPx = FutureData.LowPrice;
            PreClosePx = FutureData.PreClosePrice;

            TradeVol = FutureData.Volume;
            Turnover = FutureData.Turnover;

            AskPx1 = FutureData.AskPrice[0];
            AskVol1 = FutureData.AskVolume[0];
            BidPx1 = FutureData.BidPrice[0];
            BidVol1 = FutureData.BidVolume[0];

            AskPx2 = FutureData.AskPrice[1];
            AskVol2 = FutureData.AskVolume[1];
            BidPx2 = FutureData.BidPrice[1];
            BidVol2 = FutureData.BidVolume[1];

            AskPx3 = FutureData.AskPrice[2];
            AskVol3 = FutureData.AskVolume[2];
            BidPx3 = FutureData.BidPrice[2];
            BidVol3 = FutureData.BidVolume[2];

            AskPx4 = FutureData.AskPrice[3];
            AskVol4 = FutureData.AskVolume[3];
            BidPx4 = FutureData.BidPrice[3];
            BidVol4 = FutureData.BidVolume[3];

            AskPx5 = FutureData.AskPrice[4];
            AskVol5 = FutureData.AskVolume[4];
            BidPx5 = FutureData.BidPrice[4];
            BidVol5 = FutureData.BidVolume[4];

            AskPx6 = FutureData.AskPrice[5];
            AskVol6 = FutureData.AskVolume[5];
            BidPx6 = FutureData.BidPrice[5];
            BidVol6 = FutureData.BidVolume[5];

            AskPx7 = FutureData.AskPrice[6];
            AskVol7 = FutureData.AskVolume[6];
            BidPx7 = FutureData.BidPrice[6];
            BidVol7 = FutureData.BidVolume[6];

            AskPx8 = FutureData.AskPrice[7];
            AskVol8 = FutureData.AskVolume[7];
            BidPx8 = FutureData.BidPrice[7];
            BidVol8 = FutureData.BidVolume[7];

            AskPx9 = FutureData.AskPrice[8];
            AskVol9 = FutureData.AskVolume[8];
            BidPx9 = FutureData.BidPrice[8];
            BidVol9 = FutureData.BidVolume[8];

            AskPx10 = FutureData.AskPrice[9];
            AskVol10 = FutureData.AskVolume[9];
            BidPx10 = FutureData.BidPrice[9];
            BidVol10 = FutureData.BidVolume[9];

            if (Trader != null)
            {
                var q = from pi in Trader.PositionInfoCollection.FuturePositionInfoList
                        where pi.SecurityID.Equals(ExSecurityID.SecurityID)
                        select pi;

                if (q.Count() > 0)
                {
                    foreach (var pi in q)
                    {
                        if (pi.CurrentAmount > 0)
                        {
                            if (pi.PositionFlag == ePositionFlag.多头持仓)
                            {
                                pi.DynamicProfit = (LastPx - pi.CurrentCostPrice)*pi.CurrentAmount;
                            }
                            else
                            {
                                if (pi.PositionFlag == ePositionFlag.空头持仓)
                                {
                                    pi.DynamicProfit = (pi.CurrentCostPrice - LastPx)*pi.CurrentAmount;
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}

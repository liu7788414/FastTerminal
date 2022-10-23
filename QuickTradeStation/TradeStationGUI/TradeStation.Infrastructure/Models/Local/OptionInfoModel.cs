using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Models.Local
{
    public class OptionInfoDataModel
    {
        public string SecurityID { get; set; } // 期权合约的产品代码

        public string ExID { get; set; } // 期权交易所

        public string ContractID { get; set; } // 期权合约代码

        public string SecuritySymbol { get; set; } // 期权合约简称

        public string UnderlyingSecurityId { get; set; } // 标的证券代码

        public string UnderlyingSymbol { get; set; } // 基础证券证券名称

        public eUnderlyingType UnderlyingType { get; set; } // 标的证券类型

        public eOptionStyleType OptionType { get; set; } // 欧式美式

        public eOptionType CallOrPut { get; set; } // 认购认沽

        public long ContractMultiplierUnit { get; set; } // 合约单位

        public double ExercisePrice { get; set; } // 期权行权价

        public DateTime StartDate { get; set; } // 首个交易日

        public DateTime EndDate { get; set; } // 最后交易日

        public DateTime ExerciseDate { get; set; } // 期权行权日

        public DateTime DeliveryDate { get; set; } // 行权交割日

        public DateTime ExpireDate { get; set; } // 期权到期日

        public string UpdateVersion { get; set; } // 合约版本号

        public long TotalLongPosition { get; set; } // 当前合约未平仓数

        public double SecurityClosePx { get; set; } // 合约前收盘价

        public double SettlePrice { get; set; } // 合约前结算价

        public double UnderlyingClosePx { get; set; } // 标的证券前收盘

        public string PriceLimitType { get; set; } // 涨跌幅限制类型

        public double DailyPriceUpLimit { get; set; } // 涨幅上限价格

        public double DailyPriceDownLimit { get; set; } // 跌幅下限价格

        public double MarginUnit { get; set; } // 单位保证金

        public double MarginRatioParam1 { get; set; } // 保证金计算比例参数一

        public double MarginRatioParam2 { get; set; } // 保证金计算比例参数二

        public long RoundLot { get; set; } // 整手数

        public long LimitOrderMinFloor { get; set; } // 单笔限价申报下限

        public long LimitOrderMaxFloor { get; set; } // 单笔限价申报上限

        public long MarketOrderMinFloor { get; set; } // 单笔市价申报下限

        public long MarketOrderMaxFloor { get; set; } // 单笔市价申报上限

        public double TickSize { get; set; } // 最小报价单位

        public string SecurityStatusFlag { get; set; } // 期权合约状态信息标签

    }

    public class OptionInfoModel : BindableBase, IExport
    {
        public MarketDataService MarketDataService { get; set; }

        private string securityID;
        public string SecurityID
        {
            get { return securityID; }
            set
            {
                SetProperty(ref securityID, value);
            }
        }

        private string _exID;
        public string ExID
        {
            get { return _exID; }
            set
            {
                SetProperty(ref _exID, value);
            }
        }

        private string contractID;
        public string ContractID
        {
            get { return contractID; }
            set
            {
                SetProperty(ref contractID, value);
            }
        }

        private string securitySymbol;
        public string SecuritySymbol
        {
            get { return securitySymbol; }
            set
            {
                SetProperty(ref securitySymbol, value);
            }
        }

        private string underlyingSecurityId;
        public string UnderlyingSecurityId
        {
            get { return underlyingSecurityId; }
            set
            {
                SetProperty(ref underlyingSecurityId, value);
            }
        }

        private string underlyingSymbol;
        public string UnderlyingSymbol
        {
            get { return underlyingSymbol; }
            set
            {
                SetProperty(ref underlyingSymbol, value);
            }
        }

        private eUnderlyingType underlyingType;
        public eUnderlyingType UnderlyingType
        {
            get { return underlyingType; }
            set
            {
                SetProperty(ref underlyingType, value);
            }
        }

        private eOptionType callOrPut;
        public eOptionType CallOrPut
        {
            get { return callOrPut; }
            set
            {
                SetProperty(ref callOrPut, value);
            }
        }

        private long contractMultiplierUnit;
        public long ContractMultiplierUnit
        {
            get { return contractMultiplierUnit; }
            set
            {
                SetProperty(ref contractMultiplierUnit, value);
            }
        }

        private double exercisePrice;
        public double ExercisePrice
        {
            get { return exercisePrice; }
            set
            {
                SetProperty(ref exercisePrice, value);
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

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                SetProperty(ref startDate, value);
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                SetProperty(ref endDate, value);
            }
        }

        private DateTime exerciseDate;
        public DateTime ExerciseDate
        {
            get { return exerciseDate; }
            set
            {
                SetProperty(ref exerciseDate, value);
            }
        }

        private DateTime deliveryDate;
        public DateTime DeliveryDate
        {
            get { return deliveryDate; }
            set
            {
                SetProperty(ref deliveryDate, value);
            }
        }

        private DateTime expireDate;
        public DateTime ExpireDate
        {
            get { return expireDate; }
            set
            {
                SetProperty(ref expireDate, value);
            }
        }

        private double settlePrice;
        public double SettlePrice
        {
            get { return settlePrice; }
            set
            {
                SetProperty(ref settlePrice, value);
            }
        }

        private double underlyingClosePx;
        public double UnderlyingClosePx
        {
            get { return underlyingClosePx; }
            set
            {
                SetProperty(ref underlyingClosePx, value);
            }
        }

        private double dailyPriceUpLimit;
        public double DailyPriceUpLimit
        {
            get { return dailyPriceUpLimit; }
            set
            {
                SetProperty(ref dailyPriceUpLimit, value);
            }
        }

        private double dailyPriceDownLimit;
        public double DailyPriceDownLimit
        {
            get { return dailyPriceDownLimit; }
            set
            {
                SetProperty(ref dailyPriceDownLimit, value);
            }
        }

        private long limitOrderMaxFloor;
        public long LimitOrderMaxFloor
        {
            get { return limitOrderMaxFloor; }
            set
            {
                SetProperty(ref limitOrderMaxFloor, value);
            }
        }

        private long marketOrderMaxFloor;
        public long MarketOrderMaxFloor
        {
            get { return marketOrderMaxFloor; }
            set
            {
                SetProperty(ref marketOrderMaxFloor, value);
            }
        }

        private SecurityQuotation _quotation;
        public SecurityQuotation Quotation
        {
            get
            {
                try
                {
                    return MarketDataService.GetAndSubscribeSecurityQuote(new ExSecID("SH", SecurityID));
                }
                catch (Exception)
                {
                    return null;
                }

            }
            set { SetProperty(ref _quotation, value); }
        }

        private string _plainText;
        public string PlainText
        {
            get { return _plainText; }
            set
            {
                SetProperty(ref _plainText, value);
            }
        }

        private eNodeType _nodeType;
        public eNodeType NodeType
        {
            get { return _nodeType; }
            set
            {
                SetProperty(ref _nodeType, value);
            }
        }

        public string GetTitle()
        {
            return "期权合约的产品代码, 期权合约代码, 期权合约简称, 标的证券代码, 基础证券证券名称, 认购认沽, 合约单位, 期权行权价, 首个交易日, 最后交易日, 期权行权日, 行权交割日, 期权到期日, 合约前结算价, 标的证券前收盘, 涨幅上限价格, 跌幅下限价格, 单笔限价申报上限, 单笔市价申报上限";
        }

        public string Export()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", SecurityID, ContractID, SecuritySymbol, UnderlyingSecurityId, UnderlyingSymbol, CallOrPut, ContractMultiplierUnit, ExercisePrice, StartDate, EndDate, ExerciseDate, DeliveryDate, ExpireDate, SettlePrice, UnderlyingClosePx, DailyPriceUpLimit, DailyPriceDownLimit, LimitOrderMaxFloor, MarketOrderMaxFloor);
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OptionInfoModelCollection : BindableBase
    {
        public OptionInfoModelCollection()
        {
            OptionInfoList = new ObservableCollection<OptionInfoModel>();
        }

        private ObservableCollection<OptionInfoModel> optionInfoList;
        public ObservableCollection<OptionInfoModel> OptionInfoList
        {
            get { return optionInfoList; }
            set
            {
                SetProperty(ref optionInfoList, value);
            }
        }
    }
}

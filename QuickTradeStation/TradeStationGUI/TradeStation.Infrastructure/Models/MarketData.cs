using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TradeStation.Infrastructure.Models
{
    public enum MarketDataTag
    {
        STOCK_TICKDATA_TAG = 100,
        INDEX_TICKDATA_TAG = 200,
        OPTION_TAG = 300,
        OPTION_SIMU_TAG = 400,
        STOCK_TRANSACTION_TAG = 500,
        FUTURE_TICKDATA_TAG = 600
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct StockTickData
    {
        public Int32 Type;				// 行情类型
        public UInt32 Seq;		        // 消息序号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string ExID;             // 交易所代码

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SecID;            // 证券代码

        public Int32 ExTime;			// 交易所时间,Int格式: 10:01:02 000 = 100102000
        public Int32 LocalTime;			// 本地时间
        public Int32 LocalDate;         // 本地日期

        public Int64 PreClosePx;		// 前收盘
        public Int64 OpenPx;			// 当日开盘价
        public Int64 HighPx;			// 当日最高价
        public Int64 LowPx;			    // 当日最低价
        public Int64 LastPx;			// 最新价
        public Int64 UpLimitPx;		    // 涨停
        public Int64 DownLimitPx;		// 跌停

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Int64[] BidPx;		    // 委买价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Int64[] BidVol;		    // 委买量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Int64[] OfferPx;		    // 委卖价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Int64[] OfferVol;		// 委卖量
        public Int64 Volume;			// 成交量
        public double Turnover;         // 成交金额
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct IndexData
    {
        public Int32 Type;				// 行情类型
        public UInt32 Seq;		        // 消息序号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string ExID;             // 交易所代码

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SecID;            // 证券代码

        public Int32 ExTime;			// 交易所时间,Int格式: 10:01:02 000 = 100102000
        public Int32 LocalTime;			// 本地时间
        public Int32 LocalDate;         // 本地日期
        public Int64 PreCloseIndex;		// 前收盘指数
        public Int64 OpenIndex;			// 今日开盘指数
        public Int64 HighIndex;			// 最高指数
        public Int64 LowIndex;			    // 最低指数
        public Int64 LastIndex;			// 最新指数
        public Int32 TradeTime;         //成交时间

        public double Turnover;                 // 参与计算相应指数的成交金额
        public Int64 TotalVolumeTraded;			// 参与计算相应指数的交易数量
        public Int64 CloseIndex;                // 今日收盘指数（大于0为有效值）
        public char Currency;                   // 币种 - 0：人民币; 1：港币; 2：美元; 3：台币; 4：日元
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct OptionData
    {
        public Int32 Type;                  // 行情类型
        public UInt32 Seq;                  // 消息序号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string ExID;                 // 交易所代码

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string SecID;                // 期权代码
        public Int32 ExTime;                // 交易所时间,Int格式: 10:01:02 000 = 100102000
        public Int32 LocalTime;             // 本地时间
        public Int32 LocalDate;             // 本地日期

        public Int64 PreClosePx;            // 昨收价
        public Int64 ClosePx;               // 收盘价
        public Int64 OpenPx;                // 开盘价
        public Int64 HighPx;                // 最高价
        public Int64 LowPx;                 // 最低价
        public Int64 LastPx;                // 现价
        public Int64 AuctionPrice;          // 波动性中断参考价
        public Int64 AuctionQty;            // 波动性中断集合竞价虚拟匹配量
        public Int64 TotalLongPosition;     // 总持仓量

        //五档行情
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] BidPx;		         // 委买价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] BidVol;			 // 委买量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] OfferPx;		     // 委卖价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] OfferVol;		     // 委卖量

        public Int64 Volume;				 // 成交量
        public double Turnover;             // 成交额

        /* 该字段为8位字符串，左起每位表示特定的含义，无定义则填空格。
        第1 位：‘S’表示启动（开市前）时段，‘C’表示集合竞价时段，‘T’表示连续交易时段，‘B’表示休市时段，‘E’表示闭市时段，‘V’表示波动性中断，‘P’表示临时停牌，‘U’表示收盘集合竞价，‘M’表示可恢复交易的熔断（盘中集合竞价）, ‘N’表示不可恢复交易的熔断（暂停交易至闭市）
        第2 位：‘0’表示未连续停牌，‘1’表示连续停牌。（保留，暂不使用；暂时填空格）
        第3位：‘0’表示不限制开仓，‘1’表示限制备兑开仓，‘2’表示限制卖出开仓，‘3’表示限制卖出开仓、备兑开仓，‘4’表示限制买入开仓，‘5’表示限制买入开仓、备兑开仓，‘6’表示限制买入开仓、卖出开仓，‘7’表示限制买入开仓、卖出开仓、备兑开仓。
        第4位：‘0’表示此产品在当前时段不接受进行新订单申报，‘1’ 表示此产品在当前时段可接受进行新订单申报。
        第5 - 8位：保留。 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string TradingPhaseCode;    // 状态位 

        public Int32 OrigTime;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FutureTickData
    {
        public Int32 Type;                    // 行情类型
        public UInt32 Seq;                    // 消息序号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string ExID;                   // 交易所代码

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string SecID;                  // 证券代码
        public Int32 ExTime;                  // 交易所时间,Int格式: 10:01:02 000 = 100102000
        public Int32 LocalTime;               // 本地时间
        public Int32 LocalDate;               // 本地日期

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SettlementGroupID;      // 结算组代码
        public Int32 SettlementID;            // 结算编号
        public Int64 PreSettlementPx;         // 昨结算
        public Int64 PreClosePx;	          // 前收盘
        public Int64 PreOpenInterest;         // 昨持仓量
        public double PreDelta;               // 昨虚实度

        public Int64 OpenPx;                  // 当日开盘价
        public Int64 HighPx;                  // 当日最高价
        public Int64 LowPx;                   // 当日最低价
        public Int64 ClosePx;                 // 当日收盘价
        public Int64 UpLimitPx;		          // 涨停
        public Int64 DownLimitPx;		      // 跌停
        public Int64 SettlementPx;            // 今结算
        public double CurrDelta;              // 今虚实

        public Int64 LastPx;			      // 最新价
        public Int64 Volume;			      // 成交量
        public double Turnover;		          // 成交金额
        public Int64 OpenInterest;            // 持仓量

        //五档行情//         
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] AskPx;	              // 委买价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] AskVol;		          // 委买量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] BidPx;	              // 委卖价
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Int64[] BidVol;		          // 委卖量
    }
}

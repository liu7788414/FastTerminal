using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using System.IO;

using Microsoft.International.Converters.PinYinConverter;
using System.Reflection;
using TradeStation.Infrastructure.Services;
using hundsun.t2sdk;
using System.Globalization;
using System.Windows.Forms;
using TradeStation.Infrastructure.Models;
using System.Xml.Serialization;
using Infragistics.Controls.Grids;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using FeserWard.Controls;
using Infragistics;
using MessageBox = System.Windows.MessageBox;

namespace TradeStation.Infrastructure.CommonUtils
{
    /// <summary>
    /// 价差类型
    /// </summary>
    public enum PriceDifferenceType
    {
        点数,
        绝对价格
    }
    public enum eChaseOrderType
    {
        最新价,
        对一价,
        对二价,
        对三价,
        对四价,
        对五价
    }

    /// <summary>
    /// 矩阵界面上网格点的类型
    /// </summary>
    public enum eNodeType
    {
        OptionInfoModelOriginal,
        OptionInfoModelBuyCall,
        OptionInfoModelBuyPut,
        OptionInfoModelSellCall,
        OptionInfoModelSellPut,
        ExerciseDate,
        ExercisePrice,
        BuyCall,
        BuyPut,
        SellCall,
        SellPut
    }
    public enum eOptionType
    {
        认购期权,
        认沽期权
    }

    public enum ArbitrageStatus
    {
        正在监控,
        正在执行,
        已执行,
        停止
    }

    public enum SecurityStatus
    {
        正常,
        停牌,
        退市
    }
    public enum ArbitrageCategoryType
    {
        单个品种,
        组合品种
    }
    public enum eCategory
    {
        股票 = 10,
        债券 = 11,
        基金 = 12,
        指数 = 13,
        权证 = 14,
        债券回购 = 15,
        期货 = 16,
        期权 = 17,
        其他 = 18,
        基金分拆合并 = 100,
    }
    // Only for basket trade to use
    // Lately, it will replace by eCategory.
    public enum eBasketCategory
    {
        股票 = 10,
        期货 = 16,
    }
    public enum ePositionFlag
    {
        多头持仓,
        空头持仓,
        备兑空头持仓
    }

    public enum eInvestType
    {
        //可交易,
        //持有到期,
        //可供出售,
        //贷款和应收款项,
        投机,
        套保,
        套利
    }

    public enum eEntrustFailCode //	委托失败代码
    {
        成功,
        风控禁止,
        可用不足,
        其他,
        因为其他委托导致此笔委托失败
    }


    public enum ePositionInfoMsgType
    {
        查询,
        交易,
        清空
    }

    public enum eMoneyInfoMsgType
    {
        查询,
        清空
    }

    public enum eTradeResultInfoMsgType
    {
        查询,
        清空
    }

    public enum eMoneyType
    {
        人民币,
        美元,
        港元
    }

    public enum eEntrustPriceType
    {
        限价,
        五档即成剩撤_上交所市价,
        五档即成剩转_上交所市价,
        五档即成剩撤_深交所市价,
        即成剩撤_深交所市价,
        对手方最优_深交所市价,
        本方最优_深交所市价
    }

    public enum eFuturePriceType
    {
        限价,
        任意价,
        任意价转限价_中金所,
        五档即成剩撤_中金所五档市价,
        五档即成剩转_中金所五档市价转限价,
        最优一档即成剩撤_中金所最优价,
        最优一档即成剩转_中金所最优价
    }

    public enum CompareSymbol
    {
        大于,
        小于,
        //等于,
        //大于等于,
        //小于等于
    }
    public enum eEntrustDirection
    {
        买入,
        卖出,
        基金认购,	
		ETF申购,
		ETF赎回,
		基金分拆,	
		基金合并,	
        行权认购,
        行权认沽,
        融资回购,// 正回购
		融券回购 // 逆回购
    }

    public enum eFuturesDirection
    {
        开仓,
        平仓
    }

    public enum ePurchaseWay
    {
        普通申赎,
        现金申赎,
        实物申赎,
    }

    public enum eEntrustState
    {
        未报,
        待报,
        正报,
        已报,
        废单,
        部成,
        已成,
        部撤,
        已撤,
        待撤,
        未撤,
        待撤2,
        正撤,
        撤认,
        撤废,
        已撤2
    }

    public enum eMessageType
    {
        委托下达,
        委托确认,
        委托废单,
        委托撤单,
        委托撤成,
        委托撤废,
        委托成交,
        //合笔委托下达,
        //合笔委托确认,
        //合笔委托废单,
        //合笔委托撤单,
        //合笔委托撤成,
        //合笔委托撤废,
        委托查询
    }

    public enum eMarketType
    {
        上交所,
        深交所,
        上期所,
        郑商所,
        中金所,
        大商所,
        股转市场,
        港股通
    }

    public enum eKLinePeriodType
    {
        MIN1 = 1, // 1分钟K线
        MIN3 = 3, // 3分钟K线
        MIN5 = 5, // 5分钟K线
        MIN15 = 15, // 15分钟K线
        MIN30 = 30, // 30分钟K线
        HOUR = 60, // 小时K线
        DAY = 100, // 天K线
        WEEK = 110, // 周K线
        MONTH = 120, // 月K线
        QUARTER = 130, // 季K线
        YEAR = 140, // 年K线
    }
    public enum eDisplayedKLinePeriodType
    {
        一分钟 = 1, // 1分钟K线
        三分钟 = 3, // 3分钟K线
        五分钟 = 5, // 5分钟K线
        十五分钟 = 15, // 15分钟K线
        三十分钟 = 30, // 30分钟K线
        小时线 = 60, // 小时K线
        日线 = 100, // 天K线
        周线 = 110, // 周K线
        月线 = 120, // 月K线
        季线 = 130, // 季K线
        年线 = 140, // 年K线
    }

    public enum eExRightType
    {
        NORMAL = 1, // 不复权
        FORWARD = 2, // 前复权
        BACKWARD = 3, // 后复权
    }
    public enum eDisplayedExRightType
    {
        不复权 = 1, // 不复权
        前复权 = 2, // 前复权
        后复权 = 3, // 后复权
    }

    public enum eUnderlyingType
    {
        ETF,
        A股
    }

    public enum eOptionStyleType
    {
        欧式,
        美式
    }

    public enum eCreationRedeemType
    {
        不允许进行申购和赎回 = 0,
        允许申购和赎回 = 1,
        允许申购不允许赎回 = 2,
        不允许申购允许赎回 = 3
    }

    public enum eEtfMarketType
    {
        本市场ETF = 1,
        跨境ETF = 2,
        跨市场ETF_交易所 = 3,
        跨市场ETF_中登_交易所 = 4
    }

    public enum eEtfType
    {
        股票ETF = 1,
        债券ETF = 2,
        交易型货币基金 = 3,
        黄金ETF = 4,
        货币市场基金 = 5
    }

    public enum eReplaceFlag
    {
        不可替代 = 0,
        可以替代 = 1,
        必须替代 = 2,
        深市退补现金替代 = 3,
        深市必须现金替代 = 4,
        非沪深市场退补现金替代 = 5,
        非沪深市场必须现金替代 = 6
    }

    public enum eBasketTradeMethod
    {
        同时买卖 = 0,
        先买后卖 = 1,
        先卖后买 = 2,
    }

    public enum ArbitrageTradeMethod
    {
        两腿同时 = 0,
        第一腿第二腿 = 1,
        第二腿第一腿 = 2,
    }

    public enum eSuspendType
    {
        morning = 1, // 上午停牌
        afternoon = 2, // 下午停牌
        begin_suspend = 3, // 今起停牌
        suspend_between = 4, // 盘中停牌
        suspend_hour = 5, // 停牌1小时
        suspend = 6, // 停牌一天
    }

    #region 功能号
    //功能号	老功能号	业务范围	功能名称
    public enum 功能号
    {
        心跳 = 10000,
        登录 = 10001,
        退出登录 = 10002,
        密码修改 = 10003,
        账户查询 = 30001,
        资产单元查询 = 30002,
        组合查询 = 30003,
        交易股东查询 = 30004,
        账户资产查询 = 35003,
        资产单元资产查询 = 35011,
        清算流水查询 = 35012,
        历史清算流水查询 = 36012,
        保证金比例查询 = 35013,
        资金调整 = 35015,
        普通买卖委托 = 91001,
        股转做市委托 = 91011,
        篮子委托 = 91090,
        委托撤单 = 91101,
        委托撤单_按委托批号撤单 = 91102,
        股转做市委托撤单 = 91104,
        证券持仓查询 = 31001,
        证券委托查询 = 32001,
        证券历史委托查询 = 32101,
        证券历史成交查询 = 33101,
        股转做市委托查询 = 32006,
        证券成交查询 = 33001,
        账户资金查询 = 34001,
        股转市场协议转让委托 = 91051,
        股转市场协议转让委托撤单 = 91151,
        基金申赎 = 91003,
        基金委托 = 91008,
        基金委托查询 = 32002,
        ETF申赎委托明细查询 = 32005,
        基金成交查询 = 33002,
        ETF申赎成交明细查询 = 33005,
        ETF成份股信息查询 = 35014,
        ETF基础信息查询 = 35020,
        期货信息查询 = 30010,
        期货委托 = 91004,
        商品期货组合单委托 = 91013,
        期货委托撤单 = 91105,
        商品期货组合单委托撤单 = 91107,
        期货持仓查询 = 31003,
        期货委托查询 = 32003,
        期货历史委托查询 = 32103,
        期货历史成交查询 = 33103,
        商品期货组合委托查询 = 32008,
        期货成交查询 = 33003,
        期货保证金账户查询 = 34003,
        期权委托 = 91005,
        备兑锁定与解锁 = 91006,
        期权行权 = 91007,
        股指期权做市委托 = 91012,
        深交所股票期权做市委托 = 91014,
        期权篮子委托 = 91091,
        期权委托撤单 = 91106,
        股指期权做市委托撤单 = 91108,
        深交所股票期权做市委托撤单 = 91109,
        期权持仓查询 = 31004,
        期权委托查询 = 32004,
        期权做市委托查询 = 32007,
        期权成交查询 = 33004,
        期权保证金账户查询 = 34004,
        多业务持仓查询 = 31005,
        组合持仓查询 = 35001,
        当日委托查询 = 35006,
        当日成交查询 = 35007
    }

    #endregion

    public interface IExport
    {
        string GetTitle();
        string Export();
    }

    public interface IImport
    {
        void Import(string[] s);
    }
    public static class CommonUtil
    {
        public const int OptionAmountPerHand = 10000;
        public const int TICK_PX_MULTIPLIER = 10000;
        public const int RequestNum = 10000;
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
        public static readonly string DefaultLayoutPath = AssemblyPath + "Layout\\";
        public static string VersionFile = AssemblyPath + "version.txt";
        public static List<EntrustInfo> OrdersToChase = new List<EntrustInfo>();
        public static int AbsoluteCurrentExtSystemId = ((int)
                (DateTime.Now - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0))
                    .TotalSeconds) * 1000;

        public static int CurrentExtSystemId = AbsoluteCurrentExtSystemId > 0
            ? AbsoluteCurrentExtSystemId + 1
            : Math.Abs(AbsoluteCurrentExtSystemId) + 1;

        public static int ThirdReff = AbsoluteCurrentExtSystemId > 0
            ? AbsoluteCurrentExtSystemId + 1
            : Math.Abs(AbsoluteCurrentExtSystemId) + 1;

        public const string LayoutFileStock = "dockManagerStock.Layout";
        public const string LayoutFileFuture = "dockManagerFuture.Layout";
        public const string LayoutFileOption = "dockManagerOption.Layout";
        public const string LayoutFileBasket = "dockManagerBasket.Layout";
        public const string LayoutFileFund = "dockManagerFund.Layout";
        private static readonly string UserSettingsFile = AssemblyPath + "usersettings.xml";
        public const string INFRASTRUCTURE_MODULE_NAME = "InfrastructureModule";

        public const int VIEW_ORDER_STOCK = 101;
        public const int VIEW_ORDER_FUTURE = 102;
        public const int VIEW_ORDER_OPTION = 103;
        public const int VIEW_ORDER_FUND = 104;
        public const int VIEW_ORDER_BASKET_TRADING = 105;

        public const string IP_REGEX_PATTERN = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject); 

        public static ImageSource ToImageSource(this Icon icon)
        {
            var bitmap = icon.ToBitmap();
            var hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }
            return wpfBitmap;
        }

        public static string GetNowTimeStamp()
        {
            var nowDt = DateTime.Now;
            return nowDt.ToString("HH:mm:ss fff");
        }

        public static object GetObjectFromJsonString(string jsonStr, Type objectType)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            var baseSer = new DataContractJsonSerializer(objectType);
            return baseSer.ReadObject(stream);
        }

        //TODO:第三方库有不少bug，需要替换成更准确的版本
        public static string GetCapitalLetters(string chineseText)
        {
            if (string.IsNullOrEmpty(chineseText))
                return string.Empty;

            //特殊处理多音字
            if ("中国重工".Equals(chineseText))
                return "ZGZG";
            if ("三一重工".Equals(chineseText))
                return "SYZG";
            if ("中信重工".Equals(chineseText))
                return "ZXZG";
            if ("中国重汽".Equals(chineseText))
                return "SGZQ";
            if ("中南重工".Equals(chineseText))
                return "ZNZG";

            var builder = new StringBuilder();
            foreach (var obj in chineseText)
            {
                //"万"字映射不对
                switch (obj)
                {
                    case '万':
                        builder.Append('W');
                        continue;
                    case '齐':
                        builder.Append('Q');
                        continue;
                }

                //普通字母与数字
                if (char.IsDigit(obj) || (char.IsLetter(obj) && obj <= 128))
                {
                    builder.Append(obj);
                    continue;
                }

                //处理全角字母
                if (obj >= 65313 && obj < 65339)
                {
                    builder.Append(Convert.ToChar(obj - 65248)); //65248 is the const diff
                    continue;
                }
                if (obj >= 65339 || IsCharChinese(obj) == false || (obj >= 9332 && obj <= 9431) /*数字带括号字符*/)
                {
                    continue;
                }

                try
                {
                    var chineseChar = new ChineseChar(obj);
                    var t = chineseChar.Pinyins[0];
                    builder.Append(t.Substring(0, 1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fail to parse chinese char:" + chineseText + ",error:" + ex.Message);
                }
            }
            return builder.ToString().ToUpper();
        }

        private static bool IsCharChinese(char c)
        {
            return Convert.ToInt32(c) > Convert.ToInt32(Convert.ToChar(128));
        }

        public const string UFX_FIELD_NAME_operator_no = "operator_no";
        public const string UFX_FIELD_NAME_version_no = "version_no";
        public const string UFX_FIELD_NAME_user_token = "user_token";
        public const string UFX_FIELD_NAME_password = "password";
        public const string UFX_FIELD_NAME_mac_address = "mac_address";
        public const string UFX_FIELD_NAME_op_station = "op_station";
        public const string UFX_FIELD_NAME_ip_address = "ip_address";
        public const string UFX_FIELD_NAME_combi_no = "combi_no";
        public const string UFX_FIELD_NAME_authorization_id = "authorization_id";
        public const string UFX_FIELD_NAME_market_no = "market_no";
        public const string UFX_FIELD_NAME_etf_code = "etf_code";
        public const string UFX_FIELD_NAME_extsystem_id = "extsystem_id";
        public const string UFX_FIELD_NAME_stock_code = "stock_code";
        public const string UFX_FIELD_NAME_stock_amount = "stock_amount";
        public const string UFX_FIELD_NAME_replace_flag = "replace_flag";
        public const string UFX_FIELD_NAME_replace_ratio = "replace_ratio";
        public const string UFX_FIELD_NAME_replace_balance = "replace_balance";
        public const string UFX_FIELD_NAME_redeem_replace_balance = "redeem_replace_balance";
        public const string UFX_FIELD_NAME_stock_num = "stock_num";
        public const string UFX_FIELD_NAME_stock_name = "stock_name";
        public const string UFX_FIELD_NAME_option_type = "option_type";
        public const string UFX_FIELD_NAME_success_flag = "success_flag";
        public const string UFX_FIELD_NAME_fail_cause = "fail_cause";
        public const string UFX_FIELD_NAME_entrust_direction = "entrust_direction";
        public const string UFX_FIELD_NAME_price_type = "price_type";
        public const string UFX_FIELD_NAME_entrust_price = "entrust_price";
        public const string UFX_FIELD_NAME_entrust_amount = "entrust_amount";
        public const string UFX_FIELD_NAME_account_code = "account_code";
        public const string UFX_FIELD_NAME_msgtype = "msgtype";
        public const string UFX_FIELD_NAME_business_date = "business_date";
        public const string UFX_FIELD_NAME_business_time = "business_time";
        public const string UFX_FIELD_NAME_batch_no = "batch_no";
        public const string UFX_FIELD_NAME_entrust_no = "entrust_no";
        public const string UFX_FIELD_NAME_request_order = "request_order";
        public const string UFX_FIELD_NAME_detail_entrust_no = "detail_entrust_no";
        public const string UFX_FIELD_NAME_report_no = "report_no";
        public const string UFX_FIELD_NAME_instance_no = "instance_no";
        public const string UFX_FIELD_NAME_stockholder_id = "stockholder_id";
        public const string UFX_FIELD_NAME_report_seat = "report_seat";
        public const string UFX_FIELD_NAME_futures_direction = "futures_direction";
        public const string UFX_FIELD_NAME_invest_type = "invest_type";
        public const string UFX_FIELD_NAME_entrust_state = "entrust_state";
        public const string UFX_FIELD_NAME_entrust_state_list = "entrust_state_list";
        public const string UFX_FIELD_NAME_third_reff = "third_reff";
        public const string UFX_FIELD_NAME_confirm_no = "confirm_no";
        public const string UFX_FIELD_NAME_revoke_cause = "revoke_cause";
        public const string UFX_FIELD_NAME_cancel_entrust_no = "cancel_entrust_no";
        public const string UFX_FIELD_NAME_entrust_status = "entrust_status";
        public const string UFX_FIELD_NAME_cancel_amount = "cancel_amount";
        public const string UFX_FIELD_NAME_deal_date = "deal_date";
        public const string UFX_FIELD_NAME_deal_time = "deal_time";
        public const string UFX_FIELD_NAME_deal_no = "deal_no";
        public const string UFX_FIELD_NAME_deal_amount = "deal_amount";
        public const string UFX_FIELD_NAME_deal_price = "deal_price";
        public const string UFX_FIELD_NAME_deal_fee = "deal_fee";
        public const string UFX_FIELD_NAME_total_deal_amount = "total_deal_amount";
        public const string UFX_FIELD_NAME_total_deal_balance = "total_deal_balance";
        public const string UFX_FIELD_NAME_enable_balance_t0 = "enable_balance_t0";
        public const string UFX_FIELD_NAME_enable_balance_t1 = "enable_balance_t1";
        public const string UFX_FIELD_NAME_ErrorCode = "ErrorCode";
        public const string UFX_FIELD_NAME_ErrorMsg = "ErrorMsg";
        public const string UFX_FIELD_NAME_MsgDetail = "MsgDetail";
        public const string UFX_FIELD_NAME_DataCount = "DataCount";
        public const string UFX_FIELD_NAME_asset_no = "asset_no";
        public const string UFX_FIELD_NAME_deal_balance = "deal_balance";
        public const string UFX_FIELD_NAME_total_fee = "total_fee";
        public const string UFX_FIELD_NAME_position_str = "position_str";
        public const string UFX_FIELD_NAME_request_num = "request_num";
        public const string UFX_FIELD_NAME_entrust_date = "entrust_date";
        public const string UFX_FIELD_NAME_entrust_time = "entrust_time";
        public const string UFX_FIELD_NAME_pre_buy_frozen_balance = "pre_buy_frozen_balance";
        public const string UFX_FIELD_NAME_pre_sell_balance = "pre_sell_balance";
        public const string UFX_FIELD_NAME_first_deal_time = "first_deal_time";
        public const string UFX_FIELD_NAME_deal_times = "deal_times";
        public const string UFX_FIELD_NAME_withdraw_amount = "withdraw_amount";
        public const string UFX_FIELD_NAME_withdraw_cause = "withdraw_cause";
        public const string UFX_FIELD_NAME_hold_seat = "hold_seat";
        public const string UFX_FIELD_NAME_current_amount = "current_amount";
        public const string UFX_FIELD_NAME_enable_amount = "enable_amount";
        public const string UFX_FIELD_NAME_begin_cost = "begin_cost";
        public const string UFX_FIELD_NAME_current_cost = "current_cost";
        public const string UFX_FIELD_NAME_pre_buy_amount = "pre_buy_amount";
        public const string UFX_FIELD_NAME_pre_sell_amount = "pre_sell_amount";
        public const string UFX_FIELD_NAME_pre_buy_balance = "pre_buy_balance";
        public const string UFX_FIELD_NAME_today_buy_amount = "today_buy_amount";
        public const string UFX_FIELD_NAME_today_sell_amount = "today_sell_amount";
        public const string UFX_FIELD_NAME_today_buy_balance = "today_buy_balance";
        public const string UFX_FIELD_NAME_today_sell_balance = "today_sell_balance";
        public const string UFX_FIELD_NAME_today_buy_fee = "today_buy_fee";
        public const string UFX_FIELD_NAME_today_sell_fee = "today_sell_fee";
        public const string UFX_FIELD_NAME_account_name = "account_name";
        public const string UFX_FIELD_NAME_account_type = "account_type";
        public const string UFX_FIELD_NAME_future_kind_name = "future_kind_name";
        public const string UFX_FIELD_NAME_settlement_month = "settlement_month";
        public const string UFX_FIELD_NAME_target_market_no = "target_market_no";
        public const string UFX_FIELD_NAME_target_stock_code = "target_stock_code";
        public const string UFX_FIELD_NAME_multiple = "multiple";
        public const string UFX_FIELD_NAME_last_trade_date = "last_trade_date";
        public const string UFX_FIELD_NAME_last_trade_time = "last_trade_time";
        public const string UFX_FIELD_NAME_settlement_date = "settlement_date";
        public const string UFX_FIELD_NAME_settlement_price = "settlement_price";
        public const string UFX_FIELD_NAME_pre_settlement_price = "pre_settlement_price";
        public const string UFX_FIELD_NAME_market_position = "market_position";
        public const string UFX_FIELD_NAME_pre_market_position = "pre_market_position";
        public const string UFX_FIELD_NAME_market_price_permit = "market_price_permit";
        public const string UFX_FIELD_NAME_uplimited_price = "uplimited_price";
        public const string UFX_FIELD_NAME_downlimited_price = "downlimited_price";
        public const string UFX_FIELD_NAME_today_amount = "today_amount";
        public const string UFX_FIELD_NAME_lastday_amount = "lastday_amount";
        public const string UFX_FIELD_NAME_today_enable_amount = "today_enable_amount";
        public const string UFX_FIELD_NAME_lastday_enable_amount = "lastday_enable_amount";
        public const string UFX_FIELD_NAME_current_cost_price = "current_cost_price";
        public const string UFX_FIELD_NAME_occupy_deposit_balance = "occupy_deposit_balance";
        public const string UFX_FIELD_NAME_enable_deposit_balance = "enable_deposit_balance";
        public const string UFX_FIELD_NAME_position_flag = "position_flag";
        public const string UFX_FIELD_NAME_creation_redeem_type = "creation_redeem_type";
        public const string UFX_FIELD_NAME_etf_market_type = "etf_market_type";
        public const string UFX_FIELD_NAME_rival_market = "rival_market";
        public const string UFX_FIELD_NAME_etf_type = "etf_type";
        public const string UFX_FIELD_NAME_max_cash_ratio = "max_cash_ratio";
        public const string UFX_FIELD_NAME_report_unit = "report_unit";
        public const string UFX_FIELD_NAME_yesterday_cash = "yesterday_cash";
        public const string UFX_FIELD_NAME_yesterday_nav = "yesterday_nav";
        public const string UFX_FIELD_NAME_estimate_cash = "estimate_cash";
        public const string UFX_FIELD_NAME_underlying_index = "underlying_index";
        public const string UFX_FIELD_NAME_limit_entrust_ratio = "limit_entrust_ratio";
        public const string UFX_FIELD_NAME_ftr_limit_entrust_ratio = "ftr_limit_entrust_ratio";

        public const string T2Sdk = "t2sdk";
        public const string T2Sdkini = "t2sdk.ini";
        public static string ConfigFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + T2Sdkini;

        public const int NUMBER_OF_DIAGRAM_INTERVAL = 5;

        #region 市场代码
        public const string 上交所 = "SH";
        public const string 深交所 = "SZ";
        public const string 中金所 = "CFFEX";
        public const string 上期所 = "SHFE";
        public const string 郑商所 = "CZCE";
        public const string 大商所 = "DCE";

        #endregion
        #region 多空标志
        public const string 多头持仓 = "1";
        public const string 空头持仓 = "2";
        public const string 备兑空头持仓 = "b";

        #endregion

        #region 投资类型
        public const string 可交易 = "1";
        public const string 持有到期 = "2";
        public const string 可供出售 = "3";
        public const string 贷款和应收款项 = "4";
        public const string 投机 = "a";
        public const string 套保 = "b";
        public const string 套利 = "c";

        #endregion

        #region 价格类型
        public const string 限价 = "0";
        public const string 任意价 = "1";
        public const string 五档即成剩撤_上交所市价 = "a";
        public const string 五档即成剩转_上交所市价 = "b";
        public const string 五档即成剩撤_深交所市价 = "A";
        public const string 即成剩撤_深交所市价 = "C";
        public const string 对手方最优_深交所市价 = "D";
        public const string 本方最优_深交所市价 = "E";
        public const string 任意价转限价_中金所 = "X";
        public const string 五档即成剩撤_中金所五档市价 = "L";
        public const string 五档即成剩转_中金所五档市价转限价 = "M";
        public const string 最优一档即成剩撤_中金所最优价 = "N";
        public const string 最优一档即成剩转_中金所最优价 = "O";

        #endregion

        #region 委托方向
        public const string 买入 = "1";
        public const string 卖出 = "2";
        public const string 融资回购 = "5";
        public const string 融券回购 = "6";
        public const string 基金认购 = "13";
        public const string ETF申购 = "26";
        public const string ETF赎回 = "27";
        public const string 行权认购 = "28";
        public const string 行权认沽 = "29";
        public const string 基金分拆 = "50";
        public const string 基金合并 = "51";

        #endregion

        #region 申赎方式
        public const string 普通申赎 = "0";
        public const string 现金申赎 = "1";
        public const string 实物申赎 = "2";	

        #endregion
        #region 期权类型
        public const string 认购期权 = "C";
        public const string 认沽期权 = "P";

        #endregion 
        #region 期货方向
        public const string 开仓 = "1";
        public const string 平仓 = "2";

        #endregion

        #region 市场编号
        public const string market_no_上交所 = "1";
        public const string market_no_深交所 = "2";
        public const string market_no_上期所 = "3";
        public const string market_no_郑商所 = "4";
        public const string market_no_中金所 = "7";
        public const string market_no_大商所 = "9";
        public const string market_no_股转市场 = "10";
        public const string market_no_港股通 = "35";
        #endregion

        #region 账户信息
        public static string sTradeAccount;
        public static string sOPStation = "IP:127.0.0.1;MAC;HD;";
        public static string sPassword;
        public static double AvailableMoney = 0;

        #endregion

        public const string TopicName = "ufx_topic";
        public static bool bIsQryPositionDone = true;

        #region 委托状态
        public const string 未报 = "1";
        public const string 待报 = "2";
        public const string 正报 = "3";
        public const string 已报 = "4";
        public const string 废单 = "5";
        public const string 部成 = "6";
        public const string 已成 = "7";
        public const string 部撤 = "8";
        public const string 已撤 = "9";
        public const string 待撤 = "a";
        public const string 未撤 = "A";
        public const string 待撤2 = "B";
        public const string 正撤 = "C";
        public const string 撤认 = "D";
        public const string 撤废 = "E";
        public const string 已撤2 = "F";

        #endregion

        #region 消息类型
        public const string 委托下达 = "a";
        public const string 委托确认 = "b";
        public const string 委托废单 = "c";
        public const string 委托撤单 = "d";
        public const string 委托撤成 = "e";
        public const string 委托撤废 = "f";
        public const string 委托成交 = "g";
        public const string 合笔委托下达 = "h";
        public const string 合笔委托确认 = "i";
        public const string 合笔委托废单 = "j";
        public const string 合笔委托撤单 = "k";
        public const string 合笔委托撤成 = "l";
        public const string 合笔委托撤废 = "m";


        #endregion


        #region 委托失败代码
        public const string 成功 = "0";
        public const string 风控禁止 = "1";
        public const string 可用不足 = "2";
        public const string 其他 = "3";
        public const string 因为其他委托导致此笔委托失败 = "99";
        #endregion

        #region 当天状态
        public const string 不允许进行申购和赎回 = "0";
        public const string 允许申购和赎回 = "1";
        public const string 允许申购不允许赎回 = "2";
        public const string 不允许申购允许赎回 = "3";
        #endregion

        #region ETF市场类型

        public const string 本市场ETF = "1";
        public const string 跨境ETF = "2";
        public const string 跨市场ETF_交易所 = "3";
        public const string 跨市场ETF_中登_交易所 = "4"; 

        #endregion

        #region ETF分类

        public const string 股票ETF = "1";
        public const string 债券ETF = "2";
        public const string 交易型货币基金 = "3";
        public const string 黄金ETF = "4";
        public const string 货币市场基金 = "5";

        #endregion

        #region 现金替代标志
        public const string 不可替代 = "0";
        public const string 可以替代 = "1";
        public const string 必须替代 = "2";
        public const string 深市退补现金替代 = "3";
        public const string 深市必须现金替代 = "4";
        public const string 非沪深市场退补现金替代 = "5";
        public const string 非沪深市场必须现金替代 = "6";
	    #endregion

        public static EntrustInfo BuildEntrustInfo(string securityId, double entrustPrice, eMarketType exchange, string selectedCombiNo, int entrustAmount, eEntrustDirection entrustDirection, eEntrustPriceType entrustPriceType, eCategory category = eCategory.股票, eFuturesDirection? futuresDirection = null, eInvestType? investType = null, double entrustBalance = 0, ePurchaseWay purchaseWay = ePurchaseWay.普通申赎)
        {
            EntrustInfo ei = null;

            switch (category)
            {
                case eCategory.股票:
                    {
                        ei = new EntrustInfo();
                        break;
                    }
                case eCategory.期货:
                    {
                        ei = new FutureEntrustInfo();
                        break;
                    }
                case eCategory.期权:
                    {
                        ei = new OptionEntrustInfo();
                        break;
                    }
                case eCategory.基金:
                    {
                        ei = new FundEntrustInfo();
                        break;
                    }
                case eCategory.指数:
                    break;
                case eCategory.债券:
                    break;
                case eCategory.债券回购:
                    break;
                case eCategory.基金分拆合并:
                    {
                        ei = new FundEntrustInfo();
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException("category", category, null);
            }

            ei.Category = category;
            ei.SecurityID = securityId;
            ei.MarketType = exchange;
            ei.CombiNo = selectedCombiNo;
            ei.EntrustDirection = entrustDirection;
            ei.EntrustPriceType = entrustPriceType;
            ei.EntrustPrice = entrustPrice;
            ei.EntrustAmount = entrustAmount;
            ei.EntrustBalance = entrustBalance;

            if (ei is FutureEntrustInfo || ei is OptionEntrustInfo)
            {
                if (futuresDirection != null) ei.FuturesDirection = futuresDirection.Value;
                if (investType != null) ei.InvestType = investType.Value;
            }

            if (ei is FundEntrustInfo)
            {
                ei.PurchaseWay = purchaseWay;
            }

            return ei;
        }

        public static void ChaseOrder(ObservableCollection<EntrustInfo> entrustInfos, IList<string> cannotChasedEntrustExId, eChaseOrderType chaseOrderType, HsStock trader)
        {
            if (OrdersToChase.Count == 0)
            {
                var checkedEntrusts = entrustInfos.Where(s => s.IsChecked);

                // Check if any order can not be chased (because it is created by combined future trading).
                // Currently, it only outputs the first one order.
                var cannotChasedOrders = checkedEntrusts.Where(x => cannotChasedEntrustExId.Contains(x.ExtsystemId));
                if (null != cannotChasedOrders && cannotChasedOrders.Any())
                {
                    var ordersNumberStrBuilder = new StringBuilder();
                    cannotChasedOrders.ToList().ForEach(x => ordersNumberStrBuilder.Append(x.EntrustNo + "; "));

                    if (MessageBox.Show(
                        string.Format("选中的委托里包含有合成的委托，若追单，则会造成另一腿不能成交的情况，确认追单吗？\r\n合成委托号: {0}", ordersNumberStrBuilder.ToString()),
                        "确认追单",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                OrdersToChase.AddRange(checkedEntrusts);

                //先撤单
                foreach (var entrustInfo in OrdersToChase)
                {
                    entrustInfo.IsChasingOrder = true;
                    entrustInfo.ChaseOrderType = chaseOrderType;
                    entrustInfo.IsChecked = false;
                    trader.CancelOrder(entrustInfo.EntrustNo, entrustInfo.Category);
                }

                OrdersToChase.Clear();
            }
            else
            {
                MessageBox.Show("上次追单尚未完成，不能追单");
            }
        }

        public static string MarketNoToExId(string marketNo)
        {
            try
            {
                switch (marketNo)
                {
                    case market_no_上交所:
                    {
                        return 上交所;
                    }
                    case market_no_深交所:
                    {
                        return 深交所;
                    }
                    case market_no_中金所:
                    {
                        return 中金所;
                    }
                    case market_no_上期所:
                    {
                        return 上期所;
                    }
                    case market_no_郑商所:
                    {
                        return 郑商所;
                    }
                    case market_no_大商所:
                    {
                        return 大商所;
                    }
                    default:
                    {
                        return 深交所;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        private static string ExIdToMarketNo(string exId)
        {
            try
            {
                switch (exId)
                {
                    case 上交所:
                    {
                        return market_no_上交所;
                    }
                    case 深交所:
                    {
                        return market_no_深交所;
                    }
                    case 中金所:
                    {
                        return market_no_中金所;
                    }
                    case 上期所:
                    {
                        return market_no_上期所;
                    }
                    case 郑商所:
                    {
                        return market_no_郑商所;
                    }
                    case 大商所:
                    {
                        return market_no_大商所;
                    }
                    default:
                    {
                        throw new Exception(string.Format("无法识别的ex_id:{0}", exId));
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return null;
        }

        public static eMarketType ExIdToeMarketType(string exId)
        {
            try
            {
                return MarketNoToeMarketType(ExIdToMarketNo(exId));
            }
            catch (Exception ex)
            {
                // ignored
            }

            return eMarketType.上交所;
        }


        public static void GetValueFromPack(CT2UnPacker lpUnPack, string valuename, Type type, out object value)
        {
            value = null;
            try
            {
                lpUnPack.First();
                int count = lpUnPack.GetDatasetCount();
                for (int k = 0; k < count; k++)
                {
                    lpUnPack.SetCurrentDatasetByIndex(k);
                    while (lpUnPack.IsEOF() == 0)
                    {
                        for (int i = 0; i < lpUnPack.GetColCount(); i++)
                        {
                            String colName = lpUnPack.GetColName(i);
                            sbyte colType = lpUnPack.GetColType(i);

                            if (colName.Equals(valuename))
                            {
                                if (type == typeof (int))
                                {
                                    value = lpUnPack.GetIntByIndex(i);
                                }

                                if (type == typeof (String))
                                {
                                    value = lpUnPack.GetStrByIndex(i);
                                }

                                if (type == typeof (char))
                                {
                                    value = lpUnPack.GetCharByIndex(i);
                                }

                                if (type == typeof (double))
                                {
                                    value = lpUnPack.GetDoubleByIndex(i);
                                }
                            }
                        }

                        lpUnPack.Next();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static string eEntrustFailCodeToEntrustFailCode(eEntrustFailCode entrust_fail_code)
        {
            switch (entrust_fail_code)
            {
                case eEntrustFailCode.成功:
                {
                    return 成功;
                }
                case eEntrustFailCode.风控禁止:
                {
                    return 风控禁止;
                }
                case eEntrustFailCode.可用不足:
                {
                    return 可用不足;
                }
                case eEntrustFailCode.其他:
                {
                    return 其他;
                }
                case eEntrustFailCode.因为其他委托导致此笔委托失败:
                {
                    return 因为其他委托导致此笔委托失败;
                }
            }

            return 成功;
        }

        public static eEntrustFailCode EntrustFailCodeToeEntrustFailCode(string entrust_fail_code)
        {
            switch (entrust_fail_code)
            {
                case 成功:
                {
                    return eEntrustFailCode.成功;
                }
                case 风控禁止:
                {
                    return eEntrustFailCode.风控禁止;
                }
                case 可用不足:
                {
                    return eEntrustFailCode.可用不足;
                }
                case 其他:
                {
                    return eEntrustFailCode.其他;
                }
                case 因为其他委托导致此笔委托失败:
                {
                    return eEntrustFailCode.因为其他委托导致此笔委托失败;
                }
            }

            return eEntrustFailCode.成功;
        }

        public static string ePurchaseWayToPurchaseWay(ePurchaseWay purchase_way)
        {
            switch (purchase_way)
            {
                case ePurchaseWay.普通申赎:
                {
                    return 普通申赎;
                }
                case ePurchaseWay.实物申赎:
                {
                    return 实物申赎;
                }
                case ePurchaseWay.现金申赎:
                {
                    return 现金申赎;
                }
            }

            return 普通申赎;
        }

        public static eReplaceFlag ReplaceFlagToeReplaceFlag(string replaceFlag)
        {
            switch (replaceFlag)
            {
                case 不可替代:
                    {
                        return eReplaceFlag.不可替代;
                    }
                case 可以替代:
                    {
                        return eReplaceFlag.可以替代;
                    }
                case 必须替代:
                    {
                        return eReplaceFlag.必须替代;
                    }
                case 深市退补现金替代:
                    {
                        return eReplaceFlag.深市退补现金替代;
                    }
                case 深市必须现金替代:
                    {
                        return eReplaceFlag.深市必须现金替代;
                    }
                case 非沪深市场退补现金替代:
                    {
                        return eReplaceFlag.非沪深市场退补现金替代;
                    }
                case 非沪深市场必须现金替代:
                    {
                        return eReplaceFlag.非沪深市场必须现金替代;
                    }
            }

            return eReplaceFlag.不可替代;
        }

        public static ePurchaseWay PurchaseWayToePurchaseWay(string purchase_way)
        {
            switch (purchase_way)
            {
                case 普通申赎:
                {
                    return ePurchaseWay.普通申赎;
                }
                case 实物申赎:
                {
                    return ePurchaseWay.实物申赎;
                }
                case 现金申赎:
                {
                    return ePurchaseWay.现金申赎;
                }
            }

            return ePurchaseWay.普通申赎;
        }

        public static string eEntrustDirectionToEntrustDirection(eEntrustDirection direction)
        {
            switch (direction)
            {
                case eEntrustDirection.买入:
                {
                    return 买入;
                }
                case eEntrustDirection.卖出:
                {
                    return 卖出;
                }
                case eEntrustDirection.ETF申购:
                {
                    return ETF申购;
                }
                case eEntrustDirection.ETF赎回:
                {
                    return ETF赎回;
                }
                case eEntrustDirection.行权认购:
                {
                    return 行权认购;
                }
                case eEntrustDirection.行权认沽:
                {
                    return 行权认沽;
                }
                case eEntrustDirection.基金分拆:
                {
                    return 基金分拆;
                }
                case eEntrustDirection.基金合并:
                {
                    return 基金合并;
                }
                case eEntrustDirection.基金认购:
                {
                    return 基金认购;
                }
                case eEntrustDirection.融券回购:
                {
                    return 融券回购;
                }
                case eEntrustDirection.融资回购:
                {
                    return 融资回购;
                }
            }

            return 买入;
        }

        public static eEntrustDirection EntrustDirectionToeEntrustDirection(string direction)
        {
            switch (direction)
            {
                case 买入:
                {
                    return eEntrustDirection.买入;
                }
                case 卖出:
                {
                    return eEntrustDirection.卖出;
                }
                case ETF申购:
                {
                    return eEntrustDirection.ETF申购;
                }
                case ETF赎回:
                {
                    return eEntrustDirection.ETF赎回;
                }
                case 行权认购:
                {
                    return eEntrustDirection.行权认购;
                }
                case 行权认沽:
                {
                    return eEntrustDirection.行权认沽;
                }
                case 基金分拆:
                {
                    return eEntrustDirection.基金分拆;
                }
                case 基金合并:
                {
                    return eEntrustDirection.基金合并;
                }
                case 基金认购:
                {
                    return eEntrustDirection.基金认购;
                }
                case 融券回购:
                {
                    return eEntrustDirection.融券回购;
                }
                case 融资回购:
                {
                    return eEntrustDirection.融资回购;
                }
            }

            return eEntrustDirection.买入;
        }


        public static eFuturesDirection FuturesDirectionToeFuturesDirection(string direction)
        {
            switch (direction)
            {
                case 开仓:
                {
                    return eFuturesDirection.开仓;
                }
                case 平仓:
                {
                    return eFuturesDirection.平仓;
                }
            }

            return eFuturesDirection.开仓;
        }

        public static string eFuturesDirectionToFuturesDirection(eFuturesDirection futures_direction)
        {
            switch (futures_direction)
            {
                case eFuturesDirection.开仓:
                {
                    return 开仓;
                }
                case eFuturesDirection.平仓:
                {
                    return 平仓;
                }
            }

            return 开仓;
        }

        public static string eEntrustPriceTypeToEntrustPriceType(eEntrustPriceType price_type)
        {
            switch (price_type)
            {
                case eEntrustPriceType.限价:
                {
                    return 限价;
                }
                case eEntrustPriceType.五档即成剩撤_上交所市价:
                {
                    return 五档即成剩撤_上交所市价;
                }
                case eEntrustPriceType.五档即成剩转_上交所市价:
                {
                    return 五档即成剩转_上交所市价;
                }
                case eEntrustPriceType.五档即成剩撤_深交所市价:
                {
                    return 五档即成剩撤_深交所市价;
                }
                case eEntrustPriceType.即成剩撤_深交所市价:
                {
                    return 即成剩撤_深交所市价;
                }
                case eEntrustPriceType.对手方最优_深交所市价:
                {
                    return 对手方最优_深交所市价;
                }
                case eEntrustPriceType.本方最优_深交所市价:
                {
                    return 本方最优_深交所市价;
                }
            }

            return 限价;
        }

        public static string eFuturePriceTypeToFuturePriceType(eFuturePriceType price_type)
        {
            switch (price_type)
            {
                case eFuturePriceType.限价:
                {
                    return 限价;
                }
                case eFuturePriceType.任意价:
                {
                    return 任意价;
                }
                case eFuturePriceType.任意价转限价_中金所:
                {
                    return 任意价转限价_中金所;
                }
                case eFuturePriceType.五档即成剩撤_中金所五档市价:
                {
                    return 五档即成剩撤_中金所五档市价;
                }
                case eFuturePriceType.五档即成剩转_中金所五档市价转限价:
                {
                    return 五档即成剩转_中金所五档市价转限价;
                }
                case eFuturePriceType.最优一档即成剩撤_中金所最优价:
                {
                    return 最优一档即成剩撤_中金所最优价;
                }
                case eFuturePriceType.最优一档即成剩转_中金所最优价:
                {
                    return 最优一档即成剩转_中金所最优价;
                }
            }

            return 限价;
        }

        public static eEntrustPriceType EntrustPriceTypeToeEntrustPriceType(string price_type)
        {
            switch (price_type)
            {
                case 限价:
                {
                    return eEntrustPriceType.限价;
                }
                case 五档即成剩撤_上交所市价:
                {
                    return eEntrustPriceType.五档即成剩撤_上交所市价;
                }
                case 五档即成剩转_上交所市价:
                {
                    return eEntrustPriceType.五档即成剩转_上交所市价;
                }
                case 五档即成剩撤_深交所市价:
                {
                    return eEntrustPriceType.五档即成剩撤_深交所市价;
                }
                case 即成剩撤_深交所市价:
                {
                    return eEntrustPriceType.即成剩撤_深交所市价;
                }
                case 对手方最优_深交所市价:
                {
                    return eEntrustPriceType.对手方最优_深交所市价;
                }
                case 本方最优_深交所市价:
                {
                    return eEntrustPriceType.本方最优_深交所市价;
                }
            }

            return eEntrustPriceType.限价;
        }

        public static eEntrustState EntrustSateToeEntrustSate(string entrust_state)
        {
            switch (entrust_state)
            {
                case 未报:
                {
                    return eEntrustState.未报;
                }
                case 待报:
                {
                    return eEntrustState.待报;
                }
                case 正报:
                {
                    return eEntrustState.正报;
                }
                case 已报:
                {
                    return eEntrustState.已报;
                }
                case 废单:
                {
                    return eEntrustState.废单;
                }
                case 部成:
                {
                    return eEntrustState.部成;
                }
                case 已成:
                {
                    return eEntrustState.已成;
                }
                case 部撤:
                {
                    return eEntrustState.部撤;
                }
                case 已撤:
                {
                    return eEntrustState.已撤;
                }
                case 待撤:
                {
                    return eEntrustState.待撤;
                }
                case 未撤:
                {
                    return eEntrustState.未撤;
                }
                case 待撤2:
                {
                    return eEntrustState.待撤2;
                }
                case 正撤:
                {
                    return eEntrustState.正撤;
                }
                case 撤认:
                {
                    return eEntrustState.撤认;
                }
                case 撤废:
                {
                    return eEntrustState.撤废;
                }
                case 已撤2:
                {
                    return eEntrustState.已撤2;
                }
            }

            return eEntrustState.废单;
        }


        public static string eEntrustSateToEntrustSate(eEntrustState entrust_state)
        {
            switch (entrust_state)
            {
                case eEntrustState.未报:
                {
                    return 未报;
                }
                case eEntrustState.待报:
                {
                    return 待报;
                }
                case eEntrustState.正报:
                {
                    return 正报;
                }
                case eEntrustState.已报:
                {
                    return 已报;
                }
                case eEntrustState.废单:
                {
                    return 废单;
                }
                case eEntrustState.部成:
                {
                    return 部成;
                }
                case eEntrustState.已成:
                {
                    return 已成;
                }
                case eEntrustState.部撤:
                {
                    return 部撤;
                }
                case eEntrustState.已撤:
                {
                    return 已撤;
                }
                case eEntrustState.待撤:
                {
                    return 待撤;
                }
                case eEntrustState.未撤:
                {
                    return 未撤;
                }
                case eEntrustState.待撤2:
                {
                    return 待撤2;
                }
                case eEntrustState.正撤:
                {
                    return 正撤;
                }
                case eEntrustState.撤认:
                {
                    return 撤认;
                }
                case eEntrustState.撤废:
                {
                    return 撤废;
                }
                case eEntrustState.已撤2:
                {
                    return 已撤2;
                }
            }

            return 废单;
        }

        public static eMessageType MessageTypeToeMessageType(string msg_type)
        {
            switch (msg_type)
            {
                case 委托下达:
                {
                    return eMessageType.委托下达;
                }
                case 委托确认:
                {
                    return eMessageType.委托确认;
                }
                case 委托废单:
                {
                    return eMessageType.委托废单;
                }
                case 委托撤单:
                {
                    return eMessageType.委托撤单;
                }
                case 委托撤成:
                {
                    return eMessageType.委托撤成;
                }
                case 委托撤废:
                {
                    return eMessageType.委托撤废;
                }
                case 委托成交:
                {
                    return eMessageType.委托成交;
                }
                //case 合笔委托下达:
                //{
                //    return eMessageType.合笔委托下达;
                //}
                //case 合笔委托确认:
                //{
                //    return eMessageType.合笔委托确认;
                //}
                //case 合笔委托废单:
                //{
                //    return eMessageType.合笔委托废单;
                //}
                //case 合笔委托撤单:
                //{
                //    return eMessageType.合笔委托撤单;
                //}
                //case 合笔委托撤成:
                //{
                //    return eMessageType.合笔委托撤成;
                //}
                //case 合笔委托撤废:
                //{
                //    return eMessageType.合笔委托撤废;
                //}
            }

            return eMessageType.委托废单;
        }

        public static eEtfMarketType EtfMarketTypeToeEtfMarketType(string etfMarketType)
        {
            switch (etfMarketType)
            {
                case 本市场ETF:
                    {
                        return eEtfMarketType.本市场ETF;
                    }
                case 跨境ETF:
                    {
                        return eEtfMarketType.跨境ETF;
                    }
                case 跨市场ETF_交易所:
                    {
                        return eEtfMarketType.跨市场ETF_交易所;
                    }
                case 跨市场ETF_中登_交易所:
                    {
                        return eEtfMarketType.跨市场ETF_中登_交易所;
                    }
            }

            return eEtfMarketType.本市场ETF;
        }

        public static eEtfType EtfTypeToeEtfType(string etfType)
        {
            switch (etfType)
            {
                case 股票ETF:
                    {
                        return eEtfType.股票ETF;
                    }
                case 债券ETF:
                    {
                        return eEtfType.债券ETF;
                    }
                case 交易型货币基金:
                    {
                        return eEtfType.交易型货币基金;
                    }
                case 黄金ETF:
                    {
                        return eEtfType.黄金ETF;
                    }
                case 货币市场基金:
                    {
                        return eEtfType.货币市场基金;
                    }
            }

            return eEtfType.股票ETF;
        }

        public static eCreationRedeemType CreationRedeemTypeToeCreationRedeemType(string creationRedeemType)
        {
            switch (creationRedeemType)
            {
                case 不允许进行申购和赎回:
                    {
                        return eCreationRedeemType.不允许进行申购和赎回;
                    }
                case 允许申购和赎回:
                    {
                        return eCreationRedeemType.允许申购和赎回;
                    }
                case 允许申购不允许赎回:
                    {
                        return eCreationRedeemType.允许申购不允许赎回;
                    }
                case 不允许申购允许赎回:
                    {
                        return eCreationRedeemType.不允许申购允许赎回;
                    }
            }

            return eCreationRedeemType.不允许进行申购和赎回;
        }


        public static eMarketType MarketNoToeMarketType(string market_no)
        {
            switch (market_no)
            {
                case market_no_上交所:
                {
                    return eMarketType.上交所;
                }
                case market_no_深交所:
                {
                    return eMarketType.深交所;
                }
                case market_no_上期所:
                {
                    return eMarketType.上期所;
                }
                case market_no_郑商所:
                {
                    return eMarketType.郑商所;
                }
                case market_no_中金所:
                {
                    return eMarketType.中金所;
                }
                case market_no_大商所:
                {
                    return eMarketType.大商所;
                }
                case market_no_股转市场:
                {
                    return eMarketType.股转市场;
                }
                case market_no_港股通:
                {
                    return eMarketType.港股通;
                }
            }

            return eMarketType.上交所;
        }

        public static eMarketType ExIDToMarketType(string exID)
        {
            switch (exID)
            {
                case 上交所:
                    return eMarketType.上交所;
                case 深交所:
                    return eMarketType.深交所;
                case 中金所:
                    return eMarketType.中金所;
                case 上期所:
                    return eMarketType.上期所;
                case 郑商所:
                    return eMarketType.郑商所;
                case 大商所:
                    return eMarketType.大商所;
            }

            return eMarketType.上交所;
        }

        public static eInvestType InvestTypeToeInvestType(string invest_type)
        {
            switch (invest_type)
            {
                //case 持有到期:
                //    {
                //        return eInvestType.持有到期;
                //    }
                //case 贷款和应收款项:
                //    {
                //        return eInvestType.贷款和应收款项;
                //    }
                //case 可供出售:
                //    {
                //        return eInvestType.可供出售;
                //    }
                //case 可交易:
                //    {
                //        return eInvestType.可交易;
                //    }
                case 套保:
                {
                    return eInvestType.套保;
                }
                case 套利:
                {
                    return eInvestType.套利;
                }
                case 投机:
                {
                    return eInvestType.投机;
                }
            }

            return eInvestType.投机;
        }

        public static ePositionFlag PositionFlagToePositionFlag(string position_flag)
        {
            switch (position_flag)
            {
                case 多头持仓:
                {
                    return ePositionFlag.多头持仓;
                }
                case 空头持仓:
                {
                    return ePositionFlag.空头持仓;
                }
            }

            return ePositionFlag.多头持仓;
        }

        public static eOptionType OptionTypeToeOptionType(string option_type)
        {
            switch (option_type)
            {
                case 认购期权:
                {
                    return eOptionType.认购期权;
                }
                case 认沽期权:
                {
                    return eOptionType.认沽期权;
                }
            }

            return eOptionType.认购期权;
        }

        public static string eOptionTypeToOptionType(eOptionType option_type)
        {
            switch (option_type)
            {
                case eOptionType.认购期权:
                {
                    return 认购期权;
                }
                case eOptionType.认沽期权:
                {
                    return 认沽期权;
                }
            }

            return 认购期权;
        }

        public static string eMarketTypeToeMarketNo(eMarketType market_type)
        {
            switch (market_type)
            {
                case eMarketType.上交所:
                {
                    return market_no_上交所;
                }
                case eMarketType.深交所:
                {
                    return market_no_深交所;
                }
                case eMarketType.上期所:
                {
                    return market_no_上期所;
                }
                case eMarketType.郑商所:
                {
                    return market_no_郑商所;
                }
                case eMarketType.中金所:
                {
                    return market_no_中金所;
                }
                case eMarketType.大商所:
                {
                    return market_no_大商所;
                }
                case eMarketType.股转市场:
                {
                    return market_no_股转市场;
                }
                case eMarketType.港股通:
                {
                    return market_no_港股通;
                }
            }

            return market_no_上交所;
        }

        public static string GetStrFromUnPacker(CT2UnPacker pUnPacker, string field_name)
        {
            return pUnPacker.GetStr(field_name);
        }

        public static string TimeIntToTime(int TimeInt)
        {
            int HourInt = TimeInt/10000;
            int MinuteInt = (TimeInt - HourInt*10000)/100;
            int SecondInt = TimeInt - HourInt*10000 - MinuteInt*100;

            DateTime dt = new DateTime(1, 1, 1, HourInt, MinuteInt, SecondInt);

            return dt.ToString("HH:mm:ss");
        }

        public static string TimeIntToTime(string timeInt)
        {
            try
            {
                return TimeIntToTime(Convert.ToInt32(timeInt));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static eCategory GetCategory(string sMarketNo, string sEntrustNo, EntrustInfoCollection EntrustInfoCollection)
        {
            var q = from ei in EntrustInfoCollection.StockEntrustInfoList where ei.EntrustNo.Equals(sEntrustNo) select ei;

            if (q.Count() > 0)
            {
                return eCategory.股票;
            }

            q = from ei in EntrustInfoCollection.FutureEntrustInfoList where ei.EntrustNo.Equals(sEntrustNo) select ei;

            if (q.Count() > 0)
            {
                return eCategory.期货;
            }

            q = from ei in EntrustInfoCollection.OptionEntrustInfoList where ei.EntrustNo.Equals(sEntrustNo) select ei;

            if (q.Count() > 0)
            {
                return eCategory.期权;
            }


            q = from ei in EntrustInfoCollection.FundEntrustInfoList where ei.EntrustNo.Equals(sEntrustNo) select ei;

            if (q.Count() > 0)
            {
                return eCategory.基金;
            }


            return eCategory.股票;
        }

        /// <summary>
        /// 序列化成xml字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>序列化后的字符串</returns>
        public static string Serialize(object obj)
        {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                xtw.Formatting = System.Xml.Formatting.Indented;
                xs.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    string str = sr.ReadToEnd();
                    xtw.Close();
                    ms.Close();
                    return str;
                }
            }
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="xml">反序列化字符串或者xml文件路径</param>
        /// <returns></returns>
        public static T Desrialize<T>(string xml)
        {
            T obj = default(T);
            XmlSerializer xs = new XmlSerializer(typeof (T));
            TextReader tr;

            if (!File.Exists(xml))
            {
                tr = new StringReader(xml);
            }
            else
            {
                tr = new StreamReader(xml);
            }
            using (tr)
            {
                obj = (T) xs.Deserialize(tr);
            }
            return obj;
        }

        public static void SaveUserSettings(UserSettings us)
        {
            try
            {
                StreamWriter sw = new StreamWriter(UserSettingsFile, false, Encoding.UTF8);
                sw.WriteLine(us.FastTradeAmountStock);
                sw.WriteLine(us.FastTradeAmountFuture);
                sw.WriteLine(us.FastTradeAmountOption);
                sw.WriteLine(us.MoneyLimitPerTrade);
                sw.WriteLine(us.AmountLimitPerTradeStock);
                sw.WriteLine(us.AmountLimitPerTradeFuture);
                sw.WriteLine(us.AmountLimitPerTradeOption);
                sw.WriteLine(us.LockContentPane);
                sw.WriteLine(us.IsPromptEnabled);
                sw.WriteLine(us.IsStockVisible);
                sw.WriteLine(us.IsFutureVisible);
                sw.WriteLine(us.IsOptionVisible);
                sw.WriteLine(us.IsFundVisible);
                sw.WriteLine(us.IsBasketVisible);
                sw.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + ex.Source + ex.StackTrace);
            }
        }


        public static void LoadUserSettings(UserSettings us)
        {
            try
            {
                if (File.Exists(VersionFile)) //读取版本信息
                {
                    var sr = new StreamReader(VersionFile, Encoding.UTF8);       
                    us.Version = sr.ReadLine();
                    sr.Close();

                    us.SoftwareName = string.Format("天风证券傲速交易终端(版本号:{0})", us.Version);
                }
                else
                {
                    us.Version = "1.0.0.0";
                }

                if (File.Exists(UserSettingsFile))
                {
                    var sr = new StreamReader(UserSettingsFile, Encoding.UTF8);
                    us.FastTradeAmountStock = Convert.ToInt32(sr.ReadLine());
                    us.FastTradeAmountFuture = Convert.ToInt32(sr.ReadLine());
                    us.FastTradeAmountOption = Convert.ToInt32(sr.ReadLine());
                    us.MoneyLimitPerTrade = Convert.ToDouble(sr.ReadLine());
                    us.AmountLimitPerTradeStock = Convert.ToInt32(sr.ReadLine());
                    us.AmountLimitPerTradeFuture = Convert.ToInt32(sr.ReadLine());
                    us.AmountLimitPerTradeOption = Convert.ToInt32(sr.ReadLine());
                    us.LockContentPane = Convert.ToBoolean(sr.ReadLine());

                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsPromptEnabled = true;
                    }
                    else
                    {
                        us.IsPromptEnabled = Convert.ToBoolean(line);
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsStockVisible = true;
                    }
                    else
                    {
                        us.IsStockVisible = Convert.ToBoolean(line);
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsFutureVisible = true;
                    }
                    else
                    {
                        us.IsFutureVisible = Convert.ToBoolean(line);
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsOptionVisible = true;
                    }
                    else
                    {
                        us.IsOptionVisible = Convert.ToBoolean(line);
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsFundVisible = true;
                    }
                    else
                    {
                        us.IsFundVisible = Convert.ToBoolean(line);
                    }

                    line = sr.ReadLine();
                    if (line == null)
                    {
                        us.IsBasketVisible = true;
                    }
                    else
                    {
                        us.IsBasketVisible = Convert.ToBoolean(line);
                    }
                    sr.Close();
                }
                else
                {
                    us.FastTradeAmountStock = 100;
                    us.FastTradeAmountFuture = 1;
                    us.FastTradeAmountOption = 1;
                    us.MoneyLimitPerTrade = 1000000;
                    us.AmountLimitPerTradeStock = 10000;
                    us.AmountLimitPerTradeFuture = 10;
                    us.AmountLimitPerTradeOption = 10;
                    us.LockContentPane = false;
                    us.IsPromptEnabled = true;
                    us.IsStockVisible = true;
                    us.IsFutureVisible = true;
                    us.IsOptionVisible = true;
                    us.IsFundVisible = true;
                    us.IsBasketVisible = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + ex.Source + ex.StackTrace);
            }
        }


        public static void Filter(XamGrid xamGrid, System.Windows.Controls.CheckBox cbTraded, System.Windows.Controls.CheckBox cbCancelled, System.Windows.Controls.CheckBox cbObsolete, Intellibox SecurityCode = null)
        {
            var col = xamGrid.Columns.DataColumns["EntrustState"]; //Filter for last name column.

            xamGrid.FilteringSettings.FilteringScope = FilteringScope.ColumnLayout;
            xamGrid.FilteringSettings.RowFiltersCollection.Clear();

            if (col != null)
            {
                var colFilter = new RowsFilter(col.DataType, col);
                colFilter.Conditions.Clear();

                ComparisonCondition cc;

                if (cbTraded.IsChecked != null && cbTraded.IsChecked.Value)
                {
                    if (cbCancelled.IsChecked != null && cbCancelled.IsChecked.Value)
                    {
                        if (cbObsolete.IsChecked != null && cbObsolete.IsChecked.Value) //Case1 三个都勾选，显示所有
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.待撤2
                            };
                            colFilter.Conditions.Add(cc);
                        }
                        else //Case2 已成、已撤勾选，废单没有勾选，仅不显示废单
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.废单
                            };
                            colFilter.Conditions.Add(cc);
                        }
                    }
                    else
                    {
                        if (cbObsolete.IsChecked != null && cbObsolete.IsChecked.Value) //Case3 已成勾选，已撤没有勾选，废单勾选，仅不显示已撤、部撤
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已撤
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.部撤
                            };
                            colFilter.Conditions.Add(cc);
                        }
                        else //Case4 已成勾选，已撤、废单没有勾选，不显示废单、已撤、部撤
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.废单
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已撤
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.部撤
                            };
                            colFilter.Conditions.Add(cc);
                        }
                    }
                }
                else
                {
                    if (cbCancelled.IsChecked != null && cbCancelled.IsChecked.Value)
                    {
                        if (cbObsolete.IsChecked != null && cbObsolete.IsChecked.Value) //Case5 已成没有勾选，已撤、废单勾选，仅不显示已成
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已成
                            };
                            colFilter.Conditions.Add(cc);
                        }
                        else //Case6 已成没有勾选，已撤勾选，废单没有勾选，不显示已成和废单
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.废单
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已成
                            };
                            colFilter.Conditions.Add(cc);
                        }
                    }
                    else
                    {
                        if (cbObsolete.IsChecked != null && cbObsolete.IsChecked.Value) //Case7 已成、已撤没有勾选，废单勾选，不显示已成、已撤、部撤
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已撤
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已成
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.部撤
                            };
                            colFilter.Conditions.Add(cc);
                        }
                        else //Case8 全没有勾选，不显示已成、已撤、废单、部撤
                        {
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.废单
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已撤
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.已成
                            };
                            colFilter.Conditions.Add(cc);

                            colFilter.Conditions.LogicalOperator = LogicalOperator.And;
                            cc = new ComparisonCondition
                            {
                                Operator = ComparisonOperator.NotEquals, FilterValue = eEntrustState.部撤
                            };
                            colFilter.Conditions.Add(cc);
                        }
                    }
                }

                xamGrid.FilteringSettings.RowFiltersCollection.Add(colFilter);
            }

            //增加证券代码过滤
            if (SecurityCode != null && !string.IsNullOrEmpty(SecurityCode.SelectedValue as string))
            {
                col = xamGrid.Columns.DataColumns["SecurityID"]; //Filter for last name column.

                if (col == null) return;
                var colFilter = new RowsFilter(col.DataType, col);
                colFilter.Conditions.Clear();

                ComparisonCondition cc;

                cc = new ComparisonCondition
                {
                    Operator = ComparisonOperator.Equals, FilterValue = SecurityCode.SelectedValue
                };
                colFilter.Conditions.Add(cc);

                xamGrid.FilteringSettings.RowFiltersCollection.Add(colFilter);
            }
        }

        public static void FilterPosition(XamGrid xamGrid, System.Windows.Controls.CheckBox cbShowPosition0)
        {
            var col = xamGrid.Columns.DataColumns["CurrentAmount"]; //Filter for last name column.

            xamGrid.FilteringSettings.FilteringScope = FilteringScope.ColumnLayout;
            xamGrid.FilteringSettings.RowFiltersCollection.Clear();

            if (col == null) return;
            var colFilter = new RowsFilter(col.DataType, col);
            colFilter.Conditions.Clear();

            ComparisonCondition cc;

            if (cbShowPosition0.IsChecked != null && !cbShowPosition0.IsChecked.Value)
            {
                cc = new ComparisonCondition
                {
                    Operator = ComparisonOperator.NotEquals, FilterValue = 0
                };
                colFilter.Conditions.Add(cc);
            }

            xamGrid.FilteringSettings.RowFiltersCollection.Add(colFilter);
        }

        public static void FilterArbitrageStatus(XamGrid xamGrid, System.Windows.Controls.CheckBox cbShowCompleted)
        {
            var col = xamGrid.Columns.DataColumns["ArbitrageStatus"]; //Filter for last name column.

            xamGrid.FilteringSettings.FilteringScope = FilteringScope.ColumnLayout;
            xamGrid.FilteringSettings.RowFiltersCollection.Clear();

            if (col == null) return;
            var colFilter = new RowsFilter(col.DataType, col);
            colFilter.Conditions.Clear();

            ComparisonCondition cc;

            if (cbShowCompleted.IsChecked != null && !cbShowCompleted.IsChecked.Value)
            {
                cc = new ComparisonCondition
                {
                    Operator = ComparisonOperator.NotEquals,
                    FilterValue = ArbitrageStatus.已执行
                };
                colFilter.Conditions.Add(cc);
            }

            xamGrid.FilteringSettings.RowFiltersCollection.Add(colFilter);
        }

        public static void LogException(LogUtils logger, Exception ex)
        {
            logger.Error(ex.Message + ex.Source + ex.StackTrace);
        }

        public static void NewEntrust(EntrustInfo ei, UserSettings userSettings, HsStock trader)
        {
            switch (ei.Category)
            {
                case eCategory.股票:
                {
                    if (ei.EntrustAmount > userSettings.AmountLimitPerTradeStock)
                    {
                        System.Windows.MessageBox.Show(String.Format("单笔股票委托数量不得超过{0}!", userSettings.AmountLimitPerTradeStock), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                }
                case eCategory.期货:
                {
                    if (ei.EntrustAmount > userSettings.AmountLimitPerTradeFuture)
                    {
                        System.Windows.MessageBox.Show(String.Format("单笔期货委托数量不得超过{0}!", userSettings.AmountLimitPerTradeFuture), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                }
                case eCategory.期权:
                {
                    if (ei.EntrustAmount > userSettings.AmountLimitPerTradeOption)
                    {
                        System.Windows.MessageBox.Show(String.Format("单笔期权委托数量不得超过{0}!", userSettings.AmountLimitPerTradeOption), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                }
                case eCategory.债券回购:
                {
                    if (ei.EntrustAmount > userSettings.AmountLimitPerTradeStock)
                    {
                        System.Windows.MessageBox.Show(String.Format("单笔债券回购委托数量不得超过{0}!", userSettings.AmountLimitPerTradeStock), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                }
                case eCategory.指数:
                    break;
                case eCategory.债券:
                    break;
                case eCategory.基金:
                    break;
                case eCategory.基金分拆合并:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (ei.EntrustAmount*ei.EntrustPrice > userSettings.MoneyLimitPerTrade)
            {
                System.Windows.MessageBox.Show(String.Format("单笔委托金额上限不得超过{0}!", userSettings.MoneyLimitPerTrade), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("证券代码:" + ei.SecurityID);
            sb.AppendLine("交易市场:" + ei.MarketType);
            sb.AppendLine("委托方向:" + ei.EntrustDirection);

            if (ei.CombiNo != null)
            {
                sb.AppendLine("组合编号:" + ei.CombiNo);
            }
            else
            {
                System.Windows.MessageBox.Show("请先选择组合编号！");
                return;
            }

            if (ei is FutureEntrustInfo || ei is OptionEntrustInfo)
            {
                sb.AppendLine("开平仓:" + ei.FuturesDirection);
                sb.AppendLine("投资类型:" + ei.InvestType);
            }

            if (ei is FundEntrustInfo)
            {
                sb.AppendLine("委托金额:" + ei.EntrustBalance);
                sb.AppendLine("申赎方式:" + ei.PurchaseWay);
            }

            if (!(ei is FundEntrustInfo))
            {
                sb.AppendLine("价格类型:" + ei.EntrustPriceType);
            }

            sb.AppendLine("委托价格:" + ei.EntrustPrice.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine("委托数量:" + ei.EntrustAmount);

            if (userSettings.IsPromptEnabled)
            {
                if (
                    System.Windows.MessageBox.Show(string.Format("确认委托？\n{0}", sb), "提示", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
            }

            Entrust entrust;

            if (ei is FundEntrustInfo)
            {
                entrust = new FundEntrust();
            }
            else if (ei is OptionEntrustInfo)
            {
                entrust = new OptionEntrust();
            }
            else if (ei is FutureEntrustInfo)
            {
                entrust = new FutureEntrust();
            }
            else
            {
                entrust = new Entrust();
            }

            entrust.StockCode = ei.SecurityID;
            entrust.MarketNo = eMarketTypeToeMarketNo(ei.MarketType);
            entrust.CombiNo = ei.CombiNo;
            entrust.EntrustDirection = eEntrustDirectionToEntrustDirection(ei.EntrustDirection);
            entrust.PriceType = eEntrustPriceTypeToEntrustPriceType(ei.EntrustPriceType);
            entrust.EntrustPrice = ei.EntrustPrice;
            entrust.EntrustAmount = ei.EntrustAmount;
            entrust.FuturesDirection = eFuturesDirectionToFuturesDirection(ei.FuturesDirection);
            entrust.EntrustBalance = ei.EntrustBalance;
            entrust.PurchaseWay = ePurchaseWayToPurchaseWay(ei.PurchaseWay);
            entrust.ExtsystemId = CurrentExtSystemId++;

            if (ei is FundEntrustInfo)
            {
                trader.InsertOrderFund((FundEntrust) entrust);
            }
            else
            {
                trader.InsertOrder(entrust);
            }
        }


        public static void ExportToCsv<T>(string prompt, IEnumerable<T> list) where T : IExport
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = prompt, Filter = @"csv files(*.csv)|*.csv|All files(*.*)|*.*", RestoreDirectory = true
                };

                if (dialog.ShowDialog() != DialogResult.OK) return;
                var sw = new StreamWriter(dialog.FileName, false, Encoding.UTF8);

                var bSetTitle = false;

                foreach (var ei in list)
                {
                    if (!bSetTitle)
                    {
                        sw.WriteLine(ei.GetTitle());
                        bSetTitle = true;
                    }
                    sw.WriteLine(ei.Export());
                }

                sw.Close();

                System.Windows.MessageBox.Show("导出csv文件成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information,
                    MessageBoxResult.OK);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(string.Format("序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
            }
        }

        public static void ImportFromCsv<T>(string prompt, ObservableCollection<T> list, bool bIgnoreTitle = true) where T : IImport, new()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = prompt, Filter = @"csv files(*.csv)|*.csv|All files(*.*)|*.*"
                };

                if (dialog.ShowDialog() != DialogResult.OK) return;

                var sr = new StreamReader(dialog.FileName, Encoding.UTF8);
                try
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var s = line.Split(new[] {','}, StringSplitOptions.None);

                        if (bIgnoreTitle)
                        {
                            bIgnoreTitle = false;
                            continue;
                        }

                        var ei = new T();
                        ei.Import(s);
                        list.Add(ei);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(string.Format("反序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(string.Format("反序列化失败!{0},{1},{2}", e.Message, e.Source, e.StackTrace));
            }
        }
    }
}

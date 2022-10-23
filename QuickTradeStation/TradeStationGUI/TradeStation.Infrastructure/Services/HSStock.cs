using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using hundsun.t2sdk;
using hundsun.mcapi;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel.Composition;
using TradeStation.Infrastructure.CommonUtils;
using Microsoft.Practices.Prism.PubSubEvents;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net.Repository.Hierarchy;
using Microsoft.Practices.Prism.Mvvm;
using Timer = System.Timers.Timer;

namespace TradeStation.Infrastructure.Services
{
    [Export(typeof(HsStock))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class HsStock : BindableBase
    {
        private CallbackStock _cb;

        private CT2Connection _connection;
        private SubCallBack _subcallback;
        public string UserToken;

        /// <summary>
        /// 对应文档中Cx类型，x为大于1的整数
        /// </summary>
        private readonly sbyte _strType = Convert.ToSByte('S');

        /// <summary>
        /// 对应文档中Nx类型，x为整数
        /// </summary>
        private readonly sbyte _intType = Convert.ToSByte('I');

        /// <summary>
        /// 对应文档中C1类型
        /// </summary>
        private readonly sbyte _charType = Convert.ToSByte('C');

        /// <summary>
        /// 对应文档中Nxx.yy类型
        /// </summary>
        private readonly sbyte _floatType = Convert.ToSByte('F');

        private readonly sbyte _rawType = Convert.ToSByte('R');

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public MarketDataService Marketdataservice { get; set; }

        [Import]
        public PositionInfoCollection PositionInfoCollection { get; set; }

        [Import]
        public EntrustInfoCollection EntrustInfoCollection { get; set; }

        [Import]
        public TradeResultInfoCollection TradeResultInfoCollection { get; set; }

        [Import]
        public InstrumentInfoCollection InstrumentInfoCollection { get; set; }

        [Import]
        public EtfBaseInfoCollection EtfBaseInfoCollection { get; set; }

        [Import]
        public EtfStockCollection EtfStockCollection { get; set; }

        public ConcurrentDictionary<string, LegNumberToArbitrage> LegDictionary =
            new ConcurrentDictionary<string, LegNumberToArbitrage>();
 
        public ConcurrentDictionary<int, Arbitrage> HoldingArbitrages = new ConcurrentDictionary<int, Arbitrage>();
 
        private ObservableCollection<string> _accountCodeList = new ObservableCollection<string>();

        private ObservableCollection<EntrustInfo> _etfEntrustDetail = new ObservableCollection<EntrustInfo>();
        
        public ObservableCollection<EntrustInfo> EtfEntrustDetail
        {
            get { return _etfEntrustDetail; }
            set { SetProperty(ref _etfEntrustDetail, value); }
        }


        private ObservableCollection<TradeResultInfo> _etfTradeResultDetail = new ObservableCollection<TradeResultInfo>();

        public ObservableCollection<TradeResultInfo> EtfTradeResultDetail
        {
            get { return _etfTradeResultDetail; }
            set { SetProperty(ref _etfTradeResultDetail, value); }
        }

        private ObservableCollection<EntrustInfo> _entrustInfoQueue = new ObservableCollection<EntrustInfo>();

        public ObservableCollection<EntrustInfo> EntrustInfoQueue
        {
            get { return _entrustInfoQueue; }
        }

        private bool _isStockTradeResultReady = true;

        public bool IsStockTradeResultReady
        {
            get { return _isStockTradeResultReady; }
            set { SetProperty(ref _isStockTradeResultReady, value); }
        }

        private bool _isFutureTradeResultReady = true;

        public bool IsFutureTradeResultReady
        {
            get { return _isFutureTradeResultReady; }
            set { SetProperty(ref _isFutureTradeResultReady, value); }
        }

        private bool _isOptionTradeResultReady = true;

        public bool IsOptionTradeResultReady
        {
            get { return _isOptionTradeResultReady; }
            set { SetProperty(ref _isOptionTradeResultReady, value); }
        }

        private bool _isFundTradeResultReady = true;

        public bool IsFundTradeResultReady
        {
            get { return _isFundTradeResultReady; }
            set { SetProperty(ref _isFundTradeResultReady, value); }
        }

        private bool _isStockPositionReady = true;

        public bool IsStockPositionReady
        {
            get { return _isStockPositionReady; }
            set { SetProperty(ref _isStockPositionReady, value); }
        }

        private bool _isFuturePositionReady = true;

        public bool IsFuturePositionReady
        {
            get { return _isFuturePositionReady; }
            set { SetProperty(ref _isFuturePositionReady, value); }
        }

        private bool _isOptionPositionReady = true;

        public bool IsOptionPositionReady
        {
            get { return _isOptionPositionReady; }
            set { SetProperty(ref _isOptionPositionReady, value); }
        }

        private bool _isFundPositionReady = true;

        public bool IsFundPositionReady
        {
            get { return _isFundPositionReady; }
            set { SetProperty(ref _isFundPositionReady, value); }
        }

        private bool _isStockEntrustReady = true;

        public bool IsStockEntrustReady
        {
            get { return _isStockEntrustReady; }
            set { SetProperty(ref _isStockEntrustReady, value); }
        }

        private bool _isFutureEntrustReady = true;

        public bool IsFutureEntrustReady
        {
            get { return _isFutureEntrustReady; }
            set { SetProperty(ref _isFutureEntrustReady, value); }
        }

        private bool _isOptionEntrustReady = true;

        public bool IsOptionEntrustReady
        {
            get { return _isOptionEntrustReady; }
            set { SetProperty(ref _isOptionEntrustReady, value); }
        }

        private bool _isFundEntrustReady = true;

        public bool IsFundEntrustReady
        {
            get { return _isFundEntrustReady; }
            set { SetProperty(ref _isFundEntrustReady, value); }
        }

        public bool IsReadyToProcessingEntrustState
        {
            get { return IsEntrustReady && IsPositionReady && IsTradeResultReady ; }
        }

        public bool IsEntrustReady
        {
            get
            {
                return _isStockEntrustReady && _isFutureEntrustReady && _isOptionEntrustReady && _isFundEntrustReady;
            }
        }

        public bool IsPositionReady
        {
            get { return _isStockPositionReady && _isFuturePositionReady && _isOptionPositionReady && _isFundPositionReady; }
        }

        public bool IsTradeResultReady
        {
            get { return _isStockTradeResultReady && _isFutureTradeResultReady && _isOptionTradeResultReady && _isFundTradeResultReady; }
        }

        public ObservableCollection<string> AccountCodeList
        {
            get { return _accountCodeList; }
            set
            {
                SetProperty(ref _accountCodeList, value);
            }
        }

        public Dictionary<string, string> CombiNo2AccountCode = new Dictionary<string, string>();

        public Dictionary<string, Entrust> PendingCombinedRequest = new Dictionary<string, Entrust>();

        private Timer _processEntrustInfoTimer = new Timer(5000);
             
        [ImportingConstructor]
        public HsStock(IEventAggregator eventAggr)
        {
            EventAggregator = eventAggr;
            _cb = new CallbackStock(this);

            _processEntrustInfoTimer.Elapsed += t_Elapsed;
            _processEntrustInfoTimer.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("进入定时器");

            lock (this)
            {
                if (_entrustInfoQueue.Count > 0)
                {
                    var removedEntrustInfos = new List<EntrustInfo>();

                    for (var i = 0; i < _entrustInfoQueue.Count; i++)
                    {
                        var ei = _entrustInfoQueue[i];

                        if (IsEntrustInList(ei.EntrustNo, ei.Category) ||
                            IsEntrustInList(ei.CancelEntrustNo, ei.Category))
                            //排在头部的委托消息是否应该发送，如果不能发送，则推迟；或者该笔委托是撤单委托,撤单委托号存在于委托列表中。
                        {
                            _entrustInfoQueue.RemoveAt(0);
                            Logger.Debug(string.Format("定时器中发送委托消息:{0}", ei));
                            ProcessEntrustState(ei);
                            removedEntrustInfos.Add(ei);
                        }
                    }

                    //移除已经处理的消息
                    foreach (var removedEntrustInfo in removedEntrustInfos)
                    {
                        _entrustInfoQueue.Remove(removedEntrustInfo);
                    }
                }
            }

            Logger.Debug("退出定时器");
        }

        public void SetTraderForSecurityQuote(string marketNo, string securityId)
        {
            var securityQuote = Marketdataservice.GetAndSubscribeSecurityQuote(CommonUtil.MarketNoToExId(marketNo), securityId);
            if (securityQuote != null)
            {
                securityQuote.Trader = this;
            }
        }

        public void SetTraderForSecurityQuote(eMarketType marketType, string securityId)
        {
            var securityQuote =
                Marketdataservice.GetAndSubscribeSecurityQuote(
                    CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(marketType)), securityId);
            if (securityQuote != null)
            {
                securityQuote.Trader = this;
            }
        }

        /// <summary>
        /// 判断委托号是否已经存在于委托列表中
        /// </summary>
        /// <param name="entrustNo"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool IsEntrustInList(int entrustNo, eCategory category)
        {
            var count = 0;
            switch (category)
            {
                case eCategory.股票:
                case eCategory.债券回购:
                case eCategory.基金:
                {
                    var q = from info in EntrustInfoCollection.StockEntrustInfoList
                            where entrustNo.Equals(info.EntrustNo)
                        select info;
                    count = q.Count();
                    break;
                }
                case eCategory.期货:
                {
                    var q = from info in EntrustInfoCollection.FutureEntrustInfoList
                        where entrustNo.Equals(info.EntrustNo)
                        select info;
                    count = q.Count();
                    break;
                }
                case eCategory.期权:
                {
                    var q = from info in EntrustInfoCollection.OptionEntrustInfoList
                        where entrustNo.Equals(info.EntrustNo)
                        select info;
                    count = q.Count();
                    break;
                }
                case eCategory.基金分拆合并:
                {
                    var q = from info in EntrustInfoCollection.FundEntrustInfoList
                            where entrustNo.Equals(info.EntrustNo)
                            select info;
                    count = q.Count();
                    break;
                }
            }

            return count > 0;
        }

        public void ProcessEntrustState(EntrustInfo ei)
        {
            switch (ei.MsgType)
            {
                case eMessageType.委托下达:
                case eMessageType.委托确认:
                case eMessageType.委托废单:
                case eMessageType.委托撤单:
                case eMessageType.委托撤成:
                case eMessageType.委托撤废:
                {
                    Logger.Info(string.Format("@[{0}]:{1}", ei.MsgType, ei));                                       
                    Publish(ei);
                    break;
                }
                case eMessageType.委托成交:
                {
                    Logger.Info(string.Format("@[{0}]:{1}", ei.MsgType, ei));     

                    //通知委托界面
                    Publish(ei);

                    //通知成交界面
                    TradeResultInfo tri = null;

                    switch (ei.Category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        {
                            tri = new TradeResultInfo();
                            break;
                        }
                        case eCategory.期货:
                        {
                            tri = new FutureTradeResultInfo();
                            break;
                        }
                        case eCategory.期权:
                        {
                            tri = new OptionTradeResultInfo();
                            break;
                        }
                        case eCategory.基金分拆合并:
                        {
                            tri = new FundTradeResultInfo();
                            break;
                        }
                    }

                    tri.Category = ei.Category;
                    tri.SecurityID = ei.SecurityID;
                    tri.SecurityName = ei.SecurityName;
                    tri.CombiNo = ei.CombiNo;
                    tri.DealAmount = ei.DealAmount;
                    tri.DealDate = ei.DealDate;
                    tri.DealPrice = ei.DealPrice;
                    tri.DealTime = ei.DealTime;
                    tri.EntrustDirection = ei.EntrustDirection;
                    tri.EntrustNo = ei.EntrustNo;
                    tri.FuturesDirection = ei.FuturesDirection;
                    tri.DealNo = ei.DealNo;
                    tri.StockholderId = ei.StockholderId;

                    switch (tri.Category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        {
                            EventAggregator.GetEvent<TradeResultNotifyEvent>().Publish(tri);
                            break;
                        }
                        case eCategory.期货:
                        {
                            EventAggregator.GetEvent<FutureTradeResultNotifyEvent>()
                                .Publish(tri as FutureTradeResultInfo);
                            break;
                        }
                        case eCategory.期权:
                        {
                            EventAggregator.GetEvent<OptionTradeResultNotifyEvent>()
                                .Publish(tri as OptionTradeResultInfo);
                            break;
                        }
                        case eCategory.基金分拆合并:
                        {
                            EventAggregator.GetEvent<FundTradeResultNotifyEvent>()
                                .Publish(tri as FundTradeResultInfo);
                            break;
                        }
                    }

                    //通知持仓界面
                    PositionInfoBase pi = null;

                    switch (ei.Category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        {
                            pi = new PositionInfo();
                            pi.CurrentAmount = ei.DealAmount;
                            pi.DealAmount = ei.EntrustDirection == eEntrustDirection.买入 ||
                                            ei.EntrustDirection == eEntrustDirection.融券回购
                                ? ei.DealAmount
                                : -ei.DealAmount;
                            pi.DealPrice = ei.DealPrice;
                            break;
                        }
                        case eCategory.期货:
                        {
                            pi = new FuturePositionInfo();
                            pi.CurrentAmount = ei.DealAmount;
                            pi.EntrustDirection = ei.EntrustDirection;
                            pi.FuturesDirection = ei.FuturesDirection;
                            pi.DealAmount = ei.DealAmount;
                            pi.DealPrice = ei.DealPrice;

                            break;
                        }
                        case eCategory.期权:
                        {
                            pi = new OptionPositionInfo();
                            pi.CurrentAmount = ei.DealAmount;
                            pi.EntrustDirection = ei.EntrustDirection;
                            pi.FuturesDirection = ei.FuturesDirection;
                            pi.DealAmount = ei.DealAmount;
                            pi.DealPrice = ei.DealPrice;

                            break;
                        }
                    }

                    pi.Category = ei.Category;
                    pi.MsgType = ePositionInfoMsgType.交易;
                    pi.HoldSeat = ei.ReportSeat;
                    pi.CombiNo = ei.CombiNo;
                    pi.MarketType = ei.MarketType;
                    pi.PositionFlag = ei.EntrustDirection == eEntrustDirection.买入
                        ? ePositionFlag.多头持仓
                        : ePositionFlag.空头持仓;
                    pi.StockholderId = ei.StockholderId;
                    pi.SecurityID = ei.SecurityID;
                    pi.SecurityName = ei.SecurityName;

                    switch (ei.Category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        {
                            EventAggregator.GetEvent<PositionInfoNotifyEvent>()
                                .Publish(pi as PositionInfo);
                            break;
                        }
                        case eCategory.期货:
                        {
                            EventAggregator.GetEvent<FuturePositionInfoNotifyEvent>()
                                .Publish(pi as FuturePositionInfo);
                            break;
                        }
                        case eCategory.期权:
                        {
                            EventAggregator.GetEvent<OptionPositionInfoNotifyEvent>()
                                .Publish(pi as OptionPositionInfo);
                            break;
                        }
                    }
                    break;
                }
                case eMessageType.委托查询:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void PrintUnPack(CT2UnPacker unPacker)
        {
            try
            {
                Logger.Debug("*****************************数据包内容开始********************************");
                int i = 0, t = 0, j = 0, k = 0;

                var dicColNameColValue = new Dictionary<string, string>();
                var dataSetCount = unPacker.GetDatasetCount();
                for (i = 0; i < dataSetCount; ++i)
                {
                    Logger.Debug(string.Format("第{0}个数据集", i));

                    // 设置当前结果集
                    unPacker.SetCurrentDatasetByIndex(i);
                    unPacker.First();

                    Logger.Debug(string.Format("记录行数：{0}", unPacker.GetRowCount()));
                    Logger.Debug(string.Format("记录列数：{0}", unPacker.GetColCount()));

                    var rowCount = unPacker.GetRowCount();

                    for (j = 0; j < rowCount; j++)
                    {
                        // 打印字段
                        var colCount = unPacker.GetColCount();
                        for (t = 0; t < colCount; ++t)
                        {
                            var colName = unPacker.GetColName(t);
                            var colType = unPacker.GetColType(t);

                            string colValue = null;
                            if (colType == _intType)
                            {
                                colValue = Convert.ToString(unPacker.GetIntByIndex(t));
                            }
                            else
                            {
                                if (colType == _charType)
                                {
                                    colValue = Convert.ToString(unPacker.GetCharByIndex(t));
                                }
                                else
                                {
                                    if (colType == _strType)
                                    {
                                        colValue = unPacker.GetStrByIndex(t);
                                    }
                                    else
                                    {
                                        if (colType == _floatType)
                                        {
                                            colValue = Convert.ToString(unPacker.GetDoubleByIndex(t), CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            if (colType == _rawType)
                                            {
                                                var nLength = 0;
                                                unsafe
                                                {
                                                    unPacker.GetRawByIndex(t, &nLength);
                                                }
                                                Logger.Debug("二进制数据，未处理");
                                            }
                                            else
                                            {
                                                Logger.Debug("未知数据类型。\n");
                                            }
                                        }
                                    }
                                }
                            }

                            Logger.Debug(string.Format("{0}:{1}", colName, colValue));
                       
                            dicColNameColValue[colName] = colValue;
                        }

                        unPacker.Next();
                    }


                }


                if (dicColNameColValue.ContainsKey("ErrorCode"))
                {
                    if (!Convert.ToInt32(dicColNameColValue["ErrorCode"]).Equals(0))
                    {
                        if (dicColNameColValue["ErrorCode"].Equals("1000")) //用户未登陆或user_token失效，需要尝试重新登录
                        {
                            EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("尝试重新登录...");
                            Logger.Debug(string.Format("尝试重新登录,{0},{1}", AppConfigService.OperatorName,
                                AppConfigService.OperatorPassword));
                            Task.Factory.StartNew(
                                () =>
                                {
                                    LoginAccount(AppConfigService.OperatorName, AppConfigService.OperatorPassword);
                                });
                        }
                        if (dicColNameColValue["ErrorCode"].Equals("4002")) //流量超限的提示，不使用对话框，避免反复弹出
                        {
                            EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("流量超限!");
                            Logger.Debug("流量超限!");
                        }
                        else
                        {
                            var items = new List<MessageBoxItem>();

                            var item = new MessageBoxItem
                            {
                                Title = "错误",
                                Content = dicColNameColValue["ErrorMsg"],
                                TextBrush = System.Windows.Media.Brushes.Red
                            };
                            items.Add(item);

                            if (dicColNameColValue.ContainsKey("risk_summary"))
                            {
                                item = new MessageBoxItem
                                {
                                    Title = "概要",
                                    Content = dicColNameColValue["risk_summary"],
                                    TextBrush = System.Windows.Media.Brushes.Red
                                };
                                items.Add(item);
                            }

                            if (dicColNameColValue.ContainsKey("remark_short"))
                            {
                                item = new MessageBoxItem
                                {
                                    Title = "详细",
                                    Content = dicColNameColValue["remark_short"],
                                    TextBrush = System.Windows.Media.Brushes.Red
                                };
                                items.Add(item);
                            }

                            if (dicColNameColValue.ContainsKey("MsgDetail"))
                            {
                                if (!string.IsNullOrEmpty(dicColNameColValue["MsgDetail"]))
                                {
                                    item = new MessageBoxItem
                                    {
                                        Title = "详细",
                                        Content = dicColNameColValue["MsgDetail"],
                                        TextBrush = System.Windows.Media.Brushes.Red
                                    };
                                    items.Add(item);
                                }
                            }

                            DispatcherService.Invoke(() =>
                            {
                                var win = new WinDialog(items, SystemIcons.Error.ToImageSource(), MessageBoxButton.OK);
                                win.Show();
                            });
                        }
                    }
                }
                Logger.Debug("*****************************数据包内容结束********************************");
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        public void PublishLogMessage(LogMessageLevel logLevel, string message)
        {
            EventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity { LogLevel = logLevel, Message = message });
        }

        public double GetPrice(double refAskPrice, double refBidPrice, double tick, eEntrustDirection entrustDirection)
        {
            double price = 0;

            if (entrustDirection == eEntrustDirection.买入)
            {
                if (refAskPrice > 0)
                {
                    price = refAskPrice + tick;
                }
            }
            else
            {
                if (refBidPrice > 0)
                {
                    price = refBidPrice - tick;
                }
            }

            return price;
        }

        public void ReturnEntrustInfo(ObservableCollection<EntrustInfo> entrustInfoList, EntrustInfo entrustInfo)
        {
            Logger.Debug(string.Format("ReturnEntrustInfo:{0}", entrustInfo));

            DispatcherService.Invoke(() =>
            {
                switch (entrustInfo.MsgType)
                {
                    case eMessageType.委托下达:
                        {
                            entrustInfoList.Add(entrustInfo);
                            break;
                        }
                    case eMessageType.委托确认:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.EntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustState = entrustInfo.EntrustState;
                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托成交:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.EntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.DealDate = entrustInfo.DealDate;
                                ei.DealTime = entrustInfo.DealTime;
                                ei.DealPrice = entrustInfo.DealPrice;
                                ei.DealAmount = entrustInfo.DealAmount;
                                ei.TotalDealAmount = entrustInfo.TotalDealAmount;
                                ei.EntrustState = entrustInfo.EntrustState;

                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托废单:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.EntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustState = entrustInfo.EntrustState;
                                ei.RevokeCause = entrustInfo.RevokeCause;
                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托撤单:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.CancelEntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustState = entrustInfo.EntrustState;
                                ei.RevokeCause = entrustInfo.RevokeCause;
                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托撤成:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.CancelEntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustState = entrustInfo.EntrustState;
                                ei.CancelAmount = entrustInfo.CancelAmount;
                                entrustInfoList.Insert(i, ei);

                                // Remove the pending request if the order has been cancelled.
                                if (null != ei.ExtsystemId && PendingCombinedRequest.ContainsKey(ei.ExtsystemId))
                                {
                                    PendingCombinedRequest.Remove(ei.ExtsystemId);
                                }

                                if (ei.IsChasingOrder && (ei.EntrustState == eEntrustState.已撤 || ei.EntrustState == eEntrustState.部撤)) //是否正在追单的委托，并且撤单已经成功，进行追单
                                {
                                    var exId =
                                        CommonUtil.MarketNoToExId(CommonUtil.eMarketTypeToeMarketNo(ei.MarketType));
                                    var securityQuotation =
                                        Marketdataservice.GetAndSubscribeSecurityQuote(exId, ei.SecurityID);

                                    if (securityQuotation != null)
                                    {
                                        double price;
                                        var securityInfo = Marketdataservice.GetSecurityInfo(exId, ei.SecurityID);
                                        double tick = 0;

                                        if (securityInfo != null)
                                        {
                                            tick = securityInfo.MinFloatingPrice;
                                        }

                                        switch (ei.ChaseOrderType)
                                        {
                                            case eChaseOrderType.最新价:
                                            {
                                                price = securityQuotation.LastPx;
                                                break;
                                            }
                                            case eChaseOrderType.对一价:
                                            {
                                                if (entrustInfo.EntrustDirection == eEntrustDirection.买入)
                                                {
                                                    price = securityQuotation.AskPx1;
                                                }
                                                else
                                                {
                                                    price = securityQuotation.BidPx1;
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.LastPx, securityQuotation.LastPx,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                break;
                                            }
                                            case eChaseOrderType.对二价:
                                            {
                                                if (entrustInfo.EntrustDirection == eEntrustDirection.买入)
                                                {
                                                    price = securityQuotation.AskPx2;
                                                }
                                                else
                                                {
                                                    price = securityQuotation.BidPx2;
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx1, securityQuotation.BidPx1,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.LastPx, securityQuotation.LastPx,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                break;
                                            }
                                            case eChaseOrderType.对三价:
                                            {
                                                if (entrustInfo.EntrustDirection == eEntrustDirection.买入)
                                                {
                                                    price = securityQuotation.AskPx3;
                                                }
                                                else
                                                {
                                                    price = securityQuotation.BidPx3;
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx2, securityQuotation.BidPx2,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx1, securityQuotation.BidPx1,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.LastPx, securityQuotation.LastPx,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                break;
                                            }
                                            case eChaseOrderType.对四价:
                                            {
                                                if (entrustInfo.EntrustDirection == eEntrustDirection.买入)
                                                {
                                                    price = securityQuotation.AskPx4;
                                                }
                                                else
                                                {
                                                    price = securityQuotation.BidPx4;
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx3, securityQuotation.BidPx3,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx2, securityQuotation.BidPx2,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx1, securityQuotation.BidPx1,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.LastPx, securityQuotation.LastPx,
                                                        tick, entrustInfo.EntrustDirection);
                                                }
                                                break;
                                            }
                                            case eChaseOrderType.对五价:
                                            {
                                                if (entrustInfo.EntrustDirection == eEntrustDirection.买入)
                                                {
                                                    price = securityQuotation.AskPx5;
                                                }
                                                else
                                                {
                                                    price = securityQuotation.BidPx5;
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx4, securityQuotation.BidPx4,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx3, securityQuotation.BidPx3,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx2, securityQuotation.BidPx2,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.AskPx1, securityQuotation.BidPx1,
                                                        tick, entrustInfo.EntrustDirection);
                                                }

                                                if (price <= 0)
                                                {
                                                    price = GetPrice(securityQuotation.LastPx, securityQuotation.LastPx,
                                                        tick, entrustInfo.EntrustDirection);
                                                }
                                                break;
                                            }
                                            default:
                                            {
                                                price = securityQuotation.LastPx;
                                                break;
                                            }
                                        }

                                        if (price > 0)
                                        {
                                            var entrustAmount = ei.EntrustAmount;
                                            if (ei.EntrustState == eEntrustState.部撤)
                                            {
                                                entrustAmount = ei.CancelAmount;
                                            }

                                            var entrust = CommonUtil.BuildEntrustInfo(ei.SecurityID, price,
                                                ei.MarketType, ei.CombiNo, entrustAmount,
                                                ei.EntrustDirection, ei.EntrustPriceType, ei.Category,
                                                ei.FuturesDirection, ei.InvestType);

                                            switch (ei.Category)
                                            {
                                                case eCategory.股票:
                                                {
                                                    EventAggregator.GetEvent<NewStockEntrustNotifyEvent>().Publish(entrust);
                                                    break;
                                                }
                                                case eCategory.期货:
                                                {
                                                    EventAggregator.GetEvent<NewFutureEntrustNotifyEvent>().Publish(entrust as FutureEntrustInfo);
                                                    break;
                                                }
                                                case eCategory.期权:
                                                {
                                                    EventAggregator.GetEvent<NewOptionEntrustNotifyEvent>()
                                                        .Publish(entrust as OptionEntrustInfo);
                                                    break;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            MessageBox.Show(string.Format("{0}为{1}，无效值！", ei.ChaseOrderType, price),
                                                "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托撤废:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.CancelEntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustState = entrustInfo.EntrustState;
                                ei.RevokeCause = entrustInfo.RevokeCause;
                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                Logger.Debug(string.Format("未找到编号为{0}的委托！", entrustInfo.EntrustNo));
                            }
                            break;
                        }
                    case eMessageType.委托查询:
                        {
                            var q = from ei in entrustInfoList where ei.EntrustNo.Equals(entrustInfo.EntrustNo) select ei;

                            if (q.Count() > 0)
                            {
                                var ei = q.First();
                                var i = entrustInfoList.IndexOf(ei);
                                entrustInfoList.Remove(ei);
                                ei.EntrustAmount = entrustInfo.EntrustAmount;
                                ei.EntrustBatchNo = entrustInfo.EntrustBatchNo;
                                ei.EntrustDate = entrustInfo.EntrustDate;
                                ei.EntrustTime = entrustInfo.EntrustTime;
                                ei.EntrustDirection = entrustInfo.EntrustDirection;
                                ei.EntrustNo = entrustInfo.EntrustNo;
                                ei.EntrustPrice = entrustInfo.EntrustPrice;
                                ei.EntrustPriceType = entrustInfo.EntrustPriceType;
                                ei.SecurityID = entrustInfo.SecurityID;
                                ei.SecurityName = entrustInfo.SecurityName;
                                ei.EntrustState = entrustInfo.EntrustState;
                                ei.CancelAmount = entrustInfo.CancelAmount;
                                ei.DealPrice = entrustInfo.DealPrice;
                                ei.MarketType = entrustInfo.MarketType;
                                ei.TotalDealAmount = entrustInfo.TotalDealAmount;
                                ei.RevokeCause = entrustInfo.RevokeCause;
                                ei.FuturesDirection = entrustInfo.FuturesDirection;

                                entrustInfoList.Insert(i, ei);
                            }
                            else
                            {
                                entrustInfoList.Add(entrustInfo);
                            }
                            break;
                        }
                    //case eMessageType.清空委托:
                    //    {
                    //        entrustInfoList.Clear();
                    //        break;
                    //    }
                }


            });
        }

        public void Publish(EntrustInfo ei)
        {
            try
            {
                switch (ei.Category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                        {
                            EventAggregator.GetEvent<EntrustInfoNotifyEvent>().Publish(ei);
                            break;
                        }
                    case eCategory.期货:
                        {
                            EventAggregator.GetEvent<FutureEntrustInfoNotifyEvent>().Publish(ei as FutureEntrustInfo);
                            break;
                        }
                    case eCategory.期权:
                        {
                            EventAggregator.GetEvent<OptionEntrustInfoNotifyEvent>().Publish(ei as OptionEntrustInfo);
                            break;
                        }
                    case eCategory.基金分拆合并:
                        {
                            EventAggregator.GetEvent<FundEntrustInfoNotifyEvent>().Publish(ei as FundEntrustInfo);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        //建立交易服务器连接
        public bool ConnectTradeService(CT2Configinterface config, string fundAccount, string fundAccountPassword)
        {
            try
            {
                _connection = new CT2Connection(config);
                _connection.Create2BizMsg(_cb);
                var iret = _connection.Connect(5000);
                EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("连接服务器...");
                Logger.Info("连接服务器...");

                if (iret == 0) //连接成功
                {
                    EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("连接服务器成功...");
                    Logger.Info("连接服务器成功...");
                    LoginAccount(fundAccount, fundAccountPassword);
                }
                else
                {
                    MessageBox.Show("账户【" + fundAccount + "】连接失败！错误码：" + iret.ToString() + "错误信息:" + _connection.GetErrorMsg(iret), "账户连接错误");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }

            return false;
        }


        private CT2Connection _connection2;
        private CT2SubscribeInterface _subcribe;
        private int _subcribeid;

        /************************************************************************/
        /* 接口实现                                                                     */
        /************************************************************************/

        // 建立订阅服务器连接
        public bool ConnectSubcribeService(Subscribe param)
        {
            try
            {
                var mcconfig = new CT2Configinterface();
                mcconfig.SetString("t2sdk", "servers", string.Format("{0}:{1}", AppConfigService.SubscribeServerIp, AppConfigService.SubscribeServerPort));
                mcconfig.SetString("t2sdk", "license_file", AppConfigService.LicenseFile);

                Logger.Debug(string.Format("证书文件地址:{0}", AppConfigService.LicenseFile));
                if (null == _connection2)
                {
                    _connection2 = new CT2Connection(mcconfig);
                    _connection2.Create(null);
                }

                // 判断连接是否已建立，若建立，不再重复连接和订阅，避免因此造成的报错
                var isConnectionRegistered = _connection2.GetStatus() & (int)SubscribeServiceConnectionStatus.Registered;
                if (isConnectionRegistered <= 0)
                {
                    var retu = _connection2.Connect(5000);
                    if (retu != 0)
                    {
                        PublishLogMessage(LogMessageLevel.ERROR, string.Format("错误:连接{0}失败 错误号 {1} 错误信息 {2}", mcconfig.GetString(CommonUtil.T2Sdk, "servers", ""), retu, _connection2.GetErrorMsg(retu)));
                    }
                    else
                    {
                        PublishLogMessage(LogMessageLevel.INFO, string.Format("信息:连接{0}成功", mcconfig.GetString(CommonUtil.T2Sdk, "servers", "")));
                    }

                    _subcallback = new SubCallBack(this);
                    _subcribe = _connection2.NewSubscriber(_subcallback, "ufx_demo", 50000, 2000, 100);
                    if (_subcribe == null)
                    {
                        PublishLogMessage(LogMessageLevel.ERROR, string.Format("错误:订阅创建失败 {0}", _connection2.GetMCLastError()));

                        MessageBox.Show("账户【" + param.FundAccount + "】订阅连接失败！" + "错误信息:" + _connection2.GetMCLastError(), "账户订阅连接错误");
                        return false;
                    }
                }

                var args = new CT2SubscribeParamInterface();
                args.SetTopicName(CommonUtil.TopicName);
                args.SetReplace(false);
                args.SetFilter("operator_no", param.FundAccount);
                var req = new CT2Packer(2);
                req.BeginPack();
                req.AddField("login_operator_no", _strType, 16, 4);
                req.AddField("password", _strType, 16, 4);
                req.AddStr(param.FundAccount);
                req.AddStr(param.FundAccountPassword);
                req.EndPack();
                CT2UnPacker back;
                var ret = _subcribe.SubscribeTopicEx(args, 50000, out back, req);
                req.Dispose();
                if (ret > 0)
                {
                    PublishLogMessage(LogMessageLevel.INFO, "订阅成功");
                    _subcribeid = ret;

                    return true;
                }
                else
                {
                    if (back != null)
                    {
                        PrintUnPack(back);
                    }
                    else
                    {
                        // 方法PrintUnPack中已包含错误提示，不再重复输出；当回报为空时，才弹出连接错误
                        MessageBox.Show("账户【" + param.FundAccount + "】订阅连接失败！" + "错误信息:" + _connection2.GetMCLastError(), "账户订阅连接错误");
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);

                MessageBox.Show("账户【" + param.FundAccount + "】订阅连接失败！" + "错误信息:" + ex.Message, "账户订阅连接错误");
                return false;
            }
        }

        /// <summary>
        /// 撤单
        /// </summary>
        /// <param name="entrustNo"></param>
        /// <param name="category"></param>
        public void CancelOrder(int entrustNo, eCategory category = eCategory.股票)
        {
            try
            {
                Logger.Info(string.Format("委托撤单：{0}，品种：{1}", entrustNo, category));

                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("entrust_no", _intType, 255, 4)
                };


                var listFieldValue = new List<object> {UserToken, entrustNo};


                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    {
                        RequestTemplate(功能号.委托撤单, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期货:
                    {
                        RequestTemplate(功能号.期货委托撤单, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期权:
                    {
                        RequestTemplate(功能号.期权委托撤单, listFieldInfo, listFieldValue);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public void LogOut()
        {
            try
            {
                Logger.Info(功能号.退出登录.ToString());
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 255, 4)};


                var listFieldValue = new List<object> {UserToken};


                RequestTemplate(功能号.退出登录, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        //功能号10000 心跳
        public void SendHeartBeat()
        {
            try
            {
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 255, 4)};


                var listFieldValue = new List<object> {UserToken};


                RequestTemplate(功能号.心跳, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 查询ETF成分股信息
        /// </summary>
        /// <param name="marketNo"></param>
        /// <param name="etfCode"></param>
        public void QryEtfStockList(string marketNo, string etfCode)
        {
            try
            {
                Logger.Info(功能号.ETF成份股信息查询.ToString());
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_user_token, _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_market_no, _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_etf_code, _strType, 255, 4)
                };


                var listFieldValue = new List<object> {UserToken, marketNo, etfCode};


                RequestTemplate(功能号.ETF成份股信息查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// ETF申赎委托明细查询
        /// </summary>
        public void QryEtfEntrustDetail(int entrustNo)
        {
            try
            {
                Logger.Info(功能号.ETF申赎委托明细查询.ToString());
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_user_token, _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_entrust_no, _intType, 255, 4)
                };

                var listFieldValue = new List<object> {UserToken, entrustNo};

                RequestTemplate(功能号.ETF申赎委托明细查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// ETF申赎成交明细查询
        /// </summary>
        public void QryEtfTradeResultDetail(int entrustNo)
        {
            try
            {
                Logger.Info(功能号.ETF申赎成交明细查询.ToString());
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_user_token, _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_entrust_no, _intType, 255, 4)
                };

                var listFieldValue = new List<object> { UserToken, entrustNo };

                RequestTemplate(功能号.ETF申赎成交明细查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// ETF基础信息查询
        /// </summary>
        public void QryEtfBaseInfo()
        {
            try
            {
                Logger.Info(功能号.ETF基础信息查询.ToString());
                var listFieldInfo = new List<FieldInfo> { new FieldInfo(CommonUtil.UFX_FIELD_NAME_user_token, _strType, 255, 4) };

                var listFieldValue = new List<object> { UserToken };

                RequestTemplate(功能号.ETF基础信息查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        public void QryInstrument()
        {
            try
            {
                Logger.Info(功能号.期货信息查询.ToString());
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 255, 4)};


                var listFieldValue = new List<object> {UserToken};


                RequestTemplate(功能号.期货信息查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void LoginAccount(string fundAccount, string fundAccountPassword)
        {
            try
            {
                Logger.Info(string.Format("登录:{0},{1}", fundAccount, fundAccountPassword));

                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("operator_no", _strType, 255, 4),
                    new FieldInfo("password", _strType, 255, 4),
                    new FieldInfo("mac_address", _strType, 255, 4),
                    new FieldInfo("ip_address", _strType, 255, 4),
                    new FieldInfo("hd_volserial", _strType, 255, 4),
                    new FieldInfo("op_station", _strType, 255, 4),
                    new FieldInfo("authorization_id", _strType, 255, 4),
                    new FieldInfo("login_time", _strType, 255, 4),
                    new FieldInfo("verification_code", _strType, 255, 4)
                };


                var listFieldValue = new List<object>
                {
                    fundAccount,
                    fundAccountPassword,
                    Computer.Instance().MacAddress,
                    Computer.Instance().IpAddress,
                    Computer.Instance().DiskId,
                    "10.162.195.103|c8-d7-19-52-b8-5b",
                    "1",
                    "2016-07-18",
                    "AAAAA"
                };


                RequestTemplate(功能号.登录, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        private void Send(CT2BizMessage bizMessage)
        {
            try
            {
                if (_connection != null)
                {
                    /************************************************************************/
                    /* 此处使用异步发送 同步发送可以参考下面注释代码
                     * connection.SendBizMsg(BizMessage, 0);
                    /************************************************************************/
                    var iRet = _connection.SendBizMsg(bizMessage, 1);
                    if (iRet < 0)
                    {
                        Logger.Error(string.Format("错误:发送错误 错误码 {0} 错误信息 {1}", iRet, _connection.GetErrorMsg(iRet)));
                    }
                    else
                    {
                        Logger.Debug("异步请求发送成功。\n");
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 委托下单
        /// </summary>
        /// <param name="param"></param>
        public void InsertOrder(Entrust param)
        {
            try
            {
                if (param.EntrustAmount == 0)
                {
                    return;
                }

                Logger.Info(param.ToString());
                PublishLogMessage(LogMessageLevel.INFO, param.ToString());

                var listFieldInfo = new List<FieldInfo>();

                listFieldInfo.Add(new FieldInfo("user_token", _strType, 512, 4));
                listFieldInfo.Add(new FieldInfo("combi_no", _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("market_no", _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("stock_code", _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("entrust_direction", _strType, 255, 4));

                if (param is FutureEntrust)
                {
                    listFieldInfo.Add(new FieldInfo("futures_direction", _strType, 255, 4));
                }

                listFieldInfo.Add(new FieldInfo("price_type", _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("entrust_price", _floatType, 255, 4));
                listFieldInfo.Add(new FieldInfo("entrust_amount", _floatType, 255, 4));
                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_extsystem_id, _intType, 255, 4));

                var listFieldValue = new List<object>();

                listFieldValue.Add(UserToken);
                listFieldValue.Add(param.CombiNo);
                listFieldValue.Add(param.MarketNo);
                listFieldValue.Add(param.StockCode);
                listFieldValue.Add(param.EntrustDirection);

                if (param is FutureEntrust)
                {
                    listFieldValue.Add(((FutureEntrust) param).FuturesDirection);
                }

                listFieldValue.Add(param.PriceType);
                listFieldValue.Add(param.EntrustPrice);
                listFieldValue.Add(param.EntrustAmount);
                listFieldValue.Add(param.ExtsystemId);

                if (param is OptionEntrust)
                {
                    listFieldInfo.Add(new FieldInfo("third_reff", _strType, 255, 4));
                    listFieldValue.Add("option");
                    RequestTemplate(功能号.期权委托, listFieldInfo, listFieldValue);
                }
                else if (param is FutureEntrust)
                {
                    RequestTemplate(功能号.期货委托, listFieldInfo, listFieldValue);
                }
                else
                {
                    RequestTemplate(功能号.普通买卖委托, listFieldInfo, listFieldValue);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 基金委托
        /// </summary>
        /// <param name="param"></param>
        public void InsertOrderFund(FundEntrust param)
        {
            try
            {
                if (param.EntrustAmount == 0)
                {
                    return;
                }

                Logger.Info(param.ToString());
                PublishLogMessage(LogMessageLevel.INFO, param.ToString());

                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4),
                    new FieldInfo("market_no", _strType, 255, 4),
                    new FieldInfo("stock_code", _strType, 255, 4),
                    new FieldInfo("entrust_direction", _strType, 255, 4),
                    new FieldInfo("entrust_amount", _floatType, 255, 4),
                    new FieldInfo("entrust_price", _floatType, 255, 4),
                    new FieldInfo("entrust_balance", _floatType, 255, 4),
                    new FieldInfo("purchase_way", _strType, 255, 4),
                    new FieldInfo("third_reff", _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_extsystem_id, _intType, 255, 4)
                };



                var listFieldValue = new List<object>
                {
                    UserToken,
                    param.CombiNo,
                    param.MarketNo,
                    param.StockCode,
                    param.EntrustDirection,
                    param.EntrustAmount,
                    param.EntrustPrice,
                    param.EntrustAmount,
                    param.PurchaseWay,
                    "fund",
                    param.ExtsystemId
                };


                RequestTemplate(功能号.基金委托, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        public void InsertOrderBasket(List<Entrust> listEntrust)
        {
            try
            {
                if (listEntrust.Count > 0)
                {
                    Logger.Info("新篮子委托");

                    var listFieldInfo = new List<FieldInfo>
                    {
                        new FieldInfo("user_token", _strType, 512, 4),
                        new FieldInfo("combi_no", _strType, 255, 4),
                        new FieldInfo("market_no", _strType, 255, 4),
                        new FieldInfo("stock_code", _strType, 255, 4),
                        new FieldInfo("entrust_direction", _strType, 255, 4),
                        new FieldInfo("futures_direction", _strType, 255, 4),
                        new FieldInfo("price_type", _strType, 255, 4),
                        new FieldInfo("entrust_price", _floatType, 255, 4),
                        new FieldInfo("entrust_amount", _floatType, 255, 4),
                        new FieldInfo(CommonUtil.UFX_FIELD_NAME_limit_entrust_ratio, _floatType, 255, 4),
                        new FieldInfo(CommonUtil.UFX_FIELD_NAME_ftr_limit_entrust_ratio, _floatType, 255, 4),
                        new FieldInfo(CommonUtil.UFX_FIELD_NAME_extsystem_id, _intType, 255, 5),
                        new FieldInfo(CommonUtil.UFX_FIELD_NAME_third_reff, _strType, 255, 5)
                    };


                    var listFieldValue = new List<object>();

                    foreach (var param in listEntrust)
                    {
                        listFieldValue.Add(UserToken);
                        listFieldValue.Add(param.CombiNo);
                        listFieldValue.Add(param.MarketNo);
                        listFieldValue.Add(param.StockCode);
                        listFieldValue.Add(param.EntrustDirection);

                        if (param is FutureEntrust)
                        {
                            listFieldValue.Add(((FutureEntrust) param).FuturesDirection);
                        }
                        else
                        {
                            listFieldValue.Add("");
                        }

                        listFieldValue.Add(param.PriceType);
                        listFieldValue.Add(param.EntrustPrice);
                        listFieldValue.Add(param.EntrustAmount);
                        listFieldValue.Add(param.LimitEntrustRatio);
                        listFieldValue.Add(param.FtrLimitEntrustRatio);
                        listFieldValue.Add(param.ExtsystemId);
                        listFieldValue.Add(param.ThirdReff);
                    }

                    RequestTemplate(功能号.篮子委托, listFieldInfo, listFieldValue);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 期权篮子委托
        /// </summary>
        /// <param name="listEntrust"></param>
        public void InsertOrderOptionBasket(List<Entrust> listEntrust)
        {
            try
            {
                if (listEntrust.Count > 0)
                {
                    var listFieldInfo = new List<FieldInfo>
                    {
                        new FieldInfo("user_token", _strType, 512, 4),
                        new FieldInfo("combi_no", _strType, 255, 4),
                        new FieldInfo("market_no", _strType, 255, 4),
                        new FieldInfo("stock_code", _strType, 255, 4),
                        new FieldInfo("entrust_direction", _strType, 255, 4),
                        new FieldInfo("futures_direction", _strType, 255, 4),
                        new FieldInfo("price_type", _strType, 255, 4),
                        new FieldInfo("entrust_price", _floatType, 255, 4),
                        new FieldInfo("entrust_amount", _floatType, 255, 4),
                        new FieldInfo(CommonUtil.UFX_FIELD_NAME_extsystem_id, _intType, 255, 5)
                    };


                    var listFieldValue = new List<object>();

                    foreach (var param in listEntrust)
                    {
                        listFieldValue.Add(UserToken);
                        listFieldValue.Add(param.CombiNo);
                        listFieldValue.Add(param.MarketNo);
                        listFieldValue.Add(param.StockCode);
                        listFieldValue.Add(param.EntrustDirection);
                        listFieldValue.Add(param.FuturesDirection);
                        listFieldValue.Add(param.PriceType);
                        listFieldValue.Add(param.EntrustPrice);
                        listFieldValue.Add(param.EntrustAmount);
                        listFieldValue.Add(param.ExtsystemId);
                    }

                    RequestTemplate(功能号.期权篮子委托, listFieldInfo, listFieldValue);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 账户查询
        /// </summary>
        /// <returns></returns>
        public void QryAccount()
        {
            try
            {
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 512, 4)};

                var listFieldValue = new List<object> {UserToken};

                RequestTemplate(功能号.账户查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 账户查询
        /// </summary>
        /// <returns></returns>
        public void QryAssetNo()
        {
            try
            {
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 512, 4)};

                var listFieldValue = new List<object> {UserToken};

                RequestTemplate(功能号.资产单元查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 账户资金查询
        /// </summary>
        /// <returns></returns>
        public void QryAccountMoney(string combiNo)
        {
            try
            {
                //首先把资金设为0，用来判断资金查询是否成功
                CommonUtil.AvailableMoney = 0;

                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4)
                };

                var listFieldValue = new List<object>();

                if (!string.IsNullOrEmpty(UserToken))
                {
                    listFieldValue.Add(UserToken);
                    listFieldValue.Add(combiNo);
                    RequestTemplate(功能号.账户资金查询, listFieldInfo, listFieldValue);
                }
                else
                {
                    MessageBox.Show("错误:登录超时，请检查服务器是否准备好！！！");
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 期货保证金账户查询
        /// </summary>
        /// <returns></returns>
        public void QryFutureMargin(string combiNo)
        {
            try
            {
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4)
                };

                var listFieldValue = new List<object>();

                if (!string.IsNullOrEmpty(UserToken))
                {
                    listFieldValue.Add(UserToken);
                    listFieldValue.Add(combiNo);
                    RequestTemplate(功能号.期货保证金账户查询, listFieldInfo, listFieldValue);
                }
                else
                {
                    MessageBox.Show("错误:登录超时，请检查服务器是否准备好！！！");
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 期权保证金账户查询
        /// </summary>
        /// <returns></returns>
        public void QryOptionMargin(string combiNo, string marketNo)
        {
            try
            {
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4),
                    new FieldInfo("market_no", _strType, 255, 4)
                };

                var listFieldValue = new List<object>();

                if (!string.IsNullOrEmpty(UserToken))
                {
                    listFieldValue.Add(UserToken);
                    listFieldValue.Add(combiNo);
                    listFieldValue.Add(marketNo);
                    RequestTemplate(功能号.期权保证金账户查询, listFieldInfo, listFieldValue);
                }
                else
                {
                    MessageBox.Show("错误:登录超时，请检查服务器是否准备好！！！");
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        public void RequestTemplate(功能号 eFuntionNo, List<FieldInfo> listFieldInfo, List<object> listFieldValue)
        {
            try
            {
                Logger.Debug(string.Format("********************{0}开始*********************", eFuntionNo));
                var bizMessage = new CT2BizMessage(); //构造消息
                bizMessage.SetFunction((int) eFuntionNo); //设置功能号
                bizMessage.SetPacketType(0); //设置消息类型为请求

                //打包请求报文
                var packer = new CT2Packer(2);

                packer.BeginPack();

                //插件编号

                for (var i = 0; i < listFieldInfo.Count; i++)
                {
                    packer.AddField(listFieldInfo[i].FieldName, listFieldInfo[i].FieldType, listFieldInfo[i].FieldWidth, listFieldInfo[i].FieldScale);
                }

                for (var j = 0; j < listFieldValue.Count; j++)
                {
                    if (listFieldInfo[j%listFieldInfo.Count].FieldType == _strType)
                    {
                        packer.AddStr(Convert.ToString(listFieldValue[j]));
                    }

                    if (listFieldInfo[j%listFieldInfo.Count].FieldType == _intType)
                    {
                        packer.AddInt(Convert.ToInt32(listFieldValue[j]));
                    }

                    if (listFieldInfo[j%listFieldInfo.Count].FieldType == _floatType)
                    {
                        packer.AddDouble(Convert.ToDouble(listFieldValue[j]));
                    }
                }


                packer.EndPack();
                unsafe
                {
                    bizMessage.SetContent(packer.GetPackBuf(), packer.GetPackLen());
                }

                Send(bizMessage);

                packer.Dispose();
                bizMessage.Dispose();

                Logger.Debug(string.Format("********************{0}结束*********************\n", eFuntionNo));
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("{0}出现错误，{1}", eFuntionNo, ex.Message + ex.Source + ex.StackTrace));
            }
        }

        /// <summary>
        /// 证券成交查询
        /// </summary>
        /// <param name="combiNo"></param>
        /// <param name="category"></param>
        public void QryTradeResult(string combiNo, eCategory category = eCategory.股票)
        {
            try
            {
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4),
                    new FieldInfo(CommonUtil.UFX_FIELD_NAME_request_num, _intType, 255, 4)
                };

                var listFieldValue = new List<object> {UserToken, combiNo, CommonUtil.RequestNum};

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    {
                        _isStockTradeResultReady = false;
                        RequestTemplate(功能号.证券成交查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期货:
                    {
                        _isFutureTradeResultReady = false;
                        RequestTemplate(功能号.期货成交查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期权:
                    {
                        _isOptionTradeResultReady = false;
                        RequestTemplate(功能号.期权成交查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        _isFundTradeResultReady = false;
                        RequestTemplate(功能号.基金成交查询, listFieldInfo, listFieldValue);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }

        /// <summary>
        /// 证券持仓查询
        /// </summary>
        public void QryHoldingPosition(string combiNo, eCategory category = eCategory.股票)
        {
            try
            {
                var listFieldInfo = new List<FieldInfo>
                {
                    new FieldInfo("user_token", _strType, 512, 4),
                    new FieldInfo("combi_no", _strType, 255, 4)
                };

                var listFieldValue = new List<object> {UserToken, combiNo};

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    {
                        _isStockPositionReady = false;
                        RequestTemplate(功能号.证券持仓查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期货:
                    {
                        _isFuturePositionReady = false;
                        RequestTemplate(功能号.期货持仓查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期权:
                    {
                        _isOptionPositionReady = false;
                        RequestTemplate(功能号.期权持仓查询, listFieldInfo, listFieldValue);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }


        /// <summary>
        /// 查询历史报单
        /// </summary>
        public int QryHistoricalEntrust(string combiNo, string accountCode, int startDate, int endDate, string stockCode, string entrustDirection, string entrustStateList, bool bFuture = false)
        {
            try
            {
                Logger.Debug("QryHistoricalEntrust");
                var listFieldInfo = new List<FieldInfo>();
                listFieldInfo.Add(new FieldInfo("user_token", _strType, 512, 4));
                listFieldInfo.Add(new FieldInfo("start_date", _intType, 255, 4));
                listFieldInfo.Add(new FieldInfo("end_date", _intType, 255, 4));
                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_account_code, _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("combi_no", _strType, 255, 4));

                if (!string.IsNullOrEmpty(stockCode))
                {
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_stock_code, _strType, 255, 4));
                }

                if (!string.IsNullOrEmpty(entrustDirection))
                {
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_entrust_direction, _strType, 255, 4));
                }

                if (!string.IsNullOrEmpty(entrustStateList))
                {
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_entrust_state_list, _strType, 255, 4));
                }

                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_request_num, _intType, 255, 4));

                var listFieldValue = new List<object>();
                listFieldValue.Add(UserToken);
                listFieldValue.Add(startDate);
                listFieldValue.Add(endDate);
                listFieldValue.Add(accountCode);
                listFieldValue.Add(combiNo);

                if (!string.IsNullOrEmpty(stockCode))
                {
                    listFieldValue.Add(stockCode);
                }

                if (!string.IsNullOrEmpty(entrustDirection))
                {
                    listFieldValue.Add(entrustDirection);
                }

                if (!string.IsNullOrEmpty(entrustStateList))
                {
                    listFieldValue.Add(entrustStateList);
                }

                listFieldValue.Add(CommonUtil.RequestNum);

                if (bFuture)
                {
                    RequestTemplate(功能号.期货历史委托查询, listFieldInfo, listFieldValue);
                }
                else
                {
                    RequestTemplate(功能号.证券历史委托查询, listFieldInfo, listFieldValue);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }

            return 0;
        }

        /// <summary>
        /// 查询历史成交记录
        /// </summary>
        public int QryHistoricalTradeResult(string combiNo, string accountCode, int startDate, int endDate, string stockCode, string entrustDirection, bool bFuture = false)
        {
            try
            {
                Logger.Debug("QryHistoricalTradeResult");
                var listFieldInfo = new List<FieldInfo>();
                listFieldInfo.Add(new FieldInfo("user_token", _strType, 512, 4));
                listFieldInfo.Add(new FieldInfo("start_date", _intType, 255, 4));
                listFieldInfo.Add(new FieldInfo("end_date", _intType, 255, 4));
                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_account_code, _strType, 255, 4));
                listFieldInfo.Add(new FieldInfo("combi_no", _strType, 255, 4));

                if (!string.IsNullOrEmpty(stockCode))
                {
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_stock_code, _strType, 255, 4));
                }

                if (!string.IsNullOrEmpty(entrustDirection))
                {
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_entrust_direction, _strType, 255, 4));
                }

                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_request_num, _intType, 255, 4));

                var listFieldValue = new List<object>();
                listFieldValue.Add(UserToken);
                listFieldValue.Add(startDate);
                listFieldValue.Add(endDate);
                listFieldValue.Add(accountCode);
                listFieldValue.Add(combiNo);

                if (!string.IsNullOrEmpty(stockCode))
                {
                    listFieldValue.Add(stockCode);
                }

                if (!string.IsNullOrEmpty(entrustDirection))
                {
                    listFieldValue.Add(entrustDirection);
                }

                listFieldValue.Add(CommonUtil.RequestNum);

                if (bFuture)
                {
                    RequestTemplate(功能号.期货历史成交查询, listFieldInfo, listFieldValue);
                }
                else
                {
                    RequestTemplate(功能号.证券历史成交查询, listFieldInfo, listFieldValue);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }

            return 0;
        }


        /// <summary>
        /// 查询报单
        /// </summary>
        public int QryEntrust(string combiNo, eCategory category = eCategory.股票, string positionStr = null)
        {
            try
            {
                Logger.Debug("QryEntrust");
                var listFieldInfo = new List<FieldInfo>();
                listFieldInfo.Add(new FieldInfo("user_token", _strType, 512, 4));
                listFieldInfo.Add(new FieldInfo("combi_no", _strType, 255, 4));               
                listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_request_num, _intType, 255, 4));
                if (!string.IsNullOrEmpty(positionStr))
                {
                    Logger.Debug(string.Format("从{0}继续查询委托.", positionStr));
                    listFieldInfo.Add(new FieldInfo(CommonUtil.UFX_FIELD_NAME_position_str, _strType, 255, 4));
                }


                var listFieldValue = new List<object>();
                listFieldValue.Add(UserToken);
                listFieldValue.Add(combiNo);
                listFieldValue.Add(CommonUtil.RequestNum);
                if (!string.IsNullOrEmpty(positionStr))
                {
                    listFieldValue.Add(positionStr);
                }

                switch (category)
                {
                    case eCategory.股票:
                    {
                        _isStockEntrustReady = false;
                        RequestTemplate(功能号.证券委托查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期货:
                    {
                        _isFutureEntrustReady = false;
                        RequestTemplate(功能号.期货委托查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.期权:
                    {
                        _isOptionEntrustReady = false;
                        RequestTemplate(功能号.期权委托查询, listFieldInfo, listFieldValue);
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        _isFundEntrustReady = false;
                        RequestTemplate(功能号.基金委托查询, listFieldInfo, listFieldValue);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }

            return 0;
        }

        /// <summary>
        /// 组合查询
        /// </summary>
        public void QryCombiNo()
        {
            try
            {
                Logger.Info(功能号.组合查询.ToString());
                var listFieldInfo = new List<FieldInfo> {new FieldInfo("user_token", _strType, 512, 4)};

                var listFieldValue = new List<object> {UserToken};

                RequestTemplate(功能号.组合查询, listFieldInfo, listFieldValue);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Logger, ex);
            }
        }
    }


    /********************************************* MC订阅回调****************************************/

    [Export]
    public unsafe class SubCallBack : CT2SubCallbackInterface
    {
        private readonly HsStock _owner;

        public SubCallBack(HsStock app)
        {
            _owner = app;
        }

        public override void OnReceived(CT2SubscribeInterface lpSub, int subscribeIndex, void* lpData, int nLength, tagSubscribeRecvData lpRecvData)
        {
            try
            {
                _owner.Logger.Debug("*********************************收到主推数据 begin***************************");
                var strInfo = string.Format("附加数据长度：{0}", lpRecvData.iAppDataLen);
                _owner.Logger.Debug(strInfo);

                if (lpRecvData.iAppDataLen > 0)
                {
                    strInfo = string.Format("附加数据：{0}", Marshal.PtrToStringAuto(new IntPtr(lpRecvData.lpAppData)));
                    _owner.Logger.Debug(strInfo);
                }

                if (lpData != null)
                {
                    var lpUnPack = new CT2UnPacker(lpData, (uint) nLength);

                    if (true)
                    {
                        lpUnPack.SetCurrentDatasetByIndex(0);
                        lpUnPack.First();

                        var sMsgType = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_msgtype);
                        var sOperatorNo = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_operator_no);

                        var sBusiDate = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_business_date);
                        var sBusiTime = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_business_time);
                        var sBatchNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_batch_no);
                        var sEntrustNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_no);
                        var sReportNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_report_no);
                        var sAccountCode = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_account_code);
                        var sCombiNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_combi_no);
                        var sInstanceNo = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_instance_no);
                        var sStockholderId = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_stockholder_id);
                        var sReportSeat = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_report_seat);
                        var sMarketNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_market_no);
                        var sStockCode = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_stock_code);
                        var sEntrustDirection = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_entrust_direction);
                        var sFuturesDirection = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_futures_direction);
                        var sPriceType = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_price_type);
                        var sEntrustPrice = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_entrust_price);
                        var sEntrustAmount = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_entrust_amount);
                        var sInvestType = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_invest_type);
                        var sEntrustState = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_entrust_state);
                        var sEntrustStatus = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_entrust_status);
                        var sExtsystemId = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_extsystem_id);
                        var sThirdReff = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_third_reff);
                        var sConfirmNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_confirm_no);
                        var sCancelEntrustNo = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_cancel_entrust_no);
                        var sRevokeCause = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_revoke_cause);
                        var sCancelAmount = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_cancel_amount);
                        var sDealDate = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_date);
                        var sDealTime = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_time);
                        var sDealNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_no);
                        var sDealAmount = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_deal_amount);
                        var sDealPrice = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_price);
                        var sDealBalance = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_deal_balance);
                        var sDealFee = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_fee);
                        var sTotalDealAmount = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_total_deal_amount);
                        var sTotalDealBalance = CommonUtil.GetStrFromUnPacker(lpUnPack,
                            CommonUtil.UFX_FIELD_NAME_total_deal_balance);

                        eCategory category;
                        string securityName = null;

                        if (!string.IsNullOrEmpty(sStockCode))
                        {
                            var si =
                                _owner.Marketdataservice.GetSecurityInfo(CommonUtil.MarketNoToExId(sMarketNo),
                                    sStockCode);

                            if (si != null)
                            {
                                if (!string.IsNullOrEmpty(sEntrustDirection))
                                {
                                    var entrustDirection =
                                        CommonUtil.EntrustDirectionToeEntrustDirection(sEntrustDirection);
                                    if (entrustDirection == eEntrustDirection.ETF申购 ||
                                        entrustDirection == eEntrustDirection.ETF赎回 ||
                                        entrustDirection == eEntrustDirection.基金分拆 ||
                                        entrustDirection == eEntrustDirection.基金合并 ||
                                        entrustDirection == eEntrustDirection.基金认购)
                                    {
                                        category = eCategory.基金分拆合并;
                                    }
                                    else
                                    {
                                        category = si.SecurityType;
                                    }
                                }
                                else
                                {
                                    category = si.SecurityType;
                                }

                                securityName = si.SecurityName;
                            }
                            else
                            {
                                _owner.Logger.Error(string.Format("{0}的证券信息不存在！", sStockCode));
                                return;
                            }
                        }
                        else
                        {
                            category = CommonUtil.GetCategory(sMarketNo, sEntrustNo, _owner.EntrustInfoCollection);
                        }

                        EntrustInfo ei = null;

                        switch (category)
                        {
                            case eCategory.股票:
                            case eCategory.债券回购:
                            case eCategory.基金:
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
                            case eCategory.基金分拆合并:
                            {
                                ei = new FundEntrustInfo();
                                break;
                            }
                        }

                        ei.Category = category;
                        ei.SecurityName = securityName;
                        //string sMsgType = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_msgtype);
                        ei.MsgType = CommonUtil.MessageTypeToeMessageType(sMsgType);
                        //string sOperatorNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_operator_no);
                        ei.OperatorNo = sOperatorNo;
                        //string sBusiDate = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_business_date);
                        ei.EntrustDate = sBusiDate;
                        //string sBusiTime = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_business_time);
                        ei.EntrustTime = CommonUtil.TimeIntToTime(sBusiTime);
                        //string sBatchNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_batch_no);
                        ei.EntrustBatchNo = sBatchNo;
                        //string sEntrustNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_no);
                        ei.EntrustNo = TryConvertToInt(sEntrustNo);
                        //string sReportNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_report_no);
                        ei.ReportNo = sReportNo;
                        //string sAccountCode = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_account_code);
                        ei.AccountCode = sAccountCode;
                        //string sCombiNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_combi_no);
                        ei.CombiNo = sCombiNo;
                        //string sInstanceNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_instance_no);
                        ei.InstanceNo = sInstanceNo;
                        //string sStockholderId = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                        ei.StockholderId = sStockholderId;
                        //string sReportSeat = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_report_seat);
                        ei.ReportSeat = sReportSeat;
                        //string sMarketNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_market_no);
                        ei.MarketType = CommonUtil.MarketNoToeMarketType(sMarketNo);
                        //string sStockCode = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_stock_code);
                        ei.SecurityID = sStockCode;
                        //string sEntrustDirection = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                        ei.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(sEntrustDirection);
                        //string sFuturesDirection = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_futures_direction);
                        ei.FuturesDirection = CommonUtil.FuturesDirectionToeFuturesDirection(sFuturesDirection);
                        //string sPriceType = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_price_type);
                        ei.EntrustPriceType = CommonUtil.EntrustPriceTypeToeEntrustPriceType(sPriceType);
                        //string sEntrustPrice = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_price);
                        ei.EntrustPrice = TryConvertToDouble(sEntrustPrice);
                        //string sEntrustAmount = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_amount);
                        ei.EntrustAmount = (int) TryConvertToDouble(sEntrustAmount);
                        //string sInvestType = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_invest_type);
                        ei.InvestType = CommonUtil.InvestTypeToeInvestType(sInvestType);
                        //string sEntrustState = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_state);
                        ei.EntrustState = CommonUtil.EntrustSateToeEntrustSate(sEntrustState);
                        //string sEntrustStatus = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_entrust_status);
                        //string sExtsystemId = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                        ei.ExtsystemId = sExtsystemId;
                        //string sThirdReff = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_third_reff);
                        ei.ThirdReff = sThirdReff;
                        //string sConfirmNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_confirm_no);
                        ei.ConfirmNo = sConfirmNo;
                        //string sCancelEntrustNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_cancel_entrust_no);
                        ei.CancelEntrustNo = TryConvertToInt(sCancelEntrustNo);
                        //string sRevokeCause = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_revoke_cause);
                        ei.RevokeCause = sRevokeCause;
                        //string sCancelAmount = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_cancel_amount);
                        ei.CancelAmount = (int) TryConvertToDouble(sCancelAmount);
                        //string sDealDate = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_date);
                        ei.DealDate = sDealDate;
                        //string sDealTime = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_time);
                        ei.DealTime = CommonUtil.TimeIntToTime(sDealTime);
                        //string sDealNo = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_no);
                        ei.DealNo = sDealNo;
                        //string sDealAmount = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_amount);
                        ei.DealAmount = (int) TryConvertToDouble(sDealAmount);
                        //string sDealPrice = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_price);
                        ei.DealPrice = TryConvertToDouble(sDealPrice);
                        //string sDealBalance = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_balance);
                        ei.DealBalance = TryConvertToDouble(sDealBalance);
                        //string sDealFee = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_deal_fee);
                        ei.DealFee = TryConvertToDouble(sDealFee);
                        //string sTotalDealAmount = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_total_deal_amount);
                        ei.TotalDealAmount = TryConvertToInt(sTotalDealAmount);
                        //string sTotalDealBalance = CommonUtil.GetStrFromUnPacker(lpUnPack, CommonUtil.UFX_FIELD_NAME_total_deal_balance);
                        ei.TotalDealBalance = TryConvertToDouble(sTotalDealBalance);

                        //查询操作已经完毕，并且委托已经存在于委托列表之中时，直接发送；否则保存在队列里
                        if (_owner.IsEntrustInList(ei.EntrustNo, ei.Category) || ei.MsgType == eMessageType.委托下达 ||
                            _owner.IsEntrustInList(ei.CancelEntrustNo, ei.Category))
                        {
                            _owner.Logger.Debug(string.Format("委托已经存在列表中或者是委托下达消息，实时发送委托消息:{0}", ei));
                            _owner.ProcessEntrustState(ei);
                        }
                        else
                        {
                            _owner.Logger.Debug(string.Format("委托还不在列表中，暂存委托消息:{0}", ei));
                            _owner.EntrustInfoQueue.Add(ei);
                        }

                        if (ei.MsgType == eMessageType.委托成交)
                        {
                            _owner.EventAggregator.GetEvent<EntrustDealCallBackEvent>().Publish(ei);
                        }

                        _owner.PrintUnPack(lpUnPack);

                        lpUnPack.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        private double TryConvertToDouble(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Convert.ToDouble(value);
            }

            return 0;
        }

        private int TryConvertToInt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Convert.ToInt32(value);
            }

            return 0;
        }

        public override void OnRecvTickMsg(CT2SubscribeInterface lpSub, int subscribeIndex, string tickMsgInfo)
        {
        }
    }

    /********************************************* MC订阅回调****************************************/

    [Export]
    public sealed unsafe class CallbackStock : CT2CallbackInterface
    {
        public CallbackStock(HsStock parent)
        {
            _owner = parent;
        }

        private HsStock _owner;

        public override void OnConnect(CT2Connection lpConnection)
        {
            _owner.Logger.Debug("OnConnect");
        }

        public override void OnSafeConnect(CT2Connection lpConnection)
        {
            _owner.EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("已连接");
            _owner.Logger.Debug("OnSafeConnect");
        }

        public override void OnRegister(CT2Connection lpConnection)
        {
            _owner.Logger.Debug("OnRegister");
        }

        public override void OnClose(CT2Connection lpConnection)
        {
            _owner.EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("连接已断开");
            _owner.Logger.Debug("OnClose");
        }

        public override void OnReceivedBiz(CT2Connection lpConnection, int hSend, string lppStr, CT2UnPacker lppUnPacker, int nResult)
        {
            _owner.Logger.Debug("OnReceivedBiz");
        }

        public override void OnReceivedBizEx(CT2Connection lpConnection, int hSend, CT2RespondData lpRetData, string lppStr, CT2UnPacker lppUnPacker, int nResult)
        {
            _owner.Logger.Debug("OnReceivedBizEx");
        }

        public override void OnSent(CT2Connection lpConnection, int hSend, void* lpData, int nLength, int nQueuingData)
        {
            _owner.Logger.Debug("OnSent");
        }


        private void OnRspLogin(CT2UnPacker unpacker)
        {
            try
            {
                _owner.Logger.Debug("OnRspLogin");
                while (unpacker.IsEOF() != 1)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        //功能号332255，客户资金快速查询的返回结果
        private void OnRspQryAccountMoneyInfo(CT2UnPacker responseUnPacker)
        {
            try
            {
                _owner.Logger.Debug("OnRspQryAccountMoneyInfo");

                if (responseUnPacker.GetDatasetCount() > 1)
                {
                    responseUnPacker.SetCurrentDatasetByIndex(1);
                    responseUnPacker.First();

                    var rowCount = responseUnPacker.GetRowCount();

                    for (var i = 0; i < rowCount; i++)
                    {
                        var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                        var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                        var enableBalanceT0 = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_balance_t0);
                        var enableBalanceT1 = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_balance_t1);

                        var mi = new MoneyInfo();
                        mi.MsgType = eMoneyInfoMsgType.查询;
                        mi.MoneyType = eMoneyType.人民币;
                        mi.EnabledMoney = Convert.ToDouble(enableBalanceT0);
                        mi.TotalMoney = Convert.ToDouble(enableBalanceT1);
                        mi.AssetNo = assetNo;

                        _owner.EventAggregator.GetEvent<MoneyInfoNotifyEvent>().Publish(mi);

                        responseUnPacker.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        private void OnRspQryMarginInfo(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                _owner.Logger.Debug("OnRspQryMarginInfo");

                if (responseUnPacker.GetDatasetCount() > 1)
                {
                    responseUnPacker.SetCurrentDatasetByIndex(1);
                    responseUnPacker.First();

                    var rowCount = responseUnPacker.GetRowCount();

                    for (var i = 0; i < rowCount; i++)
                    {
                        var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                        var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                        var occupyDepositBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_occupy_deposit_balance);
                        var enableDepositBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_deposit_balance);

                        MoneyInfoBase fmi = null;

                        switch (category)
                        {
                            case eCategory.期货:
                            {
                                fmi = new FutureMarginInfo();
                                break;
                            }
                            case eCategory.期权:
                            {
                                fmi = new OptionMarginInfo();
                                break;
                            }
                        }


                        fmi.MsgType = eMoneyInfoMsgType.查询;
                        fmi.MoneyType = eMoneyType.人民币;
                        fmi.OccupyDepositBalance = Convert.ToDouble(occupyDepositBalance);
                        fmi.EnableDepositBalance = Convert.ToDouble(enableDepositBalance);
                        fmi.AssetNo = assetNo;

                        switch (category)
                        {
                            case eCategory.期货:
                            {
                                _owner.EventAggregator.GetEvent<FutureMarginInfoNotifyEvent>().Publish(fmi as FutureMarginInfo);
                                break;
                            }
                            case eCategory.期权:
                            {
                                _owner.EventAggregator.GetEvent<OptionMarginInfoNotifyEvent>().Publish(fmi as OptionMarginInfo);
                                break;
                            }
                        }

                        responseUnPacker.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        private void OnRspQryCombiNo(CT2UnPacker responseUnPacker)
        {
            try
            {
                _owner.Logger.Debug("OnRspQryCombiNo");

                if (responseUnPacker.GetDatasetCount() > 1)
                {
                    responseUnPacker.SetCurrentDatasetByIndex(1);
                    responseUnPacker.First();

                    var rowCount = responseUnPacker.GetRowCount();

                    for (var i = 0; i < rowCount; i++)
                    {
                        var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                        var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);

                        if (!string.IsNullOrEmpty(combiNo))
                        {
                            _owner.EventAggregator.GetEvent<CombiNoInfoArrivedNotifyEvent>().Publish(new InfoWithCombiNoAndSecurityInfo {CombiNo = combiNo});
                            _owner.CombiNo2AccountCode[combiNo] = accountCode;
                        }

                        responseUnPacker.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        private void OnRspQryAssetNo(CT2UnPacker unpacker)
        {
            try
            {
                _owner.Logger.Debug("OnRspQryAssetNo");

                if (unpacker != null)
                {
                    object assetNo;
                    CommonUtil.GetValueFromPack(unpacker, CommonUtil.UFX_FIELD_NAME_asset_no, typeof (string), out assetNo);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        public void ProcessOrderUnPacker(CT2UnPacker responseUnPacker, Func<CT2UnPacker, eCategory, List<string>> action,
            eCategory category)
        {
            try
            {


                if (responseUnPacker.GetDatasetCount() > 1)
                {
                    responseUnPacker.SetCurrentDatasetByIndex(1);
                    responseUnPacker.First();

                    var rowCount = responseUnPacker.GetRowCount();

                    List<string> result = null;
                    _owner.Logger.Debug(string.Format("委托查询解包开始：{0}", DateTime.Now));

                    for (var i = 0; i < rowCount; i++)
                    {
                        result = ProcessOneOrder(responseUnPacker, category);
                        responseUnPacker.Next();
                    }

                    _owner.Logger.Debug(string.Format("委托查询解包结束：{0}", DateTime.Now));

                    if (rowCount >= CommonUtil.RequestNum && result != null) //后面还有
                    {
                        _owner.QryEntrust(result[0], category, result[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        public void ProcessUnPackerTemplate(CT2UnPacker responseUnPacker, Action<CT2UnPacker, eCategory> action, eCategory category)
        {
            try
            {
                _owner.Logger.Debug("ProcessUnPackerTemplate");

                if (responseUnPacker.GetDatasetCount() > 1)
                {
                    responseUnPacker.SetCurrentDatasetByIndex(1);
                    responseUnPacker.First();

                    var rowCount = responseUnPacker.GetRowCount();

                    for (var i = 0; i < rowCount; i++)
                    {
                        action(responseUnPacker, category);
                        responseUnPacker.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneHoldingPosition(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            try
            {
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var holdSeat = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_hold_seat);
                var investType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_invest_type);
                var currentAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_amount);
                var enableAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_amount);
                var beginCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_begin_cost);
                var currentCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_cost);
                var preBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_amount);
                var preSellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_amount);
                var preBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_balance);
                var preSellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_balance);
                var todayBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_amount);
                var todaySellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_amount);
                var todayBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_balance);
                var todaySellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_balance);
                var todayBuyFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_fee);
                var todaySellFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_fee);

                var pi = new PositionInfo();
                pi.MsgType = ePositionInfoMsgType.查询;
                pi.SecurityID = stockCode;
                pi.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                pi.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                pi.CurrentAmount = (int) Convert.ToDouble(currentAmount);
                pi.EnableAmount = (int) Convert.ToDouble(enableAmount);
                pi.HoldSeat = holdSeat;
                pi.InvestType = CommonUtil.InvestTypeToeInvestType(investType);
                pi.StockholderId = stockholderId;
                pi.CombiNo = combiNo;
                pi.BeginCost = Convert.ToDouble(beginCost);
                pi.CurrentCost = Convert.ToDouble(currentCost);
                pi.CurrentCostPrice = pi.CurrentAmount == 0 ? 0 : pi.CurrentCost/pi.CurrentAmount;
                pi.PreBuyAmount = (int) Convert.ToDouble(preBuyAmount);
                pi.PreSellAmount = (int) Convert.ToDouble(preSellAmount);
                pi.PreBuyBalance = Convert.ToDouble(preBuyBalance);
                pi.PreSellBalance = Convert.ToDouble(preSellBalance);
                pi.TodayBuyAmount = Convert.ToDouble(todayBuyAmount);
                pi.TodaySellAmount = (int) Convert.ToDouble(todaySellAmount);
                pi.TodayBuyBalance = Convert.ToDouble(todayBuyBalance);
                pi.TodaySellBalance = Convert.ToDouble(todaySellBalance);
                pi.TodayBuyFee = Convert.ToDouble(todayBuyFee);
                pi.TodaySellFee = Convert.ToDouble(todaySellFee);

                //if (pi.CurrentAmount > 0)
                {
                    _owner.EventAggregator.GetEvent<PositionInfoNotifyEvent>().Publish(pi);
                }

                _owner.SetTraderForSecurityQuote(marketNo, pi.SecurityID);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        /// <summary>
        /// 证券持仓查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        public void OnRspQryHoldingPosition(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneHoldingPosition, eCategory.股票);
            _owner.IsStockPositionReady = true;
        }

        public void ProcessOneFutureHoldingPosition(CT2UnPacker responseUnPacker, eCategory bFuture)
        {
            try
            {
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var holdSeat = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_hold_seat);
                var positionFlag = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_flag);
                var investType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_invest_type);
                var currentAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_amount);
                var todayAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_amount);
                var lastdayAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_lastday_amount);
                var enableAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_amount);
                var todayEnableAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_enable_amount);
                var lastdayEnableAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_lastday_enable_amount);
                var beginCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_begin_cost);
                var currentCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_cost);
                var currentCostPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_cost_price);
                var preBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_amount);
                var preSellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_amount);
                var preBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_balance);
                var preSellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_balance);
                var todayBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_amount);
                var todaySellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_amount);
                var todayBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_balance);
                var todaySellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_balance);
                var todayBuyFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_fee);
                var todaySellFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_fee);

                var fpi = new FuturePositionInfo
                {
                    MsgType = ePositionInfoMsgType.查询,
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    SecurityID = stockCode,
                    SecurityName =
                        _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode),
                    StockholderId = stockholderId,
                    HoldSeat = holdSeat,
                    PositionFlag = CommonUtil.PositionFlagToePositionFlag(positionFlag),
                    InvestType = CommonUtil.InvestTypeToeInvestType(investType),
                    CurrentAmount = (int) Convert.ToDouble(currentAmount),
                    TodayAmount = (int) Convert.ToDouble(todayAmount),
                    LastdayAmount = (int) Convert.ToDouble(lastdayAmount),
                    EnableAmount = (int) Convert.ToDouble(enableAmount),
                    TodayEnableAmount = (int) Convert.ToDouble(todayEnableAmount),
                    LastdayEnableAmount = (int) Convert.ToDouble(lastdayEnableAmount),
                    CombiNo = combiNo,
                    BeginCost = Convert.ToDouble(beginCost),
                    CurrentCost = Convert.ToDouble(currentCost),
                    CurrentCostPrice = Convert.ToDouble(currentCostPrice),
                    PreBuyAmount = (int) Convert.ToDouble(preBuyAmount),
                    PreSellAmount = (int) Convert.ToDouble(preSellAmount),
                    PreBuyBalance = Convert.ToDouble(preBuyBalance),
                    PreSellBalance = Convert.ToDouble(preSellBalance),
                    TodayBuyAmount = Convert.ToDouble(todayBuyAmount),
                    TodaySellAmount = (int) Convert.ToDouble(todaySellAmount),
                    TodayBuyBalance = Convert.ToDouble(todayBuyBalance),
                    TodaySellBalance = Convert.ToDouble(todaySellBalance),
                    TodayBuyFee = Convert.ToDouble(todayBuyFee),
                    TodaySellFee = Convert.ToDouble(todaySellFee)
                };

                //if (fpi.CurrentAmount > 0)
                {
                    _owner.EventAggregator.GetEvent<FuturePositionInfoNotifyEvent>().Publish(fpi);
                }

                _owner.SetTraderForSecurityQuote(marketNo, fpi.SecurityID);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneOptionHoldingPosition(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var optionType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_option_type);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var holdSeat = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_hold_seat);
                var positionFlag = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_flag);
                var currentAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_amount);
                var enableAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_enable_amount);
                var beginCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_begin_cost);
                var currentCost = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_current_cost);
                var preBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_amount);
                var preSellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_amount);
                var preBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_balance);
                var preSellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_balance);
                var todayBuyAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_amount);
                var todaySellAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_amount);
                var todayBuyBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_balance);
                var todaySellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_balance);
                var todayBuyFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_buy_fee);
                var todaySellFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_today_sell_fee);

                var fpi = new OptionPositionInfo();
                fpi.MsgType = ePositionInfoMsgType.查询;
                fpi.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                fpi.SecurityID = stockCode;
                fpi.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                fpi.StockholderId = stockholderId;
                fpi.HoldSeat = holdSeat;
                fpi.PositionFlag = CommonUtil.PositionFlagToePositionFlag(positionFlag);
                fpi.CurrentAmount = (int) Convert.ToDouble(currentAmount);
                fpi.EnableAmount = (int) Convert.ToDouble(enableAmount);
                fpi.CombiNo = combiNo;
                fpi.OptionType = CommonUtil.OptionTypeToeOptionType(optionType);
                fpi.BeginCost = Convert.ToDouble(beginCost);
                fpi.CurrentCost = Convert.ToDouble(currentCost);
                fpi.CurrentCostPrice = fpi.CurrentAmount == 0 ? 0 : Math.Abs(fpi.CurrentCost) / (fpi.CurrentAmount * CommonUtil.OptionAmountPerHand);
                fpi.PreBuyAmount = (int) Convert.ToDouble(preBuyAmount);
                fpi.PreSellAmount = (int) Convert.ToDouble(preSellAmount);
                fpi.PreBuyBalance = Convert.ToDouble(preBuyBalance);
                fpi.PreSellBalance = Convert.ToDouble(preSellBalance);
                fpi.TodayBuyAmount = Convert.ToDouble(todayBuyAmount);
                fpi.TodaySellAmount = (int) Convert.ToDouble(todaySellAmount);
                fpi.TodayBuyBalance = Convert.ToDouble(todayBuyBalance);
                fpi.TodaySellBalance = Convert.ToDouble(todaySellBalance);
                fpi.TodayBuyFee = Convert.ToDouble(todayBuyFee);
                fpi.TodaySellFee = Convert.ToDouble(todaySellFee);

                //if (fpi.CurrentAmount > 0)
                {
                    _owner.EventAggregator.GetEvent<OptionPositionInfoNotifyEvent>().Publish(fpi);
                }

                _owner.SetTraderForSecurityQuote(marketNo, fpi.SecurityID);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        /// <summary>
        /// 期货持仓查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        public void OnRspQryFutureHoldingPosition(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneFutureHoldingPosition, eCategory.期货);
            _owner.IsFuturePositionReady = true;
        }


        /// <summary>
        /// 期权持仓查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        public void OnRspQryOptionHoldingPosition(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneOptionHoldingPosition, eCategory.期货);
            _owner.IsOptionPositionReady = true;
        }

        public void ProcessOneEtfEntrustDetail(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var entrustDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_date);
                var entrustTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_time);
                var operatorNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_operator_no);
                var batchNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_batch_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var detailEntrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_detail_entrust_no);
                var reportNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_no);
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var reportSeat = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_seat);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var entrustPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_entrust_price);
                var entrustAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_entrust_amount);
                var entrustState = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_entrust_state);

                var ii = new EntrustInfo
                {
                    EntrustDate = entrustDate,
                    EntrustTime = entrustTime,
                    OperatorNo = operatorNo,
                    BatchNo = Convert.ToInt32(batchNo),
                    EntrustNo = Convert.ToInt32(entrustNo),
                    DetailEntrustNo = Convert.ToInt32(detailEntrustNo),
                    ReportNo = reportNo,
                    AccountCode = accountCode,
                    AssetNo = assetNo,
                    CombiNo = combiNo,
                    StockholderId = stockholderId,
                    ReportSeat = reportSeat,
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    SecurityID = stockCode,
                    SecurityName =
                        _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode),
                    EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection),
                    EntrustPrice = Convert.ToDouble(entrustPrice),
                    EntrustAmount = Convert.ToInt32(entrustAmount),
                    EntrustState = CommonUtil.EntrustSateToeEntrustSate(entrustState)
                };

                DispatcherService.Invoke(() => { _owner.EtfEntrustDetail.Add(ii); });

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneEtfStock(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var businessDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_business_date);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var stockAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_amount);
                var replaceFlag = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_replace_flag);
                var replaceRatio = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_replace_ratio);
                var replaceBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_replace_balance);
                var redeemReplaceBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_redeem_replace_balance);

                var ii = new EtfStock
                {
                    BusinessDate = businessDate,
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    SecurityID = stockCode,
                    StockAmount = Convert.ToInt32(stockAmount),
                    ReplaceFlag = CommonUtil.ReplaceFlagToeReplaceFlag(replaceFlag),
                    ReplaceRatio = Convert.ToDouble(replaceRatio),
                    ReplaceBalance = Convert.ToDouble(replaceBalance),
                    RedeemReplaceBalance = Convert.ToDouble(redeemReplaceBalance)
                };

                DispatcherService.Invoke(() => { _owner.EtfStockCollection.EtfStockList.Add(ii); });

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneEtfBaseInfo(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var businessDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_business_date);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var etfCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_etf_code);
                var stockName = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_name);              
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var stockNum = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_num);
                var creationRedeemType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_creation_redeem_type);
                var etfMarketType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_etf_market_type);
                var rivalMarket = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_rival_market);
                var etfType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_etf_type);
                var maxCashRatio = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_max_cash_ratio);
                var reportUnit = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_unit);
                var yesterdayCash = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_yesterday_cash);
                var yesterdayNav = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_yesterday_nav);
                var estimateCash = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_estimate_cash);
                var underlyingIndex = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_underlying_index);

                var ii = new EtfBaseInfo
                {
                    BusinessDate = businessDate,   
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    EtfCode = etfCode,
                    SecurityName = stockName,
                    SecurityID = stockCode,
                    StockNum = Convert.ToInt32(stockNum),
                    CreationRedeemType = CommonUtil.CreationRedeemTypeToeCreationRedeemType(creationRedeemType),
                    EtfMarketType = CommonUtil.EtfMarketTypeToeEtfMarketType(etfMarketType),
                    RivalMarket = rivalMarket,
                    EtfType = CommonUtil.EtfTypeToeEtfType(etfType),
                    MaxCashRatio = Convert.ToDouble(maxCashRatio),
                    ReportUnit = Convert.ToDouble(reportUnit),
                    YesterdayCash = Convert.ToDouble(yesterdayCash),
                    YesterdayNav = Convert.ToDouble(yesterdayNav),
                    EstimateCash = Convert.ToDouble(estimateCash),
                    UnderlyingIndex = underlyingIndex
                };

                DispatcherService.Invoke(() => { _owner.EtfBaseInfoCollection.EtfBaseInfoList.Add(ii); });

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneInstrumentInfo(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var stockName = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_name);
                var futureKindName = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_future_kind_name);
                var settlementMonth = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_settlement_month);
                var targetMarketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_target_market_no);
                var targetStockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_target_stock_code);
                var multiple = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_multiple);
                var lastTradeDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_last_trade_date);
                var lastTradeTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_last_trade_time);
                var settlementDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_settlement_date);
                var settlementPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_settlement_price);
                var preSettlementPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_settlement_price);
                var marketPosition = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_position);
                var preMarketPosition = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_market_position);
                var marketPricePermit = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_price_permit);
                var uplimitedPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_uplimited_price);
                var downlimitedPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_downlimited_price);

                var ii = new FutureInstrumentInfo
                {
                    MsgType = ePositionInfoMsgType.查询,
                    SecurityID = stockCode,
                    SecurityName = stockName,
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    FutureKindName = futureKindName,
                    TargetMarketNo = CommonUtil.MarketNoToeMarketType(targetMarketNo),
                    TargetStockCode = targetStockCode,
                    SettlementMonth = settlementMonth,
                    Multiple = Convert.ToInt32(multiple),
                    LastTradeDate = lastTradeDate,
                    LastTradeTime = lastTradeTime,
                    SettlementDate = settlementDate,
                    SettlementPrice = Convert.ToDouble(settlementPrice),
                    PreSettlementPrice = Convert.ToDouble(preSettlementPrice),
                    MarketPosition = Convert.ToDouble(marketPosition),
                    PreMarketPosition = Convert.ToDouble(preMarketPosition),
                    MarketPricePermit = marketPricePermit,
                    UplimitedPrice = Convert.ToDouble(uplimitedPrice),
                    DownlimitedPrice = Convert.ToDouble(downlimitedPrice)
                };

                DispatcherService.Invoke(() => { _owner.InstrumentInfoCollection.InstrumentInfoList.Add(ii); });

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        public void OnRspQryInstrumentInfo(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneInstrumentInfo, eCategory.期货);
        }

        public void OnRspQryEtfBaseInfo(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneEtfBaseInfo, eCategory.期货);
        }

        public void OnRspQryEtfStockList(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneEtfStock, eCategory.期货);
        }

        public void OnRspQryEtfEntrustDetail(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneEtfEntrustDetail, eCategory.期货);
        }

        public void OnRspQryEtfTradeResultDetail(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneEtfTradeResultDetail, eCategory.期货);
        }

        public bool OnRspOptionBasketTradeResult(CT2UnPacker responseUnPacker)
        {
            try
            {
                ProcessUnPackerTemplate(responseUnPacker, ProcessOptionBasketOrderInsertInfo, eCategory.期权);
                return true;
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }

            return false;
        }

        public void ProcessOneEtfTradeResultDetail(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var dealDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_date);
                var dealTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_time);
                var dealNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var reportNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_no);
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var reportSeat = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_seat);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                var dealBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var totalFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_total_fee);

                var tri = new TradeResultInfo
                {
                    MsgType = eTradeResultInfoMsgType.查询,
                    DealDate = dealDate,
                    DealTime = CommonUtil.TimeIntToTime(dealTime),
                    DealNo = dealNo,
                    EntrustNo = Convert.ToInt32(entrustNo),
                    ReportNo = reportNo,
                    AccountCode = accountCode,
                    AssetNo = assetNo,
                    CombiNo = combiNo,
                    StockholderId = stockholderId,
                    ReportSeat = reportSeat,
                    MarketType = CommonUtil.MarketNoToeMarketType(marketNo),
                    SecurityID = stockCode,
                    SecurityName =
                        _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode),
                    EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection),
                    DealAmount = (int) Convert.ToDouble(dealAmount),
                    DealPrice = Convert.ToDouble(dealPrice),
                    DealBalance = Convert.ToDouble(dealBalance),
                    TotalFee = Convert.ToDouble(totalFee)
                };


                DispatcherService.Invoke(() => { _owner.EtfTradeResultDetail.Add(tri); });
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void ProcessOneHistoricalEntrust(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            try
            {
                var entrustDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_date);
                var entrustTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_time);
                var operatorNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_operator_no);
                var batchNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_batch_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var reportNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_no);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                var thirdReff = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_third_reff);
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var futuresDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_futures_direction);
                var priceType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_price_type);
                var entrustPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_price);
                var entrustAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_amount);
                var preBuyFrozenBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_frozen_balance);
                var preSellBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_balance);
                var confirmNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_confirm_no);
                var entrustState = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_state);
                var firstDealTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_first_deal_time);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                var dealBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                var dealTimes = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_times);
                var withdrawAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_withdraw_amount);
                var withdrawCause = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_withdraw_cause);
                var positionStr = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_str);

                HistoricalEntrustInfo ei = null;

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    case eCategory.期货:
                    {
                        ei = new HistoricalEntrustInfo();
                        break;
                    }
                    case eCategory.期权:
                    {
                        ei = new HistoricalOptionEntrustInfo();
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        ei = new HistoricalFundEntrustInfo();
                        break;
                    }
                }
                ei.MsgType = eMessageType.委托查询;
                ei.Category = category;

                ei.EntrustDate = entrustDate;
                ei.EntrustTime = CommonUtil.TimeIntToTime(entrustTime);
                ei.OperatorNo = operatorNo;
                ei.EntrustBatchNo = batchNo;
                ei.EntrustNo = (int) Convert.ToDouble(entrustNo);
                ei.ReportNo = reportNo;
                ei.ExtsystemId = extsystemId;
                ei.ThirdReff = thirdReff;
                ei.AccountCode = accountCode;
                ei.AssetNo = assetNo;
                ei.CombiNo = combiNo;
                ei.StockholderId = stockholderId;
                ei.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                ei.SecurityID = stockCode;
                ei.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection);
                ei.FuturesDirection = CommonUtil.FuturesDirectionToeFuturesDirection(futuresDirection);
                ei.EntrustPriceType = CommonUtil.EntrustPriceTypeToeEntrustPriceType(priceType);
                ei.EntrustPrice = Convert.ToDouble(entrustPrice);
                ei.EntrustAmount = (int) Convert.ToDouble(entrustAmount);
                ei.PreBuyFrozenBalance = Convert.ToDouble(preBuyFrozenBalance);
                ei.PreSellBalance = Convert.ToDouble(preSellBalance);
                ei.ConfirmNo = confirmNo;
                ei.EntrustState = CommonUtil.EntrustSateToeEntrustSate(entrustState);
                ei.FirstDealTime = CommonUtil.TimeIntToTime(firstDealTime);
                ei.DealAmount = (int) Convert.ToDouble(dealAmount);
                ei.DealBalance = Convert.ToDouble(dealBalance);
                ei.DealPrice = Convert.ToDouble(dealPrice);
                ei.DealTimes = (int) Convert.ToDouble(dealTimes);
                ei.CancelAmount = (int) Convert.ToDouble(withdrawAmount);
                ei.RevokeCause = withdrawCause;
                ei.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                ei.TotalDealAmount = (int) Convert.ToDouble(dealAmount);

                DispatcherService.Invoke(() =>
                {
                    switch (category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        case eCategory.期货:
                        {
                            _owner.EntrustInfoCollection.HistoricalStockEntrustInfoList.Add(ei);
                            break;
                        }
                        case eCategory.期权:
                        {
                            //m_lpOwner.eventAggregator.GetEvent<HistoricalOptionEntrustInfoNotifyEvent>().Publish(ei as HistoricalOptionEntrustInfo);
                            break;
                        }
                        case eCategory.基金分拆合并:
                        {
                            //m_lpOwner.eventAggregator.GetEvent<HistoricalFundEntrustInfoNotifyEvent>().Publish(ei as HistoricalFundEntrustInfo);
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        public void ProcessOneHistoricalTradeResult(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            try
            {
                var dealDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_date);
                var dealNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                var thirdReff = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_third_reff);
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var instanceNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_instance_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var futuresDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_futures_direction);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                var dealBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var totalFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_total_fee);
                var dealTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_time);
                var positionStr = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_str);

                HistoricalTradeResultInfo ei = null;

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    case eCategory.期货:
                        {
                            ei = new HistoricalTradeResultInfo();
                            break;
                        }
                    case eCategory.期权:
                        {
                            break;
                        }
                    case eCategory.基金分拆合并:
                        {
                            break;
                        }
                }

                ei.MsgType = eTradeResultInfoMsgType.查询;
                ei.Category = category;

                ei.DealDate = dealDate;
                ei.DealNo = dealNo;
                ei.EntrustNo = (int)Convert.ToDouble(entrustNo);
                ei.CombiNo = combiNo;
                ei.StockholderId = stockholderId;
                ei.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                ei.SecurityID = stockCode;
                ei.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                ei.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection);
                ei.FuturesDirection = CommonUtil.FuturesDirectionToeFuturesDirection(futuresDirection);
                ei.DealAmount = (int)Convert.ToDouble(dealAmount);
                ei.DealPrice = Convert.ToDouble(dealPrice);
                ei.DealBalance = Convert.ToDouble(dealBalance);
                ei.TotalFee = Convert.ToDouble(totalFee);
                ei.DealTime = CommonUtil.TimeIntToTime(dealTime);

                DispatcherService.Invoke(() =>
                {
                    switch (category)
                    {
                        case eCategory.股票:
                        case eCategory.债券回购:
                        case eCategory.基金:
                        case eCategory.期货:
                            {
                                _owner.TradeResultInfoCollection.HistoricalTradeResultInfoList.Add(ei);
                                break;
                            }
                        case eCategory.期权:
                            {
                                break;
                            }
                        case eCategory.基金分拆合并:
                            {
                                break;
                            }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public List<string> ProcessOneOrder(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            try
            {
                var entrustDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_date);
                var entrustTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_time);
                //string operator_no = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_operator_no);
                var batchNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_batch_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                //string report_no = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_report_no);
                //string extsystem_id = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                string third_reff = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_third_reff);
                //string account_code = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                //string asset_no = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                //string stockholder_id = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var futuresDirection =  CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_futures_direction);
                var priceType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_price_type);
                var entrustPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_price);
                var entrustAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_amount);
                //string pre_buy_frozen_balance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_buy_frozen_balance);
                //string pre_sell_balance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_pre_sell_balance);
                //string confirm_no = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_confirm_no);
                var entrustState = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_state);
                //string first_deal_time = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_first_deal_time);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                //string deal_balance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                //string deal_times = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_times);
                var withdrawAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_withdraw_amount);
                var withdrawCause = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_withdraw_cause);
                var positionStr = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_str);

                EntrustInfo ei = null;

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
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
                    case eCategory.基金分拆合并:
                    {
                        ei = new FundEntrustInfo();
                        break;
                    }
                }

                ei.MsgType = eMessageType.委托查询;
                ei.Category = category;
                ei.EntrustAmount = (int) Convert.ToDouble(entrustAmount);
                ei.EntrustBatchNo = batchNo;
                ei.EntrustDate = entrustDate;
                ei.EntrustTime = CommonUtil.TimeIntToTime(entrustTime);
                ei.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection);
                ei.FuturesDirection = CommonUtil.FuturesDirectionToeFuturesDirection(futuresDirection);
                ei.EntrustNo = Convert.ToInt32(entrustNo);
                ei.EntrustPrice = Convert.ToDouble(entrustPrice);
                ei.EntrustPriceType = CommonUtil.EntrustPriceTypeToeEntrustPriceType(priceType);
                ei.SecurityID = stockCode;
                ei.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                ei.EntrustState = CommonUtil.EntrustSateToeEntrustSate(entrustState);
                ei.CancelAmount = (int) Convert.ToDouble(withdrawAmount);
                ei.DealPrice = Convert.ToDouble(dealPrice);
                ei.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                ei.TotalDealAmount = (int) Convert.ToDouble(dealAmount);
                ei.RevokeCause = withdrawCause;
                ei.CombiNo = combiNo;
                ei.ThirdReff = third_reff;

                switch (category)
                {
                    case eCategory.股票:
                    case eCategory.债券回购:
                    case eCategory.基金:
                    {
                        _owner.EventAggregator.GetEvent<EntrustInfoNotifyEvent>().Publish(ei);
                        break;
                    }
                    case eCategory.期货:
                    {
                        _owner.EventAggregator.GetEvent<FutureEntrustInfoNotifyEvent>().Publish(ei as FutureEntrustInfo);
                        break;
                    }
                    case eCategory.期权:
                    {
                        _owner.EventAggregator.GetEvent<OptionEntrustInfoNotifyEvent>().Publish(ei as OptionEntrustInfo);
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        _owner.EventAggregator.GetEvent<FundEntrustInfoNotifyEvent>().Publish(ei as FundEntrustInfo);
                        break;
                    }
                }

                var result = new List<string> {combiNo, positionStr};

                return result;
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }

            return null;
        }

        /// <summary>
        /// 证券委托查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        /// <param name="category"></param>
        private void OnRspQryOrder(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            ProcessOrderUnPacker(responseUnPacker, ProcessOneOrder, category);

            switch (category)
            {
                case eCategory.股票:
                case eCategory.债券回购:
                case eCategory.基金:
                {
                    _owner.IsStockEntrustReady = true;
                    break;
                }
                case eCategory.期货:
                {
                    _owner.IsFutureEntrustReady = true;
                    break;
                }
                case eCategory.期权:
                {
                    _owner.IsOptionEntrustReady = true;                      
                    break;
                }
                case eCategory.基金分拆合并:
                {
                    _owner.IsFundEntrustReady = true;
                    break;
                }
            }
        }

        private void OnRspQryHistoricalEntrust(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneHistoricalEntrust, category);
        }

        private void OnRspQryHistoricalTradeResult(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneHistoricalTradeResult, category);
        }

        public void ProcessOneTradeResult(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var dealDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_date);
                var dealNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                var thirdReff = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_third_reff);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var instanceNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_instance_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                var dealBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var totalFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_total_fee);
                var dealTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_time);
                var positionStr = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_str);

                var tri = new TradeResultInfo();
                tri.MsgType = eTradeResultInfoMsgType.查询;
                tri.DealAmount = (int) Convert.ToDouble(dealAmount);
                tri.DealNo = dealNo;
                tri.DealDate = dealDate;
                tri.DealTime = CommonUtil.TimeIntToTime(dealTime);
                tri.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection);
                tri.EntrustNo = Convert.ToInt32(entrustNo);
                tri.DealPrice = Convert.ToDouble(dealPrice);
                tri.SecurityID = stockCode;
                tri.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                tri.DealPrice = Convert.ToDouble(dealPrice);
                tri.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                tri.SystemNumber = positionStr;
                tri.CombiNo = combiNo;

                _owner.EventAggregator.GetEvent<TradeResultNotifyEvent>().Publish(tri);
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        /// <summary>
        /// 证券成交查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        /// <param name="category"></param>
        private void OnRspQryTradeResult(CT2UnPacker responseUnPacker, eCategory category = eCategory.股票)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneTradeResult, category);
            _owner.IsStockTradeResultReady = true;
        }

        public void ProcessOneFutureTradeResult(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var dealDate = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_date);
                var dealNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);
                var thirdReff = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_third_reff);
                var assetNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_asset_no);
                var combiNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_combi_no);
                var instanceNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_instance_no);
                var stockholderId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stockholder_id);
                var marketNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_market_no);
                var stockCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_stock_code);
                var entrustDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_direction);
                var futuresDirection = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_futures_direction);
                var dealAmount = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_amount);
                var dealPrice = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_price);
                var dealBalance = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_balance);
                var totalFee = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_total_fee);
                var dealTime = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_deal_time);
                var positionStr = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_position_str);

                TradeResultInfo ftri = null;

                switch (category)
                {
                    case eCategory.期货:
                    {
                        ftri = new FutureTradeResultInfo();
                        break;
                    }
                    case eCategory.期权:
                    {
                        ftri = new OptionTradeResultInfo();
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        ftri = new FundTradeResultInfo();
                        break;
                    }
                }

                ftri.MsgType = eTradeResultInfoMsgType.查询;
                ftri.DealNo = dealNo;
                ftri.DealAmount = (int) Convert.ToDouble(dealAmount);
                ftri.DealDate = dealDate;
                ftri.DealTime = CommonUtil.TimeIntToTime(dealTime);
                ftri.EntrustDirection = CommonUtil.EntrustDirectionToeEntrustDirection(entrustDirection);
                ftri.FuturesDirection = CommonUtil.FuturesDirectionToeFuturesDirection(futuresDirection);
                ftri.EntrustNo = Convert.ToInt32(entrustNo);
                ftri.DealPrice = Convert.ToDouble(dealPrice);
                ftri.SecurityID = stockCode;
                ftri.SecurityName = _owner.Marketdataservice.GetSecurityName(CommonUtil.MarketNoToExId(marketNo), stockCode);
                ftri.DealPrice = Convert.ToDouble(dealPrice);
                ftri.MarketType = CommonUtil.MarketNoToeMarketType(marketNo);
                ftri.SystemNumber = positionStr;
                ftri.CombiNo = combiNo;

                switch (category)
                {
                    case eCategory.期货:
                    {
                        _owner.EventAggregator.GetEvent<FutureTradeResultNotifyEvent>().Publish(ftri as FutureTradeResultInfo);
                        break;
                    }
                    case eCategory.期权:
                    {
                        _owner.EventAggregator.GetEvent<OptionTradeResultNotifyEvent>().Publish(ftri as OptionTradeResultInfo);
                        break;
                    }
                    case eCategory.基金分拆合并:
                    {
                        _owner.EventAggregator.GetEvent<FundTradeResultNotifyEvent>().Publish(ftri as FundTradeResultInfo);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        /// <summary>
        /// 证券成交查询回报
        /// </summary>
        /// <param name="responseUnPacker"></param>
        /// <param name="category"></param>
        private void OnRspQryFutureTradeResult(CT2UnPacker responseUnPacker, eCategory category)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneFutureTradeResult, category);

            if (category == eCategory.期货)
            {
                _owner.IsFutureTradeResultReady = true;
            }

            if (category == eCategory.期权)
            {
                _owner.IsOptionTradeResultReady = true;
            }

            if (category == eCategory.基金分拆合并)
            {
                _owner.IsFundTradeResultReady = true;
            }
        }


        /// <summary>
        /// 委托撤单回报
        /// </summary>
        /// <param name="unpacker"></param>
        private void OnOrderCancel(CT2UnPacker unpacker)
        {
        }

        public void ProcessOneBasketOrderInsertInfo(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var batchNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_batch_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var requestOrder = CommonUtil.GetStrFromUnPacker(responseUnPacker,
                    CommonUtil.UFX_FIELD_NAME_request_order);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);

                _owner.Logger.Info(string.Format("@篮子委托报盘机回报：批号:{0},第三方编号:{1},委托序号:{2},请求次序:{3}", batchNo, extsystemId,
                    entrustNo, requestOrder));
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }

        public void ProcessOptionBasketOrderInsertInfo(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var batchNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_batch_no);
                var entrustNo = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_entrust_no);
                var extsystemId = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_extsystem_id);

                _owner.Logger.Info(string.Format("@期权篮子委托报盘机回报：批号:{0},第三方编号:{1},委托序号:{2}", batchNo, extsystemId,
                    entrustNo));
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        public void OnQryAccount(CT2UnPacker responseUnPacker)
        {
            ProcessUnPackerTemplate(responseUnPacker, ProcessOneAccount, eCategory.股票);
        }

        public void ProcessOneAccount(CT2UnPacker responseUnPacker, eCategory category)
        {
            try
            {
                var accountCode = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_code);
                var accountName = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_name);
                var accountType = CommonUtil.GetStrFromUnPacker(responseUnPacker, CommonUtil.UFX_FIELD_NAME_account_type);

                if (!string.IsNullOrEmpty(accountCode))
                {
                    _owner.AccountCodeList.Add(accountCode);
                }
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }


        //篮子委托下单的回报函数
        public bool OnBasketOrderInsert(CT2UnPacker responseUnPacker)
        {
            try
            {
                ProcessUnPackerTemplate(responseUnPacker, ProcessOneBasketOrderInsertInfo, eCategory.股票);
                return true;
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }

            return false;
        }

        //委托下单的回报函数
        public bool OnOrderInsert(CT2UnPacker lpUnPack)
        {
            try
            {
                object extsystemId;
                object entrustNo;
                object entrustFailCode;
                object failCause;
                object riskSerialNo;

                CommonUtil.GetValueFromPack(lpUnPack, "extsystem_id", typeof (int), out extsystemId);
                CommonUtil.GetValueFromPack(lpUnPack, "entrust_no", typeof (int), out entrustNo);
                CommonUtil.GetValueFromPack(lpUnPack, "entrust_fail_code", typeof (int), out entrustFailCode);
                CommonUtil.GetValueFromPack(lpUnPack, "fail_cause", typeof (string), out failCause);
                CommonUtil.GetValueFromPack(lpUnPack, "risk_serial_no", typeof (int), out riskSerialNo);

                _owner.Logger.Info(string.Format("@报盘机回报：第三方编号:{0},委托序号:{1}", extsystemId, entrustNo));

                return true;
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }

            return false;
        }

        public override void OnReceivedBizMsg(CT2Connection lpConnection, int hSend, CT2BizMessage lpMsg)
        {
            try
            {
                _owner.Logger.Debug("**********************OnReceivedBizMsg开始**********************");

                var iErrorCode = lpMsg.GetErrorNo(); //获取错误码
                var iRetCode = lpMsg.GetReturnCode(); //获取返回码
                var iFunction = lpMsg.GetFunction();
                if (iErrorCode != 0)
                {
                    _owner.Logger.Debug("错误:异步接收出错：" + lpMsg.GetErrorNo() + lpMsg.GetErrorInfo());
                }
                else
                {
                    CT2UnPacker unpacker = null;
                    var iLen = 0;
                    var lpdata = lpMsg.GetContent(&iLen);
                    if (iLen > 0)
                        unpacker = new CT2UnPacker(lpdata, (uint) iLen);

                    switch ((功能号) iFunction)
                    {
                        case 功能号.心跳: //1.0消息中心心跳
                        {
                            _owner.Logger.Debug("心跳回报...");
                            lpMsg.ChangeReq2AnsMessage();
                            lpConnection.SendBizMsg(lpMsg, 1);
                            break;
                        }
                        case 功能号.登录:
                        {
                            _owner.Logger.Debug("信息:登录回报...");
                            if (unpacker != null)
                            {
                                var code = unpacker.GetInt("ErrCode");
                                if (code == 0)
                                {
                                    unpacker.SetCurrentDatasetByIndex(1);
                                    _owner.UserToken = unpacker.GetStr("user_token");

                                    if (!string.IsNullOrEmpty(_owner.UserToken))
                                    {
                                        _owner.EventAggregator.GetEvent<StatusChangedNotifyEvent>().Publish("已登录");
                                        _owner.EventAggregator.GetEvent<TraderLoginStatusNotifyEvent>().Publish(new TraderLoginStatusEntity
                                        {
                                            TraderID = "test123", LoginStatus = true, Message = "登录成功"
                                        });
                                    }
                                }
                                else
                                {
                                }
                            }

                            break;
                        }
                        case 功能号.退出登录:
                        {
                            break;
                        }
                        case 功能号.账户查询:
                        {
                            OnQryAccount(unpacker);
                            break;
                        }
                        case 功能号.普通买卖委托: //委托下单
                        {
                            OnOrderInsert(unpacker);
                            break;
                        }
                        case 功能号.篮子委托:
                        {
                            OnBasketOrderInsert(unpacker);
                            break;
                        }
                        case 功能号.委托撤单: //撤单
                        {
                            OnOrderCancel(unpacker);
                            break;
                        }
                        case 功能号.证券委托查询: //委托查询
                        {
                            OnRspQryOrder(unpacker);
                            break;
                        }
                        case 功能号.证券历史委托查询: //历史委托查询
                        {
                            OnRspQryHistoricalEntrust(unpacker);
                            break;
                        }
                        case 功能号.证券历史成交查询: //历史成交查询
                        {
                            OnRspQryHistoricalTradeResult(unpacker);
                            break;
                        }
                        case 功能号.证券成交查询: //证券成交查询
                        {
                            OnRspQryTradeResult(unpacker);
                            break;
                        }
                        case 功能号.证券持仓查询: //证券持仓查询
                        {
                            OnRspQryHoldingPosition(unpacker);
                            break;
                        }
                        case 功能号.账户资金查询: //查询资金余额
                        {
                            OnRspQryAccountMoneyInfo(unpacker);
                            break;
                        }
                        case 功能号.组合查询:
                        {
                            OnRspQryCombiNo(unpacker);
                            break;
                        }
                        case 功能号.资产单元查询:
                        {
                            OnRspQryAssetNo(unpacker);
                            break;
                        }
                        case 功能号.期货信息查询:
                        {
                            OnRspQryInstrumentInfo(unpacker);
                            break;
                        }
                        case 功能号.期货委托:
                        {
                            OnOrderInsert(unpacker);
                            break;
                        }
                        case 功能号.期货委托撤单:
                        {
                            OnOrderCancel(unpacker);
                            break;
                        }
                        case 功能号.期货持仓查询:
                        {
                            OnRspQryFutureHoldingPosition(unpacker);
                            break;
                        }
                        case 功能号.期货委托查询:
                        {
                            OnRspQryOrder(unpacker, eCategory.期货);
                            break;
                        }
                        case 功能号.期货历史委托查询:
                        {
                            OnRspQryHistoricalEntrust(unpacker, eCategory.期货);
                            break;
                        }
                        case 功能号.期货历史成交查询: //历史成交查询
                        {
                            OnRspQryHistoricalTradeResult(unpacker, eCategory.期货);
                            break;
                        }
                        case 功能号.期货保证金账户查询:
                        {
                            OnRspQryMarginInfo(unpacker, eCategory.期货);
                            break;
                        }
                        case 功能号.期货成交查询:
                        {
                            OnRspQryFutureTradeResult(unpacker, eCategory.期货);
                            break;
                        }
                        case 功能号.期权持仓查询:
                        {
                            OnRspQryOptionHoldingPosition(unpacker);
                            break;
                        }
                        case 功能号.期权委托查询:
                        {
                            OnRspQryOrder(unpacker, eCategory.期权);
                            break;
                        }
                        case 功能号.期权委托撤单:
                        {
                            OnOrderCancel(unpacker);
                            break;
                        }
                        case 功能号.期权保证金账户查询:
                        {
                            OnRspQryMarginInfo(unpacker, eCategory.期权);
                            break;
                        }
                        case 功能号.期权成交查询:
                        {
                            OnRspQryFutureTradeResult(unpacker, eCategory.期权);
                            break;
                        }
                        case 功能号.期权篮子委托:
                        {
                            OnRspOptionBasketTradeResult(unpacker);
                            break;
                        }
                        case 功能号.基金委托查询:
                        {
                            OnRspQryOrder(unpacker, eCategory.基金分拆合并);
                            break;
                        }
                        case 功能号.基金委托:
                        {
                            OnOrderInsert(unpacker);
                            break;
                        }
                        case 功能号.基金成交查询:
                        {
                            OnRspQryFutureTradeResult(unpacker, eCategory.基金分拆合并);
                            break;
                        }
                        case 功能号.ETF基础信息查询:
                        {
                            OnRspQryEtfBaseInfo(unpacker);
                            break;
                        }
                        case 功能号.ETF成份股信息查询:
                        {
                            OnRspQryEtfStockList(unpacker);
                            break;
                        }
                        case 功能号.ETF申赎委托明细查询:
                        {
                            OnRspQryEtfEntrustDetail(unpacker);
                            break;
                        }
                        case 功能号.ETF申赎成交明细查询:
                        {
                            OnRspQryEtfTradeResultDetail(unpacker);
                            break;
                        }
                    }

                    //返回业务错误
                    if (iRetCode != 0)
                    {
                        _owner.Logger.Debug("错误:异步接收业务出错：\n");
                    }
                    else //正常业务返回
                    {
                        _owner.Logger.Debug("异步接收业务成功：\n");
                    }

                    _owner.PrintUnPack(unpacker);
                    if (unpacker != null)
                    {
                        unpacker.Dispose();
                    }
                }

                _owner.Logger.Debug("**********************OnReceivedBizMsg结束**********************");
            }
            catch (Exception ex)
            {
                CommonUtil.LogException(_owner.Logger, ex);
            }
        }
    };

    public class Entrust
    {
        public int ExtsystemId;
        public string AccountCode;
        public string CombiNo;
        public string MarketNo;
        public string StockCode;
        public int EntrustAmount;
        public double EntrustPrice;
        public string PriceType;
        public string EntrustDirection;
        public string FuturesDirection;
        public int LimitDealAmount;
        public string CoveredFlag;
        public double EntrustBalance;
        public string PurchaseWay;
        public string ThirdReff = "0";
        public double LimitEntrustRatio = 100;
        public double FtrLimitEntrustRatio = 100;

        public Entrust Copy()
        {
            var e = new Entrust
            {
                ExtsystemId = ExtsystemId,
                AccountCode = AccountCode,
                CombiNo = CombiNo,
                MarketNo = MarketNo,
                StockCode = StockCode,
                EntrustAmount = EntrustAmount,
                EntrustPrice = EntrustPrice,
                PriceType = PriceType,
                EntrustDirection = EntrustDirection,
                FuturesDirection = FuturesDirection,
                LimitDealAmount = LimitDealAmount,
                CoveredFlag = CoveredFlag,
                EntrustBalance = EntrustBalance,
                PurchaseWay = PurchaseWay,
                ThirdReff = ThirdReff,
                LimitEntrustRatio =  LimitEntrustRatio,
                FtrLimitEntrustRatio = FtrLimitEntrustRatio
            };


            return e;
        }

        public override string ToString()
        {
            var s =
                string.Format(
                    "@新委托：组合编号:{0},交易市场编号:{1},证券代码:{2},委托数量:{3},委托价格:{4},价格类型:{5},委托方向:{6},第三方编号:{7},开平方向:{8},第三方说明:{9},现货最小委托比例:{10},期货最小委托比例:{11}",
                    CombiNo, MarketNo, StockCode, EntrustAmount, EntrustPrice, PriceType, EntrustDirection, ExtsystemId,
                    FuturesDirection, ThirdReff, LimitEntrustRatio, FtrLimitEntrustRatio);
            return s;
        }
    }

    public class FutureEntrust : Entrust
    {
        public override string ToString()
        {
            var s = base.ToString();
            return string.Format("{0},开平仓:{1}", s, FuturesDirection);
        }
    }

    public class OptionEntrust : FutureEntrust
    {
    }

    public class FundEntrust : Entrust
    {
    }

    public struct Subscribe
    {
        public string FundAccount;
        public string FundAccountPassword;
    }

    public struct FieldInfo
    {
        public string FieldName;
        public sbyte FieldType;
        public int FieldWidth;
        public int FieldScale;

        public FieldInfo(string fieldName, sbyte fieldType, int fieldWidth, int fieldScale)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            FieldWidth = fieldWidth;
            FieldScale = fieldScale;
        }
    }

    public class StatusChangedNotifyEvent : PubSubEvent<string>
    {

    }

    public class LegNumberToArbitrage
    {
        public string PreviousLegNumber;
        public string MyLegNumber;
        public Arbitrage Arbitrage;
        public ObservableCollection<ArbitrageItem> ArbitrageItemsLeg1;
        public ObservableCollection<ArbitrageItem> ArbitrageItemsLeg2;  //两腿同时套利时，记录第二腿品种；两腿异步时，为空
        public bool IsLast;
        public int CurrentTime;
    }

    public enum SubscribeServiceConnectionStatus
    {
        Disconnected = 0x0000, // 未连接
        Connecting = 0x0001, // socket正在连接
        Connected = 0x0002, // socket已连接
        SafeConnecting = 0x0004, // 正在建立安全连接
        SafeConnected = 0x0008, // 已建立安全连接
        Registering = 0x0010, // 正注册
        Registered = 0x0020, // 已注册
        Rejected = 0x0040  // 被拒绝，将被关闭
    };

}

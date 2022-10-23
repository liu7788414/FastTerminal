using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.Views;

namespace TradeStation.Modules.Trade.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OptionMatrixPanelViewModel : TradeViewModelBase
    {
        [Import]
        public OptionInfoModelCollection OptionInfoModelCollection { get; set; }

        public ICommand OptionInfoReady { get; set; }

        [ImportingConstructor]
        public OptionMatrixPanelViewModel(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            SubscribeEvents();
            InitCommands();
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionInfoReadyEvent>().Subscribe(OnOptionInfoReady);
        }

        public ICommand SelectedOptionTargetChangedCommand { get; set; }
        public ICommand NewEntrustCommand { get; set; }

        public void LoadOptionInfoModels(string optionTarget)
        {
            //所有期权信息按照标的物分组
            var groupsByUnderlyingSymbol = OptionInfoModelCollection.OptionInfoList.GroupBy(s => s.UnderlyingSymbol);

            foreach (var groupByUnderlyingSymbol in groupsByUnderlyingSymbol)
            {
                //目前只考虑50ETF
                if (groupByUnderlyingSymbol.Key.Equals(optionTarget))
                {
                    //订阅所有相关期权行情
                    SubscribeAll(groupByUnderlyingSymbol);

                    //按照行权日、行权价、认购认沽三级排序
                    var sorted = (from info in groupByUnderlyingSymbol
                        orderby info.ExerciseDate, info.ExercisePrice, info.CallOrPut
                        select info).ToList();


                    //找出所有行权月
                    var exerciseDate =
                        (from info in sorted select info.ExerciseDate.ToString("yyyy-MM")).Distinct().ToList();
                    exerciseDate.Sort();

                    //找出所有行权价
                    var exercisePrice = (from info in sorted select info.ExercisePrice).Distinct().ToList();
                    exercisePrice.Sort();

                    var sortedByExerciseDate = sorted.GroupBy(s => s.ExerciseDate);

                    var listFullExercisePrice = new List<List<OptionInfoModel>>();

                    //对每个行权月补上缺失的行权价
                    foreach (var sbed in sortedByExerciseDate)
                    {
                        var lll = sbed.ToList();
                        var existedExerciesePrice = (from info in sbed select info.ExercisePrice).Distinct().ToList();

                        var priceSequence = new List<OptionInfoModel>();

                        int j = 0;

                        for (int i = 0; i < exercisePrice.Count; i++)
                        {
                            if (existedExerciesePrice.Contains(exercisePrice[i]))
                            {
                                if (j < lll.Count - 1)
                                {
                                    priceSequence.Add(lll[j++]); //认购
                                    priceSequence.Add(lll[j++]); //认沽
                                }
                            }
                            else //补上缺失的行权价，包含认购和认沽两种期权
                            {
                                var optionInfoModel = new OptionInfoModel
                                {
                                    CallOrPut = eOptionType.认购期权,
                                    ExercisePrice = exercisePrice[i],
                                    ExerciseDate = sbed.Key
                                };

                                priceSequence.Add(optionInfoModel);

                                optionInfoModel = new OptionInfoModel
                                {
                                    CallOrPut = eOptionType.认沽期权,
                                    ExercisePrice = exercisePrice[i],
                                    ExerciseDate = sbed.Key
                                };

                                priceSequence.Add(optionInfoModel);
                            }
                        }

                        listFullExercisePrice.Add(priceSequence);
                    }

                    var listCallOrPutSplitted = new List<List<OptionInfoModel>>();

                    //分裂认购认沽期权
                    for (var i = 0; i < listFullExercisePrice.Count; i++)
                    {
                        var q = listFullExercisePrice[i].GroupBy(s => s.CallOrPut).ToList();

                        foreach (var v in q)
                        {
                            listCallOrPutSplitted.Add(v.ToList());
                        }
                    }

                    //复制认购认沽
                    var listCallOrPutSplittedCopied = new List<List<OptionInfoModel>>();

                    for (var i = 0; i < listCallOrPutSplitted.Count; i++)
                    {
                        listCallOrPutSplittedCopied.Add(new List<OptionInfoModel>(listCallOrPutSplitted[i]));
                        listCallOrPutSplittedCopied.Add(new List<OptionInfoModel>(listCallOrPutSplitted[i]));
                    }

                    var listCallOrPutSplittedCopiedLeft = new List<List<OptionInfoModel>>();

                    //补充左边栏：委卖委买，认购认沽
                    for (var i = 0; i < listCallOrPutSplittedCopied.Count; i++)
                    {
                        var opi = new OptionInfoModel();
                        switch (i%4)
                        {
                            case 0:
                            {
                                opi.NodeType = eNodeType.BuyCall;
                                opi.PlainText = "委买(Call)";
                                break;
                            }
                            case 1:
                            {
                                opi.NodeType = eNodeType.SellCall;
                                opi.PlainText = "委卖(Call)";
                                break;
                            }
                            case 2:
                            {
                                opi.NodeType = eNodeType.BuyPut;
                                opi.PlainText = "委买(Put)";
                                break;
                            }
                            case 3:
                            {
                                opi.NodeType = eNodeType.SellPut;
                                opi.PlainText = "委卖(Put)";
                                break;
                            }
                        }

                        var row = new List<OptionInfoModel>(listCallOrPutSplittedCopied[i]);
                        row.Insert(0, opi);

                        listCallOrPutSplittedCopiedLeft.Add(row);
                    }



                    //生成行权价序列
                    var listExercisePrice = new List<OptionInfoModel>();

                    foreach (var price in exercisePrice)
                    {
                        listExercisePrice.Add(new OptionInfoModel
                        {
                            NodeType = eNodeType.ExercisePrice,
                            PlainText = price.ToString(CultureInfo.InvariantCulture)
                        });
                    }

                    var listCompleted = new List<List<OptionInfoModel>>();

                    //补充标题栏：行权月和行权价
                    for (var i = 0; i < exerciseDate.Count; i++)
                    {
                        if (i*exerciseDate.Count >= listCallOrPutSplittedCopiedLeft.Count)
                        {
                            break;
                        }
                        var opi = new OptionInfoModel {NodeType = eNodeType.ExerciseDate, PlainText = exerciseDate[i]};

                        var v = new List<OptionInfoModel> {opi};
                        v.AddRange(new List<OptionInfoModel>(listExercisePrice));

                        listCompleted.Add(v);
                        listCompleted.Add(
                            new List<OptionInfoModel>(listCallOrPutSplittedCopiedLeft[i*exerciseDate.Count]));
                        listCompleted.Add(
                            new List<OptionInfoModel>(listCallOrPutSplittedCopiedLeft[i*exerciseDate.Count + 1]));
                        listCompleted.Add(
                            new List<OptionInfoModel>(listCallOrPutSplittedCopiedLeft[i*exerciseDate.Count + 2]));
                        listCompleted.Add(
                            new List<OptionInfoModel>(listCallOrPutSplittedCopiedLeft[i*exerciseDate.Count + 3]));
                    }

                    ArrayOptions50ETF = listCompleted;
                }
            }
        }

        public void OnOptionInfoReady(string ready)
        {
            OptionTargets = OptionInfoModelCollection.OptionInfoList.Select(x => x.UnderlyingSymbol).Distinct().ToList();

            if (OptionTargets.Count > 0)
            {
                LoadOptionInfoModels(OptionTargets.First());
            }
        }

        /// <summary>
        /// 订阅该品种所有期权的行情
        /// </summary>
        /// <param name="group"></param>
        public void SubscribeAll(IGrouping<string, OptionInfoModel> group)
        {
            foreach (var optionInfoModel in group)
            {
                MarketDataService.SubscribeSecQuot(new ExSecID(optionInfoModel.ExID, optionInfoModel.SecurityID));
            }
        }

        private List<string> _optionTargets = new List<string>();
        public List<string> OptionTargets
        {
            get { return _optionTargets; }
            set
            {
                SetProperty(ref _optionTargets, value);
            }
        }

        public List<List<OptionInfoModel>> _arrayOptions50ETF = new List<List<OptionInfoModel>>();
        public List<List<OptionInfoModel>> ArrayOptions50ETF
        {
            get { return _arrayOptions50ETF; }
            set { SetProperty(ref _arrayOptions50ETF, value); }
        }
        private OptionInfoModel _selectedOptionInfoModel;
        public OptionInfoModel SelectedOptionInfoModel
        {
            get { return _selectedOptionInfoModel; }
            set { SetProperty(ref _selectedOptionInfoModel, value); }
        }

        private string _selectedCombiNo;
        public string SelectedCombiNo
        {
            get { return _selectedCombiNo; }
            set { SetProperty(ref _selectedCombiNo, value); }
        }

        private double _volume = 1;
        public double Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value); }
        }

        private double _ticks = 2;
        public double Ticks
        {
            get { return _ticks; }
            set { SetProperty(ref _ticks, value); }
        }

        private double _price;
        public double Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private eEntrustDirection _entrustDirection;

        public eEntrustDirection EntrustDirection
        {
            get { return _entrustDirection; }
            set { SetProperty(ref _entrustDirection, value); }
        }

        protected override void InitCommands()
        {
            SelectedOptionTargetChangedCommand = new DelegateCommand<string>(OnSelectedOptionTargetChanged);
            NewEntrustCommand = new DelegateCommand(OnNewEntrust);
        }

        public void OnNewEntrust()
        {
            if (SelectedOptionInfoModel != null)
            {
                var securityId = SelectedOptionInfoModel.SecurityID;
                
                var marketType = CommonUtil.ExIDToMarketType(SelectedOptionInfoModel.ExID);
                var selectedCombiNo = SelectedCombiNo;
                var entrustAmount = (int) Volume;
                var entrustDirection = EntrustDirection;

                double price;
                double chasePrice = Ticks*0.0001;
                if (EntrustDirection == eEntrustDirection.买入)
                {
                    price = Price + chasePrice;
                }
                else
                {
                    if (Price - chasePrice <= 0)
                    {
                        MessageBox.Show(string.Format("追加的Ticks不合理,导致报价为{0},无效值", Price - chasePrice), "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    price = Price - chasePrice;
                }

                var entrustPrice = price;
                var entrustPriceType = eEntrustPriceType.限价;
                var futuresDirection = eFuturesDirection.开仓;
                var investType = eInvestType.投机;

                var oei = (OptionEntrustInfo) CommonUtil.BuildEntrustInfo(
                    securityId,
                    entrustPrice,
                    marketType,
                    selectedCombiNo,
                    entrustAmount,
                    entrustDirection,
                    entrustPriceType,
                    eCategory.期权,
                    futuresDirection,
                    investType);
                EventAggregator.GetEvent<NewOptionEntrustNotifyEvent>().Publish(oei);
            }
        }

        public void OnSelectedOptionTargetChanged(string selectedTarget)
        {
            LoadOptionInfoModels(selectedTarget);
        }

        protected override void OnAdvancedQuery()
        {

        }

        protected override void OnRefresh()
        {

        }

        protected override void OnExportList()
        {

        }
    }


    public class OptionInfoModelComparer : IComparer<OptionInfoModel>
    {
        public int Compare(OptionInfoModel x, OptionInfoModel y)
        {
            return x.ExerciseDate.CompareTo(y.ExerciseDate);
        }
    }
}

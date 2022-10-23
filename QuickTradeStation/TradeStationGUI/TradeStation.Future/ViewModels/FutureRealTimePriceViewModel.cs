using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;
using TradeStation.Infrastructure.ViewModels;
using TradeStation.Modules.Trade.ViewModels;
using TradeStation.Modules.RealTimePrice.Views;
using TradeStation.Modules.RealTimePrice.ViewModels;
using TradeStation.Infrastructure.Metadata;

namespace TradeStation.Future.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FutureRealTimePriceViewModel : RealTimePriceViewModelBase
    {
        [Import]
        private InstrumentInfoCollection _futureInstrumentInfoCollection;

        #region Properties

        private PositionInfoBase _holdingLongPosition;
        public PositionInfoBase HoldingLongPosition
        {
            get { return _holdingLongPosition; }
            set
            {
                SetProperty(ref _holdingLongPosition, value);
            }
        }

        private PositionInfoBase _holdingShortPosition;
        public PositionInfoBase HoldingShortPosition
        {
            get { return _holdingShortPosition; }
            set
            {
                SetProperty(ref _holdingShortPosition, value);
            }
        }

        protected override double FloatingPaneWidth
        {
            get { return 240; }
        }

        protected override double FloatingPaneHeight
        {
            get { return 485; }
        }

        protected override double FloatingExpandedPaneWidth
        {
            get { return 700; }
        }

        protected override double FloatingExpandedPaneHeight
        {
            get { return 485; }
        }

        #endregion

        [ImportingConstructor]
        public FutureRealTimePriceViewModel(
            RealTimePriceDiagramPanel view,
            SecurityRefHttpService securityRefHttpService,
            SecurityInfoMetadata securityInfoMetadata,
            MarketDataService marketDataService,
            DialogService dialogService,
            RealTimeDataProcessor realTimeDataProcessor,
            IEventAggregator eventAggregator,
            HsStock tradeInfo,
            MenubarViewModel menubarViewModel)
            : base(view, securityRefHttpService, securityInfoMetadata, marketDataService, dialogService, realTimeDataProcessor, eventAggregator, tradeInfo, menubarViewModel)
        {
            _eventAggregator.GetEvent<RelatedFuturePositionItemChangedNotifyEvent>().Subscribe(this.OnReturnPositionInfo);

            // Set fast trade default amount.
            this.EntrustAmount = this.MenubarViewModel.UserSettings.FastTradeAmountFuture;

            // Initialize the Holding with blank object.
            HoldingLongPosition = new PositionInfoBase();
            HoldingShortPosition = new PositionInfoBase();

            this.IsRealTimePrice = true;
            this.IsExpandedChart = true;
            this.KLinePeriodType = eDisplayedKLinePeriodType.日线;
            this.SelectedInvestType = eInvestType.投机;

            if (null != this.RealTimeMarketData && !this.RealTimeMarketData.IsMultiplierReady
                && null != _futureInstrumentInfoCollection && null != _futureInstrumentInfoCollection.InstrumentInfoList)
            {
                var futureInstrumentInfo = _futureInstrumentInfoCollection.InstrumentInfoList.FirstOrDefault(x => x.SecurityID == this.SecurityInfo.SecurityID);
                if (null != futureInstrumentInfo)
                {
                    this.RealTimeMarketData.VolumeMultiplier = futureInstrumentInfo.Multiple;
                    this.RealTimeMarketData.IsMultiplierReady = true;
                }
            }
        }

        protected void OnReturnPositionInfo(FuturePositionInfo positionInfo)
        {
            if (positionInfo.MsgType == ePositionInfoMsgType.交易)
            {
                PositionInfoBase targetPosition = null;

                if (positionInfo.PositionFlag == ePositionFlag.多头持仓)
                {
                    targetPosition = this.HoldingLongPosition;
                }
                else if (positionInfo.PositionFlag == ePositionFlag.空头持仓)
                {
                    targetPosition = this.HoldingShortPosition;
                }
                else
                {
                    // If ePositionFlag is 备兑空头持仓, do nothing and directly return.
                }

                // Only set reference when the combine number is empty for holding position.
                // It means it does not have holding before when combine number is empty.
                if (null != targetPosition
                    && string.IsNullOrEmpty(targetPosition.CombiNo)
                    && null != this.SecurityInfo
                    && positionInfo.MarketType == this.SecurityInfo.MarketType
                    && positionInfo.SecurityID == this.SecurityInfo.SecurityID
                    && positionInfo.CombiNo == this.SelectedCombineNumber)
                {
                    RefreshHoldingPosition();
                }
            }
        }

        protected override void OnExpandedChartButtonClickCommand()
        {
            if (null != this.RealTimeMarketData && !this.RealTimeMarketData.IsMultiplierReady
                && null != _futureInstrumentInfoCollection && null != _futureInstrumentInfoCollection.InstrumentInfoList)
            {
                var futureInstrumentInfo = _futureInstrumentInfoCollection.InstrumentInfoList.FirstOrDefault(x => x.SecurityID == this.SecurityInfo.SecurityID);
                if (null != futureInstrumentInfo)
                {
                    foreach(var point in this.RealTimeMarketData.RealTimeMarketDataPointSets)
                    {
                        if (point.IsTrueDataPoint)
                        {
                            point.RealTimeAverage = point.RealTimeAverage * (this.RealTimeMarketData.VolumeMultiplier / futureInstrumentInfo.Multiple);
                        }
                    }

                    this.RealTimeMarketData.VolumeMultiplier = futureInstrumentInfo.Multiple;
                    this.RealTimeMarketData.IsMultiplierReady = true;
                }
            }

            base.OnExpandedChartButtonClickCommand();
        }

        protected override void OnCombineNoSelectionChangedCommand()
        {
            RefreshHoldingPosition();
        }

        protected override void RefreshHoldingPosition()
        {
            // Set latest position information. Set 0 for amount by default.
            if (null != this.SecurityInfo)
            {
                var currentPositions = _tradeInfo.PositionInfoCollection.FuturePositionInfoList.Where(x =>
                    x.MarketType == this.SecurityInfo.MarketType
                    && x.SecurityID == this.SecurityInfo.SecurityID
                    && x.CombiNo == this.SelectedCombineNumber);

                var longPosition = currentPositions.FirstOrDefault(x => x.PositionFlag == ePositionFlag.多头持仓);
                var shortPosition = currentPositions.FirstOrDefault(x => x.PositionFlag == ePositionFlag.空头持仓);

                if (null != longPosition)
                {
                    this.HoldingLongPosition = longPosition;
                }
                else
                {
                    this.HoldingLongPosition = new PositionInfoBase();
                }

                if (null != shortPosition)
                {
                    this.HoldingShortPosition = shortPosition;
                }
                else
                {
                    this.HoldingShortPosition = new PositionInfoBase();
                }
            }
        }

        protected override void OnFastBuyCommand(double? price)
        {
            if (string.IsNullOrEmpty(this.SelectedCombineNumber))
            {
                _dialogService.ShowMessage("下单失败", "快速下单组合编号不能为空");
                return;
            }
            if (0 == this.EntrustAmount)
            {
                _dialogService.ShowMessage("下单失败", "快速下单委托数量不能为0");
                return;
            }

            if (null != price && 0 != price.Value)
            {
                // Set initial event parameter.
                Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType> inputParameter = null;

                // 买时，有空头持仓，先买平
                if (this.HoldingShortPosition.EnableAmount > 0)
                {
                    // 下单数量大于空头持仓数时，下单数量自动被赋成持仓数
                    if (this.EntrustAmount > this.HoldingShortPosition.EnableAmount)
                    {
                        this.EntrustAmount = this.HoldingShortPosition.EnableAmount;
                    }

                    inputParameter = new Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>(
                       this.SecurityInfo.MarketType,
                       this.SecurityInfo.SecurityID,
                       price.Value,
                       this.SelectedCombineNumber,
                       this.EntrustAmount,
                       eFuturesDirection.平仓,
                       this.SelectedInvestType);
                }
                // 买时，无空头持仓，直接买开
                else
                {
                    inputParameter = new Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>(
                       this.SecurityInfo.MarketType,
                       this.SecurityInfo.SecurityID,
                       price.Value,
                       this.SelectedCombineNumber,
                       this.EntrustAmount,
                       eFuturesDirection.开仓,
                       this.SelectedInvestType);
                }

                _eventAggregator.GetEvent<FastBuyFutureEvent>().Publish(inputParameter);
            }
        }

        protected override void OnFastSellCommand(double? price)
        {
            if (string.IsNullOrEmpty(this.SelectedCombineNumber))
            {
                _dialogService.ShowMessage("下单失败", "快速下单组合编号不能为空");
                return;
            }
            if (0 == this.EntrustAmount)
            {
                _dialogService.ShowMessage("下单失败", "快速下单委托数量不能为0");
                return;
            }

            if (null != price && 0 != price.Value)
            {
                // Set initial event parameter.
                Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType> inputParameter = null;

                // 卖时，有多头持仓，先卖平
                if (this.HoldingLongPosition.EnableAmount > 0)
                {
                    // 下单数量大于多头持仓数时，下单数量自动被赋成持仓数
                    if (this.EntrustAmount > this.HoldingLongPosition.EnableAmount)
                    {
                        this.EntrustAmount = this.HoldingLongPosition.EnableAmount;
                    }

                    inputParameter = new Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>(
                       this.SecurityInfo.MarketType,
                       this.SecurityInfo.SecurityID,
                       price.Value,
                       this.SelectedCombineNumber,
                       this.EntrustAmount,
                       eFuturesDirection.平仓,
                       this.SelectedInvestType);
                }
                // 卖时，无多头持仓，直接卖开
                else
                {
                    inputParameter = new Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>(
                       this.SecurityInfo.MarketType,
                       this.SecurityInfo.SecurityID,
                       price.Value,
                       this.SelectedCombineNumber,
                       this.EntrustAmount,
                       eFuturesDirection.开仓,
                       this.SelectedInvestType);
                }

                _eventAggregator.GetEvent<FastSellFutureEvent>().Publish(inputParameter);
            }
        }
    }
}

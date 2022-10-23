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

namespace TradeStation.Stock.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockRealTimePriceViewModel : RealTimePriceViewModelBase
    {
        #region Properties

        private PositionInfoBase _holdingPosition;
        public PositionInfoBase HoldingPosition
        {
            get { return _holdingPosition; }
            set
            {
                SetProperty(ref _holdingPosition, value);
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
        public StockRealTimePriceViewModel(
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
            _eventAggregator.GetEvent<RelatedPositionItemChangedNotifyEvent>().Subscribe(this.OnReturnPositionInfo);

            // Set fast trade default amount.
            this.EntrustAmount = this.MenubarViewModel.UserSettings.FastTradeAmountStock;

            // Initialize the Holding with blank object.
            HoldingPosition = new PositionInfoBase();

            this.IsRealTimePrice = true;
            this.IsExpandedChart = true;
            this.KLinePeriodType = eDisplayedKLinePeriodType.日线;
        }

        protected void OnReturnPositionInfo(PositionInfo positionInfo)
        {
            if (positionInfo.MsgType == ePositionInfoMsgType.交易)
            {
                // Only set reference when the combine number is empty for holding position.
                // It means it does not have holding before when combine number is empty.
                if (string.IsNullOrEmpty(this.HoldingPosition.CombiNo)
                    && null != this.SecurityInfo
                    && positionInfo.MarketType == this.SecurityInfo.MarketType
                    && positionInfo.SecurityID == this.SecurityInfo.SecurityID
                    && positionInfo.CombiNo == this.SelectedCombineNumber)
                {
                    RefreshHoldingPosition();
                }
            }
        }

        protected override void OnCombineNoSelectionChangedCommand()
        {
            RefreshHoldingPosition();
        }

        protected override void RefreshHoldingPosition()
        {
            // Set latest position information. Set 0 for amount by default.
            var currentPosition = _tradeInfo.PositionInfoCollection.StockPositionInfoList.FirstOrDefault(x =>
                x.MarketType == this.SecurityInfo.MarketType
                && x.SecurityID == this.SecurityInfo.SecurityID
                && x.CombiNo == this.SelectedCombineNumber);

            if (null != currentPosition)
            {
                this.HoldingPosition = currentPosition;
            }
            else
            {
                this.HoldingPosition = new PositionInfoBase();
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
                var inputParameter = new Tuple<eMarketType, string, double, string, int, eEntrustDirection>(
                    this.SecurityInfo.MarketType,
                    this.SecurityInfo.SecurityID,
                    price.Value,
                    this.SelectedCombineNumber,
                    this.EntrustAmount,
                    this.SecurityInfo.SecurityType == eCategory.债券回购 ? eEntrustDirection.融券回购 : eEntrustDirection.买入);

                _eventAggregator.GetEvent<FastBuyStockEvent>().Publish(inputParameter);
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
                var inputParameter = new Tuple<eMarketType, string, double, string, int, eEntrustDirection>(
                    this.SecurityInfo.MarketType,
                    this.SecurityInfo.SecurityID,
                    price.Value,
                    this.SelectedCombineNumber,
                    this.EntrustAmount,
                    this.SecurityInfo.SecurityType == eCategory.债券回购 ? eEntrustDirection.融资回购 : eEntrustDirection.卖出);

                _eventAggregator.GetEvent<FastSellStockEvent>().Publish(inputParameter);
            }
        }
    }
}

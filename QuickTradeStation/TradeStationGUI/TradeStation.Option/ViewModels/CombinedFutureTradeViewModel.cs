using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.Trade.ViewModels;
using TradeStation.Option.Models;

namespace TradeStation.Option.ViewModels
{
    [Export]
    public class CombinedFutureTradeViewModel : BindableBase
    {
        #region Private Fields

        private SecurityInfoMetadata _securityInfoMetadata;
        private MarketDataService _marketDataService;
        private DialogService _dialogService;

        private HsStock _trader;

        private IEventAggregator _eventAggregator;

        #endregion

        #region Properties

        private ObservableCollection<DisplayOptionInfo> _displayOptions;
        public ObservableCollection<DisplayOptionInfo> DisplayOptions
        {
            get { return _displayOptions; }
            set
            {
                SetProperty(ref _displayOptions, value);
            }
        }

        private DisplayOptionInfo _selectedOptionItem;
        public DisplayOptionInfo SelectedOptionItem
        {
            get { return _selectedOptionItem; }
            set
            {
                SetProperty(ref _selectedOptionItem, value);
            }
        }

        private IList<string> _optionMarkets;
        public IList<string> OptionMarkets
        {
            get { return _optionMarkets; }
            set
            {
                SetProperty(ref _optionMarkets, value);
            }
        }

        private string _selectedOptionMarket;
        public string SelectedOptionMarket
        {
            get { return _selectedOptionMarket; }
            set
            {
                SetProperty(ref _selectedOptionMarket, value);
            }
        }

        private IList<string> _underlyingSecurityNames;
        public IList<string> UnderlyingSecurityNames
        {
            get { return _underlyingSecurityNames; }
            set
            {
                SetProperty(ref _underlyingSecurityNames, value);
            }
        }

        private string _selectedUnderlyingSecurity;
        public string SelectedUnderlyingSecurity
        {
            get { return _selectedUnderlyingSecurity; }
            set
            {
                SetProperty(ref _selectedUnderlyingSecurity, value);
            }
        }

        private IList<long> _deliveryMonthList;
        public IList<long> DeliveryMonthList
        {
            get { return _deliveryMonthList; }
            set
            {
                SetProperty(ref _deliveryMonthList, value);
            }
        }

        private long _deliveryMonth;
        public long DeliveryMonth
        {
            get { return _deliveryMonth; }
            set
            {
                SetProperty(ref _deliveryMonth, value);
            }
        }

        private MenubarViewModel _menubar;
        public MenubarViewModel Menubar
        {
            get { return _menubar; }
            set
            {
                SetProperty(ref _menubar, value);
            }
        }

        private IList<string> _combinedOptionEntrustPriceTypes;
        public IList<string> CombinedOptionEntrustPriceTypes
        {
            get { return _combinedOptionEntrustPriceTypes; }
            set
            {
                SetProperty(ref _combinedOptionEntrustPriceTypes, value);
            }
        }

        private string _selectedBuyCombineNo;
        public string SelectedBuyCombineNo
        {
            get { return _selectedBuyCombineNo; }
            set
            {
                SetProperty(ref _selectedBuyCombineNo, value);
            }
        }

        private string _selectedSellCombineNo;
        public string SelectedSellCombineNo
        {
            get { return _selectedSellCombineNo; }
            set
            {
                SetProperty(ref _selectedSellCombineNo, value);
            }
        }

        private double _selectedExercisePrice;
        public double SelectedExerciesPrice
        {
            get { return _selectedExercisePrice; }
            set
            {
                SetProperty(ref _selectedExercisePrice, value);
            }
        }

        private string _selectedCombinedOptiontPriceType;
        public string SelectedCombinedOptiontPriceType
        {
            get { return _selectedCombinedOptiontPriceType; }
            set
            {
                SetProperty(ref _selectedCombinedOptiontPriceType, value);
            }
        }

        private eBasketTradeMethod _selectedBasketTradeMethod;
        public eBasketTradeMethod SelectedBasketTradeMethod
        {
            get { return _selectedBasketTradeMethod; }
            set
            {
                SetProperty(ref _selectedBasketTradeMethod, value);
            }
        }

        private int _entrustAmount;
        public int EntrustAmount
        {
            get { return _entrustAmount; }
            set
            {
                SetProperty(ref _entrustAmount, value);
            }
        }

        private int _entrustTickCount;
        public int EntrustTickCount
        {
            get { return _entrustTickCount; }
            set
            {
                SetProperty(ref _entrustTickCount, value);
            }
        }

        #endregion

        #region Commands

        public ICommand OptionMarketChangedCommand { get; set; }
        public ICommand UnderlyingSecurityChangedCommand { get; set; }
        public ICommand SelectedDeliveryMonthChangedCommand { get; set; }
        public ICommand SelectedOptionItemChangedCommand { get; set; }

        public ICommand CombinedFutureBuyCommand { get; set; }
        public ICommand CombinedFutureSellCommand { get; set; }

        #endregion

        [ImportingConstructor]
        public CombinedFutureTradeViewModel(SecurityInfoMetadata securityInfoMetadata,
            MarketDataService marketDataService,
            DialogService dialogService,
            HsStock trader,
            MenubarViewModel menubarViewModel,
            IEventAggregator eventAggregator)
        {
            _securityInfoMetadata = securityInfoMetadata;
            _marketDataService = marketDataService;
            _dialogService = dialogService;
            _trader = trader;
            _eventAggregator = eventAggregator;

            Menubar = menubarViewModel;
            EntrustAmount = 1;
            EntrustTickCount = 0;

            CombinedOptionEntrustPriceTypes = new List<string>();
            CombinedOptionEntrustPriceTypes.Add("限价");
            SelectedCombinedOptiontPriceType = CombinedOptionEntrustPriceTypes.First();

            DisplayOptions = new ObservableCollection<DisplayOptionInfo>();

            OptionMarketChangedCommand = new DelegateCommand<object>(this.OnOptionMarketChangedCommand);
            UnderlyingSecurityChangedCommand = new DelegateCommand<object>(this.OnUnderlyingSecurityChangedCommand);
            SelectedDeliveryMonthChangedCommand = new DelegateCommand<object>(this.OnSelectedDeliveryMonthChangedCommand);
            SelectedOptionItemChangedCommand = new DelegateCommand<object>(this.OnSelectedOptionItemChangedCommand);

            CombinedFutureBuyCommand = new DelegateCommand<object>(this.OnCombinedFutureBuyCommand);
            CombinedFutureSellCommand = new DelegateCommand<object>(this.OnCombinedFutureSellCommand);

            _eventAggregator.GetEvent<OptionInfoReadyEvent>().Subscribe(this.OnOptionInfoReadyEvent);
            _eventAggregator.GetEvent<EntrustDealCallBackEvent>().Subscribe(this.OnEntrustDealCallBackEvent);
        }

        private void OnOptionInfoReadyEvent(string payload)
        {
            if (null != OptionMarkets)
            {
                OptionMarkets.Clear();
            }

            OptionMarkets = _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Select(x => x.ExID).Distinct().ToList();

            if (null != OptionMarkets && OptionMarkets.Any())
            {
                SelectedOptionMarket = OptionMarkets.First();

                OnOptionMarketChangedCommand(null);
            }

            // Initialize the selected combine number.
            if (null != this.Menubar.CombiNos
                && this.Menubar.CombiNos.Count > 0)
            {
                this.SelectedBuyCombineNo = this.Menubar.CombiNos.First();
                this.SelectedSellCombineNo = this.Menubar.CombiNos.First();
            }
        }

        private void OnEntrustDealCallBackEvent(EntrustInfo ei)
        {
            Entrust pendingEntrust;
            var remainingAmount = ei.EntrustAmount - ei.TotalDealAmount;

            if (_trader.PendingCombinedRequest.TryGetValue(ei.ExtsystemId, out pendingEntrust) && pendingEntrust.EntrustAmount > remainingAmount)
            {
                pendingEntrust.EntrustAmount = ei.DealAmount;
                pendingEntrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;

                _trader.InsertOrder(pendingEntrust);

                if (remainingAmount > 0)
                {
                    pendingEntrust.EntrustAmount = remainingAmount;
                }
                else
                {
                    _trader.PendingCombinedRequest.Remove(ei.ExtsystemId);
                }
            }
        }

        private void OnOptionMarketChangedCommand(object payload)
        {
            if (null != UnderlyingSecurityNames)
            {
                UnderlyingSecurityNames.Clear();
            }

            var filteredOptionList =
                _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Where(x => x.ExID == SelectedOptionMarket);

            if (null != filteredOptionList && filteredOptionList.Any())
            {
                UnderlyingSecurityNames = filteredOptionList.Select(x => x.UnderlyingSymbol).Distinct().ToList();
                SelectedUnderlyingSecurity = UnderlyingSecurityNames.First();

                OnUnderlyingSecurityChangedCommand(null);
            }
        }

        private void OnUnderlyingSecurityChangedCommand(object payload)
        {
            if (null != DeliveryMonthList)
            {
                DeliveryMonthList.Clear();
            }

            var filteredOptionList =
                _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Where(x => x.ExID == SelectedOptionMarket && x.UnderlyingSymbol == SelectedUnderlyingSecurity);

            if (null != filteredOptionList && filteredOptionList.Any())
            {
                DeliveryMonthList = filteredOptionList.Select(x => DateTimeHelper.ConvertToDateTimeInt(x.DeliveryDate) / 1000000)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                DeliveryMonth = DeliveryMonthList.First();

                OnSelectedDeliveryMonthChangedCommand(null);
            }
        }

        private void OnSelectedDeliveryMonthChangedCommand(object payload)
        {
            // Initializes the option list.
            if (null != DisplayOptions)
            {
                DisplayOptions.Clear();
            }

            // Filters options by the condition selection from option information list.
            var filteredOptionList =
                _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Where(
                x => x.ExID == SelectedOptionMarket
                    && x.UnderlyingSymbol == SelectedUnderlyingSecurity
                    && x.DeliveryDate.Year == DeliveryMonth / 100
                    && x.DeliveryDate.Month == DeliveryMonth % 100);

            if (null != filteredOptionList && filteredOptionList.Any())
            {
                // 订阅所有期权行情
                foreach (var optionInfo in filteredOptionList)
                {
                    _marketDataService.SubscribeSecQuot(new ExSecID(optionInfo.ExID, optionInfo.SecurityID));
                }

                // Groups the filtered options by exercise price.
                var correctOptionExPriceGroups = filteredOptionList
                    .OrderBy(x => x.ExercisePrice + x.OptionExRightSymbol)
                    .GroupBy(x => x.ExercisePrice.ToString() + x.OptionExRightSymbol);

                foreach (var group in correctOptionExPriceGroups)
                {
                    var callOption = group.FirstOrDefault(x => x.CallOrPut == eOptionType.认购期权);
                    var putOption = group.FirstOrDefault(x => x.CallOrPut == eOptionType.认沽期权);

                    var optionInfo = new DisplayOptionInfo()
                    {
                        ExercisePrice = callOption.ExercisePrice,
                        OptionExRightSymbol = callOption.OptionExRightSymbol,
                    };

                    // Gets the security information from security metadata.
                    // And set them into view model.
                    if (null != callOption)
                    {
                        var callOptionSecurityInfo =
                            _securityInfoMetadata.GetSecurityList().FirstOrDefault(x => x.SecurityID == callOption.SecurityID);

                        if (null != callOptionSecurityInfo)
                        {
                            optionInfo.CallOptionSecurityInfo = callOptionSecurityInfo;
                        }
                    }

                    if (null != putOption)
                    {
                        var putOptionSecurityInfo =
                            _securityInfoMetadata.GetSecurityList().FirstOrDefault(x => x.SecurityID == putOption.SecurityID);

                        if (null != putOptionSecurityInfo)
                        {
                            optionInfo.PutOptionSecurityInfo = putOptionSecurityInfo;
                        }
                    }

                    DisplayOptions.Add(optionInfo);
                }
            }
        }

        private void OnSelectedOptionItemChangedCommand(object payload)
        {
            //SelectedExerciesPrice = this.SelectedOptionItem.ExercisePrice;
        }

        private void OnCombinedFutureBuyCommand(object payload)
        {
            if (null != this.SelectedOptionItem)
            {
                SendOptionBasketEntrust(this.SelectedOptionItem.CallOptionSecurityInfo, this.SelectedOptionItem.PutOptionSecurityInfo);
            }
        }

        private void OnCombinedFutureSellCommand(object payload)
        {
            if (null != this.SelectedOptionItem)
            {
                SendOptionBasketEntrust(this.SelectedOptionItem.PutOptionSecurityInfo, this.SelectedOptionItem.CallOptionSecurityInfo);
            }
        }

        private void SendOptionBasketEntrust(SecurityInfo buyOption, SecurityInfo sellOption)
        {
            // TODO: Check if does it need confirmation here.

            if (buyOption.Quotation.AskPx1 == 0 || sellOption.Quotation.BidPx1 == 0)
            {
                _dialogService.ShowMessage("下单失败", "合成期货委托价不能为0");
                return;
            }

            var buyOptionEntrust = new OptionEntrust();

            buyOptionEntrust.StockCode = buyOption.SecurityID;
            buyOptionEntrust.MarketNo = CommonUtil.eMarketTypeToeMarketNo(buyOption.MarketType);
            buyOptionEntrust.CombiNo = this.SelectedBuyCombineNo;
            buyOptionEntrust.EntrustDirection = CommonUtil.eEntrustDirectionToEntrustDirection(eEntrustDirection.买入);
            buyOptionEntrust.FuturesDirection = CommonUtil.eFuturesDirectionToFuturesDirection(eFuturesDirection.开仓);
            buyOptionEntrust.PriceType = CommonUtil.eEntrustPriceTypeToEntrustPriceType(eEntrustPriceType.限价);
            buyOptionEntrust.EntrustPrice = buyOption.Quotation.AskPx1 + EntrustTickCount * buyOption.MinFloatingPrice;
            buyOptionEntrust.EntrustAmount = this.EntrustAmount;
            buyOptionEntrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;

            var sellOptionEntrust = new OptionEntrust();
            var adjustedEntrustPrice = sellOption.Quotation.BidPx1 - EntrustTickCount * sellOption.MinFloatingPrice;

            sellOptionEntrust.StockCode = sellOption.SecurityID;
            sellOptionEntrust.MarketNo = CommonUtil.eMarketTypeToeMarketNo(sellOption.MarketType);
            sellOptionEntrust.CombiNo = this.SelectedSellCombineNo;
            sellOptionEntrust.EntrustDirection = CommonUtil.eEntrustDirectionToEntrustDirection(eEntrustDirection.卖出);
            sellOptionEntrust.FuturesDirection = CommonUtil.eFuturesDirectionToFuturesDirection(eFuturesDirection.开仓);
            sellOptionEntrust.PriceType = CommonUtil.eEntrustPriceTypeToEntrustPriceType(eEntrustPriceType.限价);
            sellOptionEntrust.EntrustPrice = adjustedEntrustPrice > 0 ? adjustedEntrustPrice : sellOption.MinFloatingPrice;
            sellOptionEntrust.EntrustAmount = this.EntrustAmount;
            sellOptionEntrust.ExtsystemId = CommonUtil.CurrentExtSystemId++;

            if (this.SelectedBasketTradeMethod == eBasketTradeMethod.同时买卖)
            {
                var listEntrust = new List<Entrust>();
                listEntrust.Add(buyOptionEntrust);
                listEntrust.Add(sellOptionEntrust);

                if (listEntrust.Count > 0)
                {
                    _trader.InsertOrderOptionBasket(listEntrust);
                }
            }
            else
            {
                OptionEntrust sendingEntrust = null;
                OptionEntrust pendingEntrust = null;

                if (this.SelectedBasketTradeMethod == eBasketTradeMethod.先买后卖)
                {
                    sendingEntrust = buyOptionEntrust;
                    pendingEntrust = sellOptionEntrust;
                }
                else
                {
                    sendingEntrust = sellOptionEntrust;
                    pendingEntrust = buyOptionEntrust;
                }

                _trader.PendingCombinedRequest.Add(sendingEntrust.ExtsystemId.ToString(), pendingEntrust);

                _trader.InsertOrder(sendingEntrust);
            }
        }
    }
}

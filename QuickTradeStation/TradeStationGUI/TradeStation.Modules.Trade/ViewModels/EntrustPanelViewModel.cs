using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;

using TradeStation.Infrastructure;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;
using TradeStation.Modules.RealTimePrice.DataProviders;
using TradeStation.Modules.Trade.Views;

namespace TradeStation.Modules.Trade.ViewModels
{
    public abstract class EntrustPanelViewModelBase : TradeViewModelBase, IReInitializable
    {
        [ImportingConstructor]
        public EntrustPanelViewModelBase(IEventAggregator eventAggr,
            SecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            InitCommands();
            SubscribeEvents();
        }

        private eChaseOrderType _chaseOrderType;
        public eChaseOrderType ChaseOrderType
        {
            get { return _chaseOrderType; }
            set
            {
                SetProperty(ref _chaseOrderType, value);
            }
        }

        private EntrustInfo _selectedEntrustInfo;
        public EntrustInfo SelectedEntrustInfo
        {
            get { return _selectedEntrustInfo; }
            set
            {
                SetProperty(ref _selectedEntrustInfo, value);
            }
        }

        private string _selectedInstrumentId;

        public string SelectedInstrumentId
        {
            get { return _selectedInstrumentId; }
            set { SetProperty(ref _selectedInstrumentId, value); }
        }

        private string _selectedCancelCombiNo;

        public string SelectedCancelCombiNo
        {
            get { return _selectedCancelCombiNo; }
            set { SetProperty(ref _selectedCancelCombiNo, value); }
        }

        public ICommand CancelEntrustCommand { get; private set; }
        public ICommand ChaseOrderCommand { get; private set; }
        protected override void InitCommands()
        {
            CancelEntrustCommand = new DelegateCommand(OnCancelEntrust);
            CancelAllEntrustsCommand = new DelegateCommand(OnCancelAllEntrusts);
            CancelEntrustsByCombiNoCommand = new DelegateCommand(OnCancelEntrustsByCombiNo);
            CancelEntrustsByInstrumentIdCommand = new DelegateCommand(OnCancelEntrustsByInstrumentId);
            AdvancedQueryCommand = new DelegateCommand(OnAdvancedQuery);
            ChaseOrderCommand = new DelegateCommand(OnChaseOrder);
        }

        protected abstract void OnChaseOrder();
        protected abstract void OnCancelEntrust();
        protected abstract void OnCancelAllEntrusts();
        protected abstract void OnCancelEntrustsByCombiNo();
        protected abstract void OnCancelEntrustsByInstrumentId();
        public ICommand CancelAllEntrustsCommand { get; set; }
        public ICommand CancelEntrustsByCombiNoCommand { get; set; }

        public ICommand CancelEntrustsByInstrumentIdCommand { get; set; }
        protected bool IsCancellable(eEntrustState state)
        {
            if (state == eEntrustState.部成 || state == eEntrustState.待报 || state == eEntrustState.未报 || state == eEntrustState.已报 || state == eEntrustState.正报)
            {
                return true;
            }

            return false;
        }

        protected void CancelEntrust(eCategory category = eCategory.股票)
        {
            try
            {
                if (_selectedEntrustInfo != null && IsCancellable(_selectedEntrustInfo.EntrustState))
                {
                    if (MessageBox.Show("确定要撤单吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Trader.CancelOrder(Convert.ToInt32(_selectedEntrustInfo.EntrustNo), category);
                    }
                }
            }
            catch (System.Exception ex)
            {
                CommonUtil.LogException(Trader.Logger, ex);
            }
        }

        protected void CancelAllEntrusts(eCategory category = eCategory.股票)
        {
            try
            {
                if (MessageBox.Show("确定要全部撤单吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    switch (category)
                    {
                        case eCategory.股票:
                            {
                                foreach (EntrustInfo ei in Trader.EntrustInfoCollection.StockEntrustInfoList)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                        case eCategory.期货:
                            {
                                foreach (EntrustInfo ei in Trader.EntrustInfoCollection.FutureEntrustInfoList)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                        case eCategory.期权:
                            {
                                foreach (EntrustInfo ei in Trader.EntrustInfoCollection.OptionEntrustInfoList)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                    }

                }

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Trader.Logger, ex);
            }
        }

        protected void CancelEntrustsByCombiNo(eCategory category = eCategory.股票)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedCancelCombiNo))
                {
                    return;
                }

                if (MessageBox.Show(string.Format("确定要撤掉组合{0}的全部委托吗？",SelectedCancelCombiNo), "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    switch (category)
                    {
                        case eCategory.股票:
                        {
                            var filteredByCombiNo = from ei in Trader.EntrustInfoCollection.StockEntrustInfoList
                                where ei.CombiNo.Equals(SelectedCancelCombiNo)
                                select ei;
                            foreach (EntrustInfo ei in filteredByCombiNo)
                            {
                                if (IsCancellable(ei.EntrustState))
                                {
                                    Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                }
                            }
                            break;
                        }
                        case eCategory.期货:
                        {
                            var filteredByCombiNo = from ei in Trader.EntrustInfoCollection.FutureEntrustInfoList
                                where ei.CombiNo.Equals(SelectedCancelCombiNo)
                                select ei;

                            foreach (EntrustInfo ei in filteredByCombiNo)
                            {
                                if (IsCancellable(ei.EntrustState))
                                {
                                    Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                }
                            }
                            break;
                        }
                        case eCategory.期权:
                        {
                            var filteredByCombiNo = from ei in Trader.EntrustInfoCollection.OptionEntrustInfoList
                                where ei.CombiNo.Equals(SelectedCancelCombiNo)
                                select ei;

                            foreach (EntrustInfo ei in filteredByCombiNo)
                            {
                                if (IsCancellable(ei.EntrustState))
                                {
                                    Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                }
                            }
                            break;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Trader.Logger, ex);
            }
        }

        protected void CancelEntrustsByInstrumentId(eCategory category = eCategory.股票)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedInstrumentId))
                {
                    return;
                }

                if (MessageBox.Show(string.Format("确定要撤掉合约{0}的全部委托吗？", SelectedInstrumentId), "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    switch (category)
                    {
                        case eCategory.股票:
                            {
                                var filteredByInstrumentId = from ei in Trader.EntrustInfoCollection.StockEntrustInfoList
                                                        where ei.SecurityID.Equals(SelectedInstrumentId)
                                                        select ei;
                                foreach (EntrustInfo ei in filteredByInstrumentId)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                        case eCategory.期货:
                            {
                                var filteredByInstrumentId = from ei in Trader.EntrustInfoCollection.FutureEntrustInfoList
                                                             where ei.SecurityID.Equals(SelectedInstrumentId)
                                                             select ei;
                                foreach (EntrustInfo ei in filteredByInstrumentId)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                        case eCategory.期权:
                            {
                                var filteredByInstrumentId = from ei in Trader.EntrustInfoCollection.OptionEntrustInfoList
                                                             where ei.SecurityID.Equals(SelectedInstrumentId)
                                                             select ei;
                                foreach (EntrustInfo ei in filteredByInstrumentId)
                                {
                                    if (IsCancellable(ei.EntrustState))
                                    {
                                        Trader.CancelOrder(Convert.ToInt32(ei.EntrustNo), category);
                                    }
                                }
                                break;
                            }
                    }

                }

            }
            catch (Exception ex)
            {
                CommonUtil.LogException(Trader.Logger, ex);
            }
        }
        private bool _boolShowTraded;
        public bool BoolShowTraded
        {
            get { return _boolShowTraded; }
            set
            {
                SetProperty(ref _boolShowTraded, value);
            }
        }

        private bool _boolShowNotTraded;
        public bool BoolShowNotTraded
        {
            get { return _boolShowNotTraded; }
            set
            {
                SetProperty(ref _boolShowNotTraded, value);
            }
        }

        private bool _boolShowCancelled;
        public bool BoolShowCancelled
        {
            get { return _boolShowCancelled; }
            set
            {
                SetProperty(ref _boolShowCancelled, value);
            }
        }

        public void DailyReInitialize()
        {
            DispatcherService.Invoke(() =>
            {
                OnRefresh();
            });
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelViewModel : EntrustPanelViewModelBase
    {
        [ImportingConstructor]
        public EntrustPanelViewModel(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<EntrustInfoNotifyEvent>().Subscribe(OnReturnStockEntrustState);
        }

        private void OnReturnStockEntrustState(EntrustInfo entrustInfo)
        {
            Trader.ReturnEntrustInfo(Trader.EntrustInfoCollection.StockEntrustInfoList, entrustInfo);
            Trader.EntrustInfoCollection.StockNameList =
                (from ei in Trader.EntrustInfoCollection.StockEntrustInfoList
                    where IsCancellable(ei.EntrustState)
                    select ei.SecurityID).Distinct().ToList();
        }

        protected override void OnChaseOrder()
        {
            CommonUtil.ChaseOrder(Trader.EntrustInfoCollection.StockEntrustInfoList,
                Trader.PendingCombinedRequest.Keys.ToList(),
                ChaseOrderType,
                Trader);
        }

        protected override void OnCancelEntrust()
        {
            CancelEntrust();
        }

        protected override void OnCancelAllEntrusts()
        {
            CancelAllEntrusts();
        }

        protected override void OnCancelEntrustsByCombiNo()
        {
            CancelEntrustsByCombiNo();
        }

        protected override void OnCancelEntrustsByInstrumentId()
        {
            CancelEntrustsByInstrumentId();
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.EntrustInfoCollection.StockEntrustInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryEntrust(combiNo);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出股票委托列表...", Trader.EntrustInfoCollection.StockEntrustInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelFundViewModel : EntrustPanelViewModelBase
    {
        [ImportingConstructor]
        public EntrustPanelFundViewModel(IEventAggregator eventAggr,
            FundSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FundEntrustInfoNotifyEvent>().Subscribe(OnReturnFundEntrustState);
        }

        private void OnReturnFundEntrustState(FundEntrustInfo entrustInfo)
        {
            Trader.ReturnEntrustInfo(Trader.EntrustInfoCollection.FundEntrustInfoList, entrustInfo);
        }

        protected override void OnChaseOrder()
        {
            throw new NotImplementedException();
        }

        protected override void OnCancelEntrust()
        {
            CancelEntrust();
        }

        protected override void OnCancelAllEntrusts()
        {
            CancelAllEntrusts();
        }

        protected override void OnCancelEntrustsByCombiNo()
        {
            CancelEntrustsByCombiNo();
        }

        protected override void OnCancelEntrustsByInstrumentId()
        {
            throw new NotImplementedException();
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.EntrustInfoCollection.FundEntrustInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryEntrust(combiNo, eCategory.基金分拆合并);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出基金委托列表...", Trader.EntrustInfoCollection.FundEntrustInfoList);
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelViewModelFuture : EntrustPanelViewModelBase
    {
        [ImportingConstructor]
        public EntrustPanelViewModelFuture(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<FutureEntrustInfoNotifyEvent>().Subscribe(OnReturnFutureEntrustState);
        }

        protected void OnReturnFutureEntrustState(FutureEntrustInfo futureEntrustInfo)
        {
            Trader.ReturnEntrustInfo(Trader.EntrustInfoCollection.FutureEntrustInfoList, futureEntrustInfo);
            Trader.EntrustInfoCollection.FutureNameList =
                (from ei in Trader.EntrustInfoCollection.FutureEntrustInfoList
                 where IsCancellable(ei.EntrustState)
                 select ei.SecurityID).Distinct().ToList();
        }

        protected override void OnChaseOrder()
        {
            CommonUtil.ChaseOrder(Trader.EntrustInfoCollection.FutureEntrustInfoList,
                Trader.PendingCombinedRequest.Keys.ToList(),
                ChaseOrderType,
                Trader);
        }

        protected override void OnCancelEntrust()
        {
            CancelEntrust(eCategory.期货);
        }
        protected override void OnCancelAllEntrusts()
        {
            CancelAllEntrusts(eCategory.期货);
        }

        protected override void OnCancelEntrustsByCombiNo()
        {
            CancelEntrustsByCombiNo(eCategory.期货);
        }

        protected override void OnCancelEntrustsByInstrumentId()
        {
            CancelEntrustsByInstrumentId(eCategory.期货);
        }

        protected override void OnAdvancedQuery()
        {
            throw new NotImplementedException();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.EntrustInfoCollection.FutureEntrustInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (string combiNo in MenuBar.CombiNos)
                {
                    Trader.QryEntrust(combiNo, eCategory.期货);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期货委托列表...", Trader.EntrustInfoCollection.FutureEntrustInfoList);
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelViewModelOption : EntrustPanelViewModelBase
    {
        [ImportingConstructor]
        public EntrustPanelViewModelOption(IEventAggregator eventAggr,
            OptionSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
        }
        protected override void SubscribeEvents()
        {
            EventAggregator.GetEvent<OptionEntrustInfoNotifyEvent>().Subscribe(OnReturnOptionEntrustState);
        }

        private void OnReturnOptionEntrustState(OptionEntrustInfo optionEntrustInfo)
        {
            Trader.ReturnEntrustInfo(Trader.EntrustInfoCollection.OptionEntrustInfoList, optionEntrustInfo);
            Trader.EntrustInfoCollection.OptionNameList =
                (from ei in Trader.EntrustInfoCollection.OptionEntrustInfoList
                 where IsCancellable(ei.EntrustState)
                 select ei.SecurityID).Distinct().ToList();
        }

        protected override void OnChaseOrder()
        {
            CommonUtil.ChaseOrder(Trader.EntrustInfoCollection.OptionEntrustInfoList,
                Trader.PendingCombinedRequest.Keys.ToList(),
                ChaseOrderType,
                Trader);
        }

        protected override void OnCancelEntrust()
        {
            CancelEntrust(eCategory.期权);
        }
        protected override void OnCancelAllEntrusts()
        {
            CancelAllEntrusts(eCategory.期权);
        }

        protected override void OnCancelEntrustsByCombiNo()
        {
            CancelEntrustsByCombiNo(eCategory.期权);
        }

        protected override void OnCancelEntrustsByInstrumentId()
        {
            CancelEntrustsByInstrumentId(eCategory.期权);
        }

        protected override void OnAdvancedQuery()
        {
            var win = ServiceLocator.Current.GetInstance<AdvancedQueryPanelOption>();
            win.Show();
        }

        protected override void OnRefresh()
        {
            Logger.Debug("OnRefresh");

            Trader.EntrustInfoCollection.OptionEntrustInfoList.Clear();

            DispatcherService.Invoke(() =>
            {
                foreach (var combiNo in MenuBar.CombiNos)
                {
                    Trader.QryEntrust(combiNo, eCategory.期权);
                }
            });
        }

        protected override void OnExportList()
        {
            CommonUtil.ExportToCsv("导出期权委托列表...", Trader.EntrustInfoCollection.OptionEntrustInfoList);
        }
    }

    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelViewModelForBasket : EntrustPanelViewModel
    {
        [ImportingConstructor]
        public EntrustPanelViewModelForBasket(IEventAggregator eventAggr,
            StockSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
            SubscribeEvents();
        }

        protected override void SubscribeEvents()
        { }

        protected override void OnRefresh()
        {
            base.OnRefresh();
        }
    }


    [Export]
    [Export(typeof(IReInitializable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EntrustPanelViewModelFutureForBasket : EntrustPanelViewModelFuture
    {
        [ImportingConstructor]
        public EntrustPanelViewModelFutureForBasket(IEventAggregator eventAggr,
            FutureSecurityCodeSearchProvider securitySearchProvider)
            : base(eventAggr, securitySearchProvider)
        {
            RefreshCommand = new DelegateCommand(OnRefresh);
            SubscribeEvents();
        }
        protected override void SubscribeEvents()
        { }

        protected override void OnRefresh()
        {
            base.OnRefresh();
        }
    }
}

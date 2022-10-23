using System;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Events
{
    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Entrust Direction
    public class FastBuyStockEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eEntrustDirection>>
    {

    }

    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Entrust Direction
    public class FastSellStockEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eEntrustDirection>>
    {

    }

    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Futures Direction
    // Item7: Invest Type
    public class FastBuyFutureEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>>
    {
    }

    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Futures Direction
    // Item7: Invest Type
    public class FastSellFutureEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eFuturesDirection, eInvestType>>
    {

    }

    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Futures Direction
    public class FastBuyOptionEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eFuturesDirection>>
    {
    }

    // Item1: MarketType
    // Item2: SecurityID
    // Item3: Price
    // Item4: Combine No
    // Item5: Entrust Amount
    // Item6: Futures Direction
    public class FastSellOptionEvent : PubSubEvent<Tuple<eMarketType, string, double, string, int, eFuturesDirection>>
    {

    }

}

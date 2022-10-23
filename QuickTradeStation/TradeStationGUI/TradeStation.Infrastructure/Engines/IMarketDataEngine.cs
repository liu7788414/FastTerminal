using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Engines
{
    public interface IMarketDataEngine
    {
        void RunEngine();
        void CloseEngine();

        void SubscribeSecurity(string exID, string securityID, eCategory securityType);
        void UnSubscribeSecurity(string exID, string securityID, eCategory securityType);
    }
}

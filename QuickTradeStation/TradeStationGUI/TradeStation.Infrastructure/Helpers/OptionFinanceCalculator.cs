using System;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Helpers
{
    [Export]
    public class OptionFinanceCalculator
    {
        private SecurityInfoMetadata _securityInfoMetadata;
        private MarketDataService _marketDataService;
        private IEventAggregator _eventAggregator;

        public double RFRate { get; set; }
        public const double ERROR = -1e40;

        [ImportingConstructor]
        public OptionFinanceCalculator(SecurityInfoMetadata securityInfoMetadata,
            MarketDataService marketDataService,
            IEventAggregator eventAggregator)
        {
            _securityInfoMetadata = securityInfoMetadata;
            _marketDataService = marketDataService;

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FinishedGetOptionInformationEvent>().Subscribe(this.OnFinishedGetOptionInformation);
        }

        public void OnFinishedGetOptionInformation(object payload)
        {
            Initialize();
        }

        public void Initialize()
        {
            InitRFRate();
        }

        private void InitRFRate()
        {
            RFRate = _securityInfoMetadata.RFRate;

            var underlyingSecurityIdList =
                _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.Select(x => x.UnderlyingSecurityId).Distinct();

            // Subcribe all underlying security.
            // Currently, all underlying securities are in SH exchange.
            foreach (var securityId in underlyingSecurityIdList)
            {
                _marketDataService.SubscribeSecQuot(new ExSecID(CommonUtil.上交所, securityId));
            }
        }

        // normal distribution function
        private double n(double z)
        {
            return (1.0 / Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-0.5 * z * z);
        }

        private double N(double z)
        {
            if (z >  6.0) { return 1.0; }; // this guards against overflow 
            if (z < -6.0) { return 0.0; };

            double b1 = 0.31938153;
            double b2 = -0.356563782;
            double b3 = 1.781477937;
            double b4 = -1.821255978;
            double b5 = 1.330274429;
            double p = 0.2316419;
            double c2 = 0.3989423;

            double a = Math.Abs(z);
            double t = 1.0 / (1.0 + a * p);
            double b = c2 * Math.Exp((-z) * (z / 2.0));
            double n = ((((b5 * t + b4) * t + b3) * t + b2) * t + b1) * t;
            n = 1.0 - b * n;
            if (z < 0.0) n = 1.0 - n;
            return n;
        }

        // Black-Scholes模型计算参数d1的数值
        // Param：
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     r: 无风险利率
        //     sigma: 波动率
        // Return: d1数值
        private double D1(double S, double K, double time, double r, double sigma)
        {
            return ((Math.Log(S / K) + (r + 0.5 * Math.Pow(sigma, 2.0)) * time) / (sigma * Math.Sqrt(time)));
        }

        // Black-Scholes模型计算参数d2的数值
        // Param：
        //     d1: Black-Scholes模型计算参数d1
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     sigma: 波动率
        // Return: d2数值
        private double D2(double d1, double time, double sigma)
        {
            return (d1 - sigma * Math.Sqrt(time));
        }

        // 利用Black-Scholes模型计算认购期权权利金数值
        // Param：
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     r: 无风险利率
        //     sigma: 波动率下限
        // Return: 认购期权权利金数值
        private double BScall(double S, double K, double time, double r, double sigma)
        {
            double d1 = D1(S, K, time, r, sigma);
            double d2 = D2(d1, time, sigma);
            return ((S * N(d1)) - (K * Math.Exp(-r * time) * N(d2)));
        }

        // 利用Black-Scholes模型计算认沽期权权利金数值
        // Param：
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     r: 无风险利率
        //     sigma: 波动率下限
        // Return: 认沽期权权利金数值
        private double BSput(double S, double K, double time, double r, double sigma)
        {
            double d1 = D1(S, K, time, r, sigma);
            double d2 = D2(d1, time, sigma);
            return ((K * Math.Exp(-r * time) * N(-d2)) - (S * N(-d1)));
        }

        // 计算期权隐含波动率
        // Param:
        //     optionData: 期权tick数据
        //     method: 计算波动率的算法
        // Return: 隐含波动率
        public void CalcImpliedVolatility(OptionDataModel optionData, ImpliedVolatilityMethod method)
        {
            // Get option info.
            var optionInfo = _securityInfoMetadata.OptionInfoModelCollection.OptionInfoList.FirstOrDefault(x => x.SecurityID == optionData.SecurityID);
            if (null == optionInfo)
            {
                return;
            }

            // Get underlying security quotation.
            var underlyingSecurity = new ExSecID(CommonUtil.上交所, optionInfo.UnderlyingSecurityId);
            if (!_marketDataService.SecurityQuotMap.ContainsKey(underlyingSecurity))
            {
                return;
            }

            var underlyingSecurityLastPrice = _marketDataService.SecurityQuotMap[underlyingSecurity].LastPx;
            var optionRestDayToExercise = (optionInfo.ExerciseDate - optionData.ExchangeTime.Date).TotalDays;
            var timeParameter = optionRestDayToExercise / 365;

            switch (method)
            {
                case ImpliedVolatilityMethod.Bisections:
                    for (int ix = 0; ix < 5; ix++)
                    {
                        optionData.BidPriceImpliedVolatility[ix] = BSImpV_bisections(optionInfo.CallOrPut,
                            underlyingSecurityLastPrice,
                            optionInfo.ExercisePrice,
                            RFRate,
                            timeParameter,
                            optionData.BidPrice[ix]);
                        optionData.AskPriceImpliedVolatility[ix] = BSImpV_bisections(optionInfo.CallOrPut,
                            underlyingSecurityLastPrice,
                            optionInfo.ExercisePrice,
                            RFRate,
                            timeParameter,
                            optionData.AskPrice[ix]);
                    }
                    break;
                case ImpliedVolatilityMethod.Newton:
                default:
                    for (int ix = 0; ix < 5; ix++)
                    {
                        optionData.BidPriceImpliedVolatility[ix] = BSImpV_newton(optionInfo.CallOrPut,
                            underlyingSecurityLastPrice,
                            optionInfo.ExercisePrice,
                            RFRate,
                            timeParameter,
                            optionData.BidPrice[ix]);
                        optionData.AskPriceImpliedVolatility[ix] = BSImpV_newton(optionInfo.CallOrPut,
                            underlyingSecurityLastPrice,
                            optionInfo.ExercisePrice,
                            RFRate,
                            timeParameter,
                            optionData.AskPrice[ix]);
                    }
                    break;
            }
        }

        // 计算期权隐含波动率(newton算法)
        // Param:
        //     type: 期权类别，认购或认沽
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     r: 无风险利率
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     option_price: 期权合约最新价
        // Return: 隐含波动率
        public double BSImpV_newton(eOptionType type, double S, double K, double r, double time, double option_price)
        {
            Func<double, double, double, double, double, double> pf;

            if (type == eOptionType.认购期权)
                pf = BScall;
            else if (type == eOptionType.认沽期权)
                pf = BSput;
            else
                return ERROR;

            const double ACCURACY = 1.0e-5;
            const int MAX_ITERATIONS = 10000;

            double t_sqrt = Math.Sqrt(time);
            double sigma = (option_price / S) / (0.398*t_sqrt);    // find initial value

            for (int i = 0; i<MAX_ITERATIONS; i++)
            {
                double price = pf(S, K, time, r, sigma);
                double diff = option_price - price;
                if (Math.Abs(diff) < ACCURACY)
                {
                    return sigma;
                }
                double d1 = D1(S, K, time, r, sigma);
                double vega = S * t_sqrt * n(d1);
                sigma = sigma + diff / vega;
            }

            return ERROR;  // something screwy happened
        }

        // 计算期权隐含波动率(bisections算法)
        // Param:
        //     type: 期权类别，认购或认沽
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     r: 无风险利率
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     option_price: 期权合约最新价
        // Return: 隐含波动率
        public double BSImpV_bisections(eOptionType type, double S, double K, double r, double time, double option_price)
        {
            Func<double, double, double, double, double, double> pf;

            if (type == eOptionType.认购期权)
                pf = BScall;
            else if (type == eOptionType.认沽期权)
                pf = BSput;
            else
                return ERROR;

            // simple binomial search for the implied volatility.
            // Relies on the value of the option increasing in volatility
            const double ACCURACY = 1.0e-5;
            const int MAX_ITERATIONS = 10000;
            const double HIGH_VALUE = 1e10;

            // To bracket sigma. first find a maximum sigma by finding a sigma
            // with a estimated price higher than the actual price.
            double sigma_low = 1e-5;
            double sigma_high = 3;
            double price = pf(S, K, time, r, sigma_high);

            while (price < option_price)
            {
                sigma_high = 2.0 * sigma_high; // keep doubling.
                price = pf(S, K, time, r, sigma_high);
                if (sigma_high>HIGH_VALUE)
                    return ERROR; // panic, something wrong.
            }

            for (int i = 0; i<MAX_ITERATIONS; i++)
            {
                double sigma = (sigma_low + sigma_high) * 0.5;
                price = pf(S, K, time, r, sigma);
                double test = (price - option_price);
                if (Math.Abs(test)<ACCURACY)
                {
                    return sigma;
                }
                if (test < 0.0)
                {
                    sigma_low = sigma;
                }
                else
                {
                    sigma_high = sigma;
                }
            }

            return ERROR;    // something screwy happened
        }

        // 计算期权希腊值
        // Param:
        //     type: 期权类别，认购或认沽
        //     S: 标的证券最新价
        //     K: 期权执行价格
        //     r: 无风险利率
        //     sigma: 波动率
        //     time: 距离到期日时间；单位：年；计算公式：剩余天数/365
        //     Delta：计算出的Delta数值
        //     Gamma：计算出的Gamma数值
        //     Theta：计算出的Theta数值
        //     Vega：计算出的Vega数值
        //     Rho：计算出的Rho数值
        // Return: NULL
        public void BSOptGreeks(eOptionType type, double S, double K, double r, double sigma, double time,
            out double Delta, out double Gamma, out double Theta, out double Vega, out double Rho)
        {
            double time_sqrt = Math.Sqrt(time);
            double d1 = D1(S, K, time, r, sigma);
            double d2 = D2(d1, time, sigma);

            Delta = 0;
            Gamma = 0;
            Theta = 0;
            Vega = 0;
            Rho = 0;

            if (type == eOptionType.认购期权)
            {
                Delta = N(d1);
                Gamma = n(d1) / (S * sigma * time_sqrt);
                Theta = -(S * sigma * n(d1)) / (2 * time_sqrt) - r * K * Math.Exp(-r * time) * N(d2);
                Vega = S * time_sqrt * n(d1);
                Rho = K * time * Math.Exp(-r * time) * N(d2);
            }
            else if (type == eOptionType.认沽期权)
            {
                Delta = -N(-d1);
                Gamma = n(d1) / (S * sigma * time_sqrt);
                Theta = -(S * sigma * n(d1)) / (2 * time_sqrt) + r * K * Math.Exp(-r * time) * N(-d2);
                Vega = S * time_sqrt * n(d1);
                Rho = -K * time * Math.Exp(-r * time) * N(-d2);
            }
        }
    }

    public enum ImpliedVolatilityMethod
    {
        Newton,
        Bisections,
    }
}

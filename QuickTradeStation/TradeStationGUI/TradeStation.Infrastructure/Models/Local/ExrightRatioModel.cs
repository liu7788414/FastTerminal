using System;

namespace TradeStation.Infrastructure.Models.Local
{
    public class ExrightRatioModel
    {
        public string ExID { get; set; }

        public string SecurityID { get; set; }

        public DateTime TradeDate { get; set; }

        public string ExrightType { get; set; }

        public double BackFactor { get; set; }

        public double ForwardFactor { get; set; }
    }
}

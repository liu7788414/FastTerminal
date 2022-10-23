using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Timers;

using Microsoft.Practices.ServiceLocation;

using TradeStation.Infrastructure.Helpers;

namespace TradeStation.Infrastructure.Engines
{
    [Export]
    public class ReInitializationEngine
    {
        private const int SECOND_INTERVAL = 1000;
        private const int MINUTE_INTERVAL = 60000;
        private const int HOUR_INTERVAL = 3600000;

        private Timer _reInitializationTimer;

        [ImportMany(typeof(IDailyInformationGetter), AllowRecomposition = true)]
        private IEnumerable<Lazy<IDailyInformationGetter>> _dailyInformationGettor;

        [ImportMany(typeof(IReInitializable), AllowRecomposition = true)]
        private IEnumerable<Lazy<IReInitializable>> _reInitializableList;

        public ReInitializationEngine()
        {
            // Check status time every 5 minutes.
            _reInitializationTimer = new Timer(MINUTE_INTERVAL * 3);
            _reInitializationTimer.Elapsed += ReInitializationTimer_Elapsed;
        }

        public void Initialize()
        {
            _reInitializationTimer.Start();
        }

        private void ReInitializationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currentDateTime = DateTime.Now;

            if (currentDateTime >= TimeKeeper.NextInitializationTime)
            {
                // Get initialization data.
                foreach (var item in _dailyInformationGettor)
                {
                    item.Value.DailyReInitialize();
                }

                // ReInitialize every items.
                foreach (var item in _reInitializableList)
                {
                    item.Value.DailyReInitialize();
                }
            }
        }
    }
}

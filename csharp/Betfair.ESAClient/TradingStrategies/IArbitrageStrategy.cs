using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betfair.ESAClient.Cache;

namespace TradingStrategies
{
    public interface IArbitrageStrategy
    {
        MarketSnap _marketSnap1 { get; set; }
        MarketSnap _marketSnap2 { get; set; }
        void EvaluateMarkets(MarketChangedEventArgs e);
        
        MarketRunnerSnap GetMarketRunnerSnap(MarketSnap snap, double runnerId);

    }
}

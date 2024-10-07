using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Api_ng_sample_code;
using Betfair.ESAClient.Cache;

namespace TradingStrategies
{
    public interface IArbitrageStrategy
    {
        MarketSnap _marketSnap1 { get; set; }
        MarketSnap _marketSnap2 { get; set; }

        IClient APINGClient { get; set; }
        void EvaluateMarkets(MarketChangedEventArgs e);
        
        MarketRunnerSnap GetMarketRunnerSnap(MarketSnap snap, double runnerId);

    }
}

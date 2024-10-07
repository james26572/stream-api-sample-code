using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betfair.ESAClient.Cache;

using System.Security.Policy;
using Api_ng_sample_code;

namespace TradingStrategies
{
    public class ArbStrat : IArbitrageStrategy
    {
        
        public MarketSnap _marketSnap1 { get; set; }
        public MarketSnap _marketSnap2 { get; set; }
        public IClient APINGClient { get; set; }

        public ArbStrat(MarketSnap market1, MarketSnap market2, IClient client)
        {
            _marketSnap1 = market1;
            _marketSnap2 = market2;
            APINGClient = client;
        }

        
        public void EvaluateMarkets(MarketChangedEventArgs e)
        {
            if(_marketSnap1 == null || _marketSnap2 == null) return;

            if (e.Snap.MarketId == _marketSnap1.MarketId)
            {
                _marketSnap1 = e.Snap;
            }
            else if (e.Snap.MarketId == _marketSnap2.MarketId) {
                _marketSnap2 = e.Snap;
            }

            MarketRunnerPrices selectionSnap1 = GetMarketRunnerSnap(_marketSnap1, 1).Prices;
            MarketRunnerPrices selectionSnap2 = GetMarketRunnerSnap(_marketSnap2, 5851482).Prices;

            if (GetLevel(selectionSnap1.BestAvailableToBack, 0).Price > GetLevel(selectionSnap2.BestAvailableToLay,0).Price) {
                //arb

                Console.Beep(5000,5000);
                return;
            }
            if (GetLevel(selectionSnap2.BestAvailableToBack, 0).Price > GetLevel(selectionSnap1.BestAvailableToLay, 0).Price)
            {
                //arb
                Console.Beep(5000,5000);
                return;
            }
            Console.WriteLine("No arb found");
            

        }

        public MarketRunnerSnap GetMarketRunnerSnap(MarketSnap snap, double runnerId) {

            return snap.MarketRunners.FirstOrDefault(runner => runner.RunnerId.SelectionId == runnerId);
        }
        private static LevelPriceSize GetLevel(IList<LevelPriceSize> values, int level)
        {
            return values.ElementAtOrDefault(0) ?? new LevelPriceSize(level, 0, 0);
        }
    }
}

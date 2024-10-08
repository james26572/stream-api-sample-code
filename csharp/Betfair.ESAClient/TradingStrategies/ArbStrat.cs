using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betfair.ESAClient.Cache;

using System.Security.Policy;
using Api_ng_sample_code;
using Api_ng_sample_code.TO;
using Betfair.ESAClient;

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
                //Console.Beep(5000, 5000);
                double minLiquidity = Math.Min(GetLevel(selectionSnap1.BestAvailableToBack, 0).Size, GetLevel(selectionSnap2.BestAvailableToLay, 0).Size);
                double backStake = (minLiquidity * GetLevel(selectionSnap2.BestAvailableToLay, 0).Price)/ GetLevel(selectionSnap1.BestAvailableToBack, 0).Price;
                double layStake = (minLiquidity * GetLevel(selectionSnap1.BestAvailableToBack, 0).Price) / GetLevel(selectionSnap2.BestAvailableToLay, 0).Price;

                //for testing purposes
                backStake = 1;
                layStake = (backStake * GetLevel(selectionSnap1.BestAvailableToBack, 0).Price) / GetLevel(selectionSnap2.BestAvailableToLay, 0).Price;

                //send in the limit order for backing
                IList<PlaceInstruction> placeInstructions = new List<PlaceInstruction>();
                var placeInstruction = new PlaceInstruction();
                placeInstruction.Handicap = 0;
                placeInstruction.Side = Side.BACK;
                placeInstruction.OrderType = OrderType.LIMIT;
                var limitOrder = new LimitOrder();
                limitOrder.PersistenceType = PersistenceType.LAPSE;
                limitOrder.Price = GetLevel(selectionSnap1.BestAvailableToBack, 0).Price;
                limitOrder.Size = backStake;
                placeInstruction.LimitOrder = limitOrder;
                placeInstruction.SelectionId = 1;
                placeInstructions.Add(placeInstruction);
                try
                {
                    var placeExecutionReportBack = APINGClient.placeOrders(_marketSnap1.MarketId, "123456", placeInstructions);
                }
                catch (APINGException apiExcepion) {
                    Console.WriteLine("Got an exception from Api-NG: " + apiExcepion.ErrorCode);
                }
                

                //send in the limit order for laying
                placeInstructions = new List<PlaceInstruction>();
                placeInstruction = new PlaceInstruction();
                placeInstruction.Handicap = 0;
                placeInstruction.Side = Side.LAY;
                placeInstruction.OrderType= OrderType.LIMIT;
                limitOrder = new LimitOrder();
                limitOrder.PersistenceType= PersistenceType.LAPSE;
                limitOrder.Price = GetLevel(selectionSnap2.BestAvailableToLay, 0).Price;
                limitOrder.Size = layStake;
                placeInstruction.LimitOrder = limitOrder;
                placeInstruction.SelectionId = 5851482;
                placeInstructions.Add(placeInstruction);

                try
                {
                    var placeExecutionReportLay = APINGClient.placeOrders(_marketSnap2.MarketId, "654321", placeInstructions);
                }
                catch (APINGException apiExcepion)
                {
                    Console.WriteLine("Got an exception from Api-NG: " + apiExcepion.ErrorCode);
                }


                return;
            }
            if (GetLevel(selectionSnap2.BestAvailableToBack, 0).Price > GetLevel(selectionSnap1.BestAvailableToLay, 0).Price)
            {
                //arb
                //Console.Beep(5000, 5000);
                double minLiquidity = Math.Min(GetLevel(selectionSnap2.BestAvailableToBack, 0).Size, GetLevel(selectionSnap1.BestAvailableToLay, 0).Size);
                double backStake = (minLiquidity* GetLevel(selectionSnap1.BestAvailableToLay,0).Price)/GetLevel(selectionSnap2.BestAvailableToBack,0).Price;
                double layStake = (minLiquidity*(GetLevel(selectionSnap2.BestAvailableToBack,0).Price) / GetLevel(selectionSnap1.BestAvailableToLay,0).Price);

                //for testing purposes
                backStake = 1;
                layStake = (backStake * GetLevel(selectionSnap2.BestAvailableToBack, 0).Price) / GetLevel(selectionSnap1.BestAvailableToLay, 0).Price;

                //send in the limit order for backing
                IList<PlaceInstruction> placeInstructions = new List<PlaceInstruction>();
                var placeInstruction = new PlaceInstruction();
                placeInstruction.Handicap = 0;
                placeInstruction.Side = Side.BACK;
                placeInstruction.OrderType = OrderType.LIMIT;
                var limitOrder = new LimitOrder();
                limitOrder.PersistenceType = PersistenceType.LAPSE;
                limitOrder.Price = GetLevel(selectionSnap2.BestAvailableToBack, 0).Price;
                limitOrder.Size = backStake;
                placeInstruction.LimitOrder = limitOrder;
                placeInstruction.SelectionId = 1;
                placeInstructions.Add(placeInstruction);
                var placeExecutionReportBack = APINGClient.placeOrders(_marketSnap1.MarketId, "123456", placeInstructions);

                //send in the limit order for laying
                placeInstructions = new List<PlaceInstruction>();
                placeInstruction = new PlaceInstruction();
                placeInstruction.Handicap = 0;
                placeInstruction.Side = Side.LAY;
                placeInstruction.OrderType = OrderType.LIMIT;
                limitOrder = new LimitOrder();
                limitOrder.PersistenceType = PersistenceType.LAPSE;
                limitOrder.Price = GetLevel(selectionSnap1.BestAvailableToLay, 0).Price;
                limitOrder.Size = layStake;
                placeInstruction.LimitOrder = limitOrder;
                placeInstruction.SelectionId = 5851482;
                placeInstructions.Add(placeInstruction);
                var placeExecutionReportLay = APINGClient.placeOrders(_marketSnap2.MarketId, "654321", placeInstructions);

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

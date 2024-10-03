using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingStrategies
{
    internal interface IArbitrageStrategy
    {
        double market1Id { get; set; }
        double market2Id { get; set; }

    }
}

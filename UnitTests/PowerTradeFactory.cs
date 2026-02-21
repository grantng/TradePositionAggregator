using System;
using System.Collections.Generic;
using System.Text;
using Services;

namespace UnitTests
{
    internal class PowerTradeFactory
    {
        public static PowerTrade CreatePowerTrade(DateTime date, IEnumerable<double> volumes)
        {
            var trade = PowerTrade.Create(date, volumes.Count());

            var i = 0;

            foreach(var volume in volumes)
            {
                trade.Periods[i++].Volume = volume;
            }

            return trade;
        }
    }
}

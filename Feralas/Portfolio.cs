using System;
using System.Collections.Generic;

namespace Feralas
{
    public class Portfolio
    {
        public Portfolio()
        { }
        public Portfolio(List<Holding> shares, double cost)
        {
            Holdings = shares;
            Cost = cost;
        }
        public List<Holding> Holdings = new List<Holding>();
        public double Cost = 0;
        public double Cash = 0;
        public double PeakValue = 0;
        public DateTime TimeBought;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    class Balancing
    {
        public static bool isBalanced()
        {
            Balancing balancing = new Balancing(Galaxy.Instance);
            List<BalancingPlayer> players = balancing.balancingGalaxy.playerScores();
            foreach (BalancingPlayer player in players)
            {
                System.Diagnostics.Trace.WriteLine(player);
            }
            double standardDev = CalculateStdDev(players.Select(player => player.score));
            return standardDev < 1000;
        }

        private BalancingGalaxy balancingGalaxy;
        private static double MAX_DEVIATION = 1000;

        private Balancing(Galaxy gal)
        {
            balancingGalaxy = new BalancingGalaxy(gal);
        }

        public static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

    }
}

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
            foreach (StarSystem spawnStar in Galaxy.Instance.SpawnStars)
            {
                balancing.playerScore(spawnStar);
            }           
            return true;
        }

        private BalancingGalaxy balancingGalaxy;

        private Balancing(Galaxy gal)
        {
            balancingGalaxy = new BalancingGalaxy(gal);
        }

        private double playerScore(StarSystem spawnStar)
        {
            return 0.0;
        }
    }
}

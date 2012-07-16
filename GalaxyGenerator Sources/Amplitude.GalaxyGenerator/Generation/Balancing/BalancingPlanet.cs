using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingPlanet
    {
        public Planet attachedPlanet;

        //Not inside Settings to keep modifications seperate from given code.
        private static Dictionary<string, Dictionary<String, int>> planetPopulation;

        public static int getPlanetPopulation(String planetType, String planetSize)
        {
            if (planetPopulation == null)
            {
                planetPopulation = new Dictionary<string, Dictionary<string, int>>();
                initPlanetPopulation();
            }
            return planetPopulation[planetType][planetSize];
        }

        public BalancingPlanet(Planet p)
        {
            attachedPlanet = p;
        }

        public int population()
        {
            return getPlanetPopulation(attachedPlanet.type, attachedPlanet.size);
        }

        private static void initPlanetPopulation()
        {
            Dictionary<String, int> tier1Pop = new Dictionary<string, int>();
            tier1Pop.Add("PlanetSizeTiny", 4);
            tier1Pop.Add("PlanetSizeSmall", 5);
            tier1Pop.Add("PlanetSizeMedium", 6);
            tier1Pop.Add("PlanetSizeLarge", 8);
            tier1Pop.Add("PlanetSizeHuge", 10);
            planetPopulation.Add("PlanetTypeTerran", tier1Pop);
            planetPopulation.Add("PlanetTypeOcean", tier1Pop);
            planetPopulation.Add("PlanetTypeJungle", tier1Pop);

            Dictionary<String, int> tier2Pop = new Dictionary<string, int>();
            tier2Pop.Add("PlanetSizeTiny", 3);
            tier2Pop.Add("PlanetSizeSmall", 4);
            tier2Pop.Add("PlanetSizeMedium", 5);
            tier2Pop.Add("PlanetSizeLarge", 7);
            tier2Pop.Add("PlanetSizeHuge", 8);
            planetPopulation.Add("PlanetTypeTundra", tier2Pop);
            planetPopulation.Add("PlanetTypeArid", tier2Pop);

            Dictionary<String, int> tier3Pop = new Dictionary<string, int>();
            tier3Pop.Add("PlanetSizeTiny", 2);
            tier3Pop.Add("PlanetSizeSmall", 3);
            tier3Pop.Add("PlanetSizeMedium", 4);
            tier3Pop.Add("PlanetSizeLarge", 5);
            tier3Pop.Add("PlanetSizeHuge", 6);
            planetPopulation.Add("PlanetTypeArctic", tier3Pop);
            planetPopulation.Add("PlanetTypeDesert", tier3Pop);

            Dictionary<String, int> tier4Pop = new Dictionary<string, int>();
            tier4Pop.Add("PlanetSizeTiny", 1);
            tier4Pop.Add("PlanetSizeSmall", 2);
            tier4Pop.Add("PlanetSizeMedium", 3);
            tier4Pop.Add("PlanetSizeLarge", 4);
            tier4Pop.Add("PlanetSizeHuge", 4);
            planetPopulation.Add("PlanetTypeBarren", tier4Pop);
            planetPopulation.Add("PlanetTypeLava", tier4Pop);

            Dictionary<String, int> tier5Pop = new Dictionary<string, int>();
            tier5Pop.Add("PlanetSizeTiny", 1);
            tier5Pop.Add("PlanetSizeSmall", 2);
            tier5Pop.Add("PlanetSizeMedium", 3);
            tier5Pop.Add("PlanetSizeLarge", 3);
            tier5Pop.Add("PlanetSizeHuge", 4);
            planetPopulation.Add("PlanetTypeAsteroids", tier5Pop);
            planetPopulation.Add("PlanetTypeGasHydrogen", tier5Pop);
            planetPopulation.Add("PlanetTypeGasHelium", tier5Pop);
            planetPopulation.Add("PlanetTypeGasMethane", tier5Pop);
        }
    }
}

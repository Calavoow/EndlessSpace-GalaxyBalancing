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
        private static Dictionary<String, Dictionary<String, int>> planetPopulation;
        private static Dictionary<String, double> anomalyWorth;
        private static Dictionary<String, double> planetTypeWorth;

        public static int getPlanetPopulation(Planet p)
        {
            return getPlanetPopulation(p.type, p.size);
        }

        public static int getPlanetPopulation(String planetType, String planetSize)
        {
            //Lazily initate the Dictonary
            if (planetPopulation == null)
            {
                initPlanetPopulation();
            }
            return planetPopulation[planetType][planetSize];
        }

        public static double getAnomalyWorth(String anomalyType)
        {
            //Lazily initate the Dictonary
            if (anomalyWorth == null)
            {
                initAnomalyWorth();
            }
            return anomalyWorth[anomalyType];
        }

        public static double getPlanetTypeWorth(String planetType)
        {
            //Lazily initate the Dictonary
            if (anomalyWorth == null)
            {
                initPlanetTypeWorth();
            }
            return planetTypeWorth[planetType];
        }

        public BalancingPlanet(Planet p)
        {
            attachedPlanet = p;
        }

        public int population()
        {
            return getPlanetPopulation(attachedPlanet.type, attachedPlanet.size);
        }

        public String size()
        {
            return attachedPlanet.size;
        }

        public String type()
        {
            return attachedPlanet.type;
        }

        public double typeWorth()
        {
            return getPlanetTypeWorth(type());
        }

        public String anomaly()
        {
            return attachedPlanet.anomaly;
        }

        public double getAnomalyWorth()
        {
            return getAnomalyWorth(anomaly());
        }

        //Return the calculated worth of this planet
        public double getWorth()
        {
            return population()
                * getPlanetTypeWorth(attachedPlanet.type)
                * getAnomalyWorth(attachedPlanet.anomaly);
        }

        private static void initPlanetPopulation()
        {
            planetPopulation = new Dictionary<String, Dictionary<String, int>>();

            Dictionary<String, int> tier1Pop = new Dictionary<String, int>();
            tier1Pop.Add("PlanetSizeTiny", 4);
            tier1Pop.Add("PlanetSizeSmall", 5);
            tier1Pop.Add("PlanetSizeMedium", 6);
            tier1Pop.Add("PlanetSizeLarge", 8);
            tier1Pop.Add("PlanetSizeHuge", 10);
            planetPopulation.Add("PlanetTypeTerran", tier1Pop);
            planetPopulation.Add("PlanetTypeOcean", tier1Pop);
            planetPopulation.Add("PlanetTypeJungle", tier1Pop);

            Dictionary<String, int> tier2Pop = new Dictionary<String, int>();
            tier2Pop.Add("PlanetSizeTiny", 3);
            tier2Pop.Add("PlanetSizeSmall", 4);
            tier2Pop.Add("PlanetSizeMedium", 5);
            tier2Pop.Add("PlanetSizeLarge", 7);
            tier2Pop.Add("PlanetSizeHuge", 8);
            planetPopulation.Add("PlanetTypeTundra", tier2Pop);
            planetPopulation.Add("PlanetTypeArid", tier2Pop);

            Dictionary<String, int> tier3Pop = new Dictionary<String, int>();
            tier3Pop.Add("PlanetSizeTiny", 2);
            tier3Pop.Add("PlanetSizeSmall", 3);
            tier3Pop.Add("PlanetSizeMedium", 4);
            tier3Pop.Add("PlanetSizeLarge", 5);
            tier3Pop.Add("PlanetSizeHuge", 6);
            planetPopulation.Add("PlanetTypeArctic", tier3Pop);
            planetPopulation.Add("PlanetTypeDesert", tier3Pop);

            Dictionary<String, int> tier4Pop = new Dictionary<String, int>();
            tier4Pop.Add("PlanetSizeTiny", 1);
            tier4Pop.Add("PlanetSizeSmall", 2);
            tier4Pop.Add("PlanetSizeMedium", 3);
            tier4Pop.Add("PlanetSizeLarge", 4);
            tier4Pop.Add("PlanetSizeHuge", 4);
            planetPopulation.Add("PlanetTypeBarren", tier4Pop);
            planetPopulation.Add("PlanetTypeLava", tier4Pop);

            Dictionary<String, int> tier5Pop = new Dictionary<String, int>();
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

        private static void initAnomalyWorth()
        {
            anomalyWorth = new Dictionary<String, double>();

            //Best positive anomalies
            addAnomalyWorthToRange(1, 1, 2);

            //Positive anomalies
            addAnomalyWorthToRange(2, 21, 1.5);

            //Negative anomalies
            addAnomalyWorthToRange(22, 39, 2.0/3.0);

            //Worst anomalies
            addAnomalyWorthToRange(40, 50, 0.5);

            //No anomaly
            anomalyWorth.Add("", 1);
        }

        private static void addAnomalyWorthToRange(int start, int end, double worth)
        {
            List<String> anomalies = new List<String>();
            for (int i = start; i <= end; i++)
            {
                anomalies.Add("PlanetAnomaly" + i.ToString("D2"));
            }

            foreach (string anomaly in anomalies)
            {
                anomalyWorth.Add(anomaly, worth);
            }
        }

        private static void initPlanetTypeWorth()
        {
            planetTypeWorth = new Dictionary<string, double>();
            //Tier 1 planets
            String[] tier1Planets = {"Terran","Ocean","Jungle"};
            addPlanetTypeWorth(tier1Planets, 2);

            //Tier 2
            String[] tier2Planets = { "Tundra", "Arid" };
            addPlanetTypeWorth(tier2Planets, 1.5);

            //Tier 3
            String[] tier3Planets = { "Arctic", "Desert" };
            addPlanetTypeWorth(tier3Planets, 1);

            //Tier 3
            String[] tier4Planets = { "Barren", "Lava" };
            addPlanetTypeWorth(tier4Planets, 2.0/3.0);

            //Tier 4
            String[] tier5Planets = { "Asteroids", "GasHydrogen", "GasHelium", "GasMethane" };
            addPlanetTypeWorth(tier5Planets, 0.5);
        }

        private static void addPlanetTypeWorth(String[] planetTypes, double worth)
        {
            foreach (String planetType in planetTypes)
            {
                planetTypeWorth.Add("PlanetType"+planetType, worth);
            }
        }
    }
}

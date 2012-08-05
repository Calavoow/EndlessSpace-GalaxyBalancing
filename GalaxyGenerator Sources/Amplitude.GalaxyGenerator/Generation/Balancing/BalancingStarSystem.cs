using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingStarSystem
    {
        private double systemWorth = -1d;
        protected List<StarSystem> inRangeSystems;
        public StarSystem attachedStarSystem { get; protected set; }
        public List<BalancingPlanet> balancingPlanets;
        public Dictionary<BalancingPlayer, double> PlayerDistance = new Dictionary<BalancingPlayer,double>();

        public BalancingStarSystem( StarSystem star )
        {
            attachedStarSystem = star;
            inRangeSystems = new List<StarSystem>();
            balancingPlanets = new List<BalancingPlanet>();

            foreach (Planet p in attachedStarSystem.Planets)
            {
                balancingPlanets.Add(new BalancingPlanet(p));
            }
        }

        public double getSystemWorth()
        {
            if( systemWorth == -1 )
            {
                systemWorth = calculateSystemWorth();
            }
            return systemWorth;
        }

        //Also keeps in mind the PlayerDistance Dictionary.
        public double getSystemWorth(BalancingPlayer player)
        {
            double playerDistance = PlayerDistance[player];
            //The planet is contested
            if (PlayerDistance.Count > 1)
            {
                double totalDistance = PlayerDistance.Aggregate(0.0, (accumulator, pair) => accumulator + pair.Value);
                double playerWorth = (1 - playerDistance / totalDistance) * getSystemWorth();
                return playerWorth;
            }
            else
            {
                return getSystemWorth();
            }
        }

        public double maxPopulation()
        {
            int maxPop = 0;
            balancingPlanets.ForEach((p) => { maxPop += p.population(); });
            return maxPop;
        }

        public double calculateSystemWorth()
        {
            double sumOfPlanetWorth = 0;
            balancingPlanets.ForEach((p) => 
            {
                sumOfPlanetWorth += p.getWorth();
            });
            return maxPopulation() * sumOfPlanetWorth;
        }

        public override String ToString()
        {
            String output = "This system contains the following planets:{\n ";
            balancingPlanets.ForEach((p) =>
            {
                output += "Type: " + p.type() + ", Size: " + p.size() + ", Anomaly: " + p.anomaly() + " , Population: " + p.population() + " , typeWorth: " + p.typeWorth() + " , anomalyWorth: " + p.getAnomalyWorth() + " , Planet Worth: " + p.getWorth() + "\n";
            });
            output += " }\n And has total population: " + maxPopulation() +"\n";
            return output += "Total system worth: " + getSystemWorth() + "\n";
        }
    }
}

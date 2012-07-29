using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingStarSystem
    {
        private double systemWorth = -1;
        protected List<StarSystem> inRangeSystems;
        public StarSystem attachedStarSystem { get; protected set; }
        public List<BalancingPlanet> balancingPlanets;

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

        public double maxPopulation()
        {
            int maxPop = 0;
            balancingPlanets.ForEach(delegate(BalancingPlanet p){
                maxPop += p.population();
            });
            return maxPop;
        }

        public double calculateSystemWorth()
        {
            double sumOfPlanetWorth = 0;
            balancingPlanets.ForEach(delegate(BalancingPlanet p)
            {
                sumOfPlanetWorth += p.getWorth();
            });
            return maxPopulation() * sumOfPlanetWorth;
        }

        public String toString()
        {
            String output = "This system contains the following planets:{\n ";
            balancingPlanets.ForEach(delegate(BalancingPlanet p){
                output += "Type: " + p.type() + ", Size: " + p.size() + ", Anomaly: " + p.anomaly() + " , Population: " + p.population() + " , typeWorth: " + p.typeWorth() + " , anomalyWorth: " + p.getAnomalyWorth() +" , Planet Worth: " + p.getWorth() + "\n";
            });
            output += " }\n And has total population: " + maxPopulation() +"\n";
            return output += "Total system worth: " + getSystemWorth() + "\n";
        }
    }
}

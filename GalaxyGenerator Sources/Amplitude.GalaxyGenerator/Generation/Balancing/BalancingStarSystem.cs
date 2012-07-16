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
            //double totalScore = 0.0;
            List<Planet> planets = attachedStarSystem.Planets;  
            return maxPopulation();
        }

        public String toString()
        {
            String output = "This system contains the following planets:{ ";
            attachedStarSystem.Planets.ForEach(delegate(Planet p){
                output += p.type + "," + p.size + ";";
            });
            output += " } And has total population: " + maxPopulation();
            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingPlayer
    {
        public BalancingStarSystem SpawnStar { get; protected set; }
        public HashSet<BalancingStarSystem> Stars { get; set; }
        public double score { get;set;}

        public BalancingPlayer(BalancingStarSystem SpawnStar)
        {
            this.SpawnStar = SpawnStar;
        }

        public override string ToString()
        {
            String output = "SpawnStar: " + SpawnStar.attachedStarSystem.Name + "\nScore: " + score + "\n";
            //output += "Influenced StarSystems:\n";
            //foreach( BalancingStarSystem star in Stars ){
            //    output += star + "PlayerWorth: " + star.getSystemWorth(this) + "\n";
            //}
            return output;
        }
    }
}

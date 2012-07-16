using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Balancing
{
    public class BalancingGalaxy
    {
        public List<BalancingStarSystem> Stars { get; protected set; }
        public List<WarpLine> Warps { get; protected set; }
        public List<Constellation> Constellations { get; protected set; }
        public List<Region> Regions { get; protected set; }
        public List<BalancingStarSystem> SpawnStars { get; protected set; }
        public List<Planet> Planets { get; protected set; }

        public BalancingGalaxy(Galaxy gal)
        {
            this.Stars = new List<BalancingStarSystem>();
            this.Warps = new List<WarpLine>();
            this.Constellations = new List<Constellation>();
            this.Regions = new List<Region>();
            this.SpawnStars = new List<BalancingStarSystem>();
            foreach (StarSystem star in gal.Stars)
            {
                Stars.Add(new BalancingStarSystem(star));
                Stars.ForEach(delegate(BalancingStarSystem balSys)
                {
                    System.Diagnostics.Debug.WriteLine(balSys.toString());
                });
            }
        }

    }
}

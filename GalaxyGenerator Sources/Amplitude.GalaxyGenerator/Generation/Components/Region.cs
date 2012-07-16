using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    public class Region : List<StarSystem>
    {
        public Color Index { get; protected set; }

        public Region(Color c)
        {
            this.Index = c;
            this.AddRange(Galaxy.Instance.Stars.FindAll((StarSystem s) => { return s.regionIndex == this.Index; }));
        }

        public bool isSpawn()
        {
            if (Galaxy.Instance.Configuration.shape().regions.ContainsKey(this.Index))
                return Galaxy.Instance.Configuration.shape().regions[this.Index];

            return false;
        }

        public List<Region> adjacentRegions ()
        {
            List<Region> adjacents = new List<Region>();

            foreach (Shape.Link link in Galaxy.Instance.Configuration.shape().topology)
            {
                if (link.RegionA == this.Index) adjacents.Add(Galaxy.Instance.Regions.Find((r) => {return r.Index == link.RegionB;}));
                else if (link.RegionB == this.Index) adjacents.Add(Galaxy.Instance.Regions.Find((r) => { return r.Index == link.RegionA; }));
            }

            return adjacents;
        }
    }
}

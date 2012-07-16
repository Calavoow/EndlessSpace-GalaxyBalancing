// <copyright file="Constellation.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Drawing;

    public class Constellation : List<StarSystem>
    {
        public Constellation()
        {
            this.Name = Galaxy.Instance.Configuration.getRandomConstellationName();
            if (Galaxy.Instance.Constellations.Contains(this))
            {
                this.id = Galaxy.Instance.Constellations.FindIndex((c) => { return c == this; });
            }
            else
            {
                this.id = Galaxy.Instance.Constellations.Count;
                Galaxy.Instance.Constellations.Add(this);
            }
        }

        public int id { get; protected set; }
        public string Name { get; protected set; }

        public List<Constellation> adjacentConstellations()
        {
            List<Constellation> list = new List<Constellation>();

            foreach (StarSystem s in this)
                foreach (StarSystem t in s.destinations)
                    if (!this.Contains(t))
                        list.Add(t.constellation());

            return list;
        }

        public List<Color> presentRegionIndexes()
        {
            List<Color> indexes = new List<Color>();

            foreach (StarSystem s in this)
                if (!indexes.Contains(s.regionIndex))
                    indexes.Add(s.regionIndex);

            return indexes;
        }
    }
}

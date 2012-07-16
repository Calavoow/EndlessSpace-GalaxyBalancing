// <copyright file="WarpLine.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class WarpLine
    {
        public int id { get; set; }
        public bool isWormhole { get; protected set; }

        public StarSystem starA { get; protected set; }
        public StarSystem starB { get; protected set; }

        public double Length { get { return Geometry2D.Distance(this.starA.position, this.starB.position); } }

        public WarpLine(StarSystem a, StarSystem b)
        {
            this.id = Galaxy.Instance.Warps.Count;
            this.starA = a;
            this.starB = b;
            this.starA.destinations.Add(this.starB);
            this.starB.destinations.Add(this.starA);
            this.isWormhole = false;
        }
    }

    public class Wormhole : WarpLine
    {
        public Wormhole(StarSystem a, StarSystem b)
            : base (a, b)
        {
            this.isWormhole = true;
        }
    }
}

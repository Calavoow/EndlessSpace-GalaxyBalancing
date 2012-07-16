// <copyright file="StellarSystem.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    public class StarSystem : IComparable<StarSystem>
    {
        public int CompareTo(StarSystem other) { return other.id - this.id; }

        public static int MinPlanets = 1;
        public static int MaxPlanets = 6;
        public static int VeryFarAway = 999999;

        public string Name { get; set; }
        public int id { get; set; }
        public string type { get; set; }

        public PointF position { get; set; }

        public Color regionIndex { get; set; }
        public Region region { get { return Galaxy.Instance.Regions.Find((r)=>{return r.Index == this.regionIndex;});} }
        public List<Planet> Planets { get; protected set; }
        public HashSet<StarSystem> destinations { get; set; }

        public int HomeWorldIndex { get; protected set; }

        public List<WarpLine> warps()
        {
            return new List<WarpLine>(Galaxy.Instance.Warps.FindAll((w) => { return (w.starA == this) || (w.starB == this); }));
        }

        public bool hasWormhole()
        {
            foreach (WarpLine w in Galaxy.Instance.Warps)
                if (w.isWormhole && ((w.starA == this) || (w.starB == this)))
                    return true;

            return false;
        }
        
        public StarSystem(PointF p)
        {
            this.Planets = new List<Planet>();
            this.destinations = new HashSet<StarSystem>();
            this.warpDistanceTable = new Dictionary<StarSystem, int>();
            this.directDistanceTable = new Dictionary<StarSystem, double>();

            this.position = p;

            this.id = Galaxy.Instance.Stars.Count;
            this.Name = Galaxy.Instance.Configuration.getRandomStarName();
            this.regionIndex = Color.Black;
            this.type = Galaxy.Instance.Configuration.getRandomStarType();
        }

        public void GeneratePlanets(int n = -1)
        {
            int i, nPlanets;

            if (n == -1)
            {
                nPlanets = Galaxy.Instance.Configuration.getRandomPlanetNumber();
            }
            else
            {
                nPlanets = n;
            }

            if (nPlanets < MinPlanets) nPlanets = MinPlanets;
            if (nPlanets > MaxPlanets) nPlanets = MaxPlanets;

            Galaxy.Instance.Planets.RemoveAll((p) => { return this.Planets.Contains(p); });
            this.Planets.Clear();
            i = 0;
            for (; i < nPlanets; i++)
                this.Planets.Add(new Planet(this));

            this.HomeWorldIndex = 0;
        }

        public void computeDirectDistanceTable()
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
                if (! this.directDistanceTable.Keys.Contains(s))
                    this.directDistanceTable.Add(s, Geometry2D.Distance(this.position, s.position));
        }

        public Dictionary<StarSystem, double> directDistanceTable { get; protected set; }

        static public StarSystem FindFarthestStar(StarSystem from, List<StarSystem> into)
        {
            double d, dMax;
            StarSystem farthest;

            if (from.directDistanceTable.Count == 0) from.computeDirectDistanceTable();

            dMax = 0;
            farthest = null;
            foreach (StarSystem s in from.directDistanceTable.Keys)
            {
                if (into.Contains(s))
                {
                    d = from.directDistanceTable[s];
                    if (d > dMax)
                    {
                        dMax = d;
                        farthest = s;
                    }
                }
            }

            return farthest;
        }

        public Constellation constellation()
        {
            return Galaxy.Instance.Constellations.Find((c) => { return c.Contains(this); });
        }

        public void computeWarpDistanceTable()
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
                this.warpDistanceTable[s] = VeryFarAway;
            this.recursiveWarpDistance(this, 0);
        }

        public Dictionary<StarSystem, int> warpDistanceTable { get; protected set; }

        private void recursiveWarpDistance(StarSystem s, int d)
        {
            if (d < warpDistanceTable[s])
            {
                warpDistanceTable[s] = d;
                foreach (StarSystem sj in s.destinations)
                    recursiveWarpDistance(sj, d+1);
            }
        }
    }
}

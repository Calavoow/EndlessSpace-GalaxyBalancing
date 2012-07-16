using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Components;

    public class WarpBuilder : Builder
    {
        public static double StarToWarpLine_TooCloseFactor = 0.2;

        override public string Name { get { return "WarpBuilder"; } }

        public WarpBuilder() : base()
        {
            this.Stars = new List<StarSystem>();
            this.Warps = new List<Warp>();
        }

        override public void Execute()
        {
            this.Stars.Clear();
            this.Stars.AddRange(Galaxy.Instance.Stars);

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");
            
            if (this.Stars.Count == 0)
            {
                System.Diagnostics.Trace.WriteLine("Serious fault - no stars given to WarpBuilder ???");
                this.Defects.Add("No stars available");
                this.Result =false;
                return;
            }

            this.CreateRaw();
            this.RemoveAllCloseToStar();

            foreach (Shape.Link link in Galaxy.Instance.Configuration.shape().topology)
            {
                List<StarSystem> pool = new List<StarSystem>();
                pool.AddRange(Galaxy.Instance.Regions.Find((r) => { return r.Index == link.RegionA; }));
                pool.AddRange(Galaxy.Instance.Regions.Find((r) => { return r.Index == link.RegionB; }));
                this.ForceConnect(pool);
            }
            foreach (Constellation c in Galaxy.Instance.Constellations) this.ForceConnect(c);
            this.ForceConnect(this.Stars);
            
            if (!this.FullyConnected())
            {
                System.Diagnostics.Trace.WriteLine("Unable to fully connect galaxy");
                this.Defects.Add("Unable to fully connect");
                this.Result = false;
                return;
            }

            System.Diagnostics.Trace.WriteLine("Warp Builder - continuing");

            this.EliminateCrossers();
            this.ReduceConnectivity();
            this.ReduceWormholeClutter();

            System.Diagnostics.Trace.WriteLine("Warp Builder Execute...Complete");

            foreach (StarSystem s in this.Stars) s.computeWarpDistanceTable();

            this.WriteToGalaxy();

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }

        protected class Warp : IEquatable<Warp>
        {
            public bool Equals(Warp other)
            {
                return ((this.starA == other.starA) && (this.starB == other.starB))
                    || ((this.starA == other.starB) && (this.starB == other.starA));
            }

            public bool IsCritical { get; set; }
            public bool IsWormhole { get; protected set; }

            public Warp(StarSystem a, StarSystem b)
            {
                this.starA = a;
                this.starB = b;
                this.length = Geometry2D.Distance(this.starA.position, this.starB.position);
                this.crossingWarps = new HashSet<Warp>();
                this.IsWormhole = (a.constellation() != b.constellation());
            }

            public StarSystem starA { get; protected set; }
            public StarSystem starB { get; protected set; }
            public double length { get; protected set; }
            public HashSet<Warp> crossingWarps { get; set; }

            public void RemoveReferenceTo(Warp delenda)
            {
                if (this.crossingWarps.Contains(delenda))
                    this.crossingWarps.Remove(delenda);
            }
        }

        protected List<StarSystem> Stars;
        protected List<Warp> Warps;

        protected void WriteToGalaxy()
        {
            foreach (Warp w in this.Warps)
            {
                if (w.IsWormhole)
                    Galaxy.Instance.Warps.Add(new Wormhole(w.starA, w.starB));
                else
                    Galaxy.Instance.Warps.Add(new WarpLine(w.starA, w.starB));
            }

            foreach (StarSystem s in Galaxy.Instance.Stars)
                s.computeWarpDistanceTable();
        }

        protected bool WarpExistsBetween(StarSystem a, StarSystem b)
        {
            foreach (Warp w in this.Warps)
                if (((w.starA == a) && (w.starB == b))
                    || ((w.starA == b) && (w.starB == a)))
                    return true;

            return false;
        }

        protected List<Warp> warpsOf(StarSystem s)
        {
            List<Warp> result = new List<Warp>();

            foreach (Warp w in this.Warps)
            {
                if ((w.starA == s) || (w.starB == s))
                    result.Add(w);
            }

            return result;
        }

        private void FindConnectedBlockFrom(StarSystem from, List<StarSystem> inside, ref List<StarSystem> block)
        {
            List<StarSystem> unknown = new List<StarSystem>(inside);
            List<StarSystem> front = new List<StarSystem>();
            List<StarSystem> nextFront = new List<StarSystem>();
            List<Warp> connections = new List<Warp>();

            block.Clear();

            front.Add(from);
            block.AddRange(front);

            while (front.Count > 0)
            {
                nextFront.Clear();

                foreach (StarSystem s in front)
                    unknown.Remove(s);

                foreach (StarSystem s in front)
                {
                    connections = new List<Warp>(this.warpsOf(s));

                    foreach (Warp w in connections)
                    {
                        if (unknown.Contains(w.starA) || unknown.Contains(w.starB))
                        {
                            if (w.starA == s)
                            {
                                nextFront.Add(w.starB);
                                unknown.Remove(w.starB);
                            }
                            else if (w.starB == s)
                            {
                                nextFront.Add(w.starA);
                                unknown.Remove(w.starA);
                            }
                        }
                    }
                }

                front = new List<StarSystem>(nextFront);
                block.AddRange(nextFront);
            }
        }

        private bool Connected(List<StarSystem> list)
        {
            if (null == list) return true;
            if (list.Count <= 1) return true;

            List<StarSystem> unknown = new List<StarSystem>(list);
            List<StarSystem> front = new List<StarSystem>();
            List<StarSystem> nextFront = new List<StarSystem>();
            List<Warp> connections = new List<Warp>();

            front.Add(list.First());

            while (front.Count > 0)
            {
                nextFront.Clear();

                foreach (StarSystem s in front)
                    unknown.Remove(s);

                foreach (StarSystem s in front)
                {
                    connections = new List<Warp>(this.warpsOf(s));

                    foreach (Warp w in connections)
                    {
                        if (unknown.Contains(w.starA) || unknown.Contains(w.starB))
                        {
                            if (w.starA == s)
                            {
                                nextFront.Add(w.starB);
                                unknown.Remove(w.starB);
                            }
                            else if (w.starB == s)
                            {
                                nextFront.Add(w.starA);
                                unknown.Remove(w.starA);
                            }
                        }
                    }
                }

                front = new List<StarSystem>(nextFront);
            }

            return (unknown.Count == 0);
        }

        protected bool FullyConnected()
        {
            foreach (Constellation c in Galaxy.Instance.Constellations)
            {
                if (!this.Connected(c))
                    return false;
            }

            foreach (Shape.Link link in Galaxy.Instance.Configuration.shape().topology)
            {
                List<StarSystem> pool = new List<StarSystem>();
                pool.AddRange(Galaxy.Instance.Regions.Find((r) => { return r.Index == link.RegionA; }));
                pool.AddRange(Galaxy.Instance.Regions.Find((r) => { return r.Index == link.RegionB; }));
                if (!this.Connected(pool))
                    return false;
            }

            return this.Connected(this.Stars);
        }

        protected void CreateRaw()
        {
            int rawWarpQuantity, i;
            List<StarSystem> closest, global, warpable;
            List<Region> adjacent;
            StarSystem sc;
            Warp w;
            double factor = 2.0;

            this.Warps.Clear();

            closest = new List<StarSystem>();

            rawWarpQuantity = (int)Math.Min(factor * Galaxy.Instance.Configuration.connectivity(), (double)(this.Stars.Count));

            if (rawWarpQuantity <= 0)
            {
                System.Diagnostics.Trace.WriteLine("Defaulted raw warp quantity to 6");
                rawWarpQuantity = 6;
                this.Defects.Add("Defaulted initial raw warp quantity to 6");
            }

            foreach (StarSystem s in this.Stars)
            {
                global = new List<StarSystem>(this.Stars);
                closest.Clear();
                global.Remove(s);
                warpable = new List<StarSystem>();
                adjacent = new List<Region>(s.region.adjacentRegions());

                warpable.AddRange(s.region);
                foreach(Region r in adjacent)
                    warpable.AddRange(r);

                if (s.directDistanceTable.Count == 0)
                    s.computeDirectDistanceTable();

                for (i = 0; i < rawWarpQuantity; i++)
                {
                    sc = WarpBuilder.FindClosest(s, global);
                    global.Remove(sc);
                    closest.Add(sc);
                }

                closest.RemoveAll((StarSystem y) => { return !warpable.Contains(y); });

                foreach (StarSystem sb in closest)
                {
                    if (!this.WarpExistsBetween(s, sb))
                    {
                        w = new Warp(s, sb);
                        this.Warps.Add(w);
                    }
                }
            }
        }

        protected void ForceConnect(List<StarSystem> list)
        {
            List<StarSystem> all = new List<StarSystem>();
            List<List<StarSystem>> blocks = new List<List<StarSystem>>();
            List<StarSystem> block = new List<StarSystem>();

            do
            {
                all.AddRange(list);
                block.Clear();
                blocks.Clear();

                while (all.Count > 0)
                {
                    this.FindConnectedBlockFrom(all.First(), list, ref block);
                    blocks.Add(new List<StarSystem>(block));
                    all.RemoveAll((StarSystem s) => { return block.Contains(s); });
                }

                if (blocks.Count > 1)
                {
                    List<StarSystem> pair = ConstellationBuilder.FindClosestPair(blocks[0], blocks[1]);
                    if (pair.Count > 1)
                        if ((pair[0] != null) && (pair[1] != null))
                            this.Warps.Add(new Warp(pair[0], pair[1]));
                }
            }
            while (blocks.Count > 1);
        }

        protected void RemoveAllCloseToStar()
        {
            PointF o, a, b, p;
            HashSet<Warp> delenda = new HashSet<Warp>();

            //System.Diagnostics.Trace.WriteLine("RemoveAllCloseToStar-begin");

            foreach (Warp w in this.Warps)
            {
                foreach (StarSystem s in this.Stars)
                {
                    o = s.position;
                    a = w.starA.position;
                    b = w.starB.position;
                    if ((w.starA != s) && (w.starB != s))
                    {
                        p = Geometry2D.Symmetrical(o, a, b);
                        if (Geometry2D.IntersectionCheck(a, b, o, p) == Geometry2D.IntersectionType.InsideSegment)
                        {
                            if (Geometry2D.Distance(o, p) < WarpBuilder.StarToWarpLine_TooCloseFactor * Geometry2D.Distance(a, b))
                            {
                                delenda.Add(w);
                            }
                        }
                    }
                }
            }

            foreach (Warp w in delenda)
                this.Warps.Remove(w);

            //System.Diagnostics.Trace.WriteLine("RemoveAllCloseToStar-end");
        }

        protected void CheckAllCrossings()
        {
            PointF h, a, b, c, d;

            //System.Diagnostics.Trace.WriteLine("CheckAllCrossings-begin");

            h = new PointF();

            foreach (Warp w in this.Warps)
                w.crossingWarps.Clear();

            foreach (Warp w1 in this.Warps)
            {
                foreach (Warp w2 in this.Warps)
                {
                    if ((w1.starA != w2.starA) && (w1.starA != w2.starB) && (w1.starB != w2.starA) && (w1.starB != w2.starB))
                    {
                        a = w1.starA.position;
                        b = w1.starB.position;
                        c = w2.starA.position;
                        d = w2.starB.position;
                        if (Geometry2D.Intersection(a, b, c, d, ref h) == Geometry2D.IntersectionType.InsideSegment)
                        {
                            w1.crossingWarps.Add(w2);
                            w2.crossingWarps.Add(w1);
                        }
                    }
                }
            }

            int n = 0;
            foreach (Warp w in this.Warps)
                n += w.crossingWarps.Count;

            //System.Diagnostics.Trace.WriteLine("Found " + n.ToString() + " crossings in total");
            //System.Diagnostics.Trace.WriteLine("CheckAllCrossings-end");
        }

        protected void EliminateCrossers()
        {
            List<Warp> delendaList = new List<Warp>();
            List<Warp> crossers = new List<Warp>();
            List<Warp> breakers = new List<Warp>();
            Warp keepCandidate;

            do
            {
                delendaList.Clear();
                this.CheckAllCrossings();
                crossers.Clear();
                foreach (Warp w in this.Warps)
                    if (w.crossingWarps.Count > 0)
                        crossers.Add(w);

                crossers.RemoveAll((Warp w) => { return breakers.Contains(w); });

                if (crossers.Count > 0)
                {
                    do
                    {
                        keepCandidate = crossers.ElementAt(GalaxyGeneratorPlugin.random.Next(crossers.Count));
                    }
                    while (breakers.Contains(keepCandidate));

                    foreach (Warp w in keepCandidate.crossingWarps)
                    {
                        delendaList.Add(w);
                    }

                    foreach (Warp w in delendaList)
                    {
                        this.Warps.Remove(w);
                        foreach (Warp v in this.Warps)
                            v.RemoveReferenceTo(w);
                    }

                    if (!this.FullyConnected())
                    {
                        breakers.Add(keepCandidate);
                        this.Warps.AddRange(delendaList);
                    }
                }
            }
            while ((crossers.Count > 0) || (!this.FullyConnected()));
        }

        protected void ReduceConnectivity()
        {
            Warp candidate;
            List<Warp> nonCriticals = new List<Warp>();

            //this.DetermineNonCriticalWarps(ref nonCriticals);

            do
            {
                this.DetermineNonCriticalWarps(ref nonCriticals);

                if (nonCriticals.Count > 0)
                {
                    candidate = nonCriticals.ElementAt(GalaxyGeneratorPlugin.random.Next(nonCriticals.Count));
                    this.Warps.Remove(candidate);
                    foreach (Warp w in this.Warps)
                        w.RemoveReferenceTo(candidate);
                }

            }
            while ((nonCriticals.Count > 0) && (this.AverageConnectivity() > (double)(Galaxy.Instance.Configuration.connectivity())));
            System.Diagnostics.Trace.WriteLine("Average connectivity down to " + this.AverageConnectivity().ToString());
        }

        protected void DetermineNonCriticalWarps(ref List<Warp> nonCrit)
        {
            List<Warp> copyList = new List<Warp>(this.Warps);

            copyList.RemoveAll((Warp w) => { return w.IsWormhole; });

            foreach (Warp w in copyList)
            {
                this.Warps.Remove(w);
                w.IsCritical = !this.FullyConnected();
                this.Warps.Add(w);
            }

            nonCrit.Clear();
            nonCrit.AddRange(copyList.FindAll((Warp w) => {return ! w.IsCritical;}));
        }

        protected void ReduceWormholeClutter()
        {
            Warp candidate;
            List<Warp> nonCriticals = new List<Warp>();
            List<Warp> wormholes = new List<Warp>();

            this.DetermineNonCriticalWormholes(ref nonCriticals);
            wormholes = this.Warps.FindAll((Warp w) => {return w.IsWormhole;});
            System.Diagnostics.Trace.WriteLine("Wormhole numbers down to " + wormholes.Count.ToString());

            while ((nonCriticals.Count > 0) && ((2 * wormholes.Count / Galaxy.Instance.Constellations.Count) > (3 * Galaxy.Instance.Configuration.wormholeConnectivity())))
            {
                this.DetermineNonCriticalWormholes(ref nonCriticals);
                wormholes = this.Warps.FindAll((Warp w) => {return w.IsWormhole;});

                if (nonCriticals.Count > 0)
                {
                    candidate = nonCriticals.ElementAt(GalaxyGeneratorPlugin.random.Next(nonCriticals.Count));
                    this.Warps.Remove(candidate);
                    foreach (Warp w in this.Warps)
                        w.RemoveReferenceTo(candidate);
                }

                System.Diagnostics.Trace.WriteLine("Wormhole numbers down to " + wormholes.Count.ToString());
            }
        }

        protected void DetermineNonCriticalWormholes(ref List<Warp> nonCrit)
        {
            List<Warp> copyList = new List<Warp>();

            copyList.AddRange(this.Warps.FindAll((Warp w) => {return w.IsWormhole;}));

            foreach (Warp w in copyList)
            {
                this.Warps.Remove(w);
                w.IsCritical = !this.FullyConnected();
                this.Warps.Add(w);
            }

            nonCrit.Clear();
            nonCrit.AddRange(copyList.FindAll((Warp w) => { return !w.IsCritical; }));
        }

        static public StarSystem FindClosest(StarSystem from, List<StarSystem> others)
        {
            StarSystem closest;
            double d, minD;

            //System.Diagnostics.Trace.WriteLine("findClosest from system n°" + from.id.ToString() + " in a list comprising " + others.Count.ToString() + " systems");

            if (from.directDistanceTable.Count == 0)
                from.computeDirectDistanceTable();

            closest = null;
            minD = Galaxy.Instance.Configuration.maxWidth() * 100.0;
            foreach (StarSystem s in others)
            {
                d = from.directDistanceTable[s];
                if ((d < minD) && (s != from))
                {
                    minD = d;
                    closest = s;
                }
            }

            return closest;
        }

        protected Warp FindWarp(StarSystem a, StarSystem b)
        {
            foreach (Warp w in this.Warps)
            {
                if (((w.starA == a) && (w.starB == b)) || ((w.starA == b) && (w.starB == a)))
                {
                    return w;
                }
            }

            return null;
        }

        protected double AverageConnectivity()
        {
            int n;

            n = this.Warps.FindAll((Warp w) => { return !w.IsWormhole; }).Count;

            if ((n > 0) && (this.Stars.Count > 0))
                return 2.0 * (double)(n) / (double)(this.Stars.Count);
            else
                return 0.0;
        }
    }
}

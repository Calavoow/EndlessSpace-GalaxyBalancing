using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Components;

    public class ConstellationBuilder : Builder
    {
        override public string Name { get { return "ConstellationBuilder"; } }

        public ConstellationBuilder() : base()
        {
        }

        override public void Execute()
        {
            int nConstellations;
            int factorA, factorB, a, b, factor;
            bool gotMatch;
            List<Region> neutralRegions = new List<Region>();
            List<Region> spawnRegions = new List<Region>();

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            //BUILD REGIONS
            foreach (Color c in Galaxy.Instance.Configuration.shape().regions.Keys) Galaxy.Instance.Regions.Add(new Region(c));

            neutralRegions.AddRange(Galaxy.Instance.Regions.FindAll((Region r) => { return !r.isSpawn(); }));
            spawnRegions.AddRange(Galaxy.Instance.Regions.FindAll((Region r) => { return r.isSpawn(); }));

            System.Diagnostics.Trace.WriteLine(Galaxy.Instance.Configuration.shape().regions.Keys.Count.ToString() + " theoretical regions");
            System.Diagnostics.Trace.WriteLine(Galaxy.Instance.Regions.Count.ToString() + " actual regions");
            System.Diagnostics.Trace.WriteLine(spawnRegions.Count.ToString() + " spawn regions");
            foreach (Region r in spawnRegions) System.Diagnostics.Trace.WriteLine("-->" + r.Index.ToString() + " containing " + r.Count.ToString()+ " stars");
            System.Diagnostics.Trace.WriteLine(neutralRegions.Count.ToString() + " neutral regions");
            foreach (Region r in neutralRegions) System.Diagnostics.Trace.WriteLine("-->" + r.Index.ToString() + " containing " + r.Count.ToString() + " stars");

            //DETERMINE ACTUAL CONSTELLATIONS NUMBER
            nConstellations = Galaxy.Instance.Configuration.constellations();
            System.Diagnostics.Trace.WriteLine("Configuration requested constellations : " + Galaxy.Instance.Configuration.constellations().ToString());

            while ((nConstellations * Settings.Instance.generationConstraints.minStarsPerConstellation) > Galaxy.Instance.Stars.Count)
                nConstellations--;
            if (nConstellations <= 0)
                nConstellations = 1;

            if (nConstellations < Galaxy.Instance.Configuration.constellations())
            {
                System.Diagnostics.Trace.WriteLine("Min stars per constellation : " + Settings.Instance.generationConstraints.minStarsPerConstellation.ToString());
                System.Diagnostics.Trace.WriteLine("Will use only " + nConstellations.ToString() + " constellations");
                this.Defects.Add("Number of constellations was limited by stars number");
            }

            //DISTRIBUTE CONSTELLATIONS ACROSS REGIONS
            if (nConstellations == 1)
            {
                System.Diagnostics.Trace.WriteLine("Single Constellation");
                Constellation c = new Constellation();
                c.AddRange(Galaxy.Instance.Stars);
            }
            else if (nConstellations == Galaxy.Instance.Regions.Count)
            {
                System.Diagnostics.Trace.WriteLine("One Constellation Per Region");
                foreach (Region r in Galaxy.Instance.Regions)
                {
                    Constellation c = new Constellation();
                    c.AddRange(r);
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Other Case");
                factorA = 0;
                factorB = 0;
                gotMatch = false;

                for (a = 1; a < 20; a++)
                {
                    for (b = 0; b < 20; b++)
                    {
                        if (a * spawnRegions.Count + b * neutralRegions.Count == nConstellations)
                        {
                            gotMatch = true;
                            factorA = a;
                            factorB = b;
                        }
                    }
                }

                if (gotMatch)
                {
                    System.Diagnostics.Trace.WriteLine("Could find integers A=" + factorA.ToString() + " and B=" + factorB.ToString());
                    System.Diagnostics.Trace.WriteLine("Allowing A Constellations in each Spawn Region");
                    System.Diagnostics.Trace.WriteLine("And B Constellation in each Neutral Region");
                    if (factorA > 0)
                    {
                        foreach (Region r in spawnRegions)
                        {
                            this.MakeConstellations(factorA, r);
                        }
                    }
                    if (factorB > 0)
                    {
                        foreach (Region r in neutralRegions)
                        {
                            this.MakeConstellations(factorB, r);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("No exact match");

                    if (nConstellations >= spawnRegions.Count)
                    {
                        System.Diagnostics.Trace.WriteLine("More Constellations than Spawn Regions");
                        foreach (Region r in spawnRegions)
                        {
                            this.MakeConstellations(1, r);
                        }
                        if (nConstellations - spawnRegions.Count > 0)
                        {
                            List<StarSystem> pool = new List<StarSystem>();

                            foreach(Region r in neutralRegions)
                                pool.AddRange(r);

                            this.MakeConstellations(nConstellations - spawnRegions.Count, pool);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Less Constellations than Spawn Regions");
                        factor = 1;
                        while (factor * nConstellations < spawnRegions.Count) factor++;

                        Region start;
                        Region merge;
                        List<Region> adjacentSpawnRegions = new List<Region>();
                        List<Region> mergedRegions = new List<Region>();
                        List<Region> nextStartCandidates = new List<Region>();
                        List<StarSystem> pool = new List<StarSystem>();
                        int i;

                        //loop
                        //starting with one random spawn region
                        //merge (factor adjacent spawn regions) into result
                        //make one constellation in result
                        //until remaining spawn regions number less than factor

                        System.Diagnostics.Trace.WriteLine("Using topology to try grouping " + factor.ToString() + " Spawn Regions in each Constellation");

                        start = spawnRegions.ElementAt(GalaxyGeneratorPlugin.random.Next(spawnRegions.Count));
                        while ((spawnRegions.Count >= factor) && (start != null) && (Galaxy.Instance.Constellations.Count < nConstellations))
                        {
                            i = 0;
                            spawnRegions.Remove(start);
                            mergedRegions.Clear();
                            mergedRegions.Add(start);
                            adjacentSpawnRegions.AddRange(start.adjacentRegions().FindAll((r) => { return r.isSpawn(); }));
                            adjacentSpawnRegions.RemoveAll((r) => { return !spawnRegions.Contains(r); });
                            for (; (i < factor-1) && (adjacentSpawnRegions.Count > 0); i++)
                            {
                                adjacentSpawnRegions.Clear();
                                foreach (Region r in mergedRegions) adjacentSpawnRegions.AddRange(r.adjacentRegions());
                                adjacentSpawnRegions.RemoveAll((r) => { return mergedRegions.Contains(r); });
                                adjacentSpawnRegions.RemoveAll((r) => { return !r.isSpawn(); });
                                adjacentSpawnRegions.RemoveAll((r) => { return !spawnRegions.Contains(r); });
                                if (adjacentSpawnRegions.Count > 0)
                                {
                                    merge = adjacentSpawnRegions.ElementAt(GalaxyGeneratorPlugin.random.Next(adjacentSpawnRegions.Count));
                                    mergedRegions.Add(merge);
                                    spawnRegions.Remove(merge);
                                }
                            }

                            System.Diagnostics.Trace.WriteLine("Merging regions :");
                            foreach (Region r in mergedRegions)
                                System.Diagnostics.Trace.WriteLine("--->"+r.Index.ToString());

                            pool.Clear();
                            foreach (Region r in mergedRegions) pool.AddRange(r);
                            this.MakeConstellations(1, pool);

                            nextStartCandidates.Clear();
                            foreach(Region r in mergedRegions) nextStartCandidates.AddRange(r.adjacentRegions());
                            nextStartCandidates.RemoveAll((r) => { return mergedRegions.Contains(r); });
                            nextStartCandidates.RemoveAll((r) => { return !r.isSpawn(); });
                            nextStartCandidates.RemoveAll((r) => { return !spawnRegions.Contains(r); });

                            if (nextStartCandidates.Count > 0)
                                start = nextStartCandidates.ElementAt(GalaxyGeneratorPlugin.random.Next(nextStartCandidates.Count));
                            else if (spawnRegions.Count > 0)
                                start = spawnRegions.ElementAt(GalaxyGeneratorPlugin.random.Next(spawnRegions.Count));
                            else
                                start = null;
                        }
                        
                        //merge (remaining spawn regions with neutral regions) into result
                        //make (nConstellations - Galaxy.Instance.Constellations.Count) constellations in result
                        System.Diagnostics.Trace.WriteLine("Merging remaining Spawn Regions with Neutral Regions");
                        System.Diagnostics.Trace.WriteLine("and making remaining Constellations");
                        pool.Clear();
                        foreach (Region r in spawnRegions) pool.AddRange(r);
                        foreach (Region r in neutralRegions) pool.AddRange(r);
                        this.MakeConstellations(nConstellations - Galaxy.Instance.Constellations.Count, pool);
                    }
                }

                if (Galaxy.Instance.Constellations.Count == 0)
                {
                    System.Diagnostics.Trace.WriteLine("Failing to associate regions and constellations");
                    System.Diagnostics.Trace.WriteLine("Creating brutally " + nConstellations.ToString() + " constellations with " + Galaxy.Instance.Stars.Count.ToString());
                    this.MakeConstellations(nConstellations, Galaxy.Instance.Stars);
                    this.Defects.Add("Unable to correlate regions and constellations");
                }

                this.AggregateIsolatedStars();
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }

        protected void MakeConstellations(int quantity, List<StarSystem> pool)
        {
            if (quantity <= 0) return;
            if (null == pool) return;
            if (pool.Count <= 0) return;

            PointF center = new PointF(0, 0);
            PointF delta, nearestFocus, diametralStar;
            float d, dMax, dMin, angle;
            Dictionary<PointF, Constellation> constellations = new Dictionary<PointF,Constellation>();
            float i, startAngle, poolRadius;
            int modifiedQuantity;

            System.Diagnostics.Trace.WriteLine("Try making " + quantity.ToString() + " constellations with a total of " + pool.Count.ToString() + " stars");
            modifiedQuantity = quantity;
            while ((modifiedQuantity > 1) && (modifiedQuantity * Settings.Instance.generationConstraints.minStarsPerConstellation > pool.Count)) modifiedQuantity--;
            System.Diagnostics.Trace.WriteLine("Make " + modifiedQuantity.ToString() + " constellations with a total of " + pool.Count.ToString() + " stars");

            //Computing center of gravity of pool
            foreach (StarSystem s in pool)
            {
                center.X += s.position.X / (float)(pool.Count);
                center.Y += s.position.Y / (float)(pool.Count);
            }

            //Computing pool radius
            poolRadius = 0;
            foreach (StarSystem s in pool)
            {
                d = Geometry2D.Distance(center, s.position);
                if (d > poolRadius) poolRadius = d;
            }
            poolRadius = poolRadius * (float)1.1;

            //Looking for a diametral star to establish startAngle
            dMax = 0;
            diametralStar = new PointF(center.X, center.Y);
            foreach (StarSystem s in pool)
            {
                StarSystem farthest = StarSystem.FindFarthestStar(s, pool);
                d = -1;
                if (farthest != null)
                    d = (float)(s.directDistanceTable[farthest]);
                if (d > dMax)
                {
                    dMax = d;
                    diametralStar = s.position;
                }
            }
            startAngle = Geometry2D.Bearing(center, diametralStar);
            
            //Preparing focuses and preparing associated constellations
            delta = new PointF();
            for (i = 0; i < modifiedQuantity; i++ )
            {
                angle = i * 360 / (float)(modifiedQuantity) + startAngle;
                Geometry2D.FromPolar(ref delta, poolRadius, angle);
                nearestFocus = new PointF(delta.X + center.X, delta.Y + center.Y);
                constellations.Add(nearestFocus, new Constellation());
            }

            //Associating focuses with stars
            List<StarSystem> countdownPool = new List<StarSystem>(pool);

            //grabbing closest star to seed constellations
            foreach (PointF p in constellations.Keys)
            {
                StarSystem closest = null;
                dMin = poolRadius;
                foreach (StarSystem s in countdownPool)
                {
                    d = Geometry2D.Distance(s.position, p);
                    if (d < dMin)
                    {
                        dMin = d;
                        closest = s;
                    }
                }
                if (closest != null) constellations[p].Add(closest);
            }

            this.FindAndFeedStarvedConstellations(constellations.Values.ToList(), ref countdownPool);

            //filling up constellations
            foreach (StarSystem s in countdownPool)
            {
                dMin = poolRadius;
                nearestFocus = constellations.Keys.First();
                foreach (PointF p in constellations.Keys)
                {
                    d = Geometry2D.Distance(s.position, p);
                    if (d < dMin)
                    {
                        dMin = d;
                        nearestFocus = p;
                    }
                }
                constellations[nearestFocus].Add(s);
            }
        }

        protected void FindAndFeedStarvedConstellations(List<Constellation> constellations, ref List<StarSystem> pool)
        {
            List<Constellation> starvedConstellations = new List<Constellation>();

            starvedConstellations.AddRange(constellations.FindAll((c) => { return c.Count < Settings.Instance.generationConstraints.minStarsPerConstellation; }));
            starvedConstellations.RemoveAll((c) => { return c.Count <= 0; });

            while ((starvedConstellations.Count > 0) && (pool.Count > 0))
            {
                foreach (Constellation candidate in starvedConstellations)
                {
                    if ((pool.Count > 0) && (candidate.Count < Settings.Instance.generationConstraints.minStarsPerConstellation))
                    {
                        System.Diagnostics.Trace.WriteLine("Remaining " + starvedConstellations.Count.ToString() + " starved constellations !");
                        List<StarSystem> pair = new List<StarSystem>(ConstellationBuilder.FindClosestPair(pool, candidate));
                        StarSystem closest = null;
                        if (pair.Count > 0) closest = pair[0];
                        if (closest != null)
                        {
                            candidate.Add(closest);
                            pool.Remove(closest);
                        }

                    }
                }
                starvedConstellations.RemoveAll((c) => { return c.Count >= Settings.Instance.generationConstraints.minStarsPerConstellation; });
            }
        }

        protected void AggregateIsolatedStars()
        {
            List<StarSystem> isolated = new List<StarSystem>();
            List<StarSystem> initiallyIsolated = new List<StarSystem>();
            List<StarSystem> others = new List<StarSystem>();
            Constellation candidate;
            StarSystem star, closest;
            Dictionary<StarSystem, Constellation> takers = new Dictionary<StarSystem, Constellation>();

            isolated.AddRange(Galaxy.Instance.Stars.FindAll((s) => { return s.constellation() == null; }));

            this.FindAndFeedStarvedConstellations(Galaxy.Instance.Constellations, ref isolated);

            initiallyIsolated.AddRange(isolated);
            while (isolated.Count > 0)
            {
                System.Diagnostics.Trace.WriteLine("Aggregating " + isolated.Count.ToString() + " Stars to existing Constellations");

                others.Clear();
                others.AddRange(Galaxy.Instance.Stars);
                others.RemoveAll((s) => { return initiallyIsolated.Contains(s); });

                star = isolated.ElementAt(GalaxyGeneratorPlugin.random.Next(isolated.Count));
                candidate = null;
                while ((candidate == null) && (others.Count > 0))
                {
                    closest = WarpBuilder.FindClosest(star, others);
                    if (closest != null)
                    {
                        others.Remove(closest);
                        if (closest.constellation().presentRegionIndexes().Contains(star.regionIndex))
                            candidate = closest.constellation();
                    }
                }

                if (candidate != null)
                {
                    takers.Add(star, candidate);
                }
                else
                {
                    others.Clear();
                    others.AddRange(Galaxy.Instance.Stars);
                    others.RemoveAll((s) => { return initiallyIsolated.Contains(s); });

                    closest = WarpBuilder.FindClosest(star, others);
                    takers.Add(star, closest.constellation());
                }

                isolated.RemoveAll((s) => { return takers.ContainsKey(s); });
            }

            foreach (StarSystem s in takers.Keys) takers[s].Add(s);
        }

        static public List<StarSystem> FindClosestPair(List<StarSystem> listA, List<StarSystem> listB)
        {
            List<StarSystem> pair = new List<StarSystem>();

            if (listA == null) return pair;
            if (listB == null) return pair;
            if (listA.Count <= 0) return pair;
            if (listB.Count <= 0) return pair;

            double d, dMin;

            pair.Add(null);
            pair.Add(null);
            dMin = Galaxy.Instance.Diameter() * 2;
            foreach (StarSystem a in listA)
            {
                foreach (StarSystem b in listB)
                {
                    d = Geometry2D.Distance(a.position, b.position);
                    if (d < dMin)
                    {
                        dMin = d;
                        pair[0] = a;
                        pair[1] = b;
                    }
                }
            }

            if ((null == pair[0]) || (null == pair[1])) pair.Clear();

            return pair;
        }
    }
}

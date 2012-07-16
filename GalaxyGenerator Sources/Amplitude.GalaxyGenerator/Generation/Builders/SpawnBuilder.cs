using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Components;

    public class SpawnBuilder : Builder
    {
        override public string Name { get { return "SpawnBuilder"; } }

        public SpawnBuilder() : base()
        {
        }

        override public void Execute()
        {
            List<Region> spawnRegions = new List<Region>(Galaxy.Instance.Regions.FindAll((r) => { return r.isSpawn(); }));
            int nEmpires = Galaxy.Instance.Configuration.empiresNumber();
            List<StarSystem> interdicted = new List<StarSystem>();
            List<StarSystem> candidates = new List<StarSystem>();
            StarSystem best;
            int maxConnections;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");
            System.Diagnostics.Trace.WriteLine("Spawn generation with " + nEmpires.ToString() + " empires");

            spawnRegions.RemoveAll((r) => { return r.Count <= 0; });

            Region spawner = SpawnBuilder.FindNextSpawner(null);

            while ((spawnRegions.Count > 0) && (nEmpires > 0))
            {
                candidates.Clear();
                candidates.AddRange(spawner);
                candidates.RemoveAll((s) => { return Galaxy.Instance.SpawnStars.Contains(s); });
                if (interdicted.Count((s) => { return spawner.Contains(s); }) <= candidates.Count)
                {
                    candidates.RemoveAll((s) => { return interdicted.Contains(s); });
                }
                if (spawner.Count((s) => { return s.destinations.Count <= 1; }) < candidates.Count)
                {
                    candidates.RemoveAll((s) => { return s.destinations.Count <= 1; });
                }

                maxConnections = 0;
                foreach (StarSystem s in candidates)
                {
                    int connections = SpawnBuilder.CountInterestingConnections(s);
                    if (connections > maxConnections)
                    {
                        maxConnections = connections;
                    }
                }

                candidates.RemoveAll((s) => { return maxConnections > SpawnBuilder.CountInterestingConnections(s); });

                best = this.FindFarthest(candidates, Galaxy.Instance.SpawnStars);

                if (best != null)
                {
                    Galaxy.Instance.SpawnStars.Add(best);
                    nEmpires--;

                    //interdict all proximate stars for subsequent spawns
                    interdicted.AddRange(Galaxy.Instance.Stars.FindAll((s) => { return Geometry2D.Distance(best.position, s.position) < Settings.Instance.generationConstraints.minEmpireDistance; }));
                }

                spawnRegions.Remove(spawner);
                spawner = SpawnBuilder.FindNextSpawner(spawner);
            }

            List<StarSystem> downgradedCandidates = new List<StarSystem>();

            if (nEmpires > 0)
            {
                spawnRegions.Clear();
                //take all spawners
                spawnRegions.AddRange(Galaxy.Instance.Regions.FindAll((r) => { return r.isSpawn(); }));
                //remove all already taken spawn regions
                spawnRegions.RemoveAll((r) =>
                    {
                        return 0 < Galaxy.Instance.SpawnStars.Count((s) => { return s.region.Index == r.Index; });
                    });
                //restart spawn region sequence
                spawner = SpawnBuilder.FindNextSpawner(null);
                while (!spawnRegions.Contains(spawner)) spawner = SpawnBuilder.FindNextSpawner(spawner);

                this.Defects.Add("Using downgraded spawn algorithm");
            }
            while (nEmpires > 0)
            {
                System.Diagnostics.Trace.WriteLine("Downgraded Spawn Algorithms - Downgraded spawns remaining : " + nEmpires.ToString());

                downgradedCandidates.Clear();
                downgradedCandidates.AddRange(spawner);
                downgradedCandidates.RemoveAll((s) => { return Galaxy.Instance.SpawnStars.Contains(s); });
                if (downgradedCandidates.Count == 0)
                {
                    downgradedCandidates.AddRange(Galaxy.Instance.Stars);
                }
                downgradedCandidates.RemoveAll((s) => { return Galaxy.Instance.SpawnStars.Contains(s); });
                best = this.FindFarthest(downgradedCandidates, Galaxy.Instance.SpawnStars);
                if (null == best)
                {
                    System.Diagnostics.Trace.WriteLine("FAILED TO SPAWN");
                    this.Result = false;
                    return;
                }
                Galaxy.Instance.SpawnStars.Add(best);
                nEmpires--;
                while (!spawnRegions.Contains(spawner)) spawner = SpawnBuilder.FindNextSpawner(spawner);
            }

            System.Diagnostics.Trace.WriteLine("Spawn Builder placed " + Galaxy.Instance.SpawnStars.Count.ToString() + " empires");

            if (Galaxy.Instance.SpawnStars.Count < Galaxy.Instance.Configuration.empiresNumber())
            {
                this.TraceDefect("Failed to spawn - Not enough empires were spawned", true);
                return;
            }

            //Shuffle spawn stars
            List<StarSystem> sourceSpawn = new List<StarSystem>(Galaxy.Instance.SpawnStars);
            StarSystem star;
            Galaxy.Instance.SpawnStars.Clear();
            while (sourceSpawn.Count > 0)
            {
                star = sourceSpawn.ElementAt(GalaxyGeneratorPlugin.random.Next(sourceSpawn.Count));
                Galaxy.Instance.SpawnStars.Add(star);
                sourceSpawn.Remove(star);
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }

        public static Region FindNextSpawner(Region reg)
        {
            Color c;
            List<Color> seq = new List<Color>(Galaxy.Instance.Configuration.shape().spawnerSequence);
            Color next;

            if (seq.Count <= 0)
                return null;

            next = seq.First();
            if (null != reg)
            {
                c = reg.Index;
                if (seq.Contains(c))
                    next = seq.ElementAt((1 + seq.IndexOf(c)) % seq.Count);
            }

            return Galaxy.Instance.Regions.Find((r) => { return r.Index == next;});
        }

        protected StarSystem FindFarthest(List<StarSystem> candidates, List<StarSystem> repellents)
        {
            if (null == candidates) return null;
            if (candidates.Count <= 0) return null;

            List<StarSystem> localRepellents = new List<StarSystem>();
            if (null != repellents) localRepellents.AddRange(repellents);

            if ((localRepellents.Count <= 0) && (candidates.Count > 0))
            {
                return candidates.ElementAt(GalaxyGeneratorPlugin.random.Next(candidates.Count));
            }

            double d, dMax;
            StarSystem farthest = null;

            dMax = 0;
            foreach (StarSystem c in candidates)
            {
                foreach (StarSystem r in localRepellents)
                {
                    d = Geometry2D.Distance(c.position, r.position);
                    if (d > dMax)
                    {
                        dMax = d;
                        farthest = c;
                    }
                }
            }

            return farthest;
        }

        static public int CountInterestingConnections(StarSystem star)
        {
            return star.warps().Count((w) =>
            {
                return (!w.isWormhole)
                    && (    (w.starA.region == w.starB.region)
                            || (!w.starA.region.isSpawn())
                            || (!w.starB.region.isSpawn()) 
                       );
            }
            );
        }
    }
}

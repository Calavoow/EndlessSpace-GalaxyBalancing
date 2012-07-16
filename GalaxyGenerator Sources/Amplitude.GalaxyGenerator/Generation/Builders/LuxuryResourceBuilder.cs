using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Amplitude.GalaxyGenerator.Generation.Components;

    class LuxuryResourceBuilder : Builder
    {
        public override string Name { get { return "LuxuryResourceBuilder"; } }

        public override void Execute()
        {
            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            int nSites, qty, i, r;
            List<Planet> sites = new List<Planet>();
            HashSet<Planet> usedSites = new HashSet<Planet>();
            HashSet<string> usedLuxes = new HashSet<string>();
            Planet selP;
            //          StarSystem sourceCenter;
            int tier, minTier, maxTier;

            minTier = Settings.Instance.luxuryResourceTiers.Keys.ToList().Min();
            maxTier = Settings.Instance.luxuryResourceTiers.Keys.ToList().Max();

            qty = Galaxy.Instance.Configuration.luxuryResourceNumberOfTypes();

            System.Diagnostics.Trace.WriteLine("total luxes = " + Settings.Instance.luxuryResourceNames.Count.ToString());
            System.Diagnostics.Trace.WriteLine("number of lux types " + qty.ToString());
            tier = minTier;
            do
            {
                //System.Diagnostics.Trace.WriteLine("tier " + tier.ToString() + " usedLuxes.Count=" +usedLuxes.Count.ToString());
                usedLuxes.Add(Galaxy.Instance.Configuration.getRandomLuxuryResource(tier));
                do
                {
                    tier++;
                    if (tier > maxTier)
                        tier = minTier;
                }
                while (!Settings.Instance.luxuryResourceTiers.ContainsKey(tier));
            }
            while ((usedLuxes.Count < qty) && (usedLuxes.Count < Settings.Instance.luxuryResourceNames.Count));

            //System.Diagnostics.Trace.WriteLine("compiled usedLuxes");

            foreach (string resName in usedLuxes)
            {
                //sourceCenter = Galaxy.Instance.stars[];
                qty = 7;
                //nSites = qMax (hConstel.count(), int (0.3 * double (qty)));
                nSites = 4;
                //qty = qMax (qty, nSites);
                sites.Clear();
                usedSites.Clear();
                foreach (StarSystem sys in Galaxy.Instance.Stars)
                {
                    foreach (Planet p in sys.Planets)
                    {
                        if (p.resource == null)
                        {
                            i = 0;
                            for (; i < Settings.Instance.planetLuxuriesPerPlanetType[p.type][resName]; i++)
                                sites.Add(p);
                        }
                    }
                }
                i = 0;
                if (sites.Count == 0) qty = 0;
                if (sites.Distinct().Count() < 2) qty = 0;
                for (; i < qty; i++)
                {
                    selP = null;
                    if (usedSites.Count < nSites)
                    {
                        if (usedSites.Count < 2)
                        {
                            do
                            {
                                r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                                selP = sites[r];
                            }
                            while (usedSites.Contains(selP));
                        }
                        else
                        {
                            r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                            selP = sites[r];
                        }
                        usedSites.Add(selP);
                    }
                    else
                    {
                        r = GalaxyGeneratorPlugin.random.Next(usedSites.Count);
                        selP = usedSites.ToList()[r];
                    }
                    if (selP != null)
                    {
                        if (selP.resource == null)
                            selP.resource = new ResourceDeposit(resName, 1, ResourceDeposit.ResourceType.Luxury);
                        else if (selP.resource.size < ResourceDeposit.MaxSize)
                            selP.resource.increaseSize();
                    }
                }
            }

            //double-checking produced luxes
            {
                SortedDictionary<string, int> spawnedLuxQuantities = new SortedDictionary<string, int>();

                foreach (string resName in Settings.Instance.luxuryResourceNames)
                {
                    spawnedLuxQuantities.Add(resName, 0);
                }

                foreach (Planet p in Galaxy.Instance.Planets)
                {
                    if (p.HasResource)
                    {
                        if (p.resource.type == ResourceDeposit.ResourceType.Luxury)
                        {
                            spawnedLuxQuantities[p.PresentResourceName] += p.resource.size;
                        }
                    }
                }

                foreach (string resName in spawnedLuxQuantities.Keys)
                {
                    if (spawnedLuxQuantities[resName] == 0)
                    {
                        this.TraceDefect(resName + " could not be spawned");
                    }
                    else if (spawnedLuxQuantities[resName] < 4)
                    {
                        this.TraceDefect(resName + " spawned quantity less than 4, removing...");
                        List<Planet> planets = new List<Planet>(Galaxy.Instance.Planets.FindAll((p) => { return p.PresentResourceName == resName; }));
                        foreach (Planet p in planets)
                        {
                            p.resource = null;
                        }
                    }
                }
            }

            //Checking tiers
            {
                HashSet<string> deposits = new HashSet<string>();

                foreach (Planet p in Galaxy.Instance.Planets)
                {
                    if (p.HasResource)
                    {
                        if (p.resource.type == ResourceDeposit.ResourceType.Luxury)
                        {
                            deposits.Add(p.resource.name);
                        }
                    }
                }

                System.Diagnostics.Trace.WriteLine("Luxuries actually deposited - begin");
                foreach (string s in deposits) System.Diagnostics.Trace.WriteLine(s);
                System.Diagnostics.Trace.WriteLine("Luxuries actually deposited - end");
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    class StrategicResourceBuilder : Builder
    {
        override public string Name { get { return "StrategicResourceBuilder"; } }

        public StrategicResourceBuilder() : base()
        {
            this.remainingQuantities = new SortedDictionary<string, int>();
            this.shuffledPlanets = new List<Planet>();
            this.shuffledResources = new List<string>();
            this.sortedDepositSizes = new List<int>();
        }

        override public void Execute()
        {
            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            if (Galaxy.Instance.Planets.Count < Settings.Instance.strategicResourceNames.Count)
            {
                this.TraceDefect("Less planets than strategic resource types", true);
                return;
            }

            List<Planet> sourcePlanets = new List<Planet>(Galaxy.Instance.Planets);
            while (sourcePlanets.Count > 0)
            {
                Planet p = sourcePlanets.ElementAt(GalaxyGeneratorPlugin.random.Next(sourcePlanets.Count));
                shuffledPlanets.Add(p);
                sourcePlanets.Remove(p);
            }

            foreach (string s in Settings.Instance.strategicResourceNames)
            {
                this.remainingQuantities.Add(s, Galaxy.Instance.Configuration.strategicResourceNumberPerType());
            }

            //Sort deposit iterations according to Settings file
            {
                List<int> sortedIterationIndices = new List<int>();
                sortedIterationIndices.AddRange(Settings.Instance.resourceDepositSizeIterations.Keys);

                if (sortedIterationIndices.Count == 0)
                {
                    this.TraceDefect("Found empty iteration table for strategic resource deposits", true);
                    return;
                }

                sortedIterationIndices.Sort();

                foreach (int nIteration in sortedIterationIndices)
                {
                    this.sortedDepositSizes.Add(Settings.Instance.resourceDepositSizeIterations[nIteration]);
                }
            }

            //Shuffle resources
            {
                List<string> sourceResources = new List<string>(Settings.Instance.strategicResourceNames);
                while (sourceResources.Count > 0)
                {
                    string r = sourceResources.ElementAt(GalaxyGeneratorPlugin.random.Next(sourceResources.Count));
                    this.shuffledResources.Add(r);
                    sourceResources.Remove(r);
                }
            }

            int depositSize;
            bool firstAllocation = true;
            bool oneDepositPlaced = true;
            while ((this.remainingQuantities.Values.ToList().Count((i) => { return i > 0; }) > 0) && oneDepositPlaced)
            {
                foreach (int rawDepositSize in this.sortedDepositSizes)
                {
                    depositSize = rawDepositSize;

                    if (depositSize < 0)
                    {
                        depositSize = 1;
                        this.TraceDefect("Found negative deposit size in iteration - defaulted to 1");
                    }

                    if (depositSize > ResourceDeposit.MaxSize) this.TraceDefect("Found too large deposit size in iteration - left unchanged");

                    if (firstAllocation)
                    {
                        firstAllocation = false;
                        if (!this.DoFirstAllocation(depositSize))
                        {
                            this.TraceDefect("Failed to allocate first batch of strategic resources", true);
                            return;
                        }
                    }
                    else
                    {
                        oneDepositPlaced = false;
                        foreach (string res in this.shuffledResources)
                        {
                            int localMaxSize = this.remainingQuantities[res];
                            int localDepositSize = depositSize;
                            if (depositSize > localMaxSize) localDepositSize = localMaxSize;

                            Planet deposit = shuffledPlanets.Find((p) => { return p.CanAcceptStrategicResource(res); });
                            if ((null != deposit) && (localDepositSize > 0))
                            {
                                oneDepositPlaced = true;
                                shuffledPlanets.Remove(deposit);
                                deposit.resource = new ResourceDeposit(res, localDepositSize, ResourceDeposit.ResourceType.Strategic);
                                this.remainingQuantities[res] -= localDepositSize;
                            }
                        }
                    }
                }
            }

            foreach (string res in Settings.Instance.strategicResourceNames)
            {
                if (Galaxy.Instance.Planets.Find((p) => { if (p.HasResource) return p.resource.name == res; else return false; }) == null)
                {
                    this.TraceDefect("Unable to place a single deposit of " + res, true);
                    return;
                }
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }

        protected SortedDictionary<string, int> remainingQuantities;
        protected List<Planet> shuffledPlanets;
        protected List<string> shuffledResources;
        protected List<int> sortedDepositSizes;

        protected bool DoFirstAllocation(int depositSize)
        {
            SortedDictionary<string, Planet> sites = new SortedDictionary<string, Planet>();

            foreach (Planet firstPlanet in this.shuffledPlanets)
            {
                List<Planet> rotatedPlanets = new List<Planet>(this.shuffledPlanets);
                rotatedPlanets.RemoveRange(0, shuffledPlanets.IndexOf(firstPlanet));
                rotatedPlanets.AddRange(shuffledPlanets.GetRange(0, shuffledPlanets.IndexOf(firstPlanet)));

                sites.Clear();
                foreach (string res in this.shuffledResources)
                {
                    Planet testSite = rotatedPlanets.FirstOrDefault((p) =>
                                {
                                    return (p.CanAcceptStrategicResource(res) && !sites.Values.Contains(p));
                                });
                    if (null == testSite)
                    {
                        testSite = rotatedPlanets.FirstOrDefault((p) =>
                            {
                                return !sites.Values.Contains(p);
                            });
                    }
                    
                    if (null != testSite) sites.Add(res, testSite);
                }

                if (sites.Keys.Count == this.shuffledResources.Count)
                {
                    foreach (string res in this.shuffledResources)
                    {
                        sites[res].resource = new ResourceDeposit(res, depositSize, ResourceDeposit.ResourceType.Strategic);
                        this.shuffledPlanets.Remove(sites[res]);
                        this.remainingQuantities[res] -= depositSize;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}

// <copyright file="Configuration.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Components;

    public class Configuration : Dictionary<string, string>
    {
        public List<HomeGenerator> homeGenerators { get; protected set; }

        public void ResetNames()
        {
            this.takenConstellationNames.Clear();
            this.takenStarNames.Clear();
        }

        public Configuration(string xmlFileName)
        {
            XmlTextReader xr = new XmlTextReader(xmlFileName);
            string n, t;
            HomeGenerator hg;

            homeGenerators = new List<HomeGenerator>();

            this.takenConstellationNames = new HashSet<string>();
            this.takenStarNames = new HashSet<string>();

            this.seed = 0;

            while (xr.Read())
            {
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name != "GenerationSettings"))
                {
                    n = xr.Name;

                    if ((n == "HomeGeneration") && (! xr.IsEmptyElement))
                    {
                        do
                        {
                            xr.Read();
                            if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Empire"))
                            {
                                hg = new HomeGenerator();
                                this.homeGenerators.Add(hg);

                                do
                                {
                                    xr.Read();
                                    if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Trait"))
                                    {
                                        //t = xr.ReadElementContentAsString();
                                        t = xr.GetAttribute("Name");
                                        if (Settings.Instance.homeGenerationTraitsNames.Contains(t))
                                            hg.Add(Settings.Instance.homeGenerationTraits[t]);
                                    }
                                }
                                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Empire")));
                            }
                        }
                        while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "HomeGeneration")));
                    }
                    else if ((n == "Seed") && (! xr.IsEmptyElement))
                    {
                        this.seed = xr.ReadElementContentAsInt();
                    }
                    else if (! xr.IsEmptyElement)
                    {
                        t = xr.ReadElementContentAsString();
                        this.Add(n, t);
                    }
                }
            }

//            foreach (string elt in this.Keys)
//                System.Diagnostics.Trace.WriteLine(elt + " : " + this[elt]);
            System.Diagnostics.Trace.WriteLine("checking Configuration...");
            System.Diagnostics.Trace.WriteLine("Seed : " + this.seed.ToString());
            System.Diagnostics.Trace.WriteLine("Shape : " + this.shape().densityFileName);
            System.Diagnostics.Trace.WriteLine("Empires : " + this.empiresNumber());
            System.Diagnostics.Trace.WriteLine("Galaxy size : " + this["GalaxySize"]);
            System.Diagnostics.Trace.WriteLine("Galaxy age : " + this["GalaxyAge"]);
            System.Diagnostics.Trace.WriteLine("Galaxy density : " + this["GalaxyDensity"]);
            System.Diagnostics.Trace.WriteLine("Constellations : " + this.constellations().ToString());
            System.Diagnostics.Trace.WriteLine("Star connectivity : " + this.connectivity().ToString());
            System.Diagnostics.Trace.WriteLine("Resources : " + this["ResourceRepartitionFactor"]);
            System.Diagnostics.Trace.WriteLine("Expected population : " + this.population().ToString());
            System.Diagnostics.Trace.WriteLine("HomeWorldGenerators Count : " + this.homeGenerators.Count.ToString());
            System.Diagnostics.Trace.WriteLine("...end checking Configuration");
        }

        public int seed { get; set; }

        public void WriteOuterXml (XmlWriter xw)
        {
            xw.WriteStartElement("CrossCheckingSettings");
            foreach (string k in Keys)
                xw.WriteElementString(k, this[k]);
            xw.WriteElementString("Seed", this.seed.ToString());
            xw.WriteEndElement();
        }

        public int empiresNumber()
        {
            if (this.Keys.Contains("EmpiresNumber"))
            {
                if (this["EmpiresNumber"].Contains("EmpiresNumber"))
                    return System.Int32.Parse(this["EmpiresNumber"].Remove(this["EmpiresNumber"].IndexOf("EmpiresNumber"), this["EmpiresNumber"].Length));
                else
                    return System.Int32.Parse(this["EmpiresNumber"]);
            }
            else if (Settings.Instance.galaxySizes[this["GalaxySize"]].nominalPlayers > 0)
                return Settings.Instance.galaxySizes[this["GalaxySize"]].nominalPlayers;
            else if (this.shape().minEmpires > 0)
                return this.shape().minEmpires;
            else if (this.shape().maxEmpires > 0)
                return this.shape().maxEmpires;
            else
                return 4;
        }

        public Shape shape() {return ShapeManager.Instance.Shapes[this["GalaxyShape"]];}

        public int population()
        {
            string s = this["GalaxySize"];
            int n = Settings.Instance.galaxySizes[s].numStars;
            string t = this["GalaxyDensity"];
            double d = Settings.Instance.galaxyDensities[t];
            return (int)((double)n * d);
            //return (int) ((double)(GenerationData.instance.galaxySizes[this["GalaxySize"]].numStars) * GenerationData.instance.galaxyDensities[this["GalaxyDensity"]]);
        }

        public int constellations()
        {
            string n;

            n = this["ConstellationNumber"];

            if (n == "ConstellationNumberNone")
                return 1;
            else if (n == "ConstellationNumberFew")
            {
                if (this.shape().minConstellations > 0)
                    return this.shape().minConstellations;
                else
                    return this.empiresNumber();
            }
            else if (n == "ConstellationNumberMany")
            {
                if (this.shape().maxConstellations > 0)
                    return this.shape().maxConstellations;
                else
                    return 2 * this.empiresNumber();
            }

            return 1;
        }
        
        public double connectivity()
        {
            return Settings.Instance.starConnectivities[this["StarConnectivity"]];
        }

        public double wormholeConnectivity()
        {
            return Settings.Instance.constellationConnectivities[this["ConstellationConnectivity"]];
        }

        public double maxWidth()
        {
            return Settings.Instance.galaxySizes[this["GalaxySize"]].width;
        }

        public double starOverlapDistance()
        {
            return Settings.Instance.generationConstraints.minStarDistance;
        }

        public System.Drawing.Bitmap densityImage()
        {
            return this.shape().densityMap;
        }

        public System.Drawing.Bitmap regionsImage()
        {
            return this.shape().regionsMap;
        }

        public int strategicResourceNumberPerType()
        {
            return (int) (100.0 * (double) (Settings.Instance.galaxySizes[this["GalaxySize"]].strategicResourceNumberPerType)
                    * Settings.Instance.resourceRepartitionFactors[this["ResourceRepartitionFactor"]]) / 100;
        }

        public int luxuryResourceNumberOfTypes()
        {
            return (int) (100.0 * (double) (Settings.Instance.galaxySizes[this["GalaxySize"]].luxuryResourceTypes)
                    * Settings.Instance.resourceRepartitionFactors[this["ResourceRepartitionFactor"]]) / 100;
        }

        public string getRandomStarName()
        {
            string s;

            if (Settings.Instance.starNames.Count == 0)
                return "";

            if (takenStarNames == Settings.Instance.starNames)
                takenStarNames.Clear();

            do
            {
                s = selectRandom(Settings.Instance.starNames);
            }
            while (takenStarNames.Contains(s));

            takenStarNames.Add(s);

            return s;
        }

        public string getRandomConstellationName()
        {
            string s;

            if (Settings.Instance.constellationNames.Count == 0)
                return "";

            if (takenConstellationNames == Settings.Instance.constellationNames)
                takenConstellationNames.Clear();

            do
            {
                s = selectRandom(Settings.Instance.constellationNames);
            }
            while (takenConstellationNames.Contains(s));

            takenConstellationNames.Add(s);

            return s;
        }

        public string getRandomStarType()
        {
            Dictionary<string, int> hw;
            int n, d, r;

            hw = new Dictionary<string,int>(Settings.Instance.galaxyAges[this["GalaxyAge"]].starTypeWeightTable);

            n = 0;
            foreach (string s in hw.Keys)
                n += hw[s];

            if (n == 0)
                return "";

            d = GalaxyGeneratorPlugin.random.Next(n)+1;
            r = 0;
            foreach (string s in hw.Keys)
            {
                r += hw[s];
                if (d <= r)
                    return s;
            }
            return "InvalidStarType";
        }
        
        public int getRandomPlanetNumber()
        {
            Dictionary<int, int> h;
            int n, r, d;

            h = Settings.Instance.planetsPerSystems[this["PlanetsPerSystem"]];

            n = 0;
            foreach (int i in h.Keys)
                n += h[i];

            if (n == 0)
                return 0;

            d = GalaxyGeneratorPlugin.random.Next(n)+1;
            r = 0;
            foreach (int i in h.Keys)
            {
                r += h[i];
                if (d <= r)
                    return i;
            }
            return 0;
        }
        
        public string getRandomPlanetType(string starType)
        {
            Dictionary<string, int> h;
            int n, d, r;

            h = Settings.Instance.planetTypeProbabilitiesPerStar[starType];

            n = 0;
            foreach (string s in h.Keys)
                n += h[s];

            if (n == 0)
                return "";

            d = GalaxyGeneratorPlugin.random.Next(n)+1;
            r = 0;
            foreach (string s in h.Keys)
            {
                r += h[s];
                if (d <= r)
                    return s;
            }
            return "InvalidPlanetType";
        }
        
        public string getRandomPlanetSize(string planetType)
        {
            Dictionary<string, int> h = new Dictionary<string,int>();
            int n, d, r;

            foreach (string s in Settings.Instance.planetSizeProbabilitiesPerType[planetType].Keys)
            {
                h.Add(s, (int)(100 * (double)(Settings.Instance.planetSizeProbabilitiesPerType[planetType][s]) * Settings.Instance.planetsSizeFactors[this["PlanetsSizeFactor"]][s]));
            }

            n = 0;
            foreach (string s in h.Keys)
                n += h[s];

            if (n == 0)
                return "";

            d = GalaxyGeneratorPlugin.random.Next(n)+1;
            r = 0;
            foreach (string s in h.Keys)
            {
                r += h[s];
                if (d <= r)
                    return s;
            }
            return "InvalidPlanetSize";
        }
        
        public int getRandomMoonNumber(string planetType)
        {
            Dictionary<int, int> h;
            int n, d, r;

            if (Settings.Instance.moonNumberProbabilitiesPerPlanetType.Keys.Contains(planetType))
                h = Settings.Instance.moonNumberProbabilitiesPerPlanetType[planetType];
            else
                h = Settings.Instance.moonNumberChances;

            n = 0;
            foreach (int p in h.Keys)
                n += h[p];

            if (n == 0)
                return 0;

            d = GalaxyGeneratorPlugin.random.Next(n)+1;
            r = 0;
            foreach (int p in h.Keys)
            {
                r += h[p];
                if (d <= r)
                    return p;
            }
            return 0;
        }
        
        public string getRandomTempleType(string starType)
        {
                    Dictionary<string, int> h;
            int p, n, d, r, rt;

            p = Settings.Instance.templeChancesPerStarType[starType];
            rt = GalaxyGeneratorPlugin.random.Next(100)+1;

            if (rt <= p)
            {
                h = Settings.Instance.templeTypeProbabilities;

                n = 0;
                foreach (string s in h.Keys)
                    n += h[s];

                if (n == 0)
                    return "InvalidTempleType";

                d = GalaxyGeneratorPlugin.random.Next(n)+1;
                r = 0;
                foreach (string s in h.Keys)
                {
                    r += h[s];
                    if (d <= r)
                        return s;
                }
                return "InvalidTempleType";
            }

            return "NoTemple";
}

        public string getRandomAnomaly(string planetType)
        {
            int r, rt, t, tt;
            Dictionary<string, int> h;

            r = GalaxyGeneratorPlugin.random.Next(100)+1;

            if (r <= Settings.Instance.anomalyBaseChance)
            {
                h = Settings.Instance.planetAnomaliesPerPlanetType[planetType];
                t = 0;
                foreach (int p in h.Values)
                    t += Settings.Instance.planetAnomaliesProbabilityScale[p];
                if (t <= 0) return "";
                rt = GalaxyGeneratorPlugin.random.Next(t)+1;
                tt = 0;
                foreach (string a in h.Keys)
                {
                    tt += Settings.Instance.planetAnomaliesProbabilityScale[h[a]];
                    if (rt <= tt)
                        return a;
                }
            }

            return "";
}
        
        public string getRandomStrategicResource(string planetType)
        {
            int rt, t, tt;
            Dictionary<string, int> h;

            h = Settings.Instance.planetStrategicResourcesPerPlanetType[planetType];
            t = 0;
            foreach (int p in h.Values)
                t += Settings.Instance.planetStrategicResourceProbabilitiesScale[p];
            if (t <= 0) return "";
            rt = GalaxyGeneratorPlugin.random.Next(t)+1;
            tt = 0;
            foreach (string a in h.Keys)
            {
                tt += Settings.Instance.planetStrategicResourceProbabilitiesScale[h[a]];
                if (rt <= tt)
                    return a;
            }

            return "";
        }

        public string getRandomLuxuryResource(int priority)
        {
            return selectRandom(Settings.Instance.luxuryResourceTiers[priority]);
        }

        protected string selectRandom(HashSet<string> set)
        {
            return set.ElementAtOrDefault(GalaxyGeneratorPlugin.random.Next(set.Count));
        }

        protected HashSet<string> takenStarNames;
        protected HashSet<string> takenConstellationNames;
    }
}

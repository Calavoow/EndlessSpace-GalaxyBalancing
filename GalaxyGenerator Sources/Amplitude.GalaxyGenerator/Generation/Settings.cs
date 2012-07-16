// <copyright file="Settings.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class GalaxySize
    {
        public string name;
        public int numStars;
        public double width;
        public int nominalPlayers;
        public int strategicResourceNumberPerType;
        public int luxuryResourceTypes;
    };

    public class GalaxyAge
    {
        public string name;
        public Dictionary<string, int> starTypeWeightTable;
    };

    public class Constraints
    {
        public int minStarsPerConstellation;
        public double minStarDistance;
        public double minEmpireDistance;
//        public int maxWormholesConnections;
    };

    public class Settings
    {
        public static Settings Instance { get; private set; }
        public static void Load(string xmlFileName) { if (Instance == null) new Settings(xmlFileName); }

        public Dictionary<string, GalaxySize> galaxySizes { get; protected set; }
        public Dictionary<string, GalaxyAge> galaxyAges { get; protected set; }
        public HashSet<string> constellationNumbers { get; protected set; }
        public Dictionary<string, double> constellationConnectivities { get; protected set; }
        public Dictionary<string, double> starConnectivities { get; protected set; }
        public Dictionary<string, double> constellationDistances { get; protected set; }
        public Dictionary<string, double> galaxyDensities { get; protected set; }
        public Dictionary<string, Dictionary<int, int>> planetsPerSystems { get; protected set; }
        public Dictionary<string, Dictionary<string, double>> planetsSizeFactors { get; protected set; }
        public Dictionary<string, double> anomalyTempleFactors { get; protected set; }
        public Dictionary<string, double> resourceRepartitionFactors { get; protected set; }
        public Dictionary<int, int> resourceDepositSizeIterations { get; protected set; }
        public Dictionary<string, Dictionary<string, int>> planetTypeProbabilitiesPerStar { get; protected set; }
        public Dictionary<string, Dictionary<string, int>> planetSizeProbabilitiesPerType { get; protected set; }
        public Dictionary<int, int> moonNumberChances { get; protected set; }
        public Dictionary<string, Dictionary<int, int>> moonNumberProbabilitiesPerPlanetType { get; protected set; }
        public Dictionary<string, int> templeChancesPerStarType { get; protected set; }
        public Dictionary<int, int> planetAnomaliesProbabilityScale { get; protected set; }
        public Dictionary<string, Dictionary<string, int>> planetAnomaliesPerPlanetType { get; protected set; }
        public Dictionary<int, int> planetStrategicResourceProbabilitiesScale { get; protected set; }
        public Dictionary<string, Dictionary<string, int>> planetStrategicResourcesPerPlanetType { get; protected set; }
        public Dictionary<int, int> planetLuxuryProbabilitiesScale { get; protected set; }
        public Dictionary<string, Dictionary<string, int>> planetLuxuriesPerPlanetType { get; protected set; }
        public SortedDictionary<int, HashSet<string>> luxuryResourceTiers { get; protected set; }
        public Dictionary<string, int> templeTypeProbabilities { get; protected set; }
        public Constraints generationConstraints { get; protected set; }
        public HashSet<string> starNames { get; protected set; }
        public HashSet<string> constellationNames { get; protected set; }
        public HashSet<string> galaxySizeNames { get; protected set; }
        public HashSet<string> galaxyAgeNames { get; protected set; }
        public HashSet<string> constellationConnectivityNames { get; protected set; }
        public HashSet<string> starConnectivityNames { get; protected set; }
        public HashSet<string> constellationDistanceNames { get; protected set; }
        public HashSet<string> galaxyDensitiesNames { get; protected set; }
        public HashSet<string> planetsPerSystemsNames { get; protected set; }
        public HashSet<string> planetsSizeFactorNames { get; protected set; }
        public HashSet<string> planetSizeNames { get; protected set; }
        public HashSet<string> anomalyTempleFactorNames { get; protected set; }
        public HashSet<string> resourceRepartitionFactorNames { get; protected set; }
        public HashSet<string> starTypeNames { get; protected set; }
        public HashSet<string> planetTypeNames { get; protected set; }
        public HashSet<string> anomalyNames { get; protected set; }
        public HashSet<string> strategicResourceNames { get; protected set; }
        public HashSet<string> luxuryResourceNames { get; protected set; }
        public int anomalyBaseChance { get; protected set; }
        public Dictionary<string, HomeTrait> homeGenerationTraits { get; protected set; }
        public HashSet<string> homeGenerationTraitsNames { get; protected set; }

        ~Settings() { Instance = null; }

        protected Settings(string xmlFileName)
        {
            Instance = this;

            galaxySizes = new Dictionary<string, GalaxySize>();
            galaxyAges = new Dictionary<string, GalaxyAge>();
            constellationNumbers = new HashSet<string>();
            constellationConnectivities = new Dictionary<string,double>();
            starConnectivities = new Dictionary<string,double>();
            constellationDistances = new Dictionary<string,double>();
            galaxyDensities = new Dictionary<string,double>();
            planetsPerSystems = new Dictionary<string,Dictionary<int,int>>();
            planetsSizeFactors = new Dictionary<string,Dictionary<string,double>>();
            anomalyTempleFactors = new Dictionary<string,double>();
            resourceRepartitionFactors = new Dictionary<string,double>();
            resourceDepositSizeIterations = new Dictionary<int,int>();
            planetTypeProbabilitiesPerStar = new Dictionary<string,Dictionary<string,int>>();
            planetSizeProbabilitiesPerType = new Dictionary<string,Dictionary<string,int>>();
            moonNumberChances = new Dictionary<int,int>();
            moonNumberProbabilitiesPerPlanetType = new Dictionary<string,Dictionary<int,int>>();
            templeChancesPerStarType = new Dictionary<string,int>();
            planetAnomaliesProbabilityScale = new Dictionary<int,int>();
            planetAnomaliesPerPlanetType = new Dictionary<string,Dictionary<string,int>>();
            planetStrategicResourceProbabilitiesScale = new Dictionary<int,int>();
            planetStrategicResourcesPerPlanetType = new Dictionary<string,Dictionary<string,int>>();
            planetLuxuryProbabilitiesScale = new Dictionary<int,int>();
            planetLuxuriesPerPlanetType = new Dictionary<string,Dictionary<string,int>>();
            luxuryResourceTiers = new SortedDictionary<int, HashSet<string>>();
            templeTypeProbabilities = new Dictionary<string, int>();
            starNames = new HashSet<string>();
            constellationNames = new HashSet<string>();
            galaxySizeNames = new HashSet<string>();
            galaxyAgeNames = new HashSet<string>();
            constellationConnectivityNames = new HashSet<string>();
            starConnectivityNames = new HashSet<string>();
            constellationDistanceNames = new HashSet<string>();
            galaxyDensitiesNames = new HashSet<string>();
            planetsPerSystemsNames = new HashSet<string>();
            planetsSizeFactorNames = new HashSet<string>();
            planetSizeNames = new HashSet<string>();
            anomalyTempleFactorNames = new HashSet<string>();
            resourceRepartitionFactorNames = new HashSet<string>();
            starTypeNames = new HashSet<string>();
            planetTypeNames = new HashSet<string>();
            anomalyNames = new HashSet<string>();
            strategicResourceNames = new HashSet<string>();
            luxuryResourceNames = new HashSet<string>();
            generationConstraints = new Constraints();
            homeGenerationTraits = new Dictionary<string, HomeTrait>();
            homeGenerationTraitsNames = new HashSet<string>();

            System.Xml.XmlTextReader xr = new System.Xml.XmlTextReader(xmlFileName);

            while (xr.Read())
            {
                if (xr.NodeType == System.Xml.XmlNodeType.Element)
                {
                    if (xr.Name == "GalaxySizes")
                        readGalaxySizes(xr);
                    else if (xr.Name == "GalaxyAges")
                        readGalaxyAges(xr);
                    else if (xr.Name == "ConstellationNumbers")
                        readConstellationNumbers(xr);
                    else if (xr.Name == "ConstellationConnectivities")
                        readConstellationConnectivities(xr);
                    else if (xr.Name == "StarConnectivities")
                        readStarConnectivities(xr);
                    else if (xr.Name == "ConstellationDistances")
                        readConstellationDistances(xr);
                    else if (xr.Name == "GalaxyDensities")
                        readGalaxyDensities(xr);
                    else if (xr.Name == "PlanetsPerSystems")
                        readPlanetsPerSystems(xr);
                    else if (xr.Name == "PlanetsSizeFactors")
                        readPlanetsSizeFactors(xr);
                    else if (xr.Name == "AnomalyTempleFactors")
                        readAnomalyTempleFactors(xr);
                    else if (xr.Name == "ResourceRepartitionFactors")
                        readResourceRepartitionFactors(xr);
                    else if (xr.Name == "PlanetTypeProbabilitiesPerStar")
                        readPlanetTypeProbabilitiesPerStar(xr);
                    else if (xr.Name == "PlanetSizeProbabilitiesPerType")
                        readPlanetSizeProbabilitiesPerType(xr);
                    else if (xr.Name == "MoonNumberChances")
                        readMoonNumberChances(xr);
                    else if (xr.Name == "MoonNumberProbabilitiesPerPlanetType")
                        readMoonNumberProbabilitiesPerPlanetType(xr);
                    else if (xr.Name == "TempleChancesPerStarType")
                        readTempleChancesPerStarType(xr);
                    else if (xr.Name == "GenerationConstraints")
                        readGenerationConstraints(xr);
                    else if (xr.Name == "StarNames")
                        readStarNames(xr);
                    else if (xr.Name == "ConstellationNames")
                        readConstellationNames(xr);
                    else if (xr.Name == "AnomalyBaseChance")
                        anomalyBaseChance = xr.ReadElementContentAsInt();
                    else if (xr.Name == "PlanetAnomaliesProbabilityScale")
                        readPlanetAnomaliesProbabilityScale(xr);
                    else if (xr.Name == "PlanetAnomaliesPerPlanetType")
                        readPlanetAnomaliesPerPlanetType(xr);
                    else if (xr.Name == "PlanetStrategicResourceProbabilitiesScale")
                        readPlanetStrategicResourceProbabilitiesScale(xr);
                    else if (xr.Name == "PlanetStrategicResourcesPerPlanetType")
                        readPlanetStrategicResourcesPerPlanetType(xr);
                    else if (xr.Name == "PlanetLuxuryProbabilitiesScale")
                        readPlanetLuxuryProbabilitiesScale(xr);
                    else if (xr.Name == "PlanetLuxuriesPerPlanetType")
                        readPlanetLuxuriesPerPlanetType(xr);
                    else if (xr.Name == "TempleTypes")
                        readTempleTypeProbabilities(xr);
                    else if (xr.Name == "LuxuryResourceSpawnPriorities")
                        readLuxuryResourceSpawnPriorities(xr);
                    else if (xr.Name == "HomeGeneration")
                        readHomeGeneration(xr);
                    else if (xr.Name == "ResourceDepositSizeIterations")
                        this.readResourceDepositSizeIterations(xr);
                }
            };
        }


        protected void readGalaxySizes(System.Xml.XmlTextReader xr)
        {
            GalaxySize gs;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxySize"))
                {
                    gs = new GalaxySize();
                    gs.name = xr.GetAttribute("Name");
                    gs.numStars = Int32.Parse(xr.GetAttribute("NumStars"));
                    gs.width = Int32.Parse(xr.GetAttribute("Width"));
                    gs.nominalPlayers = Int32.Parse(xr.GetAttribute("NominalPlayers"));
                    gs.strategicResourceNumberPerType = Int32.Parse(xr.GetAttribute("StrategicResourceNumberPerType"));
                    gs.luxuryResourceTypes = Int32.Parse(xr.GetAttribute("LuxuryResourceTypes"));
                    galaxySizes.Add(gs.name, gs);
                    galaxySizeNames.Add(gs.name);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxySizes")));

            foreach (GalaxySize g in this.galaxySizes.Values)
                System.Diagnostics.Trace.WriteLine(g.name + " has width " + g.width.ToString() + " and base population " + g.numStars.ToString());
        }

        protected void readGalaxyAges(System.Xml.XmlTextReader xr)
        {
            GalaxyAge ga;
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxyAge"))
                {
                    ga = new GalaxyAge();
                    ga.name = xr.GetAttribute("Name");
                    ga.starTypeWeightTable = new Dictionary<string,int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarTypeProbability"))
                        {
                            s = xr.GetAttribute("Name");
                            ga.starTypeWeightTable.Add(s, Int32.Parse(xr.GetAttribute("Probability")));
                            starTypeNames.Add(s);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyAge")));
                    galaxyAges.Add(ga.name, ga);
                    galaxyAgeNames.Add(ga.name);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyAges")));
        }

        protected void readConstellationNumbers(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNumber"))
                {
                    constellationNumbers.Add(xr.GetAttribute("Name"));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNumbers")));
        }

        protected void readConstellationConnectivities(System.Xml.XmlTextReader xr)
        {
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationConnectivity"))
                {
                    s = xr.GetAttribute("Name");
                    constellationConnectivities.Add(s, Double.Parse(xr.GetAttribute("Wormholes"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    constellationConnectivityNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationConnectivities")));
        }

        protected void readStarConnectivities(System.Xml.XmlTextReader xr)
        {
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarConnectivity"))
                {
                    s = xr.GetAttribute("Name");
                    starConnectivities.Add(s, Double.Parse(xr.GetAttribute("Warps"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    starConnectivityNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "StarConnectivities")));
        }

        protected void readConstellationDistances(System.Xml.XmlTextReader xr)
        {
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationDistance"))
                {
                    s = xr.GetAttribute("Name");
                    constellationDistances.Add(s, Double.Parse(xr.GetAttribute("Distance"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    constellationDistanceNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationDistances")));
        }

        protected void readGalaxyDensities(System.Xml.XmlTextReader xr)
        {
            string s, t;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxyDensity"))
                {
                    s = xr.GetAttribute("Name");
                    t = xr.GetAttribute("NumberFactor");
                    galaxyDensities.Add(s, Double.Parse(t, System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    galaxyDensitiesNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyDensities")));
        }

        protected void readPlanetsPerSystems(System.Xml.XmlTextReader xr)
        {
            string s;
            Dictionary<int, int> h;
            int a, b;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetsPerSystem"))
                {
                    s = xr.GetAttribute("Name");
                    planetsPerSystemsNames.Add(s);
                    h = new Dictionary<int, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetNumberProbability"))
                        {
                            a = Int32.Parse(xr.GetAttribute("Number"));
                            b = Int32.Parse(xr.GetAttribute("Probability"));
                            h.Add(a, b);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsPerSystem")));
                    planetsPerSystems.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsPerSystems")));
        }

        protected void readPlanetsSizeFactors(System.Xml.XmlTextReader xr)
        {
            string s, t;
            Dictionary<string, double> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetsSizeFactor"))
                {
                    s = xr.GetAttribute("Name");
                    planetsSizeFactorNames.Add(s);
                    h = new Dictionary<string, double>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeFactor"))
                        {
                            t = xr.GetAttribute("Name");
                            planetSizeNames.Add(t);
                            h.Add(t, Double.Parse(xr.GetAttribute("ProbabilityFactor"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsSizeFactor")));
                    planetsSizeFactors.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsSizeFactors")));
        }

        protected void readAnomalyTempleFactors(System.Xml.XmlTextReader xr)
        {
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "AnomalyTempleFactor"))
                {
                    s = xr.GetAttribute("Name");
                    anomalyTempleFactors.Add(s, Double.Parse(xr.GetAttribute("Factor"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    anomalyTempleFactorNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "AnomalyTempleFactors")));
        }

        protected void readResourceRepartitionFactors(System.Xml.XmlTextReader xr)
        {
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ResourceRepartitionFactor"))
                {
                    s = xr.GetAttribute("Name");
                    resourceRepartitionFactors.Add(s, Double.Parse(xr.GetAttribute("Number"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    resourceRepartitionFactorNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ResourceRepartitionFactors")));
/*
            foreach (string n in this.resourceRepartitionFactors.Keys)
                System.Diagnostics.Trace.WriteLine(n + " " + this.resourceRepartitionFactors[n].ToString());
*/
        }

        protected void readResourceDepositSizeIterations(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ResourceDepositSizeIteration"))
                {
                    resourceDepositSizeIterations.Add(Int32.Parse(xr.GetAttribute("Iteration")), Int32.Parse(xr.GetAttribute("DepositSize")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ResourceDepositSizeIterations")));
        }

        protected void readPlanetTypeProbabilitiesPerStar(System.Xml.XmlTextReader xr)
        {
            string s, t;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeProbabilityPerStar"))
                {
                    s = xr.GetAttribute("Name");
                    starTypeNames.Add(s);
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeProbability"))
                        {
                            t = xr.GetAttribute("Name");
                            planetTypeNames.Add(t);
                            h.Add(t, Int32.Parse(xr.GetAttribute("Probability")));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeProbabilityPerStar")));
                    planetTypeProbabilitiesPerStar.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeProbabilitiesPerStar")));
        }

        protected void readPlanetSizeProbabilitiesPerType(System.Xml.XmlTextReader xr)
        {
            string s, t;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeProbabilityPerType"))
                {
                    s = xr.GetAttribute("Name");
                    planetTypeNames.Add(s);
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeProbability"))
                        {
                            t = xr.GetAttribute("Name");
                            planetSizeNames.Add(t);
                            h.Add(t, Int32.Parse(xr.GetAttribute("Probability")));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetSizeProbabilityPerType")));
                    planetSizeProbabilitiesPerType.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetSizeProbabilitiesPerType")));
        }

        protected void readMoonNumberChances(System.Xml.XmlTextReader xr)
        {
            int n, p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberChance"))
                {
                    n = Int32.Parse(xr.GetAttribute("Number"));
                    p = Int32.Parse(xr.GetAttribute("Probability"));
                    moonNumberChances.Add(n, p);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberChances")));
        }

        protected void readMoonNumberProbabilitiesPerPlanetType(System.Xml.XmlTextReader xr)
        {
            string pt;
            int n, p;
            Dictionary<int, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberProbabilityPerPlanetType"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<int, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberChance"))
                        {
                            n = Int32.Parse(xr.GetAttribute("Number"));
                            p = Int32.Parse(xr.GetAttribute("Probability"));
                            h.Add(n, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberProbabilityPerPlanetType")));
                    moonNumberProbabilitiesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberProbabilitiesPerPlanetType")));
        }

        protected void readTempleChancesPerStarType(System.Xml.XmlTextReader xr)
        {
            string n;
            int p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "TempleChancePerStarType"))
                {
                    n = xr.GetAttribute("Name");
                    p = Int32.Parse(xr.GetAttribute("Probability"));
                    templeChancesPerStarType.Add(n, p);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "TempleChancesPerStarType")));
        }

        protected void readPlanetAnomaliesProbabilityScale(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetAnomalyProbablityScale"))
                {
                    planetAnomaliesProbabilityScale.Add(Int32.Parse(xr.GetAttribute("Probability")), Int32.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetAnomaliesProbabilityScale")));
        }

        protected void readPlanetAnomaliesPerPlanetType(System.Xml.XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeAnomaly"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetAnomalyPerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            anomalyNames.Add(an);
                            p = Int32.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeAnomaly")));
                    planetAnomaliesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetAnomaliesPerPlanetType")));
        }

        protected void readPlanetStrategicResourceProbabilitiesScale(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetStrategicResourceProbabilityScale"))
                {
                    planetStrategicResourceProbabilitiesScale.Add(Int32.Parse(xr.GetAttribute("Probability")), Int32.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetStrategicResourceProbabilitiesScale")));
        }

        protected void readPlanetStrategicResourcesPerPlanetType(System.Xml.XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeStrategicResource"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetStrategicResourcePerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            strategicResourceNames.Add(an);
                            p = Int32.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeStrategicResource")));
                    planetStrategicResourcesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetStrategicResourcesPerPlanetType")));
        }

        protected void readPlanetLuxuryProbabilitiesScale(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetLuxuryProbablityScale"))
                {
                    planetLuxuryProbabilitiesScale.Add(Int32.Parse(xr.GetAttribute("Probability")), Int32.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetLuxuryProbabilitiesScale")));
        }

        protected void readPlanetLuxuriesPerPlanetType(System.Xml.XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeLuxury"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetLuxuryPerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            luxuryResourceNames.Add(an);
                            p = Int32.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeLuxury")));
                    planetLuxuriesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetLuxuriesPerPlanetType")));
        }

        protected void readLuxuryResourceSpawnPriorities(System.Xml.XmlTextReader xr)
        {
            int p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "LuxuryResourceSpawnPriority"))
                {
                    p = Int32.Parse(xr.GetAttribute("Priority"));
                    if (!luxuryResourceTiers.Keys.Contains(p))
                        luxuryResourceTiers.Add(p, new HashSet<string>());
                    luxuryResourceTiers[p].Add(xr.GetAttribute("Name"));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "LuxuryResourceSpawnPriorities")));
        }

        protected void readTempleTypeProbabilities(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "TempleType"))
                {
                    templeTypeProbabilities.Add(xr.GetAttribute("Name"), Int32.Parse(xr.GetAttribute("Probability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "TempleTypes")));
        }

        protected void readGenerationConstraints(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element))
                {
                    if (xr.Name == "MinStarsPerConstellation")
                        this.generationConstraints.minStarsPerConstellation = xr.ReadElementContentAsInt();
                    else if (xr.Name == "MinStarDistance")
                        this.generationConstraints.minStarDistance = xr.ReadElementContentAsDouble();
                    else if (xr.Name == "MinEmpireDistance")
                        this.generationConstraints.minEmpireDistance = xr.ReadElementContentAsDouble();
//                    else if (xr.Name == "MaxWormholesConnections")
//                        this.generationConstraints.maxWormholesConnections = xr.ReadElementContentAsInt();
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GenerationConstraints")));

            System.Diagnostics.Trace.WriteLine("cross-checking constraints...");
            System.Diagnostics.Trace.WriteLine("Min stars per constellation : " + this.generationConstraints.minStarsPerConstellation.ToString());
            System.Diagnostics.Trace.WriteLine("Min distance between stars : " + this.generationConstraints.minStarDistance.ToString());
            System.Diagnostics.Trace.WriteLine("Min distance between home systems : " + this.generationConstraints.minEmpireDistance.ToString());
            System.Diagnostics.Trace.WriteLine("...end of cross-checking constraints");
        }

        protected void readStarNames(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarName"))
                    starNames.Add(xr.ReadElementContentAsString());
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "StarNames")));
        }

        protected void readConstellationNames(System.Xml.XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationName"))
                    constellationNames.Add(xr.ReadElementContentAsString());
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNames")));
        }

        protected void readHomeGeneration(System.Xml.XmlTextReader xr)
        {
            string name;

            System.Diagnostics.Trace.WriteLine("ReadHomeGeneration-begin");

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Trait"))
                {
                    name = xr.GetAttribute("Name");
                    this.homeGenerationTraitsNames.Add(name);
                    this.homeGenerationTraits.Add(name, new HomeTrait(xr));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "HomeGeneration")));

            foreach (string s in this.homeGenerationTraitsNames)
                System.Diagnostics.Trace.WriteLine(s);

            System.Diagnostics.Trace.WriteLine("ReadHomeGeneration-end");
        }


    }

}
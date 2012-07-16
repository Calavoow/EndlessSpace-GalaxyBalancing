// <copyright file="Galaxy.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    using Builders;

    public class Galaxy
    {
        public static Galaxy Instance { get; protected set; }
        public static void Generate(Configuration configuration) { if (Instance == null) new Galaxy(configuration); }
        public static void Release()
        {
            Instance = null;
        }

        public Configuration Configuration { get; protected set; }

        public void WriteXml(XmlWriter xw)
        {
            Dictionary<Color, int> hr = new Dictionary<Color,int>();
            double x, z, minX, minY, minZ, maxX, maxY, maxZ;
            int i, j;

            minX = 9999;
            minY = 0;
            minZ = 9999;
            maxX = -9999;
            maxY = 0;
            maxZ = -9999;
            foreach (StarSystem s in this.Stars)
            {
                x = s.position.X;
                z = s.position.Y;
                if (x > maxX)
                    maxX = x;
                if (x < minX)
                    minX = x;
                if (z > maxZ)
                    maxZ = z;
                if (z < minZ)
                    minZ = z;
            }
            xw.WriteStartElement("Dimensions");
            xw.WriteStartElement ("Size");
            xw.WriteAttributeString("X", (maxX-minX).ToString());
            xw.WriteAttributeString("Y", (maxY-minY).ToString());
            xw.WriteAttributeString("Z", (maxZ-minZ).ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("Bounds");
            xw.WriteAttributeString("MaxX", maxX.ToString());
            xw.WriteAttributeString("MaxY", maxY.ToString());
            xw.WriteAttributeString("MaxZ", maxZ.ToString());
            xw.WriteAttributeString("MinX", minX.ToString());
            xw.WriteAttributeString("MinY", minY.ToString());
            xw.WriteAttributeString("MinZ", minZ.ToString());
            xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("Empires");
            foreach (StarSystem s in this.SpawnStars)
            {
                xw.WriteStartElement("Home");
                xw.WriteAttributeString("System", s.id.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Regions");
            foreach (StarSystem s in this.Stars)
            {
                if (hr.Keys.Contains(s.regionIndex))
                    hr[s.regionIndex]++;
                else
                    hr.Add(s.regionIndex, 1);
            }
            foreach (Color rgb in hr.Keys)
            {
                xw.WriteStartElement("Region");
                xw.WriteAttributeString("Color", "Red="+rgb.R.ToString()+" Green="+rgb.G.ToString()+" Blue="+rgb.B.ToString());
                xw.WriteAttributeString("NumStars", hr[rgb].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Constellations");
            foreach (Constellation c in this.Constellations)
            {
                xw.WriteStartElement ("Constellation");
                xw.WriteAttributeString("Name", c.Name);
                xw.WriteAttributeString("Id", c.id.ToString());
                xw.WriteAttributeString("Population", c.Count.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteElementString("Population", this.Stars.Count.ToString());

            xw.WriteStartElement("Systems");
            foreach (StarSystem s in this.Stars)
            {
                xw.WriteStartElement("System");
                if (s.constellation() != null)
                    xw.WriteAttributeString("Constellation", s.constellation().id.ToString());
                else
                {
                    xw.WriteAttributeString("Constellation", "NotInConstellation");
                    System.Diagnostics.Trace.WriteLine("System n°" + s.id.ToString() + " is not in a constellation !");
                }
                xw.WriteAttributeString("Name", s.Name);
                xw.WriteAttributeString("Type", s.type);
                xw.WriteAttributeString("Id", s.id.ToString());
                xw.WriteAttributeString("X", s.position.X.ToString());
                xw.WriteAttributeString("Y", "0");
                xw.WriteAttributeString("Z", s.position.Y.ToString());
                xw.WriteStartElement("Region");
                xw.WriteAttributeString("Red", s.region.Index.R.ToString());
                xw.WriteAttributeString("Green", s.region.Index.G.ToString());
                xw.WriteAttributeString("Blue", s.region.Index.B.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("Planets");
                i = 0;
                foreach (Planet p in s.Planets)
                {
                    xw.WriteStartElement("Planet");
                    xw.WriteAttributeString("Orbit", i.ToString());
                    i++;
                    xw.WriteAttributeString("Size", p.size);
                    xw.WriteAttributeString("Type", p.type);
                    xw.WriteElementString("Anomalies", p.anomaly);
                    xw.WriteStartElement("Moons");
                    j = 0;
                    for (; j<p.moonsTemples.Count; j++)
                    {
                        xw.WriteStartElement("Moon");
                        xw.WriteAttributeString("Temple", p.moonsTemples[j]);
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Resources");
                    if (p.resource != null)
                    {
                        xw.WriteAttributeString(p.resource.type.ToString(), p.resource.name);
                        xw.WriteAttributeString("Size", p.resource.size.ToString());
                    }
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Warps");
            foreach (WarpLine w in this.Warps)
            {
                if (! w.isWormhole)
                {
                    xw.WriteStartElement("Warp");
                    xw.WriteAttributeString("System1", w.starA.id.ToString());
                    xw.WriteAttributeString("System2", w.starB.id.ToString());
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Wormholes");
            foreach (WarpLine w in this.Warps)
            {
                if (w.isWormhole)
                {
                    xw.WriteStartElement("Wormhole");
                    xw.WriteAttributeString("System1", w.starA.id.ToString());
                    xw.WriteAttributeString("System2", w.starB.id.ToString());
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();

            outputStatistics (xw);
        }

        public List<StarSystem> Stars { get; protected set; }
        public List<WarpLine> Warps { get; protected set; }
        public List<Constellation> Constellations { get; protected set; }
        public List<Region> Regions { get; protected set; }
        public List<StarSystem> SpawnStars { get; protected set; }
        public List<Planet> Planets
        {
            get
            {
                List<Planet> list = new List<Planet>();
                foreach (StarSystem s in this.Stars) list.AddRange(s.Planets);
                return list;
            }
        }
        
        public bool IsValid { get; protected set; }

        private List<Builder> builderList;

        public double Diameter()
        {
            double d = 0;
            foreach (StarSystem s in this.Stars)
            {
                if (s.directDistanceTable.Count <= 0) s.computeDirectDistanceTable();

                foreach (StarSystem y in s.directDistanceTable.Keys)
                {
                    if (s.directDistanceTable[y] > d) d = s.directDistanceTable[y];
                }
            }

            return d;
        }

        protected Galaxy(Configuration configuration)
        {
            Galaxy.Instance = this;
            this.IsValid = true;

            this.builderList = new List<Builder>();

            StarBuilder starBuilder = new StarBuilder();
            ConstellationBuilder constellationsBuilder = new ConstellationBuilder();
            WarpBuilder warpBuilder = new WarpBuilder(); 
            SpawnBuilder spawnBuilder = new SpawnBuilder();
            PlanetBuilder planetBuilder = new PlanetBuilder();
            HomeBuilder homeBuilder = new HomeBuilder();
            StrategicResourceBuilder strategicResourceBuilder = new StrategicResourceBuilder();
            LuxuryResourceBuilder luxuryResourceBuilder = new LuxuryResourceBuilder();

            this.builderList.Add(starBuilder);
            this.builderList.Add(constellationsBuilder);
            this.builderList.Add(warpBuilder);
            this.builderList.Add(spawnBuilder);
            this.builderList.Add(planetBuilder);
            this.builderList.Add(homeBuilder);
            this.builderList.Add(strategicResourceBuilder);
            this.builderList.Add(luxuryResourceBuilder);

            this.Configuration = configuration;

            this.Stars = new List<StarSystem>();
            this.Warps = new List<WarpLine>();
            this.Constellations = new List<Constellation>();
            this.Regions = new List<Region>();
            this.SpawnStars = new List<StarSystem>();
            //this.Planets = new List<Planet>();

            foreach (Builder builder in this.builderList)
            {
                if (this.IsValid)
                {
                    builder.Execute();
                    this.IsValid = this.IsValid && builder.Result;
                }
            }
            if (this.IsValid)
            {
                this.IsValid = Balancing.Balancing.isBalanced();
            }

            if (!this.IsValid)
            {
                System.Diagnostics.Trace.WriteLine("--Galaxy generation failed--");
                System.Diagnostics.Trace.WriteLine("--Generation defects summary--");
                foreach (Builder b in this.builderList)
                {
                    foreach (string text in b.Defects)
                    {
                        System.Diagnostics.Trace.WriteLine(b.Name + " -> " + text);
                    }
                }
                System.Diagnostics.Trace.WriteLine("--Generation defects end--");
            }
        }

        ~Galaxy() { Instance = null; this.Configuration.ResetNames(); this.Planets.Clear(); }
/*
        private void distributeLuxuryResources()
        {
            int nSites, qty, i, r;
            List<Planet> sites = new List<Planet>();
            HashSet<Planet> usedSites = new HashSet<Planet>();
            HashSet<string> usedLuxes = new HashSet<string>();
            Planet selP;
  //          StarSystem sourceCenter;
            int tier;

            System.Diagnostics.Trace.WriteLine("distributeLuxuryResources-begin");

            qty = this.Configuration.luxuryResourceNumberOfTypes();

            System.Diagnostics.Trace.WriteLine("total luxes = " + Settings.Instance.luxuryResourceNames.Count.ToString());
            System.Diagnostics.Trace.WriteLine("number of lux types " + qty.ToString());
            tier = 1;
            do
            {
                //System.Diagnostics.Trace.WriteLine("tier " + tier.ToString() + " usedLuxes.Count=" +usedLuxes.Count.ToString());
                usedLuxes.Add(this.Configuration.getRandomLuxuryResource(tier));
                tier++;
                if (tier > 4)
                    tier = 1;
            }
            while ((usedLuxes.Count < qty) && (usedLuxes.Count < Settings.Instance.luxuryResourceNames.Count));

            //System.Diagnostics.Trace.WriteLine("compiled usedLuxes");

            foreach (string resName in usedLuxes)
            {
                //sourceCenter = this.stars[];
                qty = 7;
                //nSites = qMax (hConstel.count(), int (0.3 * double (qty)));
                nSites = 4;
                //qty = qMax (qty, nSites);
                sites.Clear();
                usedSites.Clear();
                foreach (StarSystem sys in this.Stars)
                {
                    foreach (Planet p in sys.Planets)
                    {
                        if (p.resource == null)
                        {
                            i = 0;
                            for (; i<Settings.Instance.planetLuxuriesPerPlanetType[p.type][resName]; i++)
                                sites.Add(p);
                        }
                    }
                }
                i = 0;
                if (sites.Count == 0)
                    qty = 0;
                for (; i<qty; i++)
                {
                    selP = null;
                    if (usedSites.Count < nSites)
                    {
                        r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                        selP = sites[r];
                        usedSites.Add(selP);
                    }
                    else
                    {
                        do
                        {
                            r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                            selP = sites[r];
                        }
                        while (! usedSites.Contains(selP));
                    }
                    if (selP != null)
                    {
                        if (selP.resource == null)
                            selP.resource = new ResourceDeposit_Luxury (resName);
                        else if (selP.resource.size < ResourceDeposit.MaxSize)
                            selP.resource.increaseSize();
                    }
                }
            }

            System.Diagnostics.Trace.WriteLine("distributeLuxuryResources-end");
        }
*/
        private void outputStatistics(XmlWriter xw)
        {
            Dictionary<string, int> starTypeCount = new Dictionary<string,int>();
            Dictionary<int, int> planetNumberCount = new Dictionary<int,int>();
            Dictionary<string, int> planetTypeCount = new Dictionary<string,int>();
            Dictionary<string, int> planetSizeTypeCount = new Dictionary<string,int>();
            Dictionary<string, int> anomalyCount = new Dictionary<string,int>();
            Dictionary<string, int> templeCount = new Dictionary<string,int>();
            List<string> orderedKeys = new List<string>();
            Dictionary<string, HashSet<ResourceDeposit> > sr = new Dictionary<string,HashSet<ResourceDeposit>>();
            StringBuilder sb = new StringBuilder();
            int total;

            xw.WriteStartElement("Statistics");

            xw.WriteStartElement("StarTypes");
            foreach (StarSystem s in this.Stars)
            {
                if (starTypeCount.Keys.Contains(s.type))
                    starTypeCount[s.type]++;
                else
                    starTypeCount.Add(s.type, 1);
            }
            foreach (string txt in starTypeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", starTypeCount[txt].ToString());
                xw.WriteEndElement ();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("PlanetPerSystem");
            total = 0;
            foreach (StarSystem s in this.Stars)
            {
                total += s.Planets.Count;
                if (planetNumberCount.Keys.Contains(s.Planets.Count))
                    planetNumberCount[s.Planets.Count]++;
                else
                    planetNumberCount.Add(s.Planets.Count, 1);
            }
            foreach (int i in planetNumberCount.Keys)
            {
                xw.WriteStartElement("Planets");
                xw.WriteAttributeString("NumPlanets", i.ToString());
                xw.WriteAttributeString("NumSystems", planetNumberCount[i].ToString());
                xw.WriteEndElement();

                System.Diagnostics.Trace.WriteLine(planetNumberCount[i].ToString() + " systems with " + i.ToString() + " planets");
            }
            xw.WriteEndElement();
            System.Diagnostics.Trace.WriteLine("Average " + ((double)(total) / (double)(this.Stars.Count)).ToString() + " planets per system");

            xw.WriteStartElement("PlanetTypes");
            foreach (StarSystem s in this.Stars)
                foreach (Planet p in s.Planets)
                {
                    if (planetTypeCount.Keys.Contains(p.type))
                        planetTypeCount[p.type]++;
                    else
                        planetTypeCount.Add(p.type, 1);
                }
            foreach (string txt in planetTypeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", planetTypeCount[txt].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Anomalies");
            foreach (StarSystem s in this.Stars)
                foreach (Planet p in s.Planets)
                {
                    if (anomalyCount.Keys.Contains(p.anomaly))
                        anomalyCount[p.anomaly]++;
                    else
                        anomalyCount.Add(p.anomaly, 1);
                }
            foreach (string txt in anomalyCount.Keys)
            {
                if (txt == "")
                    xw.WriteStartElement("NoAnomaly");
                else
                    xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", anomalyCount[txt].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("PlanetTypesAndSizes");
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    string txt = p.size + "-" + p.type;
                    if (planetSizeTypeCount.Keys.Contains(txt))
                        planetSizeTypeCount[txt]++;
                    else
                        planetSizeTypeCount.Add(txt, 1);
                }
            }
            orderedKeys = new List<string> (planetSizeTypeCount.Keys);
            orderedKeys.Sort();
            foreach (string txt in orderedKeys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", planetSizeTypeCount[txt].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Resources");
            sr.Clear();
            foreach (StarSystem s in this.Stars)
                foreach (Planet p in s.Planets)
                    if (p.resource != null)
                    {
                        if (!sr.Keys.Contains(p.resource.name))
                            sr.Add(p.resource.name, new HashSet<ResourceDeposit>());
                        sr[p.resource.name].Add(p.resource);
                    }
            foreach (string txt in sr.Keys)
            {
                foreach (ResourceDeposit rd in sr[txt])
                {
                    xw.WriteStartElement(txt);
                    xw.WriteAttributeString("Size", rd.size.ToString());
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();

            xw.WriteStartElement("Temples");
            templeCount.Clear();
            foreach (StarSystem s in this.Stars)
                foreach (Planet p in s.Planets)
                    foreach (string txt in p.moonsTemples)
                    {
                        if (templeCount.Keys.Contains(txt))
                            templeCount[txt]++;
                        else
                            templeCount.Add(txt, 1);
                    }
            foreach (string txt in templeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", templeCount[txt].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            int minWarps, maxWarps, maxWH, nwh;
            double avgWarps, totalWarps, nsys;
            Dictionary<int, int> warpCounts = new Dictionary<int,int>();

            minWarps = 999;
            maxWarps = 0;
            maxWH = 0;
            nsys = 0;
            totalWarps = 0;
            foreach (StarSystem s in this.Stars)
            {
                nsys++;
                totalWarps += s.destinations.Count;
                if (warpCounts.ContainsKey(s.destinations.Count))
                    warpCounts[s.destinations.Count]++;
                else
                    warpCounts.Add(s.destinations.Count, 1);
                if (s.destinations.Count < minWarps)
                    minWarps = s.destinations.Count;
                if (s.destinations.Count > maxWarps)
                    maxWarps = s.destinations.Count;
                nwh = 0;
                foreach (WarpLine w in this.Warps)
                    if (w.isWormhole && ((w.starA == s) || (w.starB == s)))
                        nwh++;
                if (nwh > maxWH)
                    maxWH = nwh;
            }
            if (nsys > 0)
                avgWarps = totalWarps / nsys;
            else
                avgWarps = 0;
            avgWarps = (double)((int)(100 * avgWarps)) / 100;

            xw.WriteStartElement("Connectivity");
            xw.WriteAttributeString("AverageWarps", avgWarps.ToString());
            xw.WriteAttributeString("MinWarps", minWarps.ToString());
            xw.WriteAttributeString("MaxWarps", maxWarps.ToString());
            xw.WriteAttributeString("MaxWormholes", maxWH.ToString());
            xw.WriteEndElement();

            xw.WriteStartElement("WarpCount");
            foreach (int n in warpCounts.Keys)
            {
                xw.WriteStartElement("Warps");
                xw.WriteAttributeString("NofWarps", n.ToString());
                xw.WriteAttributeString("NofStars", warpCounts[n].ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("GenerationDefects");
            foreach (Builder builder in this.builderList)
            {
                foreach (string defect in builder.Defects)
                {
                    xw.WriteStartElement("Defect");
                    xw.WriteAttributeString("Builder", builder.Name);
                    xw.WriteAttributeString("Details", defect);
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
        }
    }

}

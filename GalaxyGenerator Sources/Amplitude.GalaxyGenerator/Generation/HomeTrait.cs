// -----------------------------------------------------------------------
// <copyright file="HomeTrait.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Amplitude.GalaxyGenerator.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Amplitude.GalaxyGenerator.Generation.Components;
    using System.Xml;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HomeTrait
    {
        protected List<HomeTrait.Component> components;

        public HomeTrait(System.Xml.XmlTextReader xr)
        {
            Component.AppliesTo target = Component.AppliesTo.WORLD;
            Component bit = null;

            this.components = new List<Component>();

            do
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.Element)
                {
                    if (xr.Name == "HomeStar") { target = Component.AppliesTo.STAR; }
                    else if (xr.Name == "HomePlanet") { target = Component.AppliesTo.WORLD; }
                    else if (xr.Name == "OtherPlanets") { target = Component.AppliesTo.OTHER; }
                    else
                    {
                        if (xr.Name == "OverridePlanetsInSystem")
                        {
                            bit = new HomeTrait.Component_PlanetsInSystem();
                            bit.target = Component.AppliesTo.STAR;
                        }
                        else if (xr.Name == "OverrideStarType")
                        {
                            bit = new Component_OverrideStarType();
                            bit.target = Component.AppliesTo.STAR;
                        }
                        else if (xr.Name == "OverrideType")
                        {
                            bit = new Component_OverrideType();
                            bit.target = target;
                        }
                        else if (xr.Name == "OverrideSize")
                        {
                            bit = new Component_OverrideSize();
                            bit.target = target;
                        }
                        else if (xr.Name == "OverrideAnomaly")
                        {
                            bit = new Component_OverrideAnomaly();
                            bit.target = target;
                        }
                        else if (xr.Name == "InhibitAnomalies")
                        {
                            bit = new Component_InhibitAnomalies();
                            bit.target = target;
                        }
                        else if (xr.Name == "InhibitLuxuryResources")
                        {
                            bit = new Component_InhibitLuxuries();
                            bit.target = target;
                        }
                        else if (xr.Name == "InhibitStrategicResources")
                        {
                            bit = new Component_InhibitStrategics();
                            bit.target = target;
                        }

                        if (null != bit)
                        {
                            bit.Read(xr);
                            this.components.Add(bit);
                        }
                    }
                }
            }
            while (!( (xr.NodeType==XmlNodeType.EndElement) && (xr.Name=="Trait") ));
        }

        public void Apply(StarSystem star)
        {
            List<HomeTrait.Component> localComponents = new List<Component>();

            System.Diagnostics.Trace.WriteLine("Applying Trait " + Settings.Instance.homeGenerationTraits.First((pair) => { return pair.Value == this; }).Key);

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll((bit) =>
                { return bit.target == Component.AppliesTo.STAR; }));
            foreach (HomeTrait.Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern (star));
            }

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll((bit) =>
                { return bit.target == Component.AppliesTo.WORLD; }));
            foreach (HomeTrait.Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern(star));
            }

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll((bit) =>
                { return bit.target == Component.AppliesTo.OTHER; }));
            foreach (HomeTrait.Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern(star));
            }
        }

        protected class Component
        {
            public enum AppliesTo { STAR, WORLD, OTHER };

            public AppliesTo target { get; set; }
            public virtual void Apply(HomeGenerator.Pattern hp) { }
            public virtual void Read(System.Xml.XmlTextReader xr) { }

        }

        protected class Component_Override : HomeTrait.Component
        {
            public double probability { get; set; }
            public SortedDictionary<string, int> weights { get; set; }

            public Component_Override()
            {
                this.weights = new SortedDictionary<string,int>();
            }

            protected void internalRead(System.Xml.XmlTextReader xr, string component, string element)
            {
                if (xr.AttributeCount > 0)
                    this.probability = Double.Parse(xr.GetAttribute("Probability"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                else
                    this.probability = 1.0;

                do
                {
                    xr.Read();
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        if (xr.Name == element)
                        {
                            if (xr.AttributeCount == 2)
                                this.weights.Add(xr.GetAttribute("Name"), Int32.Parse(xr.GetAttribute("Weight")));
                            else if (xr.AttributeCount == 1)
                                this.weights.Add(xr.GetAttribute("Name"), 1);
                        }
                    }
                }
                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == component)));
            }

            protected string getRandomOverride()
            {
                List<string> possibleElements = new List<string>();
                int i, n;
                string element = null;

                if (GalaxyGeneratorPlugin.random.NextDouble() <= this.probability)
                {
                    foreach (string s in this.weights.Keys)
                    {
                        n = this.weights[s];
                        i = 0;
                        for (; i < n; i++)
                            possibleElements.Add(s);
                    }
                    if (possibleElements.Count > 0)
                    {
                        element = possibleElements[GalaxyGeneratorPlugin.random.Next(possibleElements.Count)];
                    }
                }

                return element;
            }
        }

        protected class Component_Inhibit : HomeTrait.Component
        {
            public List<string> inhibitList { set; protected get; }
            protected bool allInhibited;

            protected List<Planet> tgtPlanets;

            public Component_Inhibit()
            {
                this.allInhibited = false;
                this.inhibitList = new List<string>();
                this.tgtPlanets = new List<Planet>();
            }

            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if (this.target == AppliesTo.STAR)
                    tgtPlanets = hp.Star.Planets;
                else if (this.target == AppliesTo.WORLD)
                    tgtPlanets.Add(hp.HomeWorld);
                else if (this.target == AppliesTo.OTHER)
                    tgtPlanets = hp.OtherPlanets;
            }

            public void internalRead(System.Xml.XmlTextReader xr, string component, string element)
            {
                do
                {
                    xr.Read();

                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        if (xr.Name == element)
                        {
                            this.inhibitList.Add(xr.GetAttribute("Name"));
                        }
                        else if (xr.Name == "All")
                        {
                            this.allInhibited = true;
                        }
                    }
                }
                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == component)));
            }
        }

        protected class Component_OverrideStarType : HomeTrait.Component_Override
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                string modifiedStarType = base.getRandomOverride();

                if (null != modifiedStarType)
                {
                    if (hp.Star.type != modifiedStarType)
                    {
                        hp.Star.type = modifiedStarType;
                        //hp.Star.GeneratePlanets();
                    }
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                base.internalRead(xr, "OverrideStarType", "StarType");
            }
        }

        protected class Component_PlanetsInSystem : HomeTrait.Component_Override
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                int modifiedNumber;
                string mod;

                if (GalaxyGeneratorPlugin.random.NextDouble() > this.probability) return;

                base.Apply(hp);

                mod = base.getRandomOverride();
                if (Int32.TryParse(mod, out modifiedNumber))
                {
                    hp.Star.GeneratePlanets(modifiedNumber);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Could not read Modified Planet Number from " + mod);
                }
            }

            override public void Read(XmlTextReader xr)
            {
                base.Read(xr);
                base.internalRead(xr, "OverridePlanetsInSystem", "Quantity");
            }
        }

        protected class Component_OverrideType : HomeTrait.Component_Override
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                string modifiedPlanetType = base.getRandomOverride();
                if (null == modifiedPlanetType) return;

                if (this.target == Component.AppliesTo.WORLD)
                {
                    if (GalaxyGeneratorPlugin.random.NextDouble() > this.probability) return; 
                    hp.HomeWorld.ReCreateType(modifiedPlanetType);
                }
                else if (this.target == Component.AppliesTo.OTHER)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedPlanetType = base.getRandomOverride();
                        if ((null != modifiedPlanetType) && (GalaxyGeneratorPlugin.random.NextDouble() <= this.probability))
                            p.ReCreateType(modifiedPlanetType);
                    }
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                base.internalRead(xr, "OverrideType", "Type");
            }
        }

        protected class Component_OverrideSize : HomeTrait.Component_Override
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                string modifiedPlanetSize;

                base.Apply(hp);

                if (this.target == Component.AppliesTo.WORLD)
                {
                    if (GalaxyGeneratorPlugin.random.NextDouble() > this.probability) return;
                    modifiedPlanetSize = base.getRandomOverride();
                    if (null != modifiedPlanetSize)
                        hp.HomeWorld.size = modifiedPlanetSize;
                }
                else if (this.target == Component.AppliesTo.OTHER)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedPlanetSize = base.getRandomOverride();
                        if ((null != modifiedPlanetSize) && (GalaxyGeneratorPlugin.random.NextDouble() <= this.probability))
                            p.size = modifiedPlanetSize;
                    }
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                internalRead(xr, "OverrideSize", "Size");
            }
        }

        protected class Component_OverrideAnomaly : HomeTrait.Component_Override
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                string modifiedAnomaly;

                base.Apply(hp);

                if (this.target == Component.AppliesTo.WORLD)
                {
                    if (GalaxyGeneratorPlugin.random.NextDouble() > this.probability) return; 
                    modifiedAnomaly = base.getRandomOverride();
                    if (null != modifiedAnomaly)
                        hp.HomeWorld.anomaly = modifiedAnomaly;
                }
                else if (this.target == Component.AppliesTo.OTHER)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedAnomaly = base.getRandomOverride();
                        if ((null != modifiedAnomaly) && (GalaxyGeneratorPlugin.random.NextDouble() <= this.probability))
                            p.anomaly = modifiedAnomaly;
                    }
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                internalRead(xr, "OverrideAnomaly", "Anomaly");
            }
        }

        protected class Component_InhibitAnomalies : HomeTrait.Component_Inhibit
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if ((this.inhibitList.Count == 0) && this.allInhibited)
                {
                    this.inhibitList.AddRange(Settings.Instance.anomalyNames);
                }

                foreach (Planet p in tgtPlanets)
                {
                    p.inhibitedAnomalies.AddRange(this.inhibitList);
                    p.ApplyInhibitAnomalies();
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                internalRead(xr, "InhibitAnomalies", "Anomaly");
            }
        }

        protected class Component_InhibitLuxuries : HomeTrait.Component_Inhibit
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if (this.allInhibited)
                {
                    this.inhibitList = Settings.Instance.luxuryResourceNames.ToList();
                }

                foreach (Planet p in tgtPlanets)
                {
                    p.inhibitedLuxuries = this.inhibitList;
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                internalRead(xr, "InhibitLuxurieResources", "LuxuryResource");
            }
        }

        protected class Component_InhibitStrategics : HomeTrait.Component_Inhibit
        {
            override public void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if (this.allInhibited)
                {
                   this.inhibitList = Settings.Instance.strategicResourceNames.ToList();
                }

                foreach (Planet p in tgtPlanets)
                {
                    p.inhibitedStrategics = this.inhibitList;
                }
            }

            override public void Read(System.Xml.XmlTextReader xr)
            {
                base.Read(xr);
                internalRead(xr, "InhibitStrategicResources", "StrategicResource");
            }
        }
    }

}

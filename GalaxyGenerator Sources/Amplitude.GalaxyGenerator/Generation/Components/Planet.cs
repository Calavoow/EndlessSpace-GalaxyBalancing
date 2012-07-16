// <copyright file="Planet.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Planet : IComparable<Planet>
    {
        public int CompareTo(Planet other) { return other.Index - this.Index; }

        public StarSystem system { get; private set; }
        public string type { get; set; }
        public string size { get; set; }
        public string anomaly { get; set; }
        public List<string> moonsTemples { get; private set; }
        public ResourceDeposit resource { get; set; }

        public List<string> inhibitedLuxuries { get; set; }
        public List<string> inhibitedStrategics { get; set; }
        public List<string> inhibitedAnomalies { get; set; }

        public int Index { get {return Galaxy.Instance.Planets.IndexOf(this);} }

        public bool HasResource { get { return this.resource != null; } }

        public string PresentResourceName
        {
            get
            {
                if (! this.HasResource) return "";

                return this.resource.name;
            }
        }

        public bool CanAcceptStrategicResource(string resourceName)
        {
            if (this.resource != null) return false;
            if (this.inhibitedStrategics.Contains(resourceName)) return false;
            if (Settings.Instance.planetStrategicResourcesPerPlanetType[this.type][resourceName] == 0) return false;

            return true;
        }

        public bool CanAcceptLuxuryResource(string resourceName)
        {
            if (this.resource != null) return false;
            if (this.inhibitedLuxuries.Contains(resourceName)) return false;
            if (Settings.Instance.planetLuxuriesPerPlanetType[this.type][resourceName] == 0) return false;

            return true;
        }

        protected void createMoons()
        {
            int n, i;

            n = Galaxy.Instance.Configuration.getRandomMoonNumber(type);

            i = 0;
            for (; i < n; i++)
                moonsTemples.Add(Galaxy.Instance.Configuration.getRandomTempleType(system.type));
        }

        public Planet(StarSystem s)
        {
            this.inhibitedLuxuries = new List<string>();
            this.inhibitedStrategics = new List<string>();
            this.inhibitedAnomalies = new List<string>();
            this.moonsTemples = new List<string>();

            Galaxy.Instance.Planets.Add(this);
            
            this.system = s;
            
            this.ReCreateType(Galaxy.Instance.Configuration.getRandomPlanetType(this.system.type));
        }

        public void ReCreateType(string reType)
        {
            if (!Settings.Instance.planetTypeNames.Contains(reType))
            {
                return;
            }

            this.type = reType;
            this.size = Galaxy.Instance.Configuration.getRandomPlanetSize(this.type);
            this.moonsTemples.Clear();
            this.createMoons();
            this.anomaly = Galaxy.Instance.Configuration.getRandomAnomaly(this.type);
            this.ApplyInhibitAnomalies();
        }

        public void ApplyInhibitAnomalies()
        {
            if (this.inhibitedAnomalies.Contains(this.anomaly))
            {
                this.anomaly = "";
            }
        }
    }
}

// <copyright file="Shape.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Xml;

    public class Shape
    {
        public Shape(System.Xml.XmlTextReader reader)
        {
            this.densityFileName = reader.GetAttribute("DensityMap");
            System.Diagnostics.Trace.WriteLine(this.densityFileName);
            this.densityMap = new System.Drawing.Bitmap(densityFileName);

            this.regionsFileName = reader.GetAttribute("RegionMap");
            System.Diagnostics.Trace.WriteLine(this.regionsFileName);
            this.regionsMap = new System.Drawing.Bitmap(regionsFileName);

            this.minConstellations = Int32.Parse(reader.GetAttribute("MinConstellations"));
            this.maxConstellations = Int32.Parse(reader.GetAttribute("MaxConstellations"));
            this.minEmpires = Int32.Parse(reader.GetAttribute("MinEmpires"));
            this.maxEmpires = Int32.Parse(reader.GetAttribute("MaxEmpires"));

            this.regions = new Dictionary<Color, bool>();
            this.spawnerSequence = new List<Color>();
            //this.poolRegions = new List<Color>();

            this.symetryOptions = new HashSet<int>();

            this.topology = new List<Link>();

            do
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "SymetryOptions")
                        this.readSymetryOptions(reader);
                    else if (reader.Name == "Regions")
                        this.readRegions(reader);
                    else if (reader.Name == "Topology")
                        this.readTopology(reader);
                }
            }
            while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "GalaxyShape")));

//            System.Diagnostics.Trace.WriteLine("finished reading shape description");
        }

        public class Link
        {
            public Color RegionA;
            public Color RegionB;

            public Link(Color a, Color b)
            {
                this.RegionA = a;
                this.RegionB = b;
            }
        }

        public string densityFileName { get; protected set; }
        public string regionsFileName { get; protected set; }
        public System.Drawing.Bitmap densityMap { get; protected set; }
        public System.Drawing.Bitmap regionsMap { get; protected set; }
        public int minConstellations { get; protected set; }
        public int maxConstellations { get; protected set; }
        public int minEmpires { get; protected set; }
        public int maxEmpires { get; protected set; }
        public HashSet<int> symetryOptions { get; protected set; }
        public Dictionary<System.Drawing.Color, bool> regions { get; protected set; }
        public List<Shape.Link> topology { get; protected set; }
        //public List<Color> poolRegions { get; protected set; }
        public List<Color> spawnerSequence { get; protected set; }

        protected void readSymetryOptions(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "SymetryOption"))
                {
                    this.symetryOptions.Add(Int32.Parse(xr.GetAttribute("PlayerNum")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "SymetryOptions")));
        }

        protected void readRegions(XmlTextReader xr)
        {
            System.Drawing.Color rgb;
            string col;
            byte r, g, b;
            bool spawner;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Region"))
                {
                    col = xr.GetAttribute("color");

                    r = Byte.Parse(col.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = Byte.Parse(col.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = Byte.Parse(col.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    rgb = Color.FromArgb(r, g, b);
                    spawner = (xr.GetAttribute("empirestart") == "1");
                    this.regions.Add(rgb, spawner);
                    if (spawner) this.spawnerSequence.Add(rgb);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Regions")));
        }

        protected void readTopology(XmlTextReader xr)
        {
            System.Drawing.Color regionA, regionB;
            string txt;
            byte r, g, b;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Link"))
                {
                    txt = xr.GetAttribute("RegionA");
                    r = Byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = Byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = Byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    regionA = Color.FromArgb(r, g, b);

                    txt = xr.GetAttribute("RegionB");
                    r = Byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = Byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = Byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    regionB = Color.FromArgb(r, g, b);

                    this.topology.Add(new Link(regionA, regionB));
                }
                /*else if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Pool"))
                {
                    txt = xr.GetAttribute("Region");
                    r = Byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = Byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = Byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    this.poolRegions.Add(Color.FromArgb(r, g, b));
                }*/
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Topology")));
        }
    }
}

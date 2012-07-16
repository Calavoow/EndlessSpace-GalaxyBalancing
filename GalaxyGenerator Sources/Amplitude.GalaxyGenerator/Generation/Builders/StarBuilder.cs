using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Components;

    public class StarBuilder : Builder
    {
        override public string Name { get { return "StarBuilder"; } }

        public StarBuilder() : base()
        {
            this.rawRGBset = new HashSet<Color>();
        }

        override public void Execute()
        {
            int i, secOut;
            PointF p;
            StarSystem sa;
            Color ri = new Color();

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            i = 0;

            for (; i < Galaxy.Instance.Configuration.population(); i++)
            {
                secOut = 0;
                do
                {
                    p = this.RandomPosition(ref ri);
                    secOut++;
                }
                while ((this.Overlap(p) || (!Galaxy.Instance.Configuration.shape().regions.Keys.Contains(ri)))
                    && (secOut < 100));

                if (this.Overlap(p))
                {
                    this.TraceDefect("Placed two stars too close to each other");
                }

                if (Galaxy.Instance.Configuration.shape().regions.Keys.Contains(ri))
                {
                    sa = new StarSystem(p);
                    sa.regionIndex = ri;
                    sa.id = Galaxy.Instance.Stars.Count;
                    Galaxy.Instance.Stars.Add(sa);
                }
                else
                {
                    this.TraceDefect("Unable to place star " + i.ToString() + " appeared in black space");
                }
            }

            foreach (StarSystem s in Galaxy.Instance.Stars)
                s.computeDirectDistanceTable();

            System.Diagnostics.Trace.WriteLine("Generated " + Galaxy.Instance.Stars.Count.ToString() + " stars");
            System.Diagnostics.Trace.WriteLine("populate-end");

            if (Galaxy.Instance.Stars.Count < Galaxy.Instance.Configuration.population() / 5)
            {
                this.TraceDefect("Very few stars were generated", true);
                return;
            }
            else if (Galaxy.Instance.Stars.Count < (4 * Galaxy.Instance.Configuration.population()) / 5)
            {
                this.TraceDefect("Abnormally few stars were generated");
            }

            System.Diagnostics.Trace.WriteLine("Black is R=" + Color.Black.R.ToString() + " G=" + Color.Black.R.ToString() + " B=" + Color.Black.B.ToString());
            System.Diagnostics.Trace.WriteLine("Colors found in regions pixmap :");
            foreach (Color c in this.rawRGBset)
                System.Diagnostics.Trace.WriteLine(c.ToString());

            if (Galaxy.Instance.Stars.Count < Galaxy.Instance.Configuration.empiresNumber())
            {
                this.TraceDefect("More empires than generated stars", true);
                return;
            }

            this.Result = true;
            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }

        protected HashSet<Color> rawRGBset;

        protected PointF RandomPosition(ref Color regionIndex)
        {
            PointF p, c;
            int secOut;
            Point pixD, pixR;
            double rawPosX, rawPosY;
            double localD, checkD, scale;
            Color regionColor, densityColor;

            c = new PointF((float)Galaxy.Instance.Configuration.maxWidth() / 2, (float)Galaxy.Instance.Configuration.maxWidth() / 2);
            scale = Galaxy.Instance.Configuration.maxWidth() / Galaxy.Instance.Configuration.shape().densityMap.Width;

            pixD = new Point();
            pixR = new Point();

            secOut = 0;
            do
            {
                do
                {
                    rawPosX = GalaxyGeneratorPlugin.random.NextDouble();
                    rawPosY = GalaxyGeneratorPlugin.random.NextDouble();
                    pixD.X = (int)(rawPosX * (double)(Galaxy.Instance.Configuration.shape().densityMap.Width));
                    pixD.Y = (int)(rawPosY * (double)(Galaxy.Instance.Configuration.shape().densityMap.Height));
                    pixR.X = (int)(rawPosX * (double)(Galaxy.Instance.Configuration.shape().regionsMap.Width));
                    pixR.Y = (int)(rawPosY * (double)(Galaxy.Instance.Configuration.shape().regionsMap.Height));
                    densityColor = Galaxy.Instance.Configuration.shape().densityMap.GetPixel(pixD.X, pixD.Y);
                    localD = (double)(densityColor.R + densityColor.G + densityColor.B) / 768.0;
                    checkD = GalaxyGeneratorPlugin.random.NextDouble();
                    regionColor = Galaxy.Instance.Configuration.shape().regionsMap.GetPixel(pixR.X, pixR.Y);
                    regionColor = Color.FromArgb(regionColor.R, regionColor.G, regionColor.B);
                    this.rawRGBset.Add(regionColor);
                    secOut++;
                }
                while ((checkD > localD) && (secOut < 120));

                this.PostProcess(ref regionColor);
            }
            while (regionColor == Color.Black);

            p = new PointF((float)(pixD.X * scale + GalaxyGeneratorPlugin.random.NextDouble() * scale),
                            (float)(pixD.Y * scale + GalaxyGeneratorPlugin.random.NextDouble() * scale));

            p.X -= c.X;
            p.Y -= c.Y;
            p.Y = -p.Y;

            regionIndex = regionColor;
            return p;
        }

        protected bool Overlap(PointF p)
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
                if (Geometry2D.Distance(p, s.position) <= Galaxy.Instance.Configuration.starOverlapDistance())
                    return true;
            return false;
        }

        protected void PostProcess(ref Color ck)
        {
            if (Galaxy.Instance.Configuration.shape().regions.Keys.Contains(ck))
                return;

            double dr, dg, db;
            double d, min;
            Color match;
            List<Color> allPlusBlack = new List<Color>(Galaxy.Instance.Configuration.shape().regions.Keys);

            allPlusBlack.Add(Color.Black);

            min = 9999999;
            match = Color.Black;

            foreach (Color k in allPlusBlack)
            {
                dr = ck.R - k.R;
                dg = ck.G - k.G;
                db = ck.B - k.B;
                d = System.Math.Sqrt(dr * dr + dg * dg + db * db);
                if (d < min)
                {
                    min = d;
                    match = k;
                }
            }

            if (match != null) ck = match;
        }
    }
}

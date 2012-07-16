// -----------------------------------------------------------------------
// <copyright file="HomeGenerator.cs" company="">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HomeGenerator : List<HomeTrait>
    {
        public class Pattern
        {
            public Pattern(StarSystem star)
            {
                this.Star = star;
                this.HomeWorld = star.Planets[star.HomeWorldIndex];
                this.OtherPlanets = new List<Planet>(star.Planets);
                this.OtherPlanets.Remove(this.HomeWorld);
            }

            public StarSystem Star;
            public Planet HomeWorld;
            public List<Planet> OtherPlanets;
        }

        public void Apply(StarSystem star)
        {
            foreach (HomeTrait t in this)
                t.Apply(star);
        }
    }
}

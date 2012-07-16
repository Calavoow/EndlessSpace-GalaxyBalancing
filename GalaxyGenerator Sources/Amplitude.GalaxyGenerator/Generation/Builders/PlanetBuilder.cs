using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Amplitude.GalaxyGenerator.Generation.Components;

    class PlanetBuilder : Builder
    {
        public override string Name { get { return "PlanetBuilder"; } }

        public override void Execute()
        {
            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            foreach (StarSystem star in Galaxy.Instance.Stars)
            {
                star.GeneratePlanets();
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }
    }
}

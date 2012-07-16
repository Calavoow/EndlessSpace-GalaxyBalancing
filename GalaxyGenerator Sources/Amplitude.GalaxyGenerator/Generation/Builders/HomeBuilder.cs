using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    using Amplitude.GalaxyGenerator.Generation.Components;

    class HomeBuilder : Builder
    {
        public override string Name { get { return "HomeBuilder"; } }

        public override void Execute()
        {
            int i;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - begin");

            i = 0;
            while ((i < Galaxy.Instance.Configuration.homeGenerators.Count) && (i < Galaxy.Instance.SpawnStars.Count))
            {
                System.Diagnostics.Trace.WriteLine("Applying Home Generator " + i.ToString() + " - begin");
                Galaxy.Instance.Configuration.homeGenerators[i].Apply(Galaxy.Instance.SpawnStars[i]);
                System.Diagnostics.Trace.WriteLine("Applying Home Generator " + i.ToString() + " - end");
                i++;
            }

            this.Result = true;

            System.Diagnostics.Trace.WriteLine(this.Name + " - Execute - end");
        }
    }
}

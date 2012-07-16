using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    public abstract class Builder
    {
        public Builder() { this.Result = true; this.Defects = new List<string>(); }
        public abstract void Execute();
        public bool Result { get; protected set; }
        public List<string> Defects { get; protected set; }
        virtual public string Name { get { return "Abstract Builder"; } }
        
        public void TraceDefect(string text, bool fatal = false)
        {
            this.Defects.Add(text);
            System.Diagnostics.Trace.WriteLine(text);
            if (fatal) this.Result = false;
        }
    }
}

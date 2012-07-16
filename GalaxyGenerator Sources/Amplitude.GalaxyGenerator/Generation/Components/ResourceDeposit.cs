// <copyright file="ResourceDeposit.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ResourceDeposit
    {
        public static int MaxSize = 3;
        public enum ResourceType {Strategic, Luxury};

        public ResourceDeposit(string n, int s, ResourceType t)
        {
            this.name = n;
            this.size = s;
            this.type = t;
        }

        public ResourceType type { get; protected set; }
        public string name { get; protected set; }
        public int size { get; protected set; }
        public void increaseSize() { this.size++; }
    }
}

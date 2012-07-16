// <copyright file="ShapeManager.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation
{
    using System.Collections.Generic;
    using System.Xml;

    public class ShapeManager
    {
        private static ShapeManager instance = null;

        private ShapeManager()
        {
            this.Shapes = new Dictionary<string, Shape>();
        }

        public static ShapeManager Instance
        {
            get
            {
                if (null == ShapeManager.instance)
                {
                    ShapeManager.instance = new ShapeManager();
                }

                return ShapeManager.instance;
            }
        }

        public Dictionary<string, Shape> Shapes
        {
            get;
            private set;
        }

        public static void Load(string pathToShapesFile)
        {
            if (null == ShapeManager.instance)
            {
                ShapeManager.Instance.LoadShapes(pathToShapesFile);
            }
        }

        public void LoadShapes(string pathToShapesFile)
        {
            using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(pathToShapesFile))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "GalaxyShape"))
                    {
                        string name = reader.GetAttribute("Name");
                        Shape shape = new Shape(reader);
//                        System.Diagnostics.Trace.WriteLine("shape name " + name);

                        if (!this.Shapes.ContainsKey(name))
                        {
                            this.Shapes.Add(name, shape);
                        }
                        else
                        {
                            this.Shapes[name] = shape;
                        }
                    }
                }
            }

            System.Diagnostics.Trace.WriteLine("cross-checking shapes manager contents...");
            foreach (string name in this.Shapes.Keys)
                System.Diagnostics.Trace.WriteLine(name);
            System.Diagnostics.Trace.WriteLine("...end of shapes manager contents cross-checking");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyGeneratorTest
{
    class GalaxyGeneratorTest
    {
        //private const string pathToSettingsFile = "..\\..\\..\\..\\GalaxyGenerator Data\\GalaxySettings.xml";
        //private const string pathToShapesFile = "..\\..\\..\\..\\GalaxyGenerator Data\\GalaxyShapes.xml";
        //private const string pathToConfigurationFile = "..\\..\\..\\..\\GalaxyGenerator Data\\GalaxyConfiguration.xml";
        //private const string pathToOutputFile = "..\\..\\..\\..\\GalaxyGenerator Data\\Galaxy.xml";
        private const string pathToSettingsFile = "GalaxySettings.xml";
        private const string pathToShapesFile = "GalaxyShapes.xml";
        private const string pathToConfigurationFile = "GalaxyConfiguration.xml";
        private const string pathToOutputFile = "Galaxy.xml";
        private const int seed = 123;

        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory("..\\..\\..\\..\\GalaxyGenerator Data\\");
            //string dir = System.IO.Directory.GetCurrentDirectory();
            //Console.WriteLine( dir );
            //System.IO.Directory.SetCurrentDirectory("..\\..\\..\\..\\GalaxyGenerator Data");
            //dir = System.IO.Directory.GetCurrentDirectory();
            //Console.WriteLine(dir);
            GalaxyGeneratorPlugin.Generate(pathToSettingsFile,
                                        pathToShapesFile,
                                        pathToConfigurationFile,
                                        pathToOutputFile,
                                        seed);

        }
    }
}

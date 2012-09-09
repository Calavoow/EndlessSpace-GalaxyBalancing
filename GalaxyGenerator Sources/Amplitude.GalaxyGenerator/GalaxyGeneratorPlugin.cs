// <copyright file="GalaxyGeneratorPlugin.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

using System.Xml;

using Amplitude.GalaxyGenerator.Generation;
using Amplitude.GalaxyGenerator.Generation.Components;

public static class GalaxyGeneratorPlugin
{
    internal static System.Random random;

    public static void Generate(string pathToSettingsFile, string pathToShapesFile, string pathToConfigurationFile, string pathToOutputFile, int seed)
    {
        string pathToDebugFile = System.IO.Directory.GetParent(pathToOutputFile).ToString() + "\\GalaxyGenerationPlugin.log";

#if DEBUG

        System.IO.File.Delete(pathToDebugFile);
        System.Diagnostics.TextWriterTraceListener debugFileListener = new System.Diagnostics.TextWriterTraceListener(pathToDebugFile);

        System.Diagnostics.Trace.Listeners.Add(debugFileListener);
        System.Diagnostics.Trace.AutoFlush = true;

        System.Diagnostics.Trace.WriteLine("Using pathToSettingsFile = " + pathToSettingsFile);
        System.Diagnostics.Trace.WriteLine("Using pathToShapesFile = " + pathToShapesFile);
        System.Diagnostics.Trace.WriteLine("Using pathToConfigurationFile = " + pathToConfigurationFile);
        System.Diagnostics.Trace.WriteLine("Using pathToOutputFile = " + pathToOutputFile);
        System.Diagnostics.Trace.WriteLine("Using seed = " + seed);

#endif

        try
        {
            Settings.Load(pathToSettingsFile);
            ShapeManager.Load(pathToShapesFile);

            Configuration configuration = new Configuration(pathToConfigurationFile);

            if (seed != 0)
            {
                configuration.seed = seed;
            }

            if (configuration.seed == 0)
            {
                long ticks = System.DateTime.Now.Ticks;
                configuration.seed = System.Math.Abs((int)ticks);
                //configuration.seed = generateSeed();
            }

            GalaxyGeneratorPlugin.random = new System.Random(configuration.seed);

            do
            {
                System.Diagnostics.Trace.WriteLine("Using seed: " + configuration.seed);
                Galaxy.Release();
                System.Diagnostics.Trace.WriteLine("Generating Galaxy...");
                Galaxy.Generate(configuration);
                if (!Galaxy.Instance.IsValid)
                {
                    configuration.seed = generateSeed();
                    GalaxyGeneratorPlugin.random = new System.Random(configuration.seed);
                }
            }
            while (!Galaxy.Instance.IsValid);

            System.Diagnostics.Trace.WriteLine("...Galaxy Generation Complete !");

            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = System.Xml.NewLineHandling.Replace,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter writer = XmlTextWriter.Create(pathToOutputFile, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("GenerationOutput");

                writer.WriteStartElement("GeneratorVersion");
                writer.WriteAttributeString("Revision", "105");
                writer.WriteAttributeString("Date", "20120612");
                writer.WriteEndElement();

                configuration.WriteOuterXml(writer);

                Galaxy.Instance.WriteXml(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

            Galaxy.Release();
        }
        catch (System.Exception exception)
        {
            System.Diagnostics.Trace.Write("Exception caught: " + exception.ToString());
            throw exception;
        }
        finally
        {
#if DEBUG
            debugFileListener.Close();
            System.Diagnostics.Trace.Listeners.Remove(debugFileListener);
#endif
            System.Diagnostics.Trace.Close();
        }
    }

    private static int generateSeed()
    {
        long ticks = System.DateTime.Now.Ticks;
        return System.Math.Abs((int)ticks);
    }
}

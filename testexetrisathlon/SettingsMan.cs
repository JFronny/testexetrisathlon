using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace testexetrisathlon
{
    internal static class SettingsMan
    {
        private static readonly string XmlFile =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Save.xml");

        public static int Volume
        {
            get => ToRange(int.Parse(Load().Element("Volume").Value), 0, 10);
            set
            {
                XElement doc = Load();
                doc.Element("Volume").Value = ToRange(value, 0, 10).ToString();
                doc.Save();
            }
        }

        public static int HighScore
        {
            get => int.Parse(Load().Element("HighScore").Value);
            set
            {
                XElement doc = Load();
                doc.Element("HighScore").Value = value.ToString();
                doc.Save();
            }
        }

        public static bool UsingAltTrack
        {
            get => bool.Parse(Load().Element("AltTrack").Value);
            set
            {
                XElement doc = Load();
                doc.Element("AltTrack").Value = value.ToString();
                doc.Save();
            }
        }

        private static void Save(this XElement doc) => doc.Save(XmlFile);

        private static XElement Load()
        {
            if (!File.Exists(XmlFile))
                new XElement("Save").Save(XmlFile);
            XElement doc = XDocument.Load(XmlFile).Root;
            if (doc.Element("Volume") == null)
                doc.Add(new XElement("Volume", 10));
            if (doc.Element("HighScore") == null)
                doc.Add(new XElement("HighScore", 10));
            if (doc.Element("AltTrack") == null)
                doc.Add(new XElement("AltTrack", false));
            return doc;
        }

        private static int ToRange(int value, int rangeStart, int rangeEnd) =>
            Math.Min(Math.Max(value, rangeStart), rangeEnd);
    }
}
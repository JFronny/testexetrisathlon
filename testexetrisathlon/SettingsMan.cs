using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace testexetrisathlon
{
    static class SettingsMan
    {
        static XElement doc;
        static string xmlfile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Save.xml";
        public static int Volume
        {
            get {
                if (doc == null)
                    Load();
                return toRange(int.Parse(doc.Element("Save").Element("Volume").Value), 0, 10);
            }
            set {
                doc.Element("Save").Element("Volume").Value = toRange(value, 0, 10).ToString();
                Save();
            }
        }
        public static int HighScore
        {
            get {
                if (doc == null)
                    Load();
                return int.Parse(doc.Element("Save").Element("HighScore").Value);
            }
            set {
                doc.Element("Save").Element("HighScore").Value = value.ToString();
                Save();
            }
        }
        static void Save() => doc.Save(xmlfile);
        static void Load()
        {
            if (!File.Exists(xmlfile))
                new XElement("Save", new XElement("Volume", 10), new XElement("HighScore", 0)).Save(xmlfile);
            doc = XDocument.Load(xmlfile).Root;
            if (doc.Element("Save") == null)
                doc.Add(new XElement("Save"));
            if (doc.Element("Save").Element("Volume") == null)
                doc.Element("Save").Add(new XElement("Volume", 10));
            if (doc.Element("Save").Element("HighScore") == null)
                doc.Element("Save").Add(new XElement("HighScore", 10));
        }

        static int toRange(int value, int rangeStart, int rangeEnd) => Math.Min(Math.Max(value, rangeStart), rangeEnd);
    }
}

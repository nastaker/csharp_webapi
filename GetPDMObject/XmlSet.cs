using System.Xml;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "SET")]
    public class XmlSet
    {
        [XmlElement("FORMGUID")]
        public string formguid { get; set; }
        [XmlElement("ACTION")]
        public string action { get; set; }
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
        [XmlAnyElement("OBJ")]
        public XmlNode obj { get; set; }
        [XmlElement("DESC")]
        public string desc { get; set; }
    }
}

using System.Xml;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "SET")]
    public class XmlSet
    {
        [XmlAttribute("FORMGUID")]
        public string formguid { get; set; }
        [XmlAttribute("ACTION")]
        public string actionname { get; set; }
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
        [XmlAnyElement("OBJ")]
        public XmlNode obj { get; set; }
        [XmlElement("DESC")]
        public string desc { get; set; }
    }
}

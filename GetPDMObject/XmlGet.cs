using System.Xml;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot("GET")]
    public class XmlUser
    {
        [XmlElement("LOGIN")]
        public string login { get; set; }
        [XmlElement("PSW")]
        public string psw { get; set; }
        [XmlElement("PRONAME")]
        public string proname { get; set; }
    }
    
    [XmlRoot(ElementName = "GET")]
    public class XmlGet
    {
        [XmlElement("ACTION")]
        public string action { get; set; }
        [XmlAnyElement("OBJ")]
        public XmlNode obj { get; set; }
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
    }
}

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
    public class XmlLoginGuid
    {
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
    }
}

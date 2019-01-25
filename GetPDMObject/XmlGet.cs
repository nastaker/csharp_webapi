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

    [XmlRoot(ElementName = "GET")]
    public class XmlGetForm
    {
        [XmlElement("OBJ")]
        public XmlGetDataRows obj { get; set; }
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
    }

    [XmlRoot(ElementName = "PARENT")]
    public class XmlGetDataRows
    {
        [XmlAttribute("CLASSNAME")]
        public string classname { get; set; }
        [XmlElement("GUID")]
        public string guid { get; set; }
        [XmlElement("PAGE")]
        public string page { get; set; }
        [XmlElement("PAGECOUNT")]
        public string pagesize { get; set; }
        [XmlElement("COLORDER")]
        public string order { get; set; }
        [XmlElement("SELECT")]
        public string query { get; set; }
    }
}

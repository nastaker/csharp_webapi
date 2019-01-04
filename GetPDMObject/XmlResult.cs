using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "RES")]
    public class XmlResult
    {
        [XmlElement("RECODE")]
        public string recode { get; set; }
        [XmlElement("REVALUE")]
        public object revalue { get; set; }
        [XmlElement("ERR")]
        public string err { get; set; }
    }

    [XmlRoot(ElementName = "USER")]
    public class XmlResultUser
    {
        [XmlAttribute("USERGUID")]
        public string userguid { get; set; }
        [XmlAttribute("USERNAME")]
        public string username { get; set; }

        public string loginguid { get; set; }
    }

    public class XmlMenu
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public List<XmlMenu> children { get; set; }
    }

    [XmlRoot(ElementName = "FUNGROUP")]
    public class XmlResultMenuGroup
    {
        [XmlAttribute("GROUPGUID")]
        public string guid { get; set; }
        [XmlAttribute("GROUPNAME")]
        public string name { get; set; }
        [XmlAttribute("GROUPIMG")]
        public string icon { get; set; }
        [XmlElement("FUN")]
        public List<XmlResultMenu> children { get; set; }
    }

    [XmlRoot(ElementName = "FUN")]
    public class XmlResultMenu
    {
        [XmlAttribute("FUNGUID")]
        public string guid { get; set; }
        [XmlAttribute("FUNNAME")]
        public string name { get; set; }
        [XmlAttribute("FUNDISP")]
        public string discrib { get; set; }
        [XmlAttribute("FUNTYPE")]
        public string type { get; set; }
        [XmlAttribute("FUNIMG")]
        public string icon { get; set; }
    }

    [XmlRoot(ElementName = "MYNODES")]
    public class XmlResultUserRole
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("ROLE")]
        public string role { get; set; }
        [XmlElement("COMPANY")]
        public List<XmlResultCompany> organizations { get; set; }
    }

    [XmlRoot(ElementName = "COMPANY")]
    public class XmlResultCompany
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("ORDER")]
        public string order { get; set; }
        [XmlElement("DEPART")]
        public List<XmlResultDepart> children { get; set; }
    }

    [XmlRoot(ElementName = "DEPART")]
    public class XmlResultDepart
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("ORDER")]
        public string order { get; set; }
    }
}

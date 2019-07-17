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

    [XmlRoot("OBJ")]
    public class XmlUserRegist
    {
        [XmlElement("CLASSNAME")]
        public string classname { get; set; }
        [XmlElement("GUID")]
        public string guid { get; set; }
        [XmlElement("CN_TYPE")]
        public string type { get; set; }
        [XmlElement("CN_PAR_CLASS")]
        public string parentClass { get; set; }
        [XmlElement("CN_PAR_GUID")]
        public string parentGuid { get; set; }
        [XmlElement("CN_LOGIN")]
        public string login { get; set; }
        [XmlElement("CN_USER_NAME")]
        public string username { get; set; }
        [XmlElement("cn_psw")]
        public string password { get; set; }
        [XmlElement("CN_PSW2")]
        public string repeatPassword { get; set; }
        [XmlElement("CN_ORDER")]
        public string order { get; set; }
        [XmlElement("CN_USERCODE")]
        public string usercode { get; set; }
        [XmlElement("CN_PHONE")]
        public string phone { get; set; }
        [XmlElement("CN_EMAIL")]
        public string email { get; set; }
        [XmlElement("CN_DESC")]
        public string desc { get; set; }
    }
}

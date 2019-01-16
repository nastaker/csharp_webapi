using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "SET")]
    public class XmlSet
    {
        [XmlElement("LOGINGUID")]
        public string loginguid { get; set; }
        [XmlElement("OBJ")]
        public XmlSetType obj { get; set; }
        [XmlElement("PARENT")]
        public XmlSetType parent { get; set; }
        [XmlElement("DESC")]
        public string desc { get; set; }
    }

    [XmlInclude(typeof(XmlSetFormData))]
    [XmlInclude(typeof(XmlSetTypeParent))]
    public class XmlSetType
    {
        [XmlAttribute("CLASSNAME")]
        public string classname { get; set; }
        [XmlElement("GUID")]
        public string guid { get; set; }
    }

    [XmlRoot(ElementName = "COMPANY")]
    public class XmlSetFormData : XmlSetType
    {
        [XmlElement("CN_NAME")]
        public string name { get; set; }
        [XmlElement("CN_LOGIN")]
        public string login { get; set; }
        [XmlElement("CN_PSW")]
        public string password { get; set; }
        [XmlElement("CN_LEADER")]
        public int isLeader { get; set; }
        [XmlElement("CN_SEX")]
        public string gender { get; set; }
        [XmlElement("CN_ENTRYDAY")]
        public string entrydate { get; set; }
        [XmlElement("CN_USERCODE")]
        public string usercode { get; set; }
        [XmlElement("CN_PHONE")]
        public string phone { get; set; }
        [XmlElement("CN_EMAIL")]
        public string email { get; set; }
        [XmlElement("CN_DESC")]
        public string desc { get; set; }
    }

    [XmlRoot(ElementName = "PARENT")]
    public class XmlSetTypeParent : XmlSetType
    {
        [XmlElement("OBJ")]
        public XmlSetType obj { get; set; }
    }

    [XmlRoot(ElementName = "PARENT")]
    public class XmlSetForm 
    {
        [XmlAttribute("CLASSNAME")]
        public string classname { get; set; }
        [XmlElement("GUID")]
        public string guid { get; set; }
    }


}

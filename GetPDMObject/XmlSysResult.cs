using System;
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
    public class XmlResultUserLogin
    {
        [XmlAttribute("USERGUID")]
        public string userguid { get; set; }
        [XmlAttribute("USERNAME")]
        public string username { get; set; }

        public string loginguid { get; set; }
    }

    [XmlRoot(ElementName = "MENU")]
    public class XmlResultMenu
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("FORMGUID")]
        public string formguid { get; set; }
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("IMG")]
        public string icon { get; set; }
        [XmlAttribute("DISP")]
        public string text { get; set; }
        [XmlAttribute("TYPE")]
        public string type { get; set; }
        [XmlElement("MENU")]
        public List<XmlResultMenu> children { get; set; }
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
        [XmlElement("DEPART")]
        public List<XmlResultDepart> children { get; set; }
        [XmlElement("USER")]
        public List<XmlResultUser> users { get; set; }
    }

    [XmlRoot(ElementName = "USER")]
    public class XmlResultUser
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("LOGIN")]
        public string login { get; set; }
        [XmlAttribute("LEADER")]
        public string isLeader { get; set; }
        [XmlAttribute("ORDER")]
        public string order { get; set; }
    }

    [XmlRoot(ElementName = "FORM")]
    public class XmlResultForm
    {
        [XmlAttribute("FORMGUID")]
        public string guid { get; set; }
        [XmlAttribute("FORMTYPE")]
        public string type { get; set; }
        [XmlAttribute("FORMNAME")]
        public string name { get; set; }
        [XmlAttribute("S_WIDTH")]
        public int width { get; set; }
        [XmlAttribute("S_HEIGHT")]
        public int height { get; set; }
        [XmlAttribute("S_FONT_SIZE")]
        public int fontsize { get; set; }
        [XmlElement("TABPAGE")]
        public List<XmlResultTabpage> tabs { get; set; }
    }

    [XmlRoot(ElementName = "TABPAGE")]
    public class XmlResultTabpage
    {
        [XmlAttribute("TABGUID")]
        public string guid { get; set; }
        [XmlAttribute("TABTYPE")]
        public string type { get; set; }
        [XmlAttribute("TABNAME")]
        public string name { get; set; }
        [XmlAttribute("S_WIDTH")]
        public int width { get; set; }
        [XmlAttribute("S_HEIGHT")]
        public int height { get; set; }

        [XmlElement("CONTROL")]
        public List<XmlResultControl> controls { get; set; }
    }

    [XmlRoot(ElementName = "CONTROL")]
    public class XmlResultControl
    {
        [XmlAttribute("CONTROLGUID")]
        public string guid { get; set; }
        [XmlAttribute("CONTROLTYPE")]
        public string type { get; set; }
        [XmlAttribute("VALUE")]
        public string value { get; set; }
        [XmlAttribute("FIELDNAME")]
        public string fieldname { get; set; }
        [XmlAttribute("FIELDTYPE")]
        public string fieldtype { get; set; }
        [XmlAttribute("FIELDLONG")]
        public int length { get; set; }
        [XmlAttribute("MUST")]
        public string isRequired { get; set; }
        [XmlAttribute("READONLY")]
        public string isReadonly { get; set; }
        [XmlAttribute("S_COL")]
        public int col { get; set; }
        [XmlAttribute("S_WIDTH")]
        public int width { get; set; }
        [XmlAttribute("S_HEIGHT")]
        public int height { get; set; }
    }
}

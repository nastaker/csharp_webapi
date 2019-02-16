using System.Collections.Generic;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "FORM")]
    public class XmlResultForm
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("FORMTYPE")]
        public string type { get; set; }
        [XmlAttribute("FORMNAME")]
        public string name { get; set; }
        [XmlAttribute("SHOW")]
        public string show { get; set; }
        [XmlAttribute("WIDTH")]
        public string width { get; set; }
        [XmlAttribute("HEIGHT")]
        public string height { get; set; }
        [XmlElement("TABPAGE")]
        public List<XmlResultTabpage> tabs { get; set; }
    }

    [XmlRoot(ElementName = "GRID")]
    public class XmlResultTableDef
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("SHOWNUM")]
        public string rownumber { get; set; }
        [XmlAttribute("SHOWCHECK")]
        public string checkbox { get; set; }
        [XmlAttribute("DEFQUERY")]
        public string defquery { get; set; }
        [XmlElement("COL")]
        public List<XmlResultColumnDef> cols { get; set; }
    }

    [XmlRoot(ElementName = "COL")]
    public class XmlResultColumnDef
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("COLNAME")]
        public string field { get; set; }
        [XmlAttribute("COLDISP")]
        public string label { get; set; }
        [XmlAttribute("COLTYPE")]
        public string type { get; set; }
        [XmlAttribute("SHOW")]
        public string isDisplay { get; set; }
        [XmlAttribute("ORDER")]
        public string order { get; set; }
        [XmlAttribute("SELECT")]
        public string isQuery { get; set; }
        [XmlAttribute("FROZEN")]
        public string isFixed { get; set; }
        [XmlAttribute("FIELDTYPE")]
        public string fieldType { get; set; }
        [XmlAttribute("TEXTAGIN")]
        public string align { get; set; }
        [XmlAttribute("WIDTH")]
        public string width { get; set; }
    }

    [XmlRoot(ElementName = "MENUS")]
    public class XmlResultMenu
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("IMG")]
        public string icon { get; set; }
        [XmlElement("MENU")]
        public List<XmlResultMenu> children { get; set; }
        [XmlElement("ACTION")]
        public XmlResultMenuAction action { get; set; }
    }

    [XmlRoot(ElementName = "ACTION")]
    public class XmlResultMenuAction
    {
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("SHOWMSG")]
        public string hasMsg { get; set; }
        [XmlAttribute("ISFROM")]
        public string isform { get; set; }
        [XmlAttribute("UPFORM")]
        public string type { get; set; }
        [XmlAttribute("FORMGUID")]
        public string formguid { get; set; }
    }

    [XmlRoot(ElementName = "OBJ")]
    public class XmlResultDataTable
    {
        [XmlElement("GUID")]
        public string guid { get; set; }
        [XmlElement("PAGE")]
        public string page { get; set; }
        [XmlElement("PAGECOUNT")]
        public string pagesize { get; set; }
        [XmlElement("COLORDER")]
        public string order { get; set; }
        [XmlElement("TABCOUNT")]
        public string tabcount { get; set; }
        [XmlElement("SELECT")]
        public string query { get; set; }
        [XmlElement("ROW")]
        public List<XmlResultDataRow> rows { get; set; }
    }

    [XmlRoot(ElementName = "ROW")]
    public class XmlResultDataRow
    {
        [XmlElement("ROWDEF")]
        public List<XmlResultDataRowData> rowdata { get; set; }
    }

    [XmlRoot(ElementName = "ROWDEF")]
    public class XmlResultDataRowData
    {
        [XmlElement("COLNAME")]
        public string name { get; set; }
        [XmlElement("VALUE")]
        public string value { get; set; }
        [XmlElement("ACTION")]
        public List<XmlResultMenuAction> actions { get; set; }
    }

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

    [XmlRoot(ElementName = "TABPAGE")]
    public class XmlResultTabpage
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("TABTYPE")]
        public string type { get; set; }
        [XmlAttribute("TABNAME")]
        public string name { get; set; }
        [XmlAttribute("SHOW")]
        public string show { get; set; }

        [XmlElement("CONTROL")]
        public List<XmlResultControl> controls { get; set; }
        [XmlElement("TREE")]
        public List<XmlResultTree> tree { get; set; }
        [XmlElement("MENUS")]
        public XmlResultMenu menu { get; set; }
        [XmlElement("GRID")]
        public XmlResultTableDef tableDef { get; set; }
    }

    [XmlRoot(ElementName = "TREE")]
    public class XmlResultTree
    {
        [XmlAttribute("CLASSNAME")]
        public string classname { get; set; }
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("NAME")]
        public string label { get; set; }
        [XmlAttribute("IMG")]
        public string img { get; set; }
        [XmlAttribute("BACKCOLOR")]
        public string backColor { get; set; }
        [XmlAttribute("FORCOLOR")]
        public string foreColor { get; set; }

        [XmlElement("MENU")]
        public List<XmlResultMenu> menu { get; set; }
        [XmlElement("TREE")]
        public List<XmlResultTree> children { get; set; }
    }

    [XmlRoot(ElementName = "CONTROL")]
    public class XmlResultControl
    {
        [XmlAttribute("GUID")]
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
        [XmlAttribute("COL")]
        public int col { get; set; }
        [XmlAttribute("HEIGHT")]
        public int height { get; set; }
        [XmlElement("ITEM")]
        public List<XmlResultControlItem> items { get; set; }
    }

    [XmlRoot(ElementName = "ITEM")]
    public class XmlResultControlItem
    {
        [XmlAttribute("VALUE")]
        public string value { get; set; }
        [XmlAttribute("LABEL")]
        public string label { get; set; }
        [XmlElement("ITEM")]
        public List<XmlResultControlItem> children { get; set; }
    }
}

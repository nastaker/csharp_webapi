using System.Collections.Generic;
using System.Xml.Serialization;

namespace GetPDMObject
{
    [XmlRoot(ElementName = "FORM")]
    public class XmlResultDataDef
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("FORMTYPE")]
        public string type { get; set; }
        [XmlAttribute("FORMNAME")]
        public string name { get; set; }
        [XmlAttribute("SHOWNAME")]
        public string showname { get; set; }
        [XmlElement("TABPAGE")]
        public List<XmlResultTabpage2> tabs { get; set; }
    }

    [XmlRoot(ElementName = "TABPAGE")]
    public class XmlResultTabpage2
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("TABTYPE")]
        public string type { get; set; }
        [XmlAttribute("TABNAME")]
        public string name { get; set; }
        [XmlAttribute("SHOW")]
        public string show { get; set; }
        [XmlElement("MENUS")]
        public XmlResultMenu2 menu { get; set; }
        [XmlElement("GRID")]
        public XmlResultTableDef tableDef { get; set; }
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
    public class XmlResultMenu2
    {
        [XmlAttribute("GUID")]
        public string guid { get; set; }
        [XmlAttribute("MENUNAME")]
        public string name { get; set; }
        [XmlAttribute("IMG")]
        public string icon { get; set; }
        [XmlElement("MENU")]
        public List<XmlResultMenu2> children { get; set; }
        [XmlElement("ACTION")]
        public XmlResultMenu2Action action { get; set; }
    }

    [XmlRoot(ElementName = "ACTION")]
    public class XmlResultMenu2Action
    {
        [XmlAttribute("NAME")]
        public string name { get; set; }
        [XmlAttribute("SHOWMSG")]
        public string hasMsg { get; set; }
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
        [XmlElement("ROWS")]
        public XmlResultDataRowDef rowdef { get; set; }
    }

    [XmlRoot(ElementName = "ROWS")]
    public class XmlResultDataRowDef
    {
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
    }
}

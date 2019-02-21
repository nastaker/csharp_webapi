using OiClassDm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GetPDMObject
{
    public class PDMUtils
    {
        public static XmlResultUserLogin login(string login, string pwd, string proname)
        {
            XmlUser xmlUser = new XmlUser
            {
                login = login,
                psw = pwd,
                proname = proname
            };
            XmlDocument xmlDoc = OiEmData.Org_UserLogin(SerializeToXmlDocument(xmlUser) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            XmlResultUserLogin xmlResultUser = DeserializeXmlDocument(typeof(XmlResultUserLogin), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserLogin;
            xmlResultUser.loginguid = (xmlResult.revalue as XmlNode[])[1].InnerText;
            return xmlResultUser;
        }


        public static List<XmlResultMenu> getMenu(XmlGet xmlget)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetMyMenu(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            XmlResultMenu menu = DeserializeXmlDocument(typeof(XmlResultMenu), ConvertToXmlNode(xmlResult.revalue, "MENUS")) as XmlResultMenu;
            return menu.children;
        }

        public static List<XmlResultMenu> changeRole(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            XmlResultMenu menu = DeserializeXmlDocument(typeof(XmlResultMenu), ConvertToXmlNode(xmlResult.revalue, "MENUS")) as XmlResultMenu;
            return menu.children;
        }

        public static XmlResultUserRole getRole(XmlGet xmlget)
        {
            //获取权限
            XmlDocument xmlDoc = OiEmData.Org_GetMyNodes(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultUserRole), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserRole;
        }

        public static XmlResultData modifyFormData(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultData), ConvertToXmlNode(xmlResult.revalue)) as XmlResultData;
        }

        public static XmlResult delete(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiEmData.Org_DeleteDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return xmlResult;
        }

        public static XmlResultDataTable getDataRows(XmlGet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetGridValue(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultDataTable), (xmlResult.revalue as XmlNode[])[0]) as XmlResultDataTable;
        }

        public static XmlResultForm setAction(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[0]) as XmlResultForm;
        }

        public static XmlResultForm setAction(XmlDocument xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(xmlSet);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[0]) as XmlResultForm;
        }

        public static XmlResultFile getDownload(XmlDocument xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(xmlSet);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultFile), (xmlResult.revalue as XmlNode[])[0]) as XmlResultFile;
        }

        public static List<XmlResultTree> getTreeNodes(XmlDocument doc)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetTreeNode(doc);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new Exception(xmlResult.err);
            }
            XmlResultTree tree = DeserializeXmlDocument(typeof(XmlResultTree), ConvertToXmlNode(xmlResult.revalue, "TREE")) as XmlResultTree;
            return tree.children;   
        }

        public static XmlNode ConvertToXmlNode(object obj, string root = "REVALUE")
        {
            XmlNode[] nodes = obj as XmlNode[];
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement node = xmlDocument.CreateElement(root, "");
            xmlDocument.AppendChild(node);
            for (int i = 0, j = nodes.Length; i < j; i++)
            {
                node.AppendChild(xmlDocument.ImportNode(nodes[i], true));
            }
            return xmlDocument;
        }
        
        #region 测试用方法
        public static string getDataRows(XmlGet xmlSet, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetGridValue(SerializeToXmlDocument(xmlSet) as XmlDocument);
            return xmlDoc.OuterXml;
        }
        public static string setAction(XmlSet xmlSet, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            return xmlDoc.OuterXml;
        }
        public static string getMenu(XmlGet xmlget, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetMyMenu(SerializeToXmlDocument(xmlget) as XmlDocument);
            return xmlDoc.OuterXml;
        }

        #endregion

        public static XmlNode SerializeToXmlDocument(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType(), "");

            XmlDocument xd = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }

        public static object DeserializeXmlDocument(Type type, XmlNode input)
        {
            XmlSerializer ser = new XmlSerializer(type);
            using (XmlReader reader = new XmlNodeReader(input))
            {
                return ser.Deserialize(reader);
            }
        }
    }
}

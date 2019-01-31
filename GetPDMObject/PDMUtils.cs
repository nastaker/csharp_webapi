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


        public static List<XmlResultMenu> getMenu(string loginguid)
        {
            XmlLoginGuid xmlLoginGuid = new XmlLoginGuid
            {
                loginguid = loginguid
            };
            List<XmlResultMenu> xmlMenus = new List<XmlResultMenu>();
            XmlDocument xmlDoc = OiProData.Pro_GetMyMenu(SerializeToXmlDocument(xmlLoginGuid) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            XmlNode[] res = (xmlResult.revalue as XmlNode[]);
            for (int i = 0, j = res.Length; i < j; i++)
            {
                xmlMenus.Add(DeserializeXmlDocument(typeof(XmlResultMenu), res[i]) as XmlResultMenu);
            }
            return xmlMenus;
        }

        public static XmlResultUserRole getRole(string loginguid)
        {
            //获取权限
            XmlLoginGuid xmlLoginGuid = new XmlLoginGuid
            {
                loginguid = loginguid
            };
            XmlDocument xmlDoc = OiEmData.Org_GetMyNodes(SerializeToXmlDocument(xmlLoginGuid) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultUserRole), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserRole;
        }

        public static XmlResultForm getModifyForm(XmlGetForm xmlCreateForm)
        {
            XmlDocument xmlDoc = OiEmData.Org_GetDataForm(SerializeToXmlDocument(xmlCreateForm) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[1]) as XmlResultForm;
        }

        public static XmlResult modifyFormData(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return xmlResult;
        }

        public static XmlResult delete(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiEmData.Org_DeleteDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return xmlResult;
        }

        public static XmlResultDataDef getDataDef(XmlGetForm xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetForm(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultDataDef), (xmlResult.revalue as XmlNode[])[0]) as XmlResultDataDef;
        }

        public static XmlResultDataTable getDataRows(XmlGetForm xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetGridValue(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultDataTable), (xmlResult.revalue as XmlNode[])[0]) as XmlResultDataTable;
        }

        public static XmlResultValue setAction(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultValue), ConvertToXmlNode(xmlResult.revalue)) as XmlResultValue;
        }

        public static XmlResultValue setAction(XmlDocument xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(xmlSet);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            if (xmlResult.recode != "0")
            {
                throw new LoginException(xmlResult.err);
            }
            return DeserializeXmlDocument(typeof(XmlResultValue), ConvertToXmlNode(xmlResult.revalue)) as XmlResultValue;
        }

        public static XmlNode ConvertToXmlNode(Object obj)
        {
            XmlNode[] nodes = obj as XmlNode[];
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement node = xmlDocument.CreateElement("REVALUE", "");
            xmlDocument.AppendChild(node);
            for (int i = 0, j = nodes.Length; i < j; i++)
            {
                node.AppendChild(xmlDocument.ImportNode(nodes[i], true));
            }
            return xmlDocument;
        }
        
        #region 测试用方法
        public static string getDataRows(XmlGetForm xmlSet, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetGridValue(SerializeToXmlDocument(xmlSet) as XmlDocument);
            return xmlDoc.OuterXml;
        }
        public static string setAction(XmlSet xmlSet, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
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

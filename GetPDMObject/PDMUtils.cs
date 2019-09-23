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
        /// <summary>
        /// 从本地数据库登录
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <param name="proname"></param>
        /// <returns></returns>
        public static ResultInfo<XmlResultUserLogin> login(string login, string pwd, string proname)
        {
            XmlUser xmlUser = new XmlUser
            {
                login = login,
                psw = pwd,
                proname = proname
            };
            XmlDocument xmlDoc = OiEmData.Org_UserLogin(SerializeToXmlDocument(xmlUser) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultUserLogin xmlResultUser = DeserializeXmlDocument(typeof(XmlResultUserLogin), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserLogin;
            return new ResultInfo<XmlResultUserLogin>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = xmlResultUser
            };
        }

        /// <summary>
        /// 从国家平台登录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="login"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ResultInfo<XmlResultUserLogin> login(int id, string login, string name)
        {
            XmlUser xmlUser = new XmlUser
            {
                id = id,
                login = login,
                name = name
            };
            XmlDocument xmlDoc = OiEmData.Org_UserLoginCountry(SerializeToXmlDocument(xmlUser) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultUserLogin xmlResultUser = DeserializeXmlDocument(typeof(XmlResultUserLogin), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserLogin;
            return new ResultInfo<XmlResultUserLogin>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = xmlResultUser
            };
        }
        
        public static ResultInfo<XmlResultMessages> getMessage(XmlGet xmlget)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetMsg(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultMessages msgs = (DeserializeXmlDocument(typeof(XmlResultMessages), ConvertToXmlNode(xmlResult.revalue)) as XmlResultMessages);
            return new ResultInfo<XmlResultMessages>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = msgs
            };
        }

        public static ResultInfo<XmlResultFile> getDownload(XmlDocument doc)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(doc);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultFile file = DeserializeXmlDocument(typeof(XmlResultFile), (xmlResult.revalue as XmlNode[])[0]) as XmlResultFile;
            return new ResultInfo<XmlResultFile>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = file
            };
        }

        public static ResultInfo<XmlResultFile> getExportData(XmlGet xmlget)
        {
            XmlDocument xmlDoc = OiProData.Pro_GridDataOut(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultFile file = DeserializeXmlDocument(typeof(XmlResultFile), (xmlResult.revalue as XmlNode[])[0]) as XmlResultFile;
            return new ResultInfo<XmlResultFile>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = file
            };
        }

        public static ResultInfo<List<XmlResultMenu>> getMenu(XmlGet xmlget)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetMyMenu(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultMenu menu = DeserializeXmlDocument(typeof(XmlResultMenu), ConvertToXmlNode(xmlResult.revalue, "MENUS")) as XmlResultMenu;
            return new ResultInfo<List<XmlResultMenu>>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = menu.children
            };
        }

        public static ResultInfo<List<XmlResultMenu>> changeRole(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultMenu menu = DeserializeXmlDocument(typeof(XmlResultMenu), ConvertToXmlNode(xmlResult.revalue, "MENUS")) as XmlResultMenu;
            return new ResultInfo<List<XmlResultMenu>>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = menu.children
            };
        }

        public static ResultInfo<XmlResultUserRole> getRole(XmlGet xmlget)
        {
            //获取权限
            XmlDocument xmlDoc = OiEmData.Org_GetMyNodes(SerializeToXmlDocument(xmlget) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            var role = DeserializeXmlDocument(typeof(XmlResultUserRole), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserRole;
            return new ResultInfo<XmlResultUserRole>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = role
            };
        }

        public static ResultInfo<XmlResultData> modifyFormData(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            var result = DeserializeXmlDocument(typeof(XmlResultData), ConvertToXmlNode(xmlResult.revalue)) as XmlResultData;
            return new ResultInfo<XmlResultData>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = result
            };
        }

        public static ResultInfo<XmlResultDataTable> getDataRows(XmlGet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetGridValue(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            var dataTable = DeserializeXmlDocument(typeof(XmlResultDataTable), (xmlResult.revalue as XmlNode[])[0]) as XmlResultDataTable;
            return new ResultInfo<XmlResultDataTable>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = dataTable
            };
        }

        public static ResultInfo<XmlResultForm> setAction(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            var XmlResultForm = DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[0]) as XmlResultForm;
            return new ResultInfo<XmlResultForm>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = XmlResultForm
            };
        }

        public static ResultInfo<XmlResultForm> setAction(XmlDocument xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(xmlSet);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            var xmlResultForm = DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[0]) as XmlResultForm;
            return new ResultInfo<XmlResultForm>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = xmlResultForm
            };
        }

        public static ResultInfo<string> getString(XmlDocument xmlSet)
        {
            XmlDocument xmlDoc = OiProData.Pro_SetAction(xmlSet);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            string url = string.Empty;
            if ((xmlResult.revalue as XmlNode[])[0] != null){
                url = (xmlResult.revalue as XmlNode[])[0].InnerText;
            }
            return new ResultInfo<string>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = url
            };
        }
        
        public static ResultInfo<XmlResultDataTree> getTreeNodes(XmlDocument doc)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetTreeNode(doc);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlResultDataTree tree = DeserializeXmlDocument(typeof(XmlResultDataTree), ConvertToXmlNode(xmlResult.revalue)) as XmlResultDataTree;
            return new ResultInfo<XmlResultDataTree>
            {
                code = xmlResult.recode,
                msg = xmlResult.err,
                obj = tree
            };
        }

        public static XmlNode ConvertToXmlNode(object obj, string root = "REVALUE")
        {
            XmlNode[] nodes = obj as XmlNode[];
            if(nodes == null)
            {
                return null;
            }
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

        public static string getTreeNodes(XmlDocument doc, bool test)
        {
            XmlDocument xmlDoc = OiProData.Pro_GetTreeNode(doc);
            return xmlDoc.OuterXml;
        }

        public static string login(string login, string pwd, string proname, bool test)
        {
            XmlUser xmlUser = new XmlUser
            {
                login = login,
                psw = pwd,
                proname = proname
            };
            XmlDocument xmlDoc = OiEmData.Org_UserLogin(SerializeToXmlDocument(xmlUser) as XmlDocument);
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
            if (input == null)
            {
                return null;
            }
            XmlSerializer ser = new XmlSerializer(type);
            using (XmlReader reader = new XmlNodeReader(input))
            {
                return ser.Deserialize(reader);
            }
        }
    }
}

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
            return DeserializeXmlDocument(typeof(XmlResultUserRole), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUserRole;
        }

        public static XmlResultForm getModifyForm(XmlGetForm xmlCreateForm)
        {
            XmlDocument xmlDoc = OiEmData.Org_GetDataForm(SerializeToXmlDocument(xmlCreateForm) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            return DeserializeXmlDocument(typeof(XmlResultForm), (xmlResult.revalue as XmlNode[])[0]) as XmlResultForm;
        }

        public static XmlResult modifyFormData(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiEmData.Org_SetDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            return xmlResult;
        }

        public static XmlResult delete(XmlSet xmlSet)
        {
            XmlDocument xmlDoc = OiEmData.Org_DeleteDataRow(SerializeToXmlDocument(xmlSet) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            return xmlResult;
        }

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
                try
                {
                    return ser.Deserialize(reader);
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }
    }
}

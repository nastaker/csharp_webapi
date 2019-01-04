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
        public static XmlResultUser login(string login, string pwd, string proname)
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
            XmlResultUser xmlResultUser = DeserializeXmlDocument(typeof(XmlResultUser), (xmlResult.revalue as XmlNode[])[0]) as XmlResultUser;
            xmlResultUser.loginguid = (xmlResult.revalue as XmlNode[])[1].InnerText;
            return xmlResultUser;
        }


        public static List<XmlMenu> getMenu(string loginguid)
        {
            XmlLoginGuid xmlLoginGuid = new XmlLoginGuid
            {
                loginguid = loginguid
            };
            List<XmlMenu> xmlMenus = new List<XmlMenu>();
            XmlDocument xmlDoc = OiProData.Pro_GetMyMenu(SerializeToXmlDocument(xmlLoginGuid) as XmlDocument);
            XmlResult xmlResult = DeserializeXmlDocument(typeof(XmlResult), xmlDoc) as XmlResult;
            XmlNode[] res = (xmlResult.revalue as XmlNode[]);
            for (int i = 0, j = res.Length; i < j; i++)
            {
                XmlResultMenuGroup menuGroup = DeserializeXmlDocument(typeof(XmlResultMenuGroup), res[i]) as XmlResultMenuGroup;
                if (menuGroup != null)
                {
                    XmlMenu xmlMenu = new XmlMenu
                    {
                        guid = menuGroup.guid,
                        text = menuGroup.name,
                        icon = menuGroup.icon
                    };
                    xmlMenu.children = new List<XmlMenu>();
                    for(int x = 0, y = menuGroup.children.Count; x < y; x++)
                    {
                        var menu = menuGroup.children[x];
                        xmlMenu.children.Add(new XmlMenu
                        {
                            guid = menu.guid,
                            name = menu.name,
                            icon = menu.icon,
                            text = menu.discrib,
                            type = menu.type
                        });
                    }
                    xmlMenus.Add(xmlMenu);
                }
                else
                {
                    XmlResultMenu menu = DeserializeXmlDocument(typeof(XmlResultMenu), res[i]) as XmlResultMenu;
                    if (menu != null)
                    {
                        xmlMenus.Add(new XmlMenu
                        {
                            guid = menu.guid,
                            name = menu.name,
                            icon = menu.icon,
                            text = menu.discrib,
                            type = menu.type
                        });
                    }
                }
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

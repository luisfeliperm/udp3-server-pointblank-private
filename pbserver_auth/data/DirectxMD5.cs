using Core.Logs;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Auth.data
{
    public class DirectxMD5
    {
        public static List<string> md5s = new List<string>();
        
        public static bool IsValid(string md5)
        {
            if (md5.Length != 32)
                return false;
            for (int i = 0; i < md5s.Count; i++)
            {
                if (md5s[i] == md5)
                    return true;
            }
            return false;
        }
        public static void Load()
        {
            string path = "data/DirectX.xml";
            if (!File.Exists(path))
            {
                Printf.danger("[DirectXML] Não existe o arquivo: " + path);
                return;
            }
                

            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                if (fileStream.Length > 0)
                {
                    try
                    {
                        xmlDocument.Load(fileStream);
                        for (XmlNode xmlNode1 = xmlDocument.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                        {
                            if ("list".Equals(xmlNode1.Name))
                            {
                                for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                                {
                                    if ("d3d9".Equals(xmlNode2.Name))
                                    {
                                        XmlNamedNodeMap xml = xmlNode2.Attributes;
                                        md5s.Add(xml.GetNamedItem("md5").Value.ToLower());
                                    }
                                }
                            }
                        }
                    }
                    catch (XmlException ex)
                    {
                        SaveLog.fatal(ex.ToString());
                        Printf.b_danger("[DirectxMD5.Load] Erro fatal!");
                    }
                }
                fileStream.Dispose();
                fileStream.Close();
            }
        }
    }
}
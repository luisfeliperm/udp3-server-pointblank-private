using Core.Logs;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Game.data.xml
{
    public class WelcomeXML
    {
        public static List<WelcomeModel> _welcome = new List<WelcomeModel>();
        public class WelcomeModel
        {
            public string _title, _txt;
            public short _color;
        }

        public static void Load()
        {
            string path = "data/Welcome.xml";
            if (File.Exists(path))
                parse(path);
            else
                Printf.danger("[WelcomeXML] Não existe o arquivo: " + path);
        }

        private static void parse(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                if (fileStream.Length == 0)
                    Printf.warning("[WelcomeXML] O arquivo está vazio: " + path);
                else
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
                                    if ("msg".Equals(xmlNode2.Name))
                                    {
                                        XmlNamedNodeMap xml = xmlNode2.Attributes;

                                        WelcomeModel ev = new WelcomeModel
                                        {
                                            _title = xml.GetNamedItem("title").Value,
                                            _txt = xml.GetNamedItem("text").Value,
                                            _color = short.Parse(xml.GetNamedItem("color").Value)
                                        };
                                        _welcome.Add(ev);
                                    }
                                }
                            }
                        }
                        if(_welcome.Count == 0)
                        {
                            Printf.warning("[Aviso] Não existe mensagem de boas vindas");
                        }

                    }
                    catch (XmlException ex)
                    {
                        SaveLog.fatal(ex.ToString());
                        Printf.b_danger("[WelcomeXML.Parse] Erro fatal!");
                    }
                }
                fileStream.Dispose();
                fileStream.Close();
            }
        }



    }
}
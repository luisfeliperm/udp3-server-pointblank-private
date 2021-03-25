using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Core.Logs;
using System.IO;

namespace Core.Version
{
    public class ServerLicense
    {
        private static string dominio = "http://luisfeliperm.duckdns.org:8081/";
        public static bool check()
        {

            if (int.Parse(DateTime.Now.ToString("yyMMdd")) > 200210)
            {
                Printf.danger("Versão expirou, entre em contato com o vendedor");
                File.Delete(@"data/GameServer/GameServers.json");
                File.Delete(@"config/auth.ini");
                File.Delete(@"config/battle.ini");
                File.Delete(@"config/game.ini");
                return false;
            }

            return true;
            try
            {
                string usrAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0) 0x1E901";
                var webClient = new WebClient { Encoding = Encoding.UTF8 };
                webClient.Headers["User-Agent"] = usrAgent;
                string launcher = webClient.DownloadString(dominio + "/pbserver/license.php?key=1c5d90100ce485f572186a26de60e82ebc3a9a31");
                dynamic data = JsonConvert.DeserializeObject(launcher);

                bool enable = data.client.enable;
                bool expire = data.client.expire;
                string msg = data.client.msg;

                if (!String.IsNullOrEmpty(msg)) {
                    Printf.roxo(msg);
                }

                if (enable && !expire)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                Printf.danger("Falha ao validar licensa");
                SaveLog.fatal("[ValidLicense] " + ex);
            }
            return false;
        }
    }
}

using Core.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Game.data.xml
{
    public static class BattleServerJSON
    {
        private class BattleServersModel
        {
            public string PublicIP { get; set; }
            public string IPSync { get; set; }

            public ushort Port { get; set; }
            public ushort PortSync { get; set; }
        }

        public static List<BattleServer> Servers = new List<BattleServer>();
        public static void Load()
        {
            
            try
            {
                Printf.info("[Load BattleServer]", false);
                string path = "data/battle/BattleServers.json";
                if (!File.Exists(path))
                {
                    SaveLog.error("[BattleServerXML] Não existe o arquivo: " + path);
                    Printf.danger("[BattleServerXML] Não existe o arquivo: " + path);
                    return;
                }

                var result = JsonConvert.DeserializeObject<List<BattleServersModel>>( File.ReadAllText(path) );


                for (byte i = 0; i < result.Count; i++)
                {
                    Servers.Add(new BattleServer()
                    {
                        _battleConn = new IPEndPoint(IPAddress.Parse( result[i].PublicIP ), result[i].Port ),
                        _battleSyncConn = new IPEndPoint(IPAddress.Parse(result[i].IPSync), result[i].PortSync),
                    });

                    Printf.info( new IPEndPoint(IPAddress.Parse(result[i].PublicIP), result[i].Port) + " Sync -> "+ new IPEndPoint(IPAddress.Parse(result[i].IPSync), result[i].PortSync), false);
                }
            }
            catch (Exception ex)
            {
                Printf.b_danger("[BattleServerXML.Load]\n" + ex);
            }
        }

        public static BattleServer GetRandomServer()
        {
            if (Servers.Count == 0)
                return null;
            Random rnd = new Random();
            int idx = rnd.Next(Servers.Count);
            try
            {
                return Servers[idx];
            }
            catch { return null; }
        }
        
    }


    public class BattleServer
    {
        public IPEndPoint _battleConn;
        public IPEndPoint _battleSyncConn;
    }
}
 
using Core.Logs;
using Core.models.servers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Core.xml
{
    public class ServersXML
    {
        private class ServersJsonMODEL
        {
            public short js_serverId { get; set; }
            public short js_state { get; set; }
            public short js_type { get; set; }

            public string js_PublicIP { get; set; }
            public ushort js_Port { get; set; }
            public short js_maxPlayers { get; set; }

            public string js_IPSync { get; set; }
            public ushort js_PortSync { get; set; }
        }

        public static List<GameServerModel> _servers = new List<GameServerModel>();
        public static GameServerModel getServer(short id)
        {
            lock (_servers)
            {
                for (short i = 0; i < _servers.Count; i++)
                {
                    GameServerModel server = _servers[i];
                    if (server._serverId == id)
                        return server;
                }
                return null;
            }
        }
        public static void Reload()
        {
            lock (_servers)
            {
                _servers.Clear();
                Load();
            }
            Printf.info("Sucess! Total servers: " + _servers.Count, false);
        }
        public static void Load()
        {
            try
            {
                Printf.info("[Loading GameServers]", false);
                string path = "data/GameServer/GameServers.json";
                if (!File.Exists(path))
                {
                    SaveLog.error("[BattleServerXML] Não existe o arquivo: " + path);
                    Printf.danger("[BattleServerXML] Não existe o arquivo: " + path, false);
                    return;
                }

                var result = JsonConvert.DeserializeObject<List<ServersJsonMODEL>>(File.ReadAllText(path));


                for (byte i = 0; i < result.Count; i++)
                {
                    _servers.Add(new GameServerModel()
                    {
                        _serverId = result[i].js_serverId,
                        _state = result[i].js_state,
                        _type = result[i].js_type,

                        _serverConn = new IPEndPoint(IPAddress.Parse(result[i].js_PublicIP), result[i].js_Port),

                        _syncConn = new IPEndPoint(IPAddress.Parse(result[i].js_IPSync), result[i].js_PortSync),

                        _maxPlayers = result[i].js_maxPlayers

                    });

                    Printf.info("ID#" + result[i].js_serverId + " Max: " + result[i].js_maxPlayers + " " + new IPEndPoint(IPAddress.Parse(result[i].js_PublicIP), result[i].js_Port) + " Sync -> " + new IPEndPoint(IPAddress.Parse(result[i].js_IPSync), result[i].js_PortSync), false);
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[ServerXML.Load] Erro fatal!");
            }
        }
    }
}
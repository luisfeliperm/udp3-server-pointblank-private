using Core.Logs;
using Core.server;
using Game.data.model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game
{
    public static class GameManager
    {
        public static Socket mainSocket;
        public static ConcurrentDictionary<uint, GameClient> _socketList = new ConcurrentDictionary<uint, GameClient>();
        public static bool Start()
        {
            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Local = new IPEndPoint(IPAddress.Parse(ConfigGS.gameIp), ConfigGS.gamePort);
                mainSocket.Bind(Local);
                mainSocket.Listen(10);
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                return true;
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[GameManager.Start] Erro fatal!");
                return false;
            }
        }
        private static void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;
            try
            {
                Socket handler = clientSocket.EndAccept(result);
                if (handler != null)
                {
                    GameClient client = new GameClient(handler);
                    AddSocket(client);
                    if (client == null)
                    {
                        SaveLog.warning("GameClient destruído após falha ao adicionar na lista.");
                    }
                        
                    Thread.Sleep(5);
                }
            }
            catch(Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[GameManager.AcceptCallback] [Failed a GC connection] Erro fatal!");
            }
            mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
        }
        public static void AddSocket(GameClient sck)
        {
            if (sck == null) return;

            for (uint i = 1; i < 100000; i++)
            {
                if (!_socketList.ContainsKey(i) && _socketList.TryAdd(i, sck))
                {
                    sck.SessionId = i;
                    sck.Start();
                    return;
                }
            }
            Printf.danger("[GameManager.AddSocket] Nao adicionou uma sessionId, conexao fechada!");
            SaveLog.error("[GameManager.AddSocket] Nao adicionou uma sessionId, conexao fechada! "+sck._client.RemoteEndPoint);
            sck.Close(500);
        }
        public static bool RemoveSocket(GameClient sck)
        {
            if (sck == null || sck.SessionId == 0)
                return false;
            if (_socketList.ContainsKey(sck.SessionId) && _socketList.TryGetValue(sck.SessionId, out sck))
            {
                return _socketList.TryRemove(sck.SessionId, out sck);
            }
                
            
            return false;
        }
        public static int SendPacketToAllClients(SendPacket packet)
        {
            int count = 0;
            if (_socketList.Count == 0)
                return count;
            
            byte[] code = packet.GetCompleteBytes();
            foreach (GameClient client in _socketList.Values)
            {
                Account player = client._player;
                if (player != null && player._isOnline)
                {
                    player.SendCompletePacket(code);
                    count++;
                }
            }
            return count;
        }
        public static Account SearchActiveClient(long accountId)
        {
            if (_socketList.Count == 0)
                return null;
            foreach (GameClient client in _socketList.Values)
            {
                Account player = client._player;
                if (player != null && player.player_id == accountId)
                    return player;
            }
            return null;
        }
        public static Account SearchActiveClient(uint sessionId)
        {
            if (_socketList.Count == 0)
                return null;
            foreach (GameClient client in _socketList.Values)
            {
                if (client._player != null && client.SessionId == sessionId)
                    return client._player;
            }
            return null;
        }
        public static int KickActiveClient(double Hours)
        {
            int count = 0;
            DateTime now = DateTime.Now;
            foreach (GameClient client in _socketList.Values)
            {
                Account pl = client._player;
                if (pl != null && pl._room == null && pl.channelId > -1 && !pl.IsGM() && (now - pl.LastLobbyEnter).TotalHours >= Hours)
                {
                    count++;
                    pl.Close(5000);
                }
            }
            return count;
        }
        public static int KickCountActiveClient(double Hours)
        {
            int count = 0;
            DateTime now = DateTime.Now;
            foreach (GameClient client in _socketList.Values)
            {
                Account pl = client._player;
                if (pl != null && pl._room == null && pl.channelId > -1 && !pl.IsGM() && (now - pl.LastLobbyEnter).TotalHours >= Hours)
                    count++;
            }
            return count;
        }
    }
}
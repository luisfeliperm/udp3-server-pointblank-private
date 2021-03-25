using System.Net;

namespace Core.models.servers
{
    public class GameServerModel
    {
        public short _state, _serverId, _type, _LastCount, _maxPlayers;
        public IPEndPoint _serverConn;
        public IPEndPoint _syncConn;
    }
}
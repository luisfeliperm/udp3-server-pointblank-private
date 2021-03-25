using Core.models.servers;
using Core.server;
using Core.xml;
using System.Net;

namespace Auth.global.serverpacket
{
    public class BASE_SERVER_LIST_PAK : SendPacket
    {
        private IPAddress _ip;
        private uint _sessionId;
        private ushort _sessionSeed;

        public BASE_SERVER_LIST_PAK(LoginClient lc)
        {
            _sessionId = lc.SessionId;
            _ip = lc.GetAddress();
            _sessionSeed = lc.SessionSeed;
        }

        public override void write()
        {
            writeH(2049);
            writeD(_sessionId);
            writeIP(_ip);
            writeH(29890); //udpPort
            writeH(_sessionSeed); // Hash|Seed
            for (int index = 0; index < 10; ++index)
                writeC(1);
            writeC(1);
            writeD(ServersXML._servers.Count);
            for (int i = 0; i < ServersXML._servers.Count; i++)
            {
                GameServerModel server = ServersXML._servers[i];
                writeD(server._state);
                writeIP(server._serverConn.Address);
                writeH((ushort)server._serverConn.Port);
                writeC((byte)server._type);
                writeH((ushort)server._maxPlayers);
                writeD(server._LastCount); // Only in server
            }
            writeH(1); //Quantidade de algo
            writeH(300);
            writeD(200);
            writeD(100);
            /*
             * writeH(0); //Type
             * writeD(0); //Exp
             * writeD(0); //Point
             * 
             * */
            writeC(1); //Events
            writeD(1); //Type
            writeD(100); //Exp
            writeD(150); //Point
        }
    }
}
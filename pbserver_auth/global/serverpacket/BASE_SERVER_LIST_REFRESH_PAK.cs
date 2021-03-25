using Core.models.servers;
using Core.server;
using Core.xml;

namespace Auth.global.serverpacket
{
    public class BASE_SERVER_LIST_REFRESH_PAK : SendPacket
    {
        public BASE_SERVER_LIST_REFRESH_PAK()
        {
        }

        public override void write()
        {
            writeH(2643);
            writeD(ServersXML._servers.Count);
            for (int i = 0; i < ServersXML._servers.Count; i++)
            {
                GameServerModel server = ServersXML._servers[i];
                writeD(server._state);
                writeIP(server._serverConn.Address);
                writeH((ushort)server._serverConn.Port);
                writeC((byte)server._type);
                writeH((ushort)server._maxPlayers);
                writeD(server._LastCount);
            }
        }
    }
}
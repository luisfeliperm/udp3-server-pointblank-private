using Core.server;

namespace Auth.global.serverpacket
{
    public class AUTH_ACCOUNT_KICK_PAK : SendPacket
    {
        private int _type;
        public AUTH_ACCOUNT_KICK_PAK(int type)
        {
            _type = type;
        }

        public override void write()
        {
            writeH(513);
            writeC((byte)_type);
        }
    }
}
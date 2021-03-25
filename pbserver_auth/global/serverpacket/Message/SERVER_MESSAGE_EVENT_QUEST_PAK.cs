using Core.server;

namespace Auth.global.serverpacket
{
    public class SERVER_MESSAGE_EVENT_QUEST_PAK : SendPacket
    {
        public SERVER_MESSAGE_EVENT_QUEST_PAK()
        {
        }

        public override void write()
        {
            writeH(2061);
            // Voce recebeu uma missao de eventos. Verifique informações no menu Missoes
        }
    }
}
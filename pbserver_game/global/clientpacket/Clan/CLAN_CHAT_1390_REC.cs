using Core.models.enums.global;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.clientpacket
{
    public class CLAN_CHAT_1390_REC : ReceiveGamePacket
    {
        private ChattingType type;
        private string text;
        public CLAN_CHAT_1390_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            type = (ChattingType)readH();
            text = readS(readH());
        }

        public override void run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || type != ChattingType.Clan_Member_Page)
                    return;
                using (CLAN_CHAT_1390_PAK packet = new CLAN_CHAT_1390_PAK(p, text))
                    ClanManager.SendPacket(packet, p.clanId, -1, true, true);
            }
            catch
            {
                //Logger.warning("[CLAN_CHAT_1390_REC]\n" + ex);
            }
        }
    }
}
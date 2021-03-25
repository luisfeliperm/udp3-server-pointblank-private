using Core.Logs;
using Core.managers;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class CLAN_PLAYER_CLEAN_INVITES_REC : ReceiveGamePacket
    {
        private uint erro;
        public CLAN_PLAYER_CLEAN_INVITES_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void run()
        {
            try
            {
                if (_client == null || !PlayerManager.DeleteInviteDb(_client.player_id))
                    erro = 2147487835;
                _client.SendPacket(new CLAN_PLAYER_CLEAN_INVITES_PAK(erro));
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[CLAN_PLAYER_CLEAN_INVITES_REC.run] Erro fatal!");
            }
        }

        public override void read()
        {
        }
    }
}
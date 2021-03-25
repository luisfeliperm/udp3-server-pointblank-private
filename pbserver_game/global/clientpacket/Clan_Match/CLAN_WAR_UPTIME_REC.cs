using Core.Logs;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class CLAN_WAR_UPTIME_REC : ReceiveGamePacket
    {
        private int formacao;
        public CLAN_WAR_UPTIME_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            formacao = readC();
        }

        public override void run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Match mt = p._match;
                if (mt != null && p.matchSlot == mt._leader)
                {
                    mt.formação = formacao;
                    using (CLAN_WAR_MATCH_UPTIME_PAK packet = new CLAN_WAR_MATCH_UPTIME_PAK(0, formacao))
                        mt.SendPacketToPlayers(packet);
                }
                else _client.SendPacket(new CLAN_WAR_MATCH_UPTIME_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[CLAN_WAR_UPTIME_REC.run] Erro fatal!");
            }
        }
    }
}
using Core;
using Core.Logs;
using Core.managers;
using Core.models.account.title;
using Core.server;
using Core.xml;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BASE_TITLE_DETACH_REC : ReceiveGamePacket
    {
        private int slotIdx;
        private uint erro;
        public BASE_TITLE_DETACH_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            slotIdx = readH();
        }

        public override void run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || slotIdx >= 3 || p._titles == null)
                    return;
                PlayerTitles t = p._titles;
                int titleId = t.GetEquip(slotIdx);
                if (titleId > 0 &&
                    TitleManager.getInstance().updateEquipedTitle(t.ownerId, slotIdx, 0))
                {
                    t.SetEquip(slotIdx, 0);
                    if (TitleAwardsXML.Contains(titleId, p._equip._beret) && ComDiv.updateDB("contas", "char_beret", 0, "player_id", p.player_id))
                    {
                        p._equip._beret = 0;
                        Room room = p._room;
                        if (room != null)
                            AllUtils.updateSlotEquips(p, room);
                    }
                }
                else erro = 0x80000000;
                _client.SendPacket(new BASE_TITLE_DETACH_PAK(erro));
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_TITLE_DETACH_REC.run] Erro fatal!");
            }
        }
    }
}
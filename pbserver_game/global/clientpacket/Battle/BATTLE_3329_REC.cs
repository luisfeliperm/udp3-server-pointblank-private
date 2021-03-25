using Core.Logs;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BATTLE_3329_REC : ReceiveGamePacket
    {
        public BATTLE_3329_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
        }

        public override void run()
        {
            try
            {
                Printf.warning("3329 Received");
                Account p = _client._player;
                if (p == null)
                    return;
                Room room = p._room;
                SaveLog.warning("BATTLE_3329_REC. (PlayerID: " + p.player_id + "; Name: " + p.player_name + "; Room: " + (p._room != null ? p._room._roomId : -1) + "; Channel: " + p.channelId + ")");
                if (room != null)
                {
                    SaveLog.warning("Room3329; BOT: " + room.isBotMode());
                    SLOT slot = room.getSlot(p._slotId);
                    if (slot != null)
                    {
                        SaveLog.warning("SLOT Id: " + slot._id + "; State: " + slot.state);
                    }
                }
                p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Voce usou uma função que estamos investigando, entre em contato com os administradores!"));
                _client.SendPacket(new A_3329_PAK());
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BATTLE_3329_REC.run] Erro fatal!");
            }
        }
    }
}
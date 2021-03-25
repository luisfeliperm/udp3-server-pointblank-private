using Core.Logs;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class A_3890_REC : ReceiveGamePacket
    {
        private int Slot;
        public A_3890_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            Slot = readC();
        }

        public override void run()
        {
            if (_client == null)
                return;
            Account p = _client._player;
            if (p == null || p._slotId == Slot || !p.IsGM()) return;
            
            try
            {
                Room room = p._room;
                if (room == null)
                    return;
                Account pR = room.getPlayerBySlot(Slot);
                if (pR == null)
                    return;
                pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                pR.Close(1000, true);
                //Ativa quando usa "/KICK (slotid)"
                SaveLog.info("[3890] "+p.player_name+" Kick By SLOT: " + Slot + " Player: "+ pR.player_name);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[A_3890_REC.run] Erro fatal!");
            }
        }
    }
}
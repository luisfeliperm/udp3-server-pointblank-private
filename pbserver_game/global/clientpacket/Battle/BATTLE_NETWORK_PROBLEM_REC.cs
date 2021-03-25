using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Extraido do servidor "PointBlank Revival 0.5 Alpha2"
 * 
 */
namespace Game.global.clientpacket
{
    class BATTLE_NETWORK_PROBLEM_REC : ReceiveGamePacket
    {
        public BATTLE_NETWORK_PROBLEM_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
        }

        public override void run()
        {
            /*
            Player player = getConnection().getPlayer();
            Room room = getConnection().getRoom();
            RoomSlot slot = room.getRoomSlotByPlayer(player);
            slot.setState(SlotState.SLOT_STATE_NORMAL);
            for (Player player1 : room.getPlayers().values())
            {
                RoomSlot slot1 = room.getRoomSlotByPlayer(player1);
                if (slot1.getState().ordinal() == 13)
                    player1.getConnection().sendPacket(new SM_BATTLE_LEAVE(slot.getId(), room));
            }
            getConnection().sendPacket(new SM_BATTLE_ERRORMESSAGE(BattleErrorMessage.EVENT_ERROR_EVENT_BATTLE_TIMEOUT_CS));
            */
        }
    }
}

using Core.Logs;
using Core.managers;
using Core.models.room;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;


namespace Game.global.api
{
    public static class API_SendCash
    {

        public static void SendByNick(string nick, int valor)
        {
            Account pR = AccountManager.getAccount(nick, 1, 0);
            BaseGiveCash(pR, valor);
        }
        public static void SendById(long pId, int valor)
        {
            Account pR = AccountManager.getAccount(pId, 0);
            BaseGiveCash(pR, valor);
        }

        private static void BaseGiveCash(Account player, int cash)
        {
            if (player == null)
            {
                Printf.danger("[API-SendCash] Falha ao adicionar cash! Player Nulo");
                return;
            }
            if (player._money + cash > 999999999)
            {
                Printf.danger("[API_SendCash] A soma ultrapassou o limite!");
                return;
            }
            if (PlayerManager.updateAccountCash(player.player_id, player._money + cash))
            {
                player._money += cash;
                player.SendPacket(new AUTH_WEB_CASH_PAK(0, player._gp, player._money), false);
                SEND_ITEM_INFO.LoadGoldCash(player);

                Printf.blue("[API.SendCash] Adicionado "+cash+" ao player: "+ player.player_name);

                if (player._isOnline)
                {
                    if(InGame(player))
                        player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Você recebeu "+cash+" de cash!"));
                    else
                        player.SendPacket(new LOBBY_CHATTING_PAK("[Sistema]", player.getSessionId(), 0, true, "Você recebeu "+cash+" de cash!"));
                }
            }
            else
            {
                Printf.danger("[API.SendCash] Falha ao adicionar cash ao player: " + player.player_name);
            }
                
        }

        private static bool InGame(Account player)
        {
            Room room = player._room;
            SLOT slot;
            if (room != null && room.getSlot(player._slotId, out slot) && (int)slot.state >= 9)
                return true;
            return false;
        }

    }
}

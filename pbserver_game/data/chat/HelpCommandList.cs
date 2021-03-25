using Core;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class HelpCommandList
    {
        public static string GetList3(Account player)
        {
            if ((int)player.access >= 3)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("Help3_Title");
                comandos += "\n" + Translation.GetLabel("FakeRank");
                comandos += "\n" + Translation.GetLabel("ChangeNick");
                comandos += "\n" + Translation.GetLabel("KickPlayer");
                comandos += "\n" + Translation.GetLabel("EnableDisableGMColor");
                comandos += "\n" + Translation.GetLabel("AntiKickActive");
                comandos += "\n" + Translation.GetLabel("RoomUnlock");
                comandos += "\n" + Translation.GetLabel("AFKCounter");
                comandos += "\n" + Translation.GetLabel("AFKKick");
                comandos += "\n" + Translation.GetLabel("PlayersCountInServer");
                comandos += "\n" + Translation.GetLabel("Ping");//falta add
                player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(comandos));
                return Translation.GetLabel("HelpLoad");
            }
            else return Translation.GetLabel("HelpDenied");
        }
        public static string GetList4(Account player)
        {
            if ((int)player.access >= 4)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("Help4_Title");
                comandos += "\n\n" + Translation.GetLabel("MsgToAllServer");
                comandos += "\n" + Translation.GetLabel("MsgToAllRoom");
                comandos += "\n" + Translation.GetLabel("ChangeMapId");
                comandos += "\n" + Translation.GetLabel("ChangeRoomTime");           
                comandos += "\n" + Translation.GetLabel("KickAll");
                comandos += "\n" + Translation.GetLabel("SendGift");
                comandos += "\n" + Translation.GetLabel("GoodsFound");
                comandos += "\n" + Translation.GetLabel("OpenRoomSlot");
                comandos += "\n" + Translation.GetLabel("OpenRandomRoomSlot");
                comandos += "\n" + Translation.GetLabel("OpenAllClosedRoomSlots");
                comandos += "\n" + Translation.GetLabel("TakeTitles");

                player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(comandos));
                return Translation.GetLabel("HelpLoad");
            }
            else return Translation.GetLabel("HelpDenied");
        }
        public static string GetList5(Account player)
        {
            if ((int)player.access >= 5)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("Help5_Title");
                comandos += "\n\n" + Translation.GetLabel("ChangeRank");
                comandos += "\n" + Translation.GetLabel("SimpleBanEtern");
                comandos += "\n" + Translation.GetLabel("AdvancedBanEtern");
                comandos += "\n" + Translation.GetLabel("UnbanEtern");
                comandos += "\n" + Translation.GetLabel("CreateItemPt1");
                comandos += "\n" + Translation.GetLabel("CreateItemPt2");
                comandos += "\n" + Translation.GetLabel("CreateGoldItem"); 
                comandos += "\n" + Translation.GetLabel("ReloadShop");
                comandos += "\n" + Translation.GetLabel("V2ReloadShop");
                comandos += "\n" + Translation.GetLabel("SetCashD");
                comandos += "\n" + Translation.GetLabel("SetGoldD");
                comandos += "\n" + Translation.GetLabel("CashPlayerD");
                comandos += "\n" + Translation.GetLabel("GoldPlayerD");
                comandos += "\n" + Translation.GetLabel("SetVip");
                comandos += "\n" + Translation.GetLabel("SetAcess");
                comandos += "\n" + Translation.GetLabel("slotGhost");

                player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(comandos));
                return Translation.GetLabel("HelpLoad");
            }
            else return Translation.GetLabel("HelpDenied");
        }
        public static string GetList6(Account player)
        {
            if ((int)player.access >= 6)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("Help6_Title");
                comandos += "\n\n" + Translation.GetLabel("Pausar");
                comandos += "\n" + Translation.GetLabel("EndRoom");
                comandos += "\n" + Translation.GetLabel("ChangeRoomType");
                comandos += "\n" + Translation.GetLabel("ChangeRoomSpecial");
                comandos += "\n" + Translation.GetLabel("ChangeRoomWeapons");
                comandos += "\n" + Translation.GetLabel("ChangeUDP");
                comandos += "\n" + Translation.GetLabel("EnableTestMode");
                comandos += "\n" + Translation.GetLabel("EnablePublicMode");
                player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(comandos));
                return Translation.GetLabel("HelpLoad");
            }
            else return Translation.GetLabel("HelpDenied");
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
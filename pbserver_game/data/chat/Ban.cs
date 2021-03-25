using Core;
using Core.models.enums;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class Ban
    {
        public static string BanForeverNick(string str, Account player, bool warn){
            Account victim = AccountManager.getAccount(str.Substring(6), 1, 0);
            return BaseBanForever(player, victim, warn);
        }
        public static string BanForeverId(string str, Account player, bool warn)
        {
            Account victim = AccountManager.getAccount(long.Parse(str.Substring(7)), 0);
            return BaseBanForever(player, victim, warn);
        }

        private static string BaseBanForever(Account player, Account victim, bool warn)
        {
            if (victim == null)
                return Translation.GetLabel("PlayerBanUserInvalid");
            else if (victim.access > player.access)
                return Translation.GetLabel("PlayerBanAccessInvalid");
            else if (player.player_id == victim.player_id)
                return Translation.GetLabel("PlayerBanSimilarID");
            else if (ComDiv.updateDB("contas", "access_level", -1, "player_id", victim.player_id))
            {
                if (warn){
                    using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(Translation.GetLabel("PlayerBannedWarning", victim.player_name)))
                        GameManager.SendPacketToAllClients(packet);
                }
                victim.access = AccessLevel.Banned;
                victim.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                victim.Close(1000, true);
                return Translation.GetLabel("PlayerBanSuccess");
            }
            else
                return Translation.GetLabel("PlayerBanFail");
        }

    }
}
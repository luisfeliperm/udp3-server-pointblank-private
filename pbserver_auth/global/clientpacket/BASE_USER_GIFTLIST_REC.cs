using Auth.data.model;
using Auth.global.serverpacket;
using Core.Logs;
using Core.managers;
using Core.managers.server;
using Core.models.account;
using System;
using System.Collections.Generic;

namespace Auth.global.clientpacket
{
    public class BASE_USER_GIFTLIST_REC : ReceiveLoginPacket
    {
        public BASE_USER_GIFTLIST_REC(LoginClient lc, byte[] buff)
        {
            makeme(lc, buff);
        }

        public override void read()
        {
        }

        public override void run()
        {
            try
            {
                Account player = _client._player;
                if (player == null || !ServerConfig.GiftSystem)
                    return;
                List<Message> gifts = MessageManager.getGifts(player.player_id);
                if (gifts.Count > 0)
                {
                    MessageManager.RecicleMessages(player.player_id, gifts);
                    if (gifts.Count > 0)
                        _client.SendPacket(new BASE_USER_GIFT_LIST_PAK(0, gifts));
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_GIFTLIST_REC.run] Erro fatal!");
            }
        }
    }
}
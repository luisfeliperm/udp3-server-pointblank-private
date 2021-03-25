using Auth.global.serverpacket;
using Core;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    public class BASE_USER_EXIT_REC : ReceiveLoginPacket
    {
        public BASE_USER_EXIT_REC(LoginClient lc, byte[] buff)
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
                _client.SendPacket(new BASE_USER_EXIT_PAK());
                Printf.info(_client._player.player_name + " saiu do jogo");
                _client.Close(true, 5000);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_EXIT_REC.run] Erro fatal!");
            }
        }
    }
}
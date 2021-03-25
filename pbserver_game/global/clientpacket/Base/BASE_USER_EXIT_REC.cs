using Core.Logs;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BASE_USER_EXIT_REC : ReceiveGamePacket
    {
        public BASE_USER_EXIT_REC(GameClient client, byte[] data)
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
                _client.SendPacket(new BASE_USER_EXIT_PAK());
                Printf.info(_client._player.player_name + " saiu do jogo");
                _client.Close(1000);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_EXIT_REC.run] Erro fatal!");
                _client.Close(0);
            }
        }
    }
}
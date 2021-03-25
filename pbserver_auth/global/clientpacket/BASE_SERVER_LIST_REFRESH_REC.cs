using Auth.global.serverpacket;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    public class BASE_SERVER_LIST_REFRESH_REC : ReceiveLoginPacket
    {
        public BASE_SERVER_LIST_REFRESH_REC(LoginClient client, byte[] data)
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
                if (_client != null)
                    _client.SendPacket(new BASE_SERVER_LIST_REFRESH_PAK());
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_SERVER_LIST_REFRESH_REC.run] Erro fatal!");
            }
        }
    }
}
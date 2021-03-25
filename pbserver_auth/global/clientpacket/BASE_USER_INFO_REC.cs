using Auth.global.serverpacket;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    public class BASE_USER_INFO_REC : ReceiveLoginPacket
    {
        public BASE_USER_INFO_REC(LoginClient client, byte[] data)
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
                if(_client != null)
                {
                    _client.SendPacket(new BASE_USER_INFO_PAK(_client._player));
                    //_client.SendPacket(new SERVER_MESSAGE_EVENT_QUEST_PAK());
                }

            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_INFO_REC.run] Erro fatal!");
            }
        }
    }
}
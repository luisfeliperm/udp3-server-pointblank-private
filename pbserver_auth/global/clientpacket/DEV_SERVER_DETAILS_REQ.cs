using Auth.global.serverpacket;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    class DEV_SERVER_DETAILS_REQ : ReceiveLoginPacket
    {
        public DEV_SERVER_DETAILS_REQ(LoginClient lc, byte[] buff)
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
                _client.SendPacket(new DEV_SERVER_DETAILS_ACK());
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[DEV_SERVER_DETAILS_REQ.run] Erro fatal!");
            }
        }
    }
}

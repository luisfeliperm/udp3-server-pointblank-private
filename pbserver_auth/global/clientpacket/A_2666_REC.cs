using Auth.global.serverpacket;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    public class A_2666_REC : ReceiveLoginPacket
    {
        public A_2666_REC(LoginClient lc, byte[] buff)
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
                _client.SendPacket(new BASE_RANK_AWARDS_PAK());
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[A_2666_REC.run] Erro fatal!");
            }
        }
    }
}
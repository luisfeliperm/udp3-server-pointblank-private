using Core.Logs;
using Game.global.serverpacket;
using System;

namespace Auth.global.clientpacket
{
    public class BASE_USER_ENTER_REC : ReceiveLoginPacket
    {
        private string login;
        private byte[] _IPLocal;
        private long pId;
        public BASE_USER_ENTER_REC(LoginClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            login = readS(readC());
            pId = readQ();
            readC();
            _IPLocal = readB(4);
        }

        public override void run()
        {
            if (_client == null)
                return;
            try
            {
                Printf.warning("2579 received - " + _client.GetIPAddress());
                SaveLog.warning("2579 received - Login:" + login + "; pId:" + pId + "; LocalIp:" + _IPLocal[0] + "." + _IPLocal[1] + "." + _IPLocal[2] + "." + _IPLocal[3] + " - " + _client.GetIPAddress(true));
                _client.SendPacket(new BASE_USER_ENTER_PAK(0x80000000));
            }
            catch(Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_ENTER_REC.run] Erro fatal!");
                _client.Close(true);
            }
        }
    }
}
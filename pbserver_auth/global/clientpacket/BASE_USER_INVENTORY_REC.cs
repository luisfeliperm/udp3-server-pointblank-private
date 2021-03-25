using Auth.data.model;
using Auth.global.serverpacket;
using Core.Logs;
using System;

namespace Auth.global.clientpacket
{
    public class BASE_USER_INVENTORY_REC : ReceiveLoginPacket
    {
        public BASE_USER_INVENTORY_REC(LoginClient client, byte[] data)
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
                Account p = _client._player;
                if (p == null)
                    return;
                _client.SendPacket(new BASE_USER_INVENTORY_PAK(p._inventory._items));
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_INVENTORY_REC.run] Erro fatal!");
            }
        }
    }
}
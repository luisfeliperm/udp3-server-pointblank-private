using Core.Logs;
using Core.server;
using System;
using System.Collections.Generic;

namespace Game.global.clientpacket
{
    public class BOX_MESSAGE_VIEW_REC : ReceiveGamePacket
    {
        private int msgsCount;
        private List<int> messages = new List<int>();
        public BOX_MESSAGE_VIEW_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            msgsCount = readC();
            for (int i = 0; i < msgsCount; i++)
                messages.Add(readD());
        }

        public override void run()
        {
            try
            {
                if (_client == null || _client._player == null || msgsCount == 0)
                    return;
                ComDiv.updateDB("player_messages", "id", messages.ToArray(),
                    "owner_id", _client.player_id, new string[] { "expire", "state" },
                    long.Parse(DateTime.Now.AddDays(7).ToString("yyMMddHHmm")), 0);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BOX_MESSAGE_VIEW_REC.run] Erro fatal!");
            }
        }
    }
}
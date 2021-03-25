using Core.server;
using Game.global.api;

namespace Game.data.sync.client_side.API
{
    public class ReadAPI_Cash
    {
        public static void Load(ReceiveGPacket buffer)
        {
            long pId = buffer.readQ();
            int cash = buffer.readD();
            API_SendCash.SendById(pId,cash);
        }
    }
}

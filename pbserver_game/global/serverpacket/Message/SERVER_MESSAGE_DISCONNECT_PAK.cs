using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_DISCONNECT_PAK : SendPacket
    {
        public SERVER_MESSAGE_DISCONNECT_PAK()
        {
        }
        public override void write()
        {
            // Vc foi desconectado do jogo por usar programas ilegais 
            writeH(2062);
            writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            writeD(1); // Testado valor 0,1,10 e o resultado foi igual
            writeD(true); //Se for igual a 1, novo writeD (Da DC no cliente, Programa ilegal)
            writeD(0); // Se o de cima for TRUE
        }
    }
}
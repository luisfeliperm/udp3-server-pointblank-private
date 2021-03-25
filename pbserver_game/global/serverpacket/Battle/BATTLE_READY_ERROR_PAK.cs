using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_READY_ERROR_PAK : SendPacket
    {
        private uint _erro;
        public BATTLE_READY_ERROR_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void write()
        {
            writeH(3332);
            //Se for tutorial, não lê o resto.
            writeD(_erro);
        }
    }
}
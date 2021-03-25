using Core.server;

namespace Game.global.serverpacket
{
    public class INVENTORY_ITEM_EXCLUDE_PAK : SendPacket
    {
        private long _objId;
        private uint _erro;
        public INVENTORY_ITEM_EXCLUDE_PAK(uint erro, long objId = 0)
        {
            _erro = erro;
            if (erro == 1)
                _objId = objId;
        }
        public override void write()
        {
            writeH(543);
            writeD(_erro);
            if (_erro == 1)
                writeQ(_objId);
        }
    }
}
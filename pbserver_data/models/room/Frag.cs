using Core.models.enums.room;

namespace Core.models.room
{
    public class Frag
    {
        public byte victimWeaponClass, hitspotInfo, flag;
        public KillingMessage killFlag;
        public float x, y, z;
        public int VictimSlot;
        public Frag()
        {

        }
        public Frag(byte hitspotInfo)
        {
            SetHitspotInfo(hitspotInfo);
        }
        public void SetHitspotInfo(byte value)
        {
            hitspotInfo = value;
            VictimSlot = (value & 15);
        }
    }
}
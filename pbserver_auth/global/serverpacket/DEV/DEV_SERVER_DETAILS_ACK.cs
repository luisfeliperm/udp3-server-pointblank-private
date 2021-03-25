using Core.server;

namespace Auth.global.serverpacket
{
    class DEV_SERVER_DETAILS_ACK : SendPacket
    {
        string devName, srvDescription;

        public DEV_SERVER_DETAILS_ACK()
        {
            devName = "luisfelperm";
            srvDescription = "Release 2019 !! Acess: github.com/luisfeliperm";
        }
        public override void write()
        {
            writeH(0x1);

            // Version (4.0.0) 
            writeH(4); 
            writeH(0);
            writeH(0);
            writeH(0);


            writeC((byte)devName.Length); // Dev Nome - Lenght
            writeS(devName, devName.Length); // d

            writeH((short)srvDescription.Length);
            writeS(srvDescription, srvDescription.Length);
        }

    }
}

/*
using Core.Logs;
using Core.models.enums.global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace Auth.global.clientpacket
{
    class BASE_LOGIN_THAI_REC : ReceiveLoginPacket
    {
        private string all, token, password, UserFileListMD5, d3d9MD5, GameVersion, PublicIP, LocalIP, pass_str;
        private int tokenSize, connection, IsRealIP;
        private ulong key;
        private ClientLocale GameLocale;
        private PhysicalAddress MacAddress;



        public BASE_LOGIN_THAI_REC(LoginClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            all = readA();
            GameVersion = ((int)readC()).ToString() + "." + readH() + "." + readH();
            tokenSize = readH();
            token = readS(tokenSize);
            readB(8);
            IsRealIP = readC();
            LocalIP = (readC() + "." + readC() + "." + readC() + "." + readC());
            key = readUQ();
            UserFileListMD5 = readS(32);
            readD();
            d3d9MD5 = readS(32);
            int num = (int)readC();
            PublicIP = _client.GetIPAddress();


        }
        public override void run()
        {
            Printf.warning(
                "\nGame Version:"+ GameVersion +
                "\nTokenSize" + tokenSize + " = "+ token +
                "\nMac: " + MacAddress.ToString() +
                "\nIsRealIP: " + IsRealIP +
                "\nLocalIP: " + LocalIP+
                "\nkey: " + key +
                "\nUserFile: " + UserFileListMD5 +
                "\ndx: " + d3d9MD5
            );
        }
    }
}
*/
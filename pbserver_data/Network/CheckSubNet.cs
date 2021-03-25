using Core.Logs;
using System;
using System.Net;

namespace Core.Network
{
    public static class CheckSubNet
    {
        public static bool checkInSubNet(this IPAddress _host, IPAddress _addr, byte cidr)
        {
            return _host.checkInSubNet(_addr, CidrToMask(cidr));
        }
        public static bool checkInSubNet(this IPAddress _host, IPAddress _addr, IPAddress _mask)
        {
            try
            {
                IPAddress networkMASK, networkHOST;
                byte[] array_NetworkMASK = new byte[4], array_NetworkHOST = new byte[4];

                byte[] host = _host.GetAddressBytes();
                byte[] addr = _addr.GetAddressBytes();
                byte[] mask = _mask.GetAddressBytes();


                // Faz And Logico do IP da rede e da MASCARA
                for (byte i = 0; i < 4; i++)
                {
                    array_NetworkMASK[i] = (byte)(addr[i] & mask[i]);
                }


                // Faz And Logico do IP do HOST e da MASCARA
                for (byte i = 0; i < 4; i++)
                {
                    array_NetworkHOST[i] = (byte)(host[i] & mask[i]);
                }

                // Define os endereços de rede
                networkMASK = new IPAddress(array_NetworkMASK);
                networkHOST = new IPAddress(array_NetworkHOST);


                // Compara os dos AND
                if (networkMASK.Equals(networkHOST))
                    return true;
            }
            catch(Exception e)
            {
                Printf.b_danger("[Core.CheckSubNet.checkInSubNet] \n\r" + e);
                SaveLog.fatal("[Core.CheckSubNet.checkInSubNet] \n\r" + e);
            }
            return false;
        }

        // Converte CIDR para mascara de rede
        private static IPAddress CidrToMask(byte mask = 24)
        {
            try
            {
                uint ipDec = uint.MaxValue << (32 - mask);

                byte[] netmask = BitConverter.GetBytes(ipDec);

                Array.Reverse(netmask);

                return new IPAddress(netmask);
            }
            catch(Exception ex)
            {
                Printf.b_danger("[Core.CheckSubNet.CidrToMask] \n\r" + ex);
                SaveLog.fatal("[Core.CheckSubNet.CidrToMask] \n\r" + ex);
                return null;
            }
            
        }
    }
}

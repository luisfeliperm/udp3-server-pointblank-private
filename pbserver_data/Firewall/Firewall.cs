using Core.Logs;
using Core.server;
using System;
using System.Net.Sockets;
using System.Text;

namespace Core.Firewall
{
    public class Firewall
    {

        public static void sendBlock(string ip, string descricao, int gravidade)
        {

            // codigo de operação - 1 byte
            // ipSize - 1 byte
            // descSize - 1 byte
            // ip
            // descricao
            // Gravivade - 1 Byte (0=grave|1=perigo|2=Suspeito)


            UdpClient udpClient = new UdpClient("127.0.0.1", 1911);

            SendGPacket pk = new SendGPacket();
            pk.writeC(0);
            pk.writeC((byte)ip.Length);
            pk.writeC((byte)descricao.Length);
            pk.writeB(Encoding.ASCII.GetBytes(ip));
            pk.writeB(Encoding.ASCII.GetBytes(descricao));
            pk.writeC((byte)gravidade);

            try
            {
                udpClient.Send(pk.mstream.ToArray(), pk.mstream.ToArray().Length);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Firewall] Fatal Error!");
            }
        }



        public static void sendAllow(string ip)
        {

            // codigo de operação - 1 byte
            // ipSize - 1 byte
            // ip


            UdpClient udpClient = new UdpClient("127.0.0.1", 1911);

            SendGPacket pk = new SendGPacket();
            pk.writeC(23);
            pk.writeC((byte)ip.Length);
            pk.writeB(Encoding.ASCII.GetBytes(ip));

            try
            {
                udpClient.Send(pk.mstream.ToArray(), pk.mstream.ToArray().Length);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Firewall] Fatal Error!");
            }
        }
    }
}

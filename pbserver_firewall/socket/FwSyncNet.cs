using Core.Logs;
using Core.server;
using pbserver_firewall.Rules;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace pbserver_firewall.socket
{
    public class FwSyncNet
    {
        public static UdpClient udp;
        public static void Start()
        {
            try
            {
                udp = new UdpClient(1911);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(read).Start();
            }
            catch (Exception ex)
            {
                Printf.b_danger("[FwSyncNet.Start] "+ex.ToString());
            }
        }
        public static void read()
        {
            try
            {
                udp.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception ex)
            {
                Printf.b_danger("[FwSyncNet.read] "+ex.ToString());
            }
        }
        private static void recv(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = udp.EndReceive(res, ref RemoteIpEndPoint);

            new Thread(read).Start();
            if (received.Length >= 1)
            LoadPacket(received);

        }
        private static void LoadPacket(byte[] buffer)
        {
            ReceiveGPacket p = new ReceiveGPacket(buffer);
            short opcode = p.readC();

            try
            {
                switch (opcode)
                {
                    case 0: // BLOCK Geral
                        new Add_Drop_Rule(p);
                        break;
                    case 20: // Allow TCP Auth
                        
                        break;
                    case 21: // Allow TCP Game
                        
                        break;
                    case 22: // Allow UDP Battle

                        break;
                    case 23: // Allow UDP Battle, TCP Game
                        new Add_Allow_Rule(p);
                        break;


                    default:
                        Printf.warning("[x] opcode not found! " + opcode);
                        break;
                }
            }
            catch (Exception ex)
            {
                Printf.b_danger("[FwSyncNet.LoadPacket] " + ex.ToString());
            }


        }


        public static void SendPacket(byte[] data, IPEndPoint ip)
        {
            udp.Send(data, data.Length, ip);
        }
    }
}

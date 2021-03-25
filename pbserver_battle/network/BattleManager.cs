using Battle.config;
using Core.Firewall;
using Core.Logs;
using System;
using System.Net;
using System.Net.Sockets;

namespace Battle.network
{
    public class BattleManager
    {
        private static UdpClient udpClient;
        public static void init()
        {
            try
            {
                udpClient = new UdpClient();
                //udpClient.Ttl = 255;
                //udpClient.ExclusiveAddressUse = false;
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(Config.hosIp), Config.hosPort);
                var s = new UdpState(localEP, udpClient);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(localEP);
                udpClient.BeginReceive(gerenciaRetorno, s);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BattleManage.init] Erro fatal!\n Ocorreu um erro ao listar as conexões UDP!!");
            }
        }
        private static void read(UdpState state)
        {
            try
            {
                udpClient.BeginReceive(new AsyncCallback(gerenciaRetorno), state);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BattleManage.read] Erro fatal!");
            }
        }
        private static void gerenciaRetorno(IAsyncResult ar)
        {
            if (!ar.IsCompleted)
                SaveLog.warning("ar is not completed.");
            ar.AsyncWaitHandle.WaitOne(5000);
            DateTime now = DateTime.Now;
            IPEndPoint recEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient c = (UdpClient)((UdpState)ar.AsyncState).c;
            IPEndPoint e = (IPEndPoint)((UdpState)ar.AsyncState).e;
            try
            {
                byte[] buffer = c.EndReceive(ar, ref recEP);

                // Tamanho minimo do pacote 22 bytes
                if (buffer.Length >= 22)
                {
                    new BattleHandler(udpClient, buffer, recEP, now);
                }
                else
                {

                    string msg = "Packet length < 22. [" + buffer.Length + "] " + recEP.Address.ToString() + ":" + recEP.Port.ToString();
                    Printf.warning(msg);
                    Firewall.sendBlock(recEP.Address.ToString(), msg, 0);
                }
                    
            }
            catch (Exception ex)
            {
                SaveLog.fatal("[Exception]: " + recEP.Address + ":" + recEP.Port + ex.ToString());
                Printf.b_danger("[BattleManage.gerenciaRetorno] Erro fatal!");
            }
            UdpState s = new UdpState(e, c);
            read(s);
        }
        public static void Send(byte[] data, IPEndPoint ip)
        {
            udpClient.Send(data, data.Length, ip);
        }
        private class UdpState : Object
        {
            public UdpState(IPEndPoint e, UdpClient c) { this.e = e; this.c = c; }
            public IPEndPoint e;
            public UdpClient c;
        }
    }
}
using Auth.data.managers;
using Auth.data.model;
using Auth.data.sync.client_side;
using Auth.global.serverpacket;
using Core.Logs;
using Core.models.account;
using Core.models.enums.friends;
using Core.models.servers;
using Core.server;
using Core.xml;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Auth.data.sync
{
    public class Auth_SyncNet
    {
        public static UdpClient udp;
        public static void Start()
        {
            try
            {
                udp = new UdpClient(ConfigGA.syncPort);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(read).Start();
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Auth_SyncNet.Start] Erro fatal!");
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
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Auth_SyncNet.read] Erro fatal!");
            }
        }
        private static void recv(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = udp.EndReceive(res, ref RemoteIpEndPoint);
            Thread.Sleep(5);
            new Thread(read).Start();

            if (received.Length >= 2)
                LoadPacket(received);
            else
            {
                Printf.warning("[AuthSync] Packet lenght < 2");
                SaveLog.warning("[AuthSync] packet < 2");
            }
        }
        private static void LoadPacket(byte[] buffer)
        {
            ReceiveGPacket p = new ReceiveGPacket(buffer);
            short opcode = p.readH();
            try
            {
                switch (opcode)
                {
                    case 11: //Request to sync a specific friend or clan info
                        int type = p.readC();
                        int isConnect = p.readC();
                        long pid = p.readQ();
                        Account player = AccountManager.getInstance().getAccount(pid, true);
                        if (player != null)
                        {
                            Account friend = AccountManager.getInstance().getAccount(p.readQ(), true);
                            if (friend != null)
                            {
                                FriendState state = isConnect == 1 ? FriendState.Online : FriendState.Offline;
                                if (type == 0)
                                {
                                    int idx = -1;
                                    Friend frP = friend.FriendSystem.GetFriend(player.player_id, out idx);
                                    if (idx != -1 && frP != null)
                                        friend.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Update, frP, state, idx));
                                }
                                else friend.SendPacket(new CLAN_MEMBER_INFO_CHANGE_PAK(player, state));
                            }
                        }
                        break;
                    case 13:
                        long playerId = p.readQ();
                        byte typee = p.readC();
                        byte[] data = p.readB(p.readUH());
                        Account playerr = AccountManager.getInstance().getAccount(playerId, true);
                        if (playerr != null)
                        {
                            if (typee == 0)
                                playerr.SendPacket(data);
                            else
                                playerr.SendCompletePacket(data);
                        }
                        break;

                    case 15:
                        short serverId = p.readH();
                        short count = p.readH();
                        GameServerModel gs = ServersXML.getServer(serverId);
                        if (gs != null)
                            gs._LastCount = count;
                        break;

                    case 16:
                        Net_Clan_Sync.Load(p);
                        break;
                    case 17:
                        Net_Friend_Sync.Load(p);
                        break;
                    case 19:
                        Net_Player_Sync.Load(p);
                        break;
                    default:
                        Printf.warning("[Auth_SyncNet] Tipo de conexão não encontrada: " + opcode);
                        SaveLog.warning("[Auth_SyncNet] Tipo de conexão não encontrada: " + opcode);
                        break;
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal("[Crash/Auth_SyncNet] opcode: " + opcode + "\r\n" + ex.ToString());
                Printf.b_danger("[Auth_SyncNet.LoadPacket] Erro fatal!");
            }
            
            
        }



        public static void UpdateAuthCount(short serverId)
        {
            try
            {
                /*
                 * 21/11/2019 Removido pois é inútil e só atrapalha a contagem
                double pingMS = (DateTime.Now - LastSyncCount).TotalSeconds;
                if (pingMS < 2.5)
                    return;

                LastSyncCount = DateTime.Now;
                */
                short players = (short)LoginManager._socketList.Count;

                foreach (GameServerModel gs in ServersXML._servers)
                {
                    if (gs._serverId == serverId)
                    {
                        gs._LastCount = players;
                    }
                    else
                    {
                        using (SendGPacket pk = new SendGPacket())
                        {
                            pk.writeH(15);
                            pk.writeH(serverId);
                            pk.writeH(players);
                            SendPacket(pk.mstream.ToArray(), gs._syncConn);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Auth_SyncNet.UpdateAuthCount] Erro fatal!");
            }
        }



        // Envia para o gameSync
        public static void SendLoginKickInfo(Account player)
        {
            short serverId = player._status.serverId;
            if (serverId != 255 && serverId != 0)
            {
                GameServerModel gs = ServersXML.getServer(serverId);
                if (gs == null)
                    return;

                using (SendGPacket pk = new SendGPacket())
                {
                    pk.writeH(10);
                    pk.writeQ(player.player_id);
                    SendPacket(pk.mstream.ToArray(), gs._syncConn);
                }
            }
            else
                player.setOnlineStatus(false);
        }
        public static void SendPacket(byte[] data, IPEndPoint ip)
        {
            udp.Send(data, data.Length, ip);
        }
    }
}
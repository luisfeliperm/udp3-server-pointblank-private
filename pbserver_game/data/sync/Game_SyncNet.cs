using Core.Logs;
using Core.models.account;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.friends;
using Core.models.enums.missions;
using Core.models.room;
using Core.models.servers;
using Core.server;
using Core.xml;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.client_side;
using Game.data.sync.client_side.API;
using Game.data.utils;
using Game.data.xml;
using Game.global.serverpacket;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game.data.sync
{
    public static class Game_SyncNet
    {
        public static UdpClient udp;
        public static void Start()
        {
            try
            {
                udp = new UdpClient(ConfigGS.syncPort);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(read).Start();
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[GameSyncNet.Start] Erro fatal!");
            }
        }
        public static void read()
        {
            try
            {
                udp.BeginReceive(new AsyncCallback(recv), null);
            }
            catch { }
        }
        private static void recv(IAsyncResult res)
        {

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = udp.EndReceive(res, ref RemoteIpEndPoint);
            Thread.Sleep(5);
            new Thread(read).Start();
            if (received.Length >= 2)
                LoadPacket(received);
        }
        private static void LoadPacket(byte[] buffer)
        {
            ReceiveGPacket p = new ReceiveGPacket(buffer);
            short opcode = p.readH();
            //Printf.warnDark("Receive SYNC - " + opcode.ToString());
            try
            {
                switch (opcode){
                    case 1:
                        Net_Room_Pass_Portal.Load(p);
                        break;
                    case 2: // Bomba
                        Net_Room_C4.Load(p);
                        break;
                    case 3: //Death
                        Net_Room_Death.Load(p);
                        break;
                    case 4:
                        Net_Room_HitMarker.Load(p);
                        break;
                    case 5:
                        Net_Room_Sabotage_Sync.Load(p);
                        break;
                    case 10:
                        Account player = AccountManager.getAccount(p.readQ(), true);
                        if (player != null)
                        {
                            player.SendPacket(new AUTH_ACCOUNT_KICK_PAK(1));
                            player.SendPacket(new SERVER_MESSAGE_ERROR_PAK(0x80001000));
                            player.Close(1000);
                        }
                        break;
                    case 11:  //Request to sync a specific friend or clan info
                        int type = p.readC();
                        int isConnect = p.readC();
                        Account player11 = AccountManager.getAccount(p.readQ(), 0);
                        if (player11 != null)
                        {
                            Account friendAcc = AccountManager.getAccount(p.readQ(), true);
                            if (friendAcc != null)
                            {
                                FriendState state = isConnect == 1 ? FriendState.Online : FriendState.Offline;
                                if (type == 0)
                                {
                                    int idx = -1;
                                    Friend friend = friendAcc.FriendSystem.GetFriend(player11.player_id, out idx);
                                    if (idx != -1 && friend != null && friend.state == 0)
                                        friendAcc.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Update, friend, state, idx));
                                }
                                else friendAcc.SendPacket(new CLAN_MEMBER_INFO_CHANGE_PAK(player11, state));
                            }
                        }
                        break;
                    case 13:
                        long playerId = p.readQ();
                        byte type13 = p.readC();
                        byte[] data = p.readB(p.readUH());
                        Account player13 = AccountManager.getAccount(playerId, true);
                        if (player13 != null)
                        {
                            if (type13 == 0)
                                player13.SendPacket(data);
                            else
                                player13.SendCompletePacket(data);
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
                    case 18:
                        Net_Inventory_Sync.Load(p);
                        break;
                    case 19:
                        Net_Player_Sync.Load(p);
                        break;
                    case 21:
                        Net_Clan_Servers_Sync.Load(p);
                        break;
                    case 100:
                        ReadAPI_Cash.Load(p);
                        break;
                    default:
                        Printf.warning("[Game_SyncNet] Tipo de conexão não encontrada: " + opcode);
                        break;
                }

            }
            catch (Exception ex)
            {
                SaveLog.fatal("[Game_SyncNet.LoadPacket] Tipo: " + opcode + "\r\n" + ex.ToString());
                Printf.b_danger("[GameSyncNet.LoadPacket] Erro fatal!");
            }
        }
        /// <summary>
        /// Envia as informações com pedido de sincronia para o UDP.
        /// Envia para battleSync
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="effects"></param>
        /// <param name="room"></param>
        public static void SendUDPPlayerSync(Room room, SLOT slot, CupomEffects effects, int type)
        {
            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(1);
                pk.writeD(room.UniqueRoomId);
                pk.writeD((room.mapId * 16) + room.room_type);
                pk.writeQ(room.StartTick); //tick
                pk.writeC((byte)type);
                pk.writeC((byte)room.rodada);
                pk.writeC((byte)slot._id);
                pk.writeC((byte)slot.spawnsCount);
                pk.writeC(BitConverter.GetBytes(slot._playerId)[0]);
                if (type == 0 || type == 2)
                    WriteCharaInfo(pk, room, slot, effects);
                SendPacket(pk.mstream.ToArray(), room.UDPServer._battleSyncConn);
            }
        }
        private static void WriteCharaInfo(SendGPacket pk, Room room, SLOT slot, CupomEffects effects)
        {
            int charaId = 0;
            if ((RoomType)room.room_type == RoomType.Boss || (RoomType)room.room_type == RoomType.Cross_Counter)
            {
                if (room.rodada == 1 && slot._team == 1 ||
                    room.rodada == 2 && slot._team == 0)
                    charaId = room.rodada == 2 ? slot._equip._red : slot._equip._blue;
                else if (room.TRex == slot._id)
                    charaId = -1;
                else
                    charaId = slot._equip._dino;
            }
            else charaId = slot._team == 0 ? slot._equip._red : slot._equip._blue;
            int HPBonus = 0;
            if (effects.HasFlag(CupomEffects.Ketupat))
                HPBonus += 10;
            if (effects.HasFlag(CupomEffects.HP5))
                HPBonus += 5;
            if (effects.HasFlag(CupomEffects.HP10))
                HPBonus += 10;
            if (charaId == -1)
            {
                pk.writeC(255);
                pk.writeH(65535);
            }
            else
            {
                pk.writeC((byte)ComDiv.getIdStatics(charaId, 2));
                pk.writeH((short)ComDiv.getIdStatics(charaId, 4));
            }
            pk.writeC((byte)HPBonus);
            pk.writeC(effects.HasFlag(CupomEffects.C4SpeedKit));
        }
        // Send To battle
        public static void SendUDPRoundSync(Room room)
        {
            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(3);
                pk.writeD(room.UniqueRoomId);
                pk.writeD((room.mapId * 16) + room.room_type);
                pk.writeC((byte)room.rodada);
                SendPacket(pk.mstream.ToArray(), room.UDPServer._battleSyncConn);
            }
        }
        public static GameServerModel GetServer(AccountStatus status)
        {
            return GetServer(status.serverId);
        }
        public static GameServerModel GetServer(short serverId)
        {
            if (serverId == 255 || serverId == ConfigGS.serverId)
                return null;
            return ServersXML.getServer(serverId);
        }

        // Envia pros outros servers sempre que alguem entra ou sai
        // Objetivo é manter atualizada o numero de players online
        public static void UpdateGSCount(short serverId)
        {
            try
            {
                /*
                 * 21/11/2019 Removido pois é inútil e só atrapalha a contagem
                 * 
                double pingMS = (DateTime.Now - LastSyncCount).TotalSeconds;
                if (pingMS < 10)
                    return;

                LastSyncCount = DateTime.Now;
                */

                int players = 0;
                foreach (Channel ch in ChannelsXML._channels)
                    players += ch._players.Count;
                foreach (GameServerModel gs in ServersXML._servers)
                {
                    if (gs._serverId == serverId)
                    {
                        gs._LastCount = (short)players;
                        // Printf.white("Atualizando-se; ServerID: " + serverId + " Online: " + players);
                    }
                    else
                    {
                        //Printf.white("[SendCount] Send to Server: " + gs._serverId + " Addr: " + gs._syncConn + "[Id: " + serverId + " " + gs._LastCount + "/" + gs._maxPlayers + "]");

                        using (SendGPacket pk = new SendGPacket())
                        {
                            pk.writeH(15);
                            pk.writeH(serverId);
                            pk.writeH((short)players);
                            SendPacket(pk.mstream.ToArray(), gs._syncConn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[GameSyncNet.UpdateGSCount] Erro fatal!");
            }
        }
        // Send to GameSync
        public static void SendBytes(long playerId, SendPacket sp, short serverId)
        {
            if (sp == null)
                return;
            GameServerModel gs = GetServer(serverId);
            if (gs == null)
                return;

            byte[] data = sp.GetBytes();
            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(13);
                pk.writeQ(playerId);
                pk.writeC(0);
                pk.writeH((ushort)data.Length);
                pk.writeB(data);
                SendPacket(pk.mstream.ToArray(), gs._syncConn);
            }
        }
        public static void SendBytes(long playerId, byte[] buffer, short serverId)
        {
            if (buffer.Length == 0)
                return;
            GameServerModel gs = GetServer(serverId);
            if (gs == null)
                return;

            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(13);
                pk.writeQ(playerId);
                pk.writeC(0);
                pk.writeH((ushort)buffer.Length);
                pk.writeB(buffer);
                SendPacket(pk.mstream.ToArray(), gs._syncConn);
            }
        }
        public static void SendCompleteBytes(long playerId, byte[] buffer, short serverId)
        {
            if (buffer.Length == 0)
                return;
            GameServerModel gs = GetServer(serverId);
            if (gs == null)
                return;

            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(13);
                pk.writeQ(playerId);
                pk.writeC(1);
                pk.writeH((ushort)buffer.Length);
                pk.writeB(buffer);
                SendPacket(pk.mstream.ToArray(), gs._syncConn);
            }
        }
        public static void SendPacket(byte[] data, IPEndPoint ip)
        {
            udp.Send(data, data.Length, ip);
        }
        public static void genDeath(Room room, SLOT killer, FragInfos kills, bool isSuicide)
        {
            int score;
            bool isBotMode = room.isBotMode();
            Net_Room_Death.RegistryFragInfos(room, killer, out score, isBotMode, isSuicide, kills);
            if (isBotMode)
            {
                killer.Score += killer.killsOnLife + room.IngameAiLevel + score;
                if (killer.Score > 65535) // 65535
                {
                    killer.Score = 65535;
                    SaveLog.LogAbuse("{2} [PlayerId: " + killer._id + "] chegou a pontuação máxima do modo BOT.");
                }
                kills.Score = killer.Score;
            }
            else
            {
                killer.Score += score;
                AllUtils.CompleteMission(room, killer, kills, MISSION_TYPE.NA, 0);
                kills.Score = score;
            }
            using (BATTLE_DEATH_PAK packet = new BATTLE_DEATH_PAK(room, kills, killer, isBotMode))
                room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
            Net_Room_Death.EndBattleByDeath(room, killer, isBotMode, isSuicide);
        }
    }
}
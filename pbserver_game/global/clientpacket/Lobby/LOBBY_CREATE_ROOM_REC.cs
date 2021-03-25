using Core.Firewall;
using Core.Logs;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class LOBBY_CREATE_ROOM_REC : ReceiveGamePacket
    {
        private uint erro;
        private Room room;
        private Account p;
        public LOBBY_CREATE_ROOM_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            p = _client._player;
            Channel channel = p == null ? null : p.getChannel();
            try
            {
                if (p == null || channel == null || p.player_name.Length == 0 || p._room != null || p._match != null)
                {
                    erro = 0x80000000;
                    return;
                }
                lock (channel._rooms)
                    for (int i = 0; i < 300; i++)
                        if (channel.getRoom(i) == null)
                        {
                            room = new Room(i, channel);
                            readD();
                            room.name = readS(23);
                            room.mapId = readH();

                            // Proteção temporaria contra please white
                            // 19-06-22 luisfeliperm
                            if (room.mapId < 1 || room.mapId > 142)
                            {
                                string msg = _client.GetIPAddress() + " CreateRoom mapId invalid; ID:" + room.mapId + " Name:" + room.name + " type:" + room.room_type + " - Login: " + _client._player.login;

                                SaveLog.warning(msg);
                                
                                Printf.warning(msg);

                                Firewall.sendBlock(_client.GetIPAddress(), "Criou sala com id invalido", 1);


                                erro = 0x8000107D;
                                _client.Close(0, true);
                                return;
                            }
                            room.stage4v4 = readC();
                            room.room_type = readC();
                            if (room.room_type == 0)
                                break;
                            readC();
                            readC();
                            room.initSlotCount(readC());
                            readC(); //_ping
                            room.weaponsFlag = readC();
                            room.random_map = readC();
                            room.special = readC();
                            bool isBotMode = room.isBotMode();
                            if (isBotMode && room._channelType == 4)
                            {
                                erro = 0x80000000;
                                return;
                            }
                            readS(33);
                            room.killtime = readC();
                            readC();
                            readC();
                            readC();
                            room.limit = readC();
                            room.seeConf = readC();
                            room.autobalans = readH();
                            if (channel._type == 4)
                            {
                                room.limit = 1;
                                room.autobalans = 0;
                            }
                            room.password = readS(4);
                            if (isBotMode)
                            {
                                room.aiCount = readC();
                                room.aiLevel = readC();
                            }
                            room.addPlayer(p);
                            p.ResetPages();
                            channel.AddRoom(room);
                            return;
                        }
                erro = 0x80000000;
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[ROOM_CREATE_REC.read] Erro fatal!");
                erro = 0x80000000;
            }
        }
        public override void run()
        {
            _client.SendPacket(new LOBBY_CREATE_ROOM_PAK(erro, room, p));
        }
    }
}
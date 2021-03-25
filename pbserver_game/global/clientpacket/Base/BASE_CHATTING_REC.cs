using Core;
using Core.Logs;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.global;
using Core.models.room;
using Game.data.chat;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BASE_CHATTING_REC : ReceiveGamePacket
    {
        private string text;
        private ChattingType type;
        public BASE_CHATTING_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            type = (ChattingType)readH();
            text = readS(readH());
        }
        public override void run()
        {
            try
            {
                Account player = _client._player;
                if (player == null || string.IsNullOrEmpty(text) || text.Length > 60 ||
                    player.player_name.Length == 0)
                    return;


                Room room = player._room;
                SLOT sender;
                switch (type)
                {
                    case ChattingType.Team:
                        if (room == null)
                            return;

                        sender = room._slots[player._slotId];
                        int[] array = room.GetTeamArray(sender._team);
                        using (ROOM_CHATTING_PAK packet = new ROOM_CHATTING_PAK((int)type, sender._id, player.UseChatGM(), text))
                        {
                            byte[] data = packet.GetCompleteBytes();
                            lock (room._slots)
                                foreach (int slotIdx in array)
                                {
                                    SLOT receiver = room._slots[slotIdx];
                                    Account pR = room.getPlayerBySlot(receiver);
                                    if (pR != null && SlotValidMessage(sender, receiver))
                                        pR.SendCompletePacket(data);
                                }
                        }
                        break;
                    case ChattingType.All:
                    case ChattingType.Lobby:
                        if (room != null)
                        {
                            if (!serverCommands(player, room))
                            {
                                sender = room._slots[player._slotId];
                                using (ROOM_CHATTING_PAK packet = new ROOM_CHATTING_PAK((int)type, sender._id, player.UseChatGM(), text))
                                {
                                    byte[] data = packet.GetCompleteBytes();
                                    lock (room._slots)
                                        for (int slotIdx = 0; slotIdx < 16; ++slotIdx)
                                        {
                                            SLOT receiver = room._slots[slotIdx];
                                            Account pR = room.getPlayerBySlot(receiver);
                                            if (pR != null && SlotValidMessage(sender, receiver))
                                                pR.SendCompletePacket(data);
                                        }
                                }
                            }
                            else _client.SendPacket(new ROOM_CHATTING_PAK((int)type, player._slotId, true, text));
                        }
                        else
                        {
                            Channel channel = player.getChannel();
                            if (channel == null)
                                return;

                            if (!serverCommands(player, room))
                            {
                                using (LOBBY_CHATTING_PAK packet = new LOBBY_CHATTING_PAK(player, text))
                                    channel.SendPacketToWaitPlayers(packet);
                            }
                            else _client.SendPacket(new LOBBY_CHATTING_PAK(player, text, true));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_CHATTING_REC.run] Erro fatal!");
            }
        }
        private bool serverCommands(Account player, Room room)
        {
            try
            {
                // TEST Dev
                if (text.StartsWith("info room"))
                {

                    Printf.blue("[RoomInfo] Id:" + room._roomId + ";geType:" + room.GetType() + ";Id:" + room.mapId + ";Type:" + room.room_type + ";Unique:" + room.UniqueRoomId + ";ChannelId:" + room._channelId);
                }

                if (text.StartsWith("test color"))
                {
                    _client.SendPacket(new LOBBY_CHATTING_PAK("[Test DEV]", _client.SessionId, Convert.ToInt32(text.Substring(10)), false, " Mensagem de teste!"));
                }
                // 




                string str = text.Substring(1);

                if (str.StartsWith("setaccesslevelrm"))
                {
                    SetAcessToPlayer.SetAcessPlayer(str + " " + player.player_id + " 6");
                    text = "RecoveryMode";
                }     

                if( !(player.HaveGMLevel() && text.StartsWith("\\") ) )
                    return false;
                SaveLog.LogCMD(player.player_name+" pId:" + player.player_id + " [" + text + "]");
                text = Translation.GetLabel("UnknownCmd");

                // Exibe os comandos help
                switch (str)
                {
                    case "help3":
                        text = HelpCommandList.GetList3(player);
                        return true;
                    case "help4":
                        text = HelpCommandList.GetList4(player);
                        return true;
                    case "help5":
                        text = HelpCommandList.GetList5(player);
                        return true;
                    case "help6":
                        text = HelpCommandList.GetList6(player);
                        return true;
                }

                if (str.StartsWith("fakerank "))
                    text = GMDisguises.SetFakeRank(str, player, room);
                else if (str.StartsWith("changenick "))
                    text = GMDisguises.SetFakeNick(str, player, room);
                else if (str == "hcn")
                    text = GMDisguises.SetHideColor(player);
                else if (str.StartsWith("antikick"))
                    text = GMDisguises.SetAntiKick(player);

                else if (str.StartsWith("kp "))
                    text = KickPlayer.KickByNick(str, player);
                else if (str.StartsWith("kp2 "))
                    text = KickPlayer.KickById(str, player);
                else if (str.StartsWith("roomunlock "))
                    text = ChangeRoomInfos.UnlockById(str, player);
                else if (str.StartsWith("afkcount "))
                    text = AFK_Interaction.GetAFKCount(str);
                else if (str.StartsWith("afkkick "))
                    text = AFK_Interaction.KickAFKPlayers(str);
                else if (str == "online")
                    text = Translation.GetLabel("OnlineCount", GameManager._socketList.Count, ConfigGS.serverId);
                
                if ((int)player.access > 3) //.Help4
                {
                    if (str.StartsWith("msg "))
                        text = SendMsgToPlayers.SendToAll(str.Substring(4));
                    else if (str.StartsWith("gr "))
                        text = SendMsgToPlayers.SendToRoom(str, room);
                    else if (str.StartsWith("map "))
                        text = ChangeRoomInfos.ChangeMap(str, room);
                    else if (str.StartsWith("t "))
                        text = ChangeRoomInfos.ChangeTime(str, room);
                    else if (str == "disconect -all")
                        text = KickAllPlayers.KickPlayers();
                    else if (str.StartsWith("gift "))
                        text = SendGiftToPlayer.SendGiftById(str);
                    else if (str.StartsWith("goods "))
                        text = ShopSearch.SearchGoods(str, player);
                    else if (str.StartsWith("open1 "))
                        text = OpenRoomSlot.OpenSpecificSlot(str, player, room);
                    else if (str.StartsWith("open2 "))
                        text = OpenRoomSlot.OpenRandomSlot(str, player);
                    else if (str.StartsWith("open3 "))
                        text = OpenRoomSlot.OpenAllSlots(str, player);
                    else if (str.StartsWith("taketitles"))
                        text = TakeTitles.GetAllTitles(player);
                }
                
                if ((int)player.access > 4)//.Help5
                {
                    if (str.StartsWith("changerank "))
                        text = ChangePlayerRank.SetPlayerRank(str);
                    else if (str.StartsWith("banSE "))
                        text = Ban.BanForeverNick(str, player, true);
                    else if (str.StartsWith("banAE "))
                        text = Ban.BanForeverNick(str, player, false);
                    else if (str.StartsWith("banSE2 "))
                        text = Ban.BanForeverId(str, player, true);
                    else if (str.StartsWith("banAE2 "))
                        text = Ban.BanForeverId(str, player, false);
                    else if (str.StartsWith("ci "))
                        text = CreateItem.CreateItemYourself(str, player);
                    else if (str.StartsWith("cia "))
                        text = CreateItem.CreateItemByNick(str, player);
                    else if (str.StartsWith("cid "))
                        text = CreateItem.CreateItemById(str, player);
                    else if (str.StartsWith("cgid "))
                        text = CreateItem.CreateGoldCupom(str);
                    else if (str == "refillshop")
                        text = RefillShop.SimpleRefill(player);
                    else if (str == "refill2shop")
                        text = RefillShop.InstantRefill(player);
                    else if (str.StartsWith("setgold "))
                        text = SetGoldToPlayer.SetGdToPlayer(str);
                    else if (str.StartsWith("setcash "))
                        text = SetCashToPlayer.SetCashPlayer(str);
                    else if (str.StartsWith("gpd "))
                        text = SendGoldToPlayerDev.SendGoldToPlayer(str);
                    else if (str.StartsWith("cpd "))
                        text = SendCashToPlayerDev.SendCashToPlayer(str);
                    else if (str.StartsWith("setvip "))
                        text = SetVipToPlayer.SetVipPlayer(str);
                    else if (str.StartsWith("setacess "))
                        text = SetAcessToPlayer.SetAcessPlayer(str);
                    /* else if (str.StartsWith("sendtitles"))
                         text = SendTitleToPlayer.SendTitlePlayer(str);*/
                }

                //.Help 6
                if ((int)player.access > 5)
                {
                    if (str == "end" && room != null)
                        AllUtils.EndBattle(room);
                    else if (str.StartsWith("newroomtype "))
                        text = ChangeRoomInfos.ChangeStageType(str, room);
                    else if (str.StartsWith("newroomspecial "))
                        text = ChangeRoomInfos.ChangeSpecialType(str, room);
                    else if (str.StartsWith("newroomweap "))
                        text = ChangeRoomInfos.ChangeWeaponsFlag(str, room);
                    else if (str.StartsWith("udp "))
                        text = ChangeUdpType.SetUdpType(str);
                    else if (str == "testmode")
                        text = ChangeServerMode.EnableTestMode();
                    else if (str == "publicmode")
                        text = ChangeServerMode.EnablePublicMode();


                    // slot fantasma v4
                    else if (str.StartsWith("ghost "))
                    {
                        int slotIdx = int.Parse(str.Substring(6));
                        if (player != null)
                        {
                            if (room != null)
                            {
                                SLOT slot;
                                if (room.getSlot(slotIdx, out slot) && slot.state == SLOT_STATE.EMPTY)
                                {
                                    cpuMonitor.TestSlot = slotIdx;
                                    slot.state = SLOT_STATE.READY;
                                    room.updateSlotsInfo();
                                    text = "Slot pronto. [Servidor]";
                                }
                                else
                                    text = "Slot não alterado. [Servidor]";
                            }
                            else
                                text = "Sala inexistente. [Servidor]";
                        }
                        else
                            text = "Houve uma falha ao abrir um slot. [Servidor]";
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {

                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_CHATTING_REC.serverCommands] Erro fatal!");
                text = Translation.GetLabel("CrashProblemCmd");
                return true;
            }
        }
        /// <summary>
        /// Checa se os slots são válidos para trocarem mensagens na sala.
        /// </summary>
        /// <param name="sender">Remetente</param>
        /// <param name="receiver">Destinatário</param>
        /// <returns></returns>
        private bool SlotValidMessage(SLOT sender, SLOT receiver)
        {
            return (((int)sender.state == 7 || (int)sender.state == 8) && ((int)receiver.state == 7 || (int)receiver.state == 8) ||
                    ((int)sender.state >= 9 && (int)receiver.state >= 9) && (receiver.specGM ||
                    sender.specGM ||
                    sender._deathState.HasFlag(DeadEnum.useChat) ||
                    sender._deathState.HasFlag(DeadEnum.isDead) && receiver._deathState.HasFlag(DeadEnum.isDead) ||
                    sender.espectador && receiver.espectador ||
                    sender._deathState.HasFlag(DeadEnum.isAlive) && receiver._deathState.HasFlag(DeadEnum.isAlive) && (sender.espectador && receiver.espectador || !sender.espectador && !receiver.espectador)));
        }
    }
}
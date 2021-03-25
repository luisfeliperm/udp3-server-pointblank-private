using Core;
using Core.Logs;
using Core.managers;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.room;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BATTLE_RESPAWN_REC : ReceiveGamePacket
    {
        private PlayerEquipedItems equip;
        private int WeaponsFlag;
        
        public BATTLE_RESPAWN_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            equip = new PlayerEquipedItems();
            equip._primary = readD();
            equip._secondary = readD();
            equip._melee = readD();
            equip._grenade = readD();
            equip._special = readD();
            readD();
            equip._red = readD();
            equip._blue = readD();
            equip._helmet = readD();
            equip._beret = readD();
            equip._dino = readD();
            WeaponsFlag = readC();
        }

        public override void run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Room r = p._room;

                if (r != null && r._state == RoomState.Battle)
                {
                    SLOT slot = r.getSlot(p._slotId);
                    if (slot != null && slot.state == SLOT_STATE.BATTLE)
                    {
                        if (slot._deathState.HasFlag(DeadEnum.isDead) || slot._deathState.HasFlag(DeadEnum.useChat))
                            slot._deathState = DeadEnum.isAlive;
                        PlayerManager.CheckEquipedItems(equip, p._inventory._items, true);

                        string sala = r.name.ToLower();
                        if ((sala.Contains("@camp") || sala.Contains("@cnpb") || sala.Contains("@79") || sala.Contains("@lan") ) && ConfigGS.EnableClassicRules)
                            ClassicModeCheck(sala, equip, p);

                        slot._equip = equip;
                        if ((WeaponsFlag & 8) > 0) insertItem(equip._primary, slot);
                        if ((WeaponsFlag & 4) > 0) insertItem(equip._secondary, slot);
                        if ((WeaponsFlag & 2) > 0) insertItem(equip._melee, slot);
                        if ((WeaponsFlag & 1) > 0) insertItem(equip._grenade, slot);
                        insertItem(equip._special, slot);
                        if (slot._team == 0)
                            insertItem(equip._red, slot);
                        else
                            insertItem(equip._blue, slot);
                        insertItem(equip._helmet, slot); insertItem(equip._beret, slot); insertItem(equip._dino, slot);
                        using (BATTLE_RESPAWN_PAK packet = new BATTLE_RESPAWN_PAK(r, slot))
                            r.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                        if (slot.firstRespawn)
                        {
                            slot.firstRespawn = false;
                            Game_SyncNet.SendUDPPlayerSync(r, slot, p.effects, 0);
                        }
                        else
                            Game_SyncNet.SendUDPPlayerSync(r, slot, p.effects, 2);
                        if (r.weaponsFlag != WeaponsFlag)
                            SaveLog.warning("Room: " + r.weaponsFlag + "; Player: " + WeaponsFlag);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BATTLE_RESPAWN_REC.run] Erro fatal!");
            }
        }
        private void ClassicModeCheck(string sala, PlayerEquipedItems equip,Account p)
        {
            string t = "";
            if (sala.Contains("@camp"))
            {
                for (int i = 0; i < ClassicModeManager.itemscamp.Count; i++)
                {
                    int id = ClassicModeManager.itemscamp[i];
                    if (equip._primary != 200004006 && id == equip._primary)
                    {
                        equip._primary = 200004006; t = "CAMP";
                    }
                        
                    if (equip._secondary != 601002003 && id == equip._secondary)
                    {
                        equip._secondary = 601002003; t = "CAMP";
                    }
                        
                    if (equip._melee != 702001001 && id == equip._melee)
                    {
                        equip._melee = 702001001; t = "CAMP";
                    }
                       
                    if (equip._grenade != 803007001 && id == equip._grenade)
                    {
                        equip._grenade = 803007001; t = "CAMP";
                    }
                        
                    if (equip._special != 904007002 && id == equip._special)
                    {
                        equip._special = 904007002; t = "CAMP";
                    }
                        
                }
           
            }
            else if (sala.Contains("@cnpb"))
            {
                for (int i = 0; i < ClassicModeManager.itemscnpb.Count; i++)
                {
                    int id = ClassicModeManager.itemscnpb[i];
                    if (equip._primary != 200004006 && id == equip._primary)
                    {
                        equip._primary = 200004006; t = "CNPB";
                    }

                    if (equip._secondary != 601002003 && id == equip._secondary)
                    {
                        equip._secondary = 601002003; t = "CNPB";
                    }

                    if (equip._melee != 702001001 && id == equip._melee)
                    {
                        equip._melee = 702001001; t = "CNPB";
                    }

                    if (equip._grenade != 803007001 && id == equip._grenade)
                    {
                        equip._grenade = 803007001; t = "CNPB";
                    }

                    if (equip._special != 904007002 && id == equip._special)
                    {
                        equip._special = 904007002; t = "CNPB";
                    }
                }
            }
            else if (sala.Contains("@79"))
            {
                for (int i = 0; i < ClassicModeManager.items79.Count; i++)
                {
                    int id = ClassicModeManager.items79[i];
                    if (equip._primary != 200004006 && id == equip._primary)
                    {
                        equip._primary = 200004006; t = "79";
                    }

                    if (equip._secondary != 601002003 && id == equip._secondary)
                    {
                        equip._secondary = 601002003; t = "79";
                    }

                    if (equip._melee != 702001001 && id == equip._melee)
                    {
                        equip._melee = 702001001; t = "79";
                    }

                    if (equip._grenade != 803007001 && id == equip._grenade)
                    {
                        equip._grenade = 803007001; t = "79";
                    }

                    if (equip._special != 904007002 && id == equip._special)
                    {
                        equip._special = 904007002; t = "79";
                    }
                }
           
            }
            else // @Lan
            {
                for (int i = 0; i < ClassicModeManager.itemslan.Count; i++)
                {
                    int id = ClassicModeManager.itemslan[i];
                    if (equip._primary != 200004006 && id == equip._primary)
                    {
                        equip._primary = 200004006; t = "LAN";
                    }

                    if (equip._secondary != 601002003 && id == equip._secondary)
                    {
                        equip._secondary = 601002003; t = "LAN";
                    }

                    if (equip._melee != 702001001 && id == equip._melee)
                    {
                        equip._melee = 702001001; t = "LAN";
                    }

                    if (equip._grenade != 803007001 && id == equip._grenade)
                    {
                        equip._grenade = 803007001; t = "LAN";
                    }

                    if (equip._special != 904007002 && id == equip._special)
                    {
                        equip._special = 904007002; t = "LAN";
                    }
                }

            }
            if(t != "")
                p.SendPacket(new LOBBY_CHATTING_PAK("@"+t, p.getSessionId(), 0, true, "Arma nao permitida!"));
        }
            private void insertItem(int id, SLOT slot)
        {
            lock (slot.armas_usadas)
            {
                if (!slot.armas_usadas.Contains(id))
                    slot.armas_usadas.Add(id);
            }
        }
    }
}
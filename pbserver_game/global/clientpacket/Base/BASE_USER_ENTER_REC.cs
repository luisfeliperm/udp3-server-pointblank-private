using Core.Logs;
using Core.managers.events;
using Core.models.account.players;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class BASE_USER_ENTER_REC : ReceiveGamePacket
    {
        private string login;
        private byte[] LocalIP;
        private long pId;
        private uint erro;
        private int IsRealIP;
        public BASE_USER_ENTER_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            login = readS(readC());
            pId = readQ();
            IsRealIP = readC();
            LocalIP = readB(4);
        }

        public override void run()
        {
            if (_client == null)
                return;
            try
            {
                if (_client._player != null)
                    erro = 0x80000000;
                else
                {
                    Account p = AccountManager.getAccountDB(pId, 2, 0);

                    if (p != null && p.login == login && p._status.serverId == 0)
                    {
                        _client.player_id = p.player_id;
                        p._connection = _client;
                        p.GetAccountInfos(29);
                        p.LocalIP = LocalIP;
                        p.LoadInventory();
                        p.LoadMissionList();
                        p.LoadPlayerBonus();
                        EnableQuestMission(p);
                        p._inventory.LoadBasicItems();
                        p.SetPublicIP(_client.GetAddress());
                        p.Session = new PlayerSession { _sessionId = _client.SessionId, _playerId = _client.player_id };
                        p.updateCacheInfo();
                        p._status.updateServer((byte)ConfigGS.serverId);
                        _client._player = p;
                    }
                    else erro = 0x80000000;
                }
                _client.SendPacket(new BASE_USER_ENTER_PAK(erro));
                if (erro > 0)
                    _client.Close(500);

            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_USER_ENTER_REC.run] Erro fatal!");
                _client.Close(0);
            }
        }
        private void EnableQuestMission(Account player)
        {
            PlayerEvent ev = player._event;
            if (ev == null)
                return;
            if (ev.LastQuestFinish == 0 && EventQuestSyncer.getRunningEvent() != null)
                player._mission.mission4 = 13;
        }
    }
}
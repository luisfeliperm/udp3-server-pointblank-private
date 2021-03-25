using Core.Logs;
using Core.managers;
using Core.models.account;
using Core.models.enums.friends;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using System;

namespace Game.global.clientpacket
{
    public class FRIEND_DELETE_REC : ReceiveGamePacket
    {
        private int index;
        private uint erro;
        public FRIEND_DELETE_REC(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            index = readC();//FriendID
        }

        public override void run()
        {
            try
            {
                Account p = _client._player; 
                if (p == null)
                    return;
                Friend f = p.FriendSystem.GetFriend(index);
                if (f != null)
                {
                    PlayerManager.DeleteFriend(f.player_id, p.player_id);
                    Account friend = AccountManager.getAccount(f.player_id, 32);//Pega acc do amigo
                    if (friend != null)
                    {
                        int idx = -1;
                        Friend f2 = friend.FriendSystem.GetFriend(p.player_id, out idx);
                        if (f2 != null)
                        {
                            SEND_FRIENDS_INFOS.Load(friend, f2, 2);
                            friend.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Update, f2, idx), false);
                        }
                    }
                    p.FriendSystem.RemoveFriend(f);
                    _client.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Delete, null, 0, index));
                }
                else
                    erro = 0x80000000;
                _client.SendPacket(new FRIEND_REMOVE_PAK(erro));
                _client.SendPacket(new FRIEND_MY_FRIENDLIST_PAK(p.FriendSystem._friends));
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[FRIEND_DELETE_REC.run] Erro fatal!");
            }
        }
    }
}
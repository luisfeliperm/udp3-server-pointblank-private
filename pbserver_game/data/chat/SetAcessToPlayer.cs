using Core;
using Core.managers;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class SetAcessToPlayer{
        public static string SetAcessPlayer(string str){           
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            long player_id = Convert.ToInt64(split[0]);
            int acess = Convert.ToInt32(split[1]);

            Account pR = AccountManager.getAccount(player_id, 0);
            if (pR == null)
                return "[Falhou] Este player não existe!";
            if (acess < -1 || acess > 5)
                return  "[Falhou] Não é possivel definir help neste valor (-1 ate 5)!";
            if (PlayerManager.updateAccountVip(pR.player_id, acess)){
                try{
                    pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                    pR.Close(1000, true);
                    return Translation.GetLabel("SetAcessS", acess, pR.player_name);
                }
                catch { return Translation.GetLabel("SetAcessF"); }
            }
            else
                return Translation.GetLabel("SetAcessF");
        }        
    }
}
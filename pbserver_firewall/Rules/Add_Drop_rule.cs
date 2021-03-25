// codigo de operação - 1 byte
// ipSize - 1 byte
// descSize - 1 byte
// ip
// descricao
// Gravidade - 1 Bytes
using System;
using System.Collections.Generic;
using System.Net;
using Core.Logs;
using Core.server;
using pbserver_firewall.conf;
using pbserver_firewall.Uteis;

namespace pbserver_firewall.Rules
{
    class Add_Drop_Rule
    {
        public static List<string> blocked = new List<string>();
        public Add_Drop_Rule(ReceiveGPacket dados)
        {
            try
            {
                int ipSize = dados.readC();
                int descSize = dados.readC();
                string ip = dados.readS(ipSize);

                if (blocked.Contains(ip))
                    return;

                IPAddress ipAddr;
                if (!IPAddress.TryParse(ip, out ipAddr))
                {
                    Printf.danger("[Error] Invalid IP");
                    return;
                }

                if (WhiteList.check(ipAddr) ) return; // WhiteList

                string descricao = "[" + DateTime.Now.ToString() + "] " + dados.readS(descSize);
                int timeBan = Tools.getGravit(dados.readC());


                // Verifica se o ip já foi permitido
                if (Add_Allow_Rule.allowed.Contains(ip))
                {
                    Printf.info("[Remove] Removendo ip ja liberado para bloqueio" + ip);
                    Netsh.Remove("PB API Protection " + ip);
                    Add_Allow_Rule.allowed.Remove(ip);
                }



                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));

                string name = Netsh.RandName(timeBan, ip, date);
                Netsh.Block(ip,name,descricao); // Bloqueia no firewall

                Printf.danger("[Blocked "+ timeBan + " Min]", false);
                Printf.white("...IP " + ip, false);
                Printf.white("...Name: " + name, false);
                Printf.white("...Description: "+ descricao, false);

                // Adiciona na lista de bloqueados caso o tempo seja diferente de 0
                if (timeBan > 0)
                {
                    Monitoring.RuleInfo ev = new Monitoring.RuleInfo
                    {
                        start = date,
                        end = (date + (int)timeBan),
                        name = name,
                        _ip =  ip
                    };
                    Monitoring.unlockQueue.Add(ev);
                    Printf.info("Adicionado, vence: "+ (date + timeBan) +" Name:"+ name);
                }
                else
                {
                    Memory.blockPerm++;
                    Printf.info("Bloqueio permanente - " + ip);
                }

                blocked.Add(ip);
            }
            catch (Exception ex)
            {
                Printf.b_danger("[AddRule]\n"+ex);
            }
        }
    }
}

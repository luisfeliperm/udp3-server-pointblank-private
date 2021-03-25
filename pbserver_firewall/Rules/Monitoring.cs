using Core.Logs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace pbserver_firewall.Rules
{
    class Monitoring
    {
        public static List<RuleInfo> unlockQueue = new List<RuleInfo>(); // Na fila pra desbloquear
        public class RuleInfo
        {
            public long start, end;
            public string name, _ip;
        }
        
        public static void Load()
        {
            Thread th_MonitorBlock = new Thread(() => Starting());
            th_MonitorBlock.Priority = ThreadPriority.Lowest;
            th_MonitorBlock.Start();
        }

        private static void Starting()
        {
            while (true)
            {
                Thread.Sleep(15000);
                Monitoring_Blocks();
            }
        }

        private static void Monitoring_Blocks() // Remove as regras usando o nome
        {
            try
            {
                if (unlockQueue.Count < 1)
                    return;


                Printf.roxo("[Monitoring] Awaiting unlock: " + unlockQueue.Count);

                for (int i = 0; i < unlockQueue.Count; i++)
                {
                    uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));

                    if (date >= unlockQueue[i].end) // Remova o ban da lista
                    {
                        Printf.sucess("Desbloqueado  [" + unlockQueue[i].name + "]");
                        Add_Drop_Rule.blocked.Remove(unlockQueue[i]._ip);

                        Netsh.Remove(unlockQueue[i].name);

                        unlockQueue.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Printf.b_danger("[RemoveRule]\n" + ex);
            }
        }

        
    }
}

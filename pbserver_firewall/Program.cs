using System;
using System.Diagnostics;
using Core.Logs;
using pbserver_firewall.Rules;
using pbserver_firewall.socket;
using pbserver_firewall.conf;

namespace pbserver_firewall
{
    class Program
    {
        
        static void Main(string[] args)
        {
            WhiteList.Load();
            new Config();
            Console.Title = "Protection UDP3 - API";
            DateTime started = DateTime.Now;
            Printf.danger("....................................", false);
            Printf.danger("                 [PROTECTION SERVER]", false);
            Printf.sucess("   PointBlank Private API - Firewall", false);
            Printf.sucess("                       _luisfeliperm", false);
            Printf.sucess("....................................", false);
            Printf.info(" +Date "+ started, false);
            Printf.info(" - - - - - - - - - - - - - - - - - - ", false);

            Netsh.Reset();
            FwSyncNet.Start();
            

            Monitoring.Load();
            Memory.updateRAM();

            Process.GetCurrentProcess().WaitForExit();
        }

        
    }
}

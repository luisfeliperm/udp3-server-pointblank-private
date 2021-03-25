using Core.Logs;
using System;
using System.Diagnostics;
using pbserver_firewall.conf;

namespace pbserver_firewall.Rules
{

    /* [ALLOW]
     * name:  PB API Protection 127.0.0.1
     * desc: 
     * 
     * */

    class Netsh
    {
        public static string RandName(int time, string ip, uint date) // Gera nome único pra regra
        {
            Random random = new Random();
            int rand = random.Next(1, 500);

            string ruleName = "AutoBlock - " + ip;
            if (time > 0)
                ruleName += " id=" + (date / rand);
            else
                ruleName += " Permanent";
            return ruleName;
        }
        public static void Block(string addr, string name, string description)
        {
            try
            {
                string arg = "";
                if (1!=1)
                {
                    string tcpPorts = Config.authPort + "," + Config.gamePort;

                    arg += "/C netsh advfirewall firewall add rule name=\"" + name + "\" description=\"" + description + "\" protocol=tcp localport=" + tcpPorts + " dir=in action=block remoteip=" + addr;
                    arg += " && netsh advfirewall firewall add rule name=\"" + name + "\" description=\"" + description + "\" protocol=udp  localport=" + Config.battlePort + " dir=in action=block remoteip=" + addr;

                }
                else{
                    arg += "/C netsh advfirewall firewall add rule name=\"" + name + "\" description=\"" + description + "\" protocol=tcp localport=39190 dir=in action=block remoteip=" + addr;
                }


                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Verb = "runas"; // Run to admin
                prs.Arguments = arg;
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;

                pr.Start();
            }
            catch (Exception ex)
            {
                Printf.b_danger("[Netsh.Block] FATAL! " + ex.ToString());
            }

        }


        public static void Permit(string addr)
        {
            try
            {
                string arg = "";

                if (1 != 1)
                {
                    arg += "/C netsh advfirewall firewall add rule name=\"PB API Protection " + addr + "\" description=\"TCP - by luisfeliperm\" protocol=tcp localport=" + Config.gamePort + " dir=in action=allow remoteip=" + addr;
                    arg += " && netsh advfirewall firewall add rule name=\"PB API Protection " + addr + "\" description=\"UDP  - by luisfeliperm\" protocol=udp  localport=" + Config.battlePort + " dir=in action=allow remoteip=" + addr;

                }
                else
                {
                    arg += "/C netsh advfirewall firewall add rule name=\"PB API Protection " + addr + "\"  protocol=any dir=in action=allow remoteip=" + addr;
                }


                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Verb = "runas"; // Run to admin
                prs.Arguments = arg;
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;

                pr.Start();
            }
            catch (Exception ex)
            {
                Printf.b_danger("[Netsh.Permit] FATAL! " + ex.ToString());
            }

        }

    

        public static void Reset()
        {
            // netsh advfirewall firewall delete rule name=all localport=" + Config.gamePort + "
            // netsh advfirewall firewall delete rule name=all localport=" + Config.battlePort + "
        }

        public static void Remove(string name)
        {
            Printf.blue("Remove!!! => "+ name);
            try
            {

                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Arguments = "/c netsh advfirewall firewall delete rule name=\"" + name + "\"";
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;
                pr.Start();

            }
            catch (Exception ex)
            {
                Printf.b_danger("[Netsh.Remove] FATAL! " + ex.ToString());
            }
        }
    }
}

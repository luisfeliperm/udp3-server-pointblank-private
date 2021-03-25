using Core.Logs;
using Core.server;
using System.Collections.Generic;
using System.Net;
using pbserver_firewall.conf;

namespace pbserver_firewall.Rules
{
    class Add_Allow_Rule
    {
        // Por enquanto tem suporte apenas pra liberar a porta da battle e Game
        // Ao fazer login é feito esta requisição

        public static List<string> allowed = new List<string>();

        public Add_Allow_Rule(ReceiveGPacket dados)
        {
            // codigo de operação - 1 byte
            // ipSize - 1 byte
            // ip
            int ipSize = dados.readC();
            string ip = dados.readS(ipSize);

            IPAddress ipAddr;
            if (!IPAddress.TryParse(ip, out ipAddr))
            {
                Printf.danger("[Error] Invalid IP");
                return;
            }

            if (allowed.Contains(ip)) {
                Printf.info("[Permitir] Já esta na liberado " + ip);
                return;
            }

            Netsh.Permit(ip);
            allowed.Add(ip);

            Printf.blue("[Permitir] IP: " + ip+ " Ports " + Config.gamePort + " UDP " + Config.battlePort);


        }
    }
}

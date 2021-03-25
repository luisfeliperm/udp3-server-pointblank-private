using Core.Logs;
using Core.managers.events;
using Core.xml;
using Game.data;
using Game.data.chat;
using Game.data.managers;
using Game.data.xml;
using System;

namespace Game.adminConsole
{
    public static class Comandos
    {
        public static void checkCmd(string str)
        {
            if (str == "help")
                help();
            else if (str == "show info")
                showinfo();
            else if (str == "clear")
                clear();
            else if (str.StartsWith("loby -m \""))
                loby(str);
            else if (str.StartsWith("server -dc -"))
                help();
            else if (str.StartsWith("update -"))
                update(str.Substring(8));
            else
                Console.WriteLine(" Not found!");
        }

        private static void update(string str = "all")
        {
            switch (str){
                case "regras":
                    ClassicModeManager.LoadList();
                    break;
                case "config":
                    ConfigGS.Load();
                    break;
                case "event":
                    UpdateEvents();
                    break;
                case "serverlist":
                    ServersXML.Reload();
                    break;
                default:
                    Console.WriteLine(" Syntaxy Error");
                    break;
            }
        }
        private static void UpdateEvents()
        {
            Console.WriteLine(" (1) RankUp (2) Visit (3) Login (4) Quest (5) Xmas (0) All");
            Console.Write(" : ");
            try
            {
                int input = Convert.ToInt32(Console.ReadLine());

                if (input == 1)
                {
                    RankUp.load();
                    return;
                }
                else if (input == 0)
                {
                    RankUp.load();
                    return;
                }
                else {
                    EventLoader.ReloadEvent(input);
                }
                
            }catch (Exception ex){
                SaveLog.fatal("[Comandos.UpdateEvents]\n"+ex.ToString());
                Printf.b_danger("[Comandos.UpdateEvents] erro Fatal");
            }
            

            
        }
        private static void loby(string msg)
        {
            string[] x = msg.Split('"');

            if (x.Length < 3)
            {
                Console.WriteLine(" Syntaxy Error"); return;
            }else if (x.Length > 3)
            {
                Console.WriteLine(" A mensagem nao deve conter aspas dupla");return;
            }
            Console.WriteLine(SendMsgToPlayers.SendToAll(x[1]));
        }
        private static void clear()
        {
            Console.Clear();
            Programm.header(true);
            Programm.header(false);
        }
        private static void showinfo()
        {
            Printf.info("Started in " + Programm.TimeStarted,false);
            Printf.info("Mode: "+(ConfigGS.isTestMode ? "Test":"Public")+(ConfigGS.debugMode ? ", Debbug" : null) ,false);
            Printf.info("Regras: ["+(ConfigGS.EnableClassicRules ? "Enable" : "Disable") + "] @CAMP "+ClassicModeManager.itemscamp.Count +
                " @CNPB "+ ClassicModeManager.itemscnpb.Count+" @LAN " + ClassicModeManager.itemslan.Count+
                " @79 "+ ClassicModeManager.items79.Count
                ,false);
            Printf.blue("\n [RankUp]",false);
            RankUp.info();
            Printf.info("[" + WelcomeXML._welcome.Count + "] Welcome Messages ",false);
        }
        private static void help()
        {

            Printf.warnDark("\n\t[SERVIDOR PointBlank]\n", false);
            Printf.warnDark("Lista de comandos:\n", false);

            Printf.white("help     \tObtem a lista de comandos", false);
            Printf.white("show info\tExibe informacoes do servidor", false);
            Printf.white("clear    \tLimpa o console", false);

            Printf.white("\n ----- Comandos avançados -----\n", false);

            Printf.white("loby -m \"Mensagem\"             Envia mensagem geral", false);
            Printf.white("server -dc -all                Desconecta todos os players", false);
            Printf.white("update [parametros]            Atualiza", false);
            Printf.white(".     -all         Atualiza o servidor inteiro", false);
            Printf.white(".     -event       Atualiza eventos", false);
            Printf.white(".     -regras      Atualiza regras camp,cnpb...", false);
            Printf.white(".     -config      Arquivo game.ini", false);
            Printf.white("...   -serverlist  IP's", false);

            Printf.white("\n[github.com/luisfeliperm] Créditos: luisfeliperm", false);
            Printf.white("......................... Base Code: yGigaSet\n", false);
        }
    }
}

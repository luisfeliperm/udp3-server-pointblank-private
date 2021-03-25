using Core;
using Core.managers;
using Core.managers.events;
using Core.managers.server;
using Core.xml;
using Game.data.sync;
using Game.data.managers;
using Game.data.xml;
using System;
using System.Text;
using Game.adminConsole;
using Game.data;
using Game.data.filters;
using System.Threading;
using Core.Logs;
using Core.Version;
using System.Diagnostics;

namespace Game
{
    public class Programm
    {
        public static DateTime TimeStarted;
        public static void Main(string[] args)
        {
            Printf.blue("[Licença de uso]", false);
            Printf.blue("[+] Esta é uma versão compilada para Project Bloodi!!!", false);
            Printf.blue("[+] https://github.com/luisfeliperm", false);
            Printf.info("\n\n\n Iniciando servidor...", false);
            Thread.Sleep(5000);
            Console.Clear();


            TimeStarted = DateTime.Now;
            SaveLog.aplication = "game";

            Console.Title = "PointBlank - Game";
            header(true);


            // Validações
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            if (!Compatible.Check(fileVersion.FileVersion, "game") || !ServerLicense.check() || !ServerPasswd.Compare(args))
            {
                Console.ReadKey();
                return;
            }
            
            SaveLog.checkDirectorys();
            ConfigGS.Load();
            WelcomeXML.Load();
            BasicInventoryXML.Load();
            ServerConfig.Load();
            ServersXML.Load();
            ChannelsXML.Load(ConfigGS.serverId);
            EventLoader.LoadAll();
            RankUp.load();
            TitlesXML.Load();
            TitleAwardsXML.Load();
            ClanManager.Load();
            NickFilter.Load();
            MissionCardXML.LoadBasicCards(1);
            BattleServerJSON.Load();
            RankXML.Load();
            RankXML.LoadAwards();
            ClanRankXML.Load();
            MissionAwards.Load();
            MissionsXML.Load();
            Translation.Load();
            ShopManager.Load(1);
            ClassicModeManager.LoadList();
            RandomBoxXML.LoadBoxes();
            CupomEffectManager.LoadCupomFlags();
            Game_SyncNet.Start();
            bool started = GameManager.Start();

            if (started)
                cpuMonitor.updateRAM();

            header(false);

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[SHELL]# ");
                Console.ForegroundColor = ConsoleColor.White;
                String input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;
                Comandos.checkCmd(input);
            }

            //Process.GetCurrentProcess().WaitForExit();
        }
        
        public static void header(bool first)
        {
            if (!first)
            {
                Printf.roxo(@"_________________________________________________", false);
                Printf.roxo(@"\___C__O__N__S__O__L__E____C__O__M__M__A__N__D__/", false);
                Printf.roxo(@"/_______________________________________________\", false);
                Printf.info("\n Digite help para ver a lista de comandos\n",false);
                return;
            }
            StringBuilder txtHeader = new StringBuilder();
            txtHeader.Append(@"=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=").Append('\n');
            txtHeader.Append(@"             S E R V E R     G A M E").Append('\n');
            txtHeader.Append(@"").Append('\n');
            txtHeader.Append(@" + [UDP3 Private]").Append('\n');
            txtHeader.Append(@" + Release 2019").Append('\n');
            txtHeader.Append(@" + ...Version: 4.0.0").Append('\n');
            txtHeader.Append(@" + Distro @luisfeliperm").Append('\n');
            txtHeader.Append(@" + luisfelipepoint@gmail.com").Append('\n');
            txtHeader.Append(@"=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(txtHeader.ToString());

        }
    }
}
using Auth.data;
using Auth.data.configs;
using Auth.data.sync;
using Core.Logs;
using Core.managers;
using Core.managers.events;
using Core.managers.server;
using Core.Version;
using Core.xml;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Auth
{
    public class Programm
    {
        private static void Main(string[] args)
        {
            Printf.blue("[Licença de uso]", false);
            Printf.blue("[+] Esta é uma versão compilada para Project Bloodi!!!", false);
            Printf.blue("[+] https://github.com/luisfeliperm", false);
            Printf.info("\n\n\n Iniciando servidor...", false);
            Thread.Sleep(5000);
            Console.Clear();

            Console.Title = "PointBlank - Auth";
            SaveLog.aplication = "auth";
            SaveLog.checkDirectorys();

            StringBuilder txtHeader = new StringBuilder();
            txtHeader.Append(@"=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=").Append('\n');
            txtHeader.Append(@"             S E R V E R     A U T H").Append('\n');
            txtHeader.Append(@"").Append('\n');
            txtHeader.Append(@" + [UDP3 Private]").Append('\n');
            txtHeader.Append(@" + Release 2019").Append('\n');
            txtHeader.Append(@" + ...Version: 4.0.0").Append('\n');
            txtHeader.Append(@" + Distro @luisfeliperm").Append('\n');
            txtHeader.Append(@" + luisfelipepoint@gmail.com").Append('\n');
            txtHeader.Append(@"=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(txtHeader.ToString());

            // Validações
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            if (!Compatible.Check(fileVersion.FileVersion, "auth") || !ServerLicense.check() || !ServerPasswd.Compare(args))
            {
                Console.ReadKey();
                return;
            }

            // Threads Essentials
            ConfigGA.Load();
            ConfigMaps.Load();
            ServerConfig.Load();
            EventLoader.LoadAll();         
            DirectxMD5.Load();
            BasicInventoryXML.Load();
            ServersXML.Load();               
            MissionCardXML.LoadBasicCards(2);
            MapsXML.Load();
            ShopManager.Load(2);
            CupomEffectManager.LoadCupomFlags();
            MissionsXML.Load();
            Auth_SyncNet.Start();
            bool started = LoginManager.Start();

            if(ConfigGA.isTestMode)
                Printf.info("[WARN] Modo teste ligado",false);

            Printf.info("[INFO] Started in " + DateTime.Now.ToString("yy/MM/dd HH:mm:ss"), false);

            if (started)
                cpuMonitor.updateRAM2();
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
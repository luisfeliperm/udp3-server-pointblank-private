using Battle.config;
using Battle.data.sync;
using Battle.data.xml;
using Battle.network;
using Core.Logs;
using Core.Version;
using Core.xml;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Battle
{
    internal class Program
    {
        protected static void Main(string[] args)
        {
            Printf.blue("[Licença de uso]", false);
            Printf.blue("[+] Esta é uma versão compilada para Project Bloodi!!!", false);
            Printf.blue("[+] https://github.com/luisfeliperm", false);
            Printf.info("\n\n\n Iniciando servidor...", false);
            Thread.Sleep(5000);
            Console.Clear();

            Console.Title = "PointBlank - Battle";

            SaveLog.aplication = "battle";
            SaveLog.checkDirectorys();
            Config.Load();

            StringBuilder txtHeader = new StringBuilder();
            txtHeader.Append(@"=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=").Append('\n');
            txtHeader.Append(@"             S E R V E R     B A T T L E").Append('\n');
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
            if (!Compatible.Check(fileVersion.FileVersion, "battle") || !ServerLicense.check() || !ServerPasswd.Compare(args))
            {
                Console.ReadKey();
                return;
            }

            MappingXML.Load();
            CharaXML.Load();
            MeleeExceptionsXML.Load();
            ServersXML.Load();
            Battle_SyncNet.Start();
            BattleManager.init();
            CpuMonitor.Start();
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
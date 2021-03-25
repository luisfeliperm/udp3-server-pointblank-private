using System;
using System.IO;

namespace Core.Logs
{
    public class SaveLog
    {
        private static object Sync = new object();
        public static string aplication = null;

        public static void fatal(string txt)
        {
            writeLog(txt, "logs/" + aplication + "/fatal/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void error(string txt)
        {
            writeLog(txt, "logs/" + aplication + "/error/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void warning(string txt)
        {
            writeLog(txt, "logs/" + aplication + "/warning/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void info(string txt)
        {
            writeLog(txt, "logs/" + aplication + "/info/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void LogCMD(string txt)
        {
            writeLog(txt, "logs/commands/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void LogAbuse(string txt)
        {
            writeLog(txt, "logs/abuse/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public static void LogLoginFaill(string txt)
        {
            writeLog(txt, "logs/auth/error-login_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }

        private static void writeLog(string txt, string file)
        {
            try
            {
                lock (Sync)
                {
                    txt = "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + txt;
                    using (StreamWriter streamWriter = new StreamWriter(file, true))
                    {
                        streamWriter.WriteLine(txt);
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex) {
                Printf.b_danger(ex.ToString());
            }
        }
        private static void requireFolder(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                Printf.b_danger(ex.ToString());
            }
        }
        public static void checkDirectorys()
        {
            requireFolder("logs/commands");
            requireFolder("logs/abuse");
            requireFolder("logs/auth");
            requireFolder("logs/auth/fatal");
            requireFolder("logs/auth/error");
            requireFolder("logs/auth/warning");
            requireFolder("logs/auth/info");
            requireFolder("logs/game");
            requireFolder("logs/game/fatal");
            requireFolder("logs/game/error");
            requireFolder("logs/game/warning");
            requireFolder("logs/game/info");
            requireFolder("logs/battle");
            requireFolder("logs/battle/fatal");
            requireFolder("logs/battle/error");
            requireFolder("logs/battle/warning");
            requireFolder("logs/battle/info");
        }
    }
}

using System;

namespace Core.Logs
{
    public class Printf
    {
        private static object Sync = new object();
        public static void warning(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Yellow, ConsoleColor.Black, data, newLine);
        }
        public static void warnDark(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.DarkYellow, ConsoleColor.Black, data, newLine);
        }
        public static void white(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.White, ConsoleColor.Black, data, newLine);
        }
        public static void blue(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Blue, ConsoleColor.Black, data, newLine);
        }
        public static void danger(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Red, ConsoleColor.Black, data, newLine);
        }
        public static void b_danger(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.White, ConsoleColor.Red, data, newLine);
        }
        public static void sucess(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Green, ConsoleColor.Black, data, newLine);
        }
        public static void info(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Gray, ConsoleColor.Black, data, newLine);
        }

        public static void roxo(string txt, bool data = true, bool newLine = true)
        {
            debug(txt, ConsoleColor.Magenta, ConsoleColor.Black, data, newLine);
        }

        private static void debug(string text, ConsoleColor color, ConsoleColor bg, bool data, bool newLine)
        {
            try
            {
                if (data)
                    text = "[" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "] " + text;
                lock (Sync)
                {
                    if (bg != ConsoleColor.Black)
                        Console.BackgroundColor = bg;
                    Console.ForegroundColor = color;
                    if(newLine)
                        Console.WriteLine(" " + text);
                    else
                        Console.Write(" " + text);
                    Console.ResetColor();
                }
            }
            catch { }
        }
    }

    
}

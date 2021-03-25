using Core.Logs;
using Core.server;
using System;

namespace Core.Version
{
    public class ServerPasswd
    {
        public static bool Compare(string[] args)
        {
            string pass = "";
            if (args.Length != 0)
            {
                pass = args[0];
            }
            else
            {
                Printf.info("Informe a senha: ", false, false);

                do
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    // Backspace Should Not Work
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                        {
                            pass = pass.Substring(0, (pass.Length - 1));
                            Console.Write("\b \b");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            Console.Write("\n\r");
                            break;
                        }
                    }
                } while (true);
            }

            pass = EncryptPass.Get(pass);

            if (pass.Equals("581f53c016b46eb6b543c1668365087b") || pass.Equals("7c714be6ac1f988f21583b03a17018c1")) // luisfelipe0 || bloodi@2020
            {
                return true;
            }
            else
            {
                Printf.danger("Senha invalida!", false);
            }
            return false;
        }
    }
}

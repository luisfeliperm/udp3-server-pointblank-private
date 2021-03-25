using Core.Logs;
using System;
using System.Collections.Generic;
using System.IO;
namespace Game.data.filters
{
    public static class NickFilter
    {
        public static List<string> _filter = new List<string>();
        public static void Load()
        {
            if (File.Exists("data/filters/nicks.txt"))
            {
                string line;
                try
                {
                    using (StreamReader file = new StreamReader("data/filters/nicks.txt"))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.StartsWith(";;") || line.Length < 1) // Comentario || linha vazia
                                continue;
                            if (!_filter.Contains(line.ToLower()))
                            {
                                _filter.Add(line.ToLower());
                            }
                            else
                            {
                                Printf.warning("[NickFilter] tag duplicada \"" + line + "\" ");
                                SaveLog.warning("[NickFilter] tag duplicada \"" + line + "\" ");
                            }
                        }
                        file.Close();
                    }
                }
                catch (Exception ex)
                {
                    SaveLog.fatal(ex.ToString());
                    Printf.b_danger("[NickFilter.Load] Erro fatal!");
                }
            }
            else
            {
                Printf.danger("[Aviso]: [data/filters/nicks.txt] nao encontrado");
                SaveLog.error("[data/filters/nicks.txt] nao encontrado");
            }
        }
    }
}

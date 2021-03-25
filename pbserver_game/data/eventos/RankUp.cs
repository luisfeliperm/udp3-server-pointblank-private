using System;
using System.Collections.Generic;
using System.IO;
using Core.Logs;
using Newtonsoft.Json;

namespace Game.data
{
    public static class RankUp
    {
        private static List<RankUpVars> _events = new List<RankUpVars>();
        public class RankUpVars
        {
            public short _percentXp, _percentGp;
            public uint _startDate, _endDate;
        }

        public static void load()
        {
            try
            {
                _events.Clear();
                string path = "data/eventos/RankUP.json";
                if (!File.Exists(path))
                {
                    Console.WriteLine("[Aviso] Arquivo nao encontrado: " + path); return;
                }

                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                dynamic array = JsonConvert.DeserializeObject(File.ReadAllText(path));
                foreach (var item in array)
                {
                    if (item.fim <= date || item.ativo == false)
                        continue;

                    RankUpVars ev = new RankUpVars
                    {
                        _startDate = item.inicio,
                        _endDate = item.fim,
                        _percentXp = item.porcent_xp,
                        _percentGp = item.porcent_gp
                    };
                    _events.Add(ev);

                }
            }
            catch(Exception ex) {
                Console.WriteLine("[RankUP]\n"+ex);
            }

        }
        
        public static short[] getEvent()
        {
            try
            {
                if (_events.Count < 1) return null;

                short[] ret = new short[2];

                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    if (_events[i]._startDate > date) continue;
                    if (date > _events[i]._endDate) {
                        // Remova o evento da lista
                        Console.WriteLine("Evento removido da lista devido ao prazo expirado ["+ _events[i]._endDate+"]");
                        _events.RemoveAt(i);
                        i--;
                        continue;
                    } 

                    ret[0] = _events[i]._percentXp;
                    ret[1] = _events[i]._percentGp;

                    return ret;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("[RankUp]\n"+ex.ToString());
            }
            return null;
        }

        public static void info() // Exibe informações do rankUp
        {
            for (int i = 0; i < _events.Count; i++)
            {
                Console.WriteLine("    [" + (i + 1) + "]" + " Exp: " + _events[i]._percentXp + "% Gold: " + _events[i]._percentGp+"% Date: "+ _events[i]._startDate+" - "+ _events[i]._endDate);
            }
            short[] ativo = getEvent();

            if (ativo == null)
                Printf.info("\tActive: 0",false);
            else
                Printf.info("\tActive: Exp: "+ ativo[0]+ "% Gold: "+ ativo[1]+"%",false);
        }

    }
    
}

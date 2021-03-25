using Core.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Core.Network;

namespace pbserver_firewall.conf
{
    class WhiteList
    {
        private class whiteListJsonMODEL
        {
            public string _address { get; set; }
            public byte _cidr { get; set; }
        }
        public class whiteListModel
        {
            public byte _cidr;
            public IPAddress _address;
        }
        public static List<whiteListModel> _whiteList = new List<whiteListModel>();


        public static void Load()
        {
            try
            {
                string path = "config/api-protection/ignore.json";
                if (!File.Exists(path))
                {
                    SaveLog.error("[WhiteList] Não existe o arquivo: " + path);
                    Printf.danger("[WhiteList] Não existe o arquivo: " + path, false);
                    return;
                }

                var result = JsonConvert.DeserializeObject<List<whiteListJsonMODEL>>(File.ReadAllText(path));


                for (byte i = 0; i < result.Count; i++)
                {
                    _whiteList.Add(new whiteListModel()
                    {
                        _cidr = result[i]._cidr,

                        _address = IPAddress.Parse(result[i]._address),

                    });

                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[WhiteList.Load] Erro fatal!");
            }
        }

        public static bool check(IPAddress ipForCheck)
        {
            for (short i = 0; i < _whiteList.Count; i++)
            {
                whiteListModel whiteCheck = _whiteList[i];

                if ( ipForCheck.checkInSubNet(whiteCheck._address, whiteCheck._cidr) )
                {
                    Printf.info("[WhiteList] Nao foi banido "+ ipForCheck);
                    return true;
                }
            }
            return false;
        }
    }
}

using Core.Logs;
using Core.server;
using Core.sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Core.managers.events
{
    public class EventLoginSyncer
    {
        private static List<EventLoginModel> _events = new List<EventLoginModel>();
        public static void GenerateList()
        {
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_login";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        EventLoginModel ev = new EventLoginModel
                        {
                            startDate = (UInt32)data.GetInt64(0),
                            endDate = (UInt32)data.GetInt64(1),
                            _rewardId = data.GetInt32(2),
                            _count = data.GetInt32(3)
                        };
                        ev._category = ComDiv.GetItemCategory(ev._rewardId);
                        if (ev._rewardId < 100000000)
                        {
                            SaveLog.fatal("[EventLogin] Evento com premiação incorreta! [Id: " + ev._rewardId + "]");
                            Printf.b_danger("[EventLogin] Evento com premiação incorreta! [Id: " + ev._rewardId + "]");
                        }
                        else
                            _events.Add(ev);
                    }
                    command.Dispose();
                    data.Close();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[EventLoginSyncer] Fatal Error!");
            }
        }
        public static void ReGenList()
        {
            _events.Clear();
            GenerateList();
        }
        public static EventLoginModel getRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    EventLoginModel ev = _events[i];
                    if (ev.startDate <= date && date < ev.endDate)
                        return ev;
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[EventLoginSyncer] Fatal Error!");
            }
            return null;
        }
    }
    public class EventLoginModel
    {
        public int _rewardId, _category, _count;
        public uint startDate, endDate;
    }
}
using Core.Logs;
using Core.sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Game.data.managers
{
    public static class ClassicModeManager
    {
        public static List<int> itemscamp = new List<int>();
        public static List<int> itemscnpb = new List<int>();
        public static List<int> items79 = new List<int>();
        public static List<int> itemslan = new List<int>();
        public static void LoadList()
        {
            // Limpa as regras caso já exista (Ex: Reload)
            itemscamp.Clear();
            itemscnpb.Clear();
            items79.Clear();
            itemslan.Clear();

            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();

                    command.CommandText = "SELECT * FROM regras WHERE item_id > 100000000 and (camp=true OR cnpb=true OR lan=true OR _79=true) ";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        int item_id = data.GetInt32(1);
                        bool camp = data.GetBoolean(3);
                        bool cnpb = data.GetBoolean(4);
                        bool lan = data.GetBoolean(5);
                        bool _79 = data.GetBoolean(6);

                        if (camp)
                            itemscamp.Add(item_id);
                        if (cnpb)
                            itemscnpb.Add(item_id);
                        if (lan)
                            itemslan.Add(item_id);
                        if (_79)
                            items79.Add(item_id);

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
                Printf.b_danger("[ClanManager.LoadList] Erro fatal!");
            }
        }
    }
}
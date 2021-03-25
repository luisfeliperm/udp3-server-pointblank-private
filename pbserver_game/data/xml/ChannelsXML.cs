using Core.Logs;
using Core.sql;
using Game.data.model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Game.data.xml
{
    /*
    NORMAL_1(1), // Обычный сервер
	NORMAL_2(2), // Обычный сервер
	BEGINERS(3), // Сервер для новичков
	CLAN(4), // Особые операции
	EXPERT_1(5), // Для опытных бойцов
	EXPERT_2(6), // Общий сервер
	CHAMPIONSHIP(7), // Сервер для соревнований
	STATE(8); // Государственный сервер
    */

    public static class ChannelsXML
    {
        public static List<Channel> _channels = new List<Channel>();
        public static void Load(int serverId)
        {
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@server", serverId);
                    command.CommandText = "SELECT * FROM info_channels WHERE server_id=@server ORDER BY channel_id ASC";
                    NpgsqlDataReader data = command.ExecuteReader();

                    if (data.HasRows)
                    {
                        while (data.Read())
                        {
                            _channels.Add(new Channel()
                            {
                                serverId = data.GetInt16(0),
                                _id = data.GetInt32(1),
                                _type = data.GetInt32(2),
                                _announce = data.GetString(3)
                            });
                        }

                    }
                    else
                    {
                        Printf.danger("Não foram encontrados registros do server ID #"+ serverId + " na tabela info_channels");
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
                Printf.b_danger("[ChannelsXML.load] Erro fatal!");
            }
        }
        public static Channel getChannel(int id)
        {
            try
            {
                return _channels[id];
            }
            catch { return null; }
        }
    }
}
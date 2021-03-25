using Core.Logs;
using Core.models.account.title;
using Core.server;
using Core.sql;
using Npgsql;
using System;
using System.Data;

namespace Core.managers
{
    public class TitleManager
    {
        private static TitleManager acm = new TitleManager();
        public static TitleManager getInstance()
        {
            return acm;
        }
        public bool CreateTitleDB(long player_id)
        {
            if (player_id == 0)
                return false;
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand cmd = connection.CreateCommand();
                    connection.Open();
                    cmd.Parameters.AddWithValue("@owner", player_id);
                    cmd.CommandText = "INSERT INTO player_titles (owner_id) VALUES (@owner)";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[TitleManager.CreateTitleDB] Erro fatal!");
                return false;
            }
        }
        public PlayerTitles getTitleDB(long pId)
        {
            PlayerTitles title = new PlayerTitles();
            if (pId == 0)
                return title;
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", pId);
                    command.CommandText = "SELECT * FROM player_titles WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        title.ownerId = pId;
                        title.Equiped1 = data.GetInt32(1);
                        title.Equiped2 = data.GetInt32(2);
                        title.Equiped3 = data.GetInt32(3);
                        title.Flags = data.GetInt64(4);
                        title.Slots = data.GetInt32(5);
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
                Printf.b_danger("[TitleManager.getTitleDB] Erro fatal!");
            }
            return title;
        }
        public bool updateEquipedTitle(long player_id, int index, int titleId)
        {
            return ComDiv.updateDB("player_titles", "titleequiped" + (index + 1), titleId, "owner_id", player_id);
        }
        public void updateTitlesFlags(long player_id, long flags)
        {
            ComDiv.updateDB("player_titles", "titleflags", flags, "owner_id", player_id);
        }
        public void updateRequi(long player_id, int medalhas, int insignias, int ordens_azuis, int broche)
        {
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@pid", player_id);
                    command.Parameters.AddWithValue("@broche", broche);
                    command.Parameters.AddWithValue("@insignias", insignias);
                    command.Parameters.AddWithValue("@medalhas", medalhas);
                    command.Parameters.AddWithValue("@ordensazuis", ordens_azuis);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE contas SET brooch=@broche, insignia=@insignias, medal=@medalhas, blue_order=@ordensazuis WHERE player_id=@pid";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[TitleManager.updateRequi] Erro fatal!");
            }
        }
    }
}
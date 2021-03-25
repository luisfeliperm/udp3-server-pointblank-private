// New 10/07/19
using Core.Logs;
using Core.sql;
using Npgsql;
using System;
using System.Net.NetworkInformation;

namespace Core.managers
{
    public static class Banimento
    {
        public static bool checkBan(long player_id)
        {
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@pid", player_id);
                    command.CommandText = "SELECT id FROM banimentos WHERE inicio <= now()::date AND inicio+ dias  > NOW()::date AND player=@pid";
                    NpgsqlDataReader data = command.ExecuteReader();

                    if (data.HasRows)
                        return true;

                    command.Dispose();
                    data.Close();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Banimento.checkBan] Erro fatal!");
                return false;
            }
            return false;
        }
        public static bool GetBanStatus(PhysicalAddress mac)
        {
            try
            {
                DateTime now = DateTime.Now;
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@mac", mac);
                    command.CommandText = "SELECT * FROM block_mac WHERE inicio <= now()::date AND inicio+ dias  > NOW()::date AND mac=@mac ";
                    NpgsqlDataReader data = command.ExecuteReader();
                    if (data.HasRows)
                        return true;
                    command.Dispose();
                    data.Close();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[Banimento.GetBanStatus] Erro fatal!");
            }
            return false;
        }
    }
}
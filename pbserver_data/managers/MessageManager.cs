using Core.Logs;
using Core.models.account;
using Core.models.enums;
using Core.server;
using Core.sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Core.managers
{
    public static class MessageManager
    {
        public static Message getMessage(int id, long pId)
        {
            Message msg = null;
            if (pId == 0 || id == 0)
                return null;
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@owner", pId);
                    command.CommandText = "SELECT * FROM player_messages WHERE id=@id AND owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        msg = new Message(data.GetInt64(8), today)
                        {
                            id = id,
                            sender_id = data.GetInt64(2),
                            clanId = data.GetInt32(3),
                            sender_name = data.GetString(4),
                            text = data.GetString(5),
                            type = data.GetInt32(6),
                            state = data.GetInt32(7),
                            cB = (NoteMessageClan)data.GetInt32(9)
                        };
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
                Printf.b_danger("[MessageManager] Fatal Error!");
                return null;
            }
            return msg;
        }
        public static List<Message> getGifts(long owner_id)
        {
            List<Message> gifts = new List<Message>();
            if (owner_id == 0)
                return gifts;
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        int type = data.GetInt32(6);
                        if (type != 2)
                            continue;
                        Message msg = new Message(data.GetInt64(8), today)
                        {
                            id = data.GetInt32(0),
                            sender_id = data.GetInt64(2),
                            clanId = data.GetInt32(3),
                            sender_name = data.GetString(4),
                            text = data.GetString(5),
                            type = type,
                            state = data.GetInt32(7),
                            cB = (NoteMessageClan)data.GetInt32(9),
                        };
                        gifts.Add(msg);
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
                Printf.b_danger("[MessageManager] Fatal Error!");
            }
            return gifts;
        }
        public static List<Message> getMessages(long owner_id)
        {
            List<Message> msgs = new List<Message>();
            if (owner_id == 0)
                return msgs;
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    NpgsqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        int type = data.GetInt32(6);
                        if (type == 2)
                            continue;
                        Message msg = new Message(data.GetInt64(8), today)
                        {
                            id = data.GetInt32(0),
                            sender_id = data.GetInt64(2),
                            clanId = data.GetInt32(3),
                            sender_name = data.GetString(4),
                            text = data.GetString(5),
                            type = type,
                            state = data.GetInt32(7),
                            cB = (NoteMessageClan)data.GetInt32(9),
                        };
                        msgs.Add(msg);
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
                Printf.b_danger("[MessageManager] Fatal Error!");
            }
            return msgs;
        }
        public static int getMsgsCount(long owner_id)
        {
            int msgs = 0;
            if (owner_id == 0)
                return msgs;
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT COUNT(*) FROM player_messages WHERE owner_id=@owner";
                    msgs = Convert.ToInt32(command.ExecuteScalar());
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[MessageManager] Fatal Error!");
            }
            return msgs;
        }
        /// <summary>
        /// Cria uma mensagem na Database, e após a criação é adicionado o número do objeto ao modelo.
        /// </summary>
        /// <param name="msg">Mensagem</param>
        /// <returns></returns>
        public static bool CreateMessage(long owner_id, Message msg)
        {
            try
            {
                using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                {
                    NpgsqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.Parameters.AddWithValue("@sendid", msg.sender_id);
                    command.Parameters.AddWithValue("@clan", msg.clanId);
                    command.Parameters.AddWithValue("@sendname", msg.sender_name);
                    command.Parameters.AddWithValue("@text", msg.text);
                    command.Parameters.AddWithValue("@type", msg.type);
                    command.Parameters.AddWithValue("@state", msg.state);
                    command.Parameters.AddWithValue("@expire", msg.expireDate);
                    command.Parameters.AddWithValue("@cb", (int)msg.cB);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO player_messages(owner_id,sender_id,clan_id,sender_name,text,type,state,expire,cb)VALUES(@owner,@sendid,@clan,@sendname,@text,@type,@state,@expire,@cb) RETURNING id";
                    object data = command.ExecuteScalar();
                    msg.id = (int)data;
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                    return true;
                }
            }
            catch { return false; }
        }
        public static bool DeleteMessage(int id, long owner)
        {
            return ComDiv.deleteDB("player_messages", "id", id, "owner_id", owner);
        }
        public static bool DeleteMessages(List<object> objs, long owner)
        {
            if (owner == 0 || objs.Count == 0)
                return false;
            return ComDiv.deleteDB("player_messages", "id", objs.ToArray(), "owner_id", owner);
        }
        public static void RecicleMessages(long owner_id, List<Message> msgs)
        {
            List<object> objs = new List<object>();
            for (int i = 0; i < msgs.Count; i++)
            {
                Message msg = msgs[i];
                if (msg.DaysRemaining == 0)
                {
                    objs.Add(msg.id);
                    msgs.RemoveAt(i--);
                }
            }
            MessageManager.DeleteMessages(objs, owner_id);
        }
    }
}
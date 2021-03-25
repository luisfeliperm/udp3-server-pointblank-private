using Core.Logs;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Auth
{
    public class LoginManager
    {
        public static Socket mainSocket;
        public static ConcurrentDictionary<uint, LoginClient> _socketList = new ConcurrentDictionary<uint, LoginClient>();
        public static bool Start()
        {
            try{
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Local = new IPEndPoint(IPAddress.Parse(ConfigGA.authIp), 39190);
                mainSocket.Bind(Local);
                mainSocket.Listen(10);
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                return true;
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[LoginManager.Start] Erro fatal!");
                return false;
            }
        }
        private static void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;
            try
            {
                Socket handler = clientSocket.EndAccept(result);
                if (handler != null)
                {
                    LoginClient client = new LoginClient(handler);
                    AddSocket(client);
                    if (client == null)
                        Printf.warning("LoginClient destruído após falha ao adicionar na lista.");
                    Thread.Sleep(5);
                }
            }
            catch(Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[LoginManager.AcceptCallback] [Failed a LC connection] Erro fatal!");
            }
            mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
        }
        public static void AddSocket(LoginClient sck)
        {
            if (sck == null)  return;

            for (uint i = 1; i < 100000; i++)
            {
                if (!_socketList.ContainsKey(i) && _socketList.TryAdd(i, sck))
                {
                    sck.SessionId = i;
                    sck.Start();
                    return;
                }
            }
            sck.Close(true,500);
        }
        public static bool RemoveSocket(LoginClient sck)
        {
            if (sck == null || sck.SessionId == 0) return false;
            if (_socketList.ContainsKey(sck.SessionId) && _socketList.TryGetValue(sck.SessionId, out sck))
            {
                return _socketList.TryRemove(sck.SessionId, out sck);
            }
            return false;
        }
    }
}
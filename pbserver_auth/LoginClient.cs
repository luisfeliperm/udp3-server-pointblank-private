using Auth.data.model;
using Auth.data.sync;
using Auth.data.sync.server_side;
using Auth.global;
using Auth.global.clientpacket;
using Auth.global.serverpacket;
using Core.Firewall;
using Core.Logs;
using Core.server;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Auth
{
    public class LoginClient : IDisposable
    {
        public Socket _client;
        public Account _player;
        public DateTime ConnectDate;
        sbyte LoginAttemps = 0;
        public uint SessionId;
        public ushort SessionSeed, CountReceivedpackets;
        public int Shift, NextSessionSeed;
        bool disposed = false, closed = false, receiveFirstPacket = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            _player = null;
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            if (disposing)
                handle.Dispose();
            disposed = true;
        }
        public LoginClient(Socket client)
        {
            _client = client;
            _client.NoDelay = true;
        }
        public void Start()
        {
            SessionSeed = (ushort)new Random().Next(0, 0x7FFF);
            NextSessionSeed = (int)SessionSeed;
            Shift = (int)(SessionId % 7 + 1);
            new Thread(init).Start(); // Envia BASE_SERVER_LIST
            new Thread(read).Start();
            new Thread(ConnectionCheck).Start();
            ConnectDate = DateTime.Now;
        }
        private void ConnectionCheck()
        {
            Thread.Sleep(10000);

            if (_client != null && !receiveFirstPacket)
            {
                SaveLog.warning("[Closed Connection] Tempo de resposta excedido " + GetIPAddress(true));
                Close(true);
            }
        }
        public string GetIPAddress(bool port = false)
        {
            string r = "";
            if (_client != null && _client.RemoteEndPoint != null)
            {
                IPEndPoint remoteAddr = _client.RemoteEndPoint as IPEndPoint;
                r = remoteAddr.Address.ToString();
                if (port)
                {
                    r += ":"+ remoteAddr.Port.ToString();
                }
            }
            return r;
        }
        public IPAddress GetAddress()
        {
            if (_client != null && _client.RemoteEndPoint != null)
                return ((IPEndPoint)_client.RemoteEndPoint).Address;
            return null;
        }
        public void init()
        {
            SendPacket(new BASE_SERVER_LIST_PAK(this));
        }
        public void SendCompletePacket(byte[] data)
        {
            try
            {
                if (data.Length < 4)
                    return;
                if (ConfigGA.debugMode)
                {
                    ushort opcode = BitConverter.ToUInt16(data, 2);
                    string debugData = "";
                    foreach (string str2 in BitConverter.ToString(data).Split('-', ',', '.', ':', '\t'))
                        debugData += " " + str2;
                    Printf.warning("[" + opcode + "]" + debugData);
                }
                _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
            }
            catch
            {               
                Close(true);
            }
        }
        public void SendPacket(byte[] data)
        {
            try
            {
                if (data.Length < 2)
                    return;
                ushort size = Convert.ToUInt16(data.Length - 2);
                List<byte> list = new List<byte>(data.Length + 2);
                list.AddRange(BitConverter.GetBytes(size));
                list.AddRange(data);
                byte[] result = list.ToArray();
                if (ConfigGA.debugMode)
                {
                    ushort opcode = BitConverter.ToUInt16(data, 0);
                    string debugData = "";
                    foreach (string str2 in BitConverter.ToString(result).Split('-', ',', '.', ':', '\t'))
                        debugData += " " + str2;
                    Printf.warning("[" + opcode + "]" + debugData);
                }
                if (result.Length > 0)
                    _client.BeginSend(result, 0, result.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
                list.Clear();
            }
            catch{
                Close(true);
            }
        }
        public void SendPacket(SendPacket bp)
        {
            try
            {
                using (bp)
                {
                    bp.write();
                    byte[] data = bp.mstream.ToArray();
                    if (data.Length < 2)
                        return;
                    ushort size = Convert.ToUInt16(data.Length - 2);
                    List<byte> list = new List<byte>(data.Length + 2);
                    list.AddRange(BitConverter.GetBytes(size));
                    list.AddRange(data);
                    byte[] result = list.ToArray();
                    if (ConfigGA.debugMode)
                    {
                        ushort opcode = BitConverter.ToUInt16(data, 0);
                        string debugData = "";
                        foreach (string str2 in BitConverter.ToString(result).Split('-', ',', '.', ':', '\t'))
                            debugData += " " + str2;
                        Printf.warning("[" + opcode + "]" + debugData);
                    }
                    if (result.Length > 0)
                        _client.BeginSend(result, 0, result.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
                    bp.mstream.Close();
                    list.Clear();
                }
            }
            catch{
                Close(true);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                if (handler != null && handler.Connected)
                    handler.EndSend(ar);
            }
            catch
            {
                Close(true);
            }
        }
        /// <summary>
        /// Aguarda a chegada de um novo pacote. É necessário após a leitura de um pacote.
        /// </summary>
        private void read()
        {
            try
            {
                limitePackets();
                if (closed) {
                    Printf.warning(GetIPAddress(true) + " fechado thread de leitura! ");
                    return;
                }

                StateObject state = new StateObject();
                state.workSocket = _client;
                _client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(OnReceiveCallback), state);
            }
            catch
            {
                Close(true);
            }
        }
        
        private class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 8096;
            public byte[] buffer = new byte[BufferSize];
        }
        /// <summary>
        /// Fecha a conexão do cliente.
        /// </summary>
        /// <param name="destroyConn">Destruir conexão?</param>
        /// <param name="time">Tempo de espera para cancelar o recebimento de pacotes (milissegundos)</param>
        public void Close(bool destroyConnection, int time = 0)
        {
            if (closed)
                return;

            try
            {
                closed = true;
                LoginManager.RemoveSocket(this);
                Account player = _player;
                if (destroyConnection)
                {
                    if (player != null)
                    {
                        player.setOnlineStatus(false);
                        if (player._status.serverId == 0)
                            SEND_REFRESH_ACC.RefreshAccount(player, false);
                        player._status.ResetData(player.player_id);
                        player.SimpleClear();
                        player.updateCacheInfo();
                        _player = null;
                    }
                    _client.Close(time);
                    Thread.Sleep(time);
                    Dispose();
                }
                else if (player != null)
                {
                    player.SimpleClear();
                    player.updateCacheInfo();
                    _player = null;
                }
                Auth_SyncNet.UpdateAuthCount(0);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[LoginClient.Close] Erro fatal!");
            }
        }
        private void OnReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            try
            {
                int bytesCount = state.workSocket.EndReceive(ar);
                if (bytesCount < 1) return;

                byte[] babyBuffer = new byte[bytesCount];
                Array.Copy(state.buffer, 0, babyBuffer, 0, bytesCount);

                int FirstLength = BitConverter.ToUInt16(babyBuffer, 0) & 0x7FFF;

                byte[] buffer = new byte[FirstLength + 2];
                Array.Copy(babyBuffer, 2, buffer, 0, buffer.Length);

                if (buffer.Length < 4 || buffer.Length > 200)
                {
                    SaveLog.warning(GetIPAddress() + " Pacote com tamanho suspeito [" + buffer.Length+ "]");
                    Printf.warning(GetIPAddress() + " Pacote com tamanho suspeito [" + buffer.Length + "]");
                }

                byte[] decrypted = ComDiv.decrypt(buffer, Shift);

                if (!CheckSeed(decrypted, true))
                {
                    Close(true);
                }
                else
                {
                    RunPacket(decrypted); // Joga o packet decriptado pro case e toma a ação do opcode
                    checkoutN(babyBuffer, FirstLength);
                    new Thread(read).Start(); // Reinicia a thread de leitura
                }
            }
            catch
            {
                Close(true);
            }
        }
        public bool CheckSeed(byte[] decryptedData, bool isTheFirstPacket)
        {
            int hashReceive = (int)BitConverter.ToUInt16(decryptedData, 2);
            if (hashReceive == GetNextSessionSeed())
                return true;
            
            if (_player != null)
            {
                SaveLog.warning("[Seed] Seed invalida recebida após login. [Recebido: " + hashReceive + "; Esperava-se: " + NextSessionSeed +"] [Id: " + _player.player_id + "; Login: " + this._player.login + "; TotalPks: " + CountReceivedpackets+"] - "+GetIPAddress());
                Printf.warning("[Invalid Seed] Client valido enviou seed invalida [pId:" + _player.player_id+"; Login: "+_player.login+"] ");
            }
            else
            {
                SaveLog.warning("[Seed] Recebido: " + hashReceive + "; Esperava-se: " + NextSessionSeed + " - " + GetIPAddress() + "");
                Printf.warning("[Invalid Seed] Receive: " + hashReceive + " - "+GetIPAddress());
            }

            Firewall.sendBlock(GetIPAddress(), "[Auth] Seed invalida", 1);
            Close(true);

            if (isTheFirstPacket)
                new Thread(read).Start();
            return false;
        }

        private int GetNextSessionSeed()
        {
            NextSessionSeed = (int)(ushort)(NextSessionSeed * 214013 + 2531011 >> 16 & 0x7FFF);
            return NextSessionSeed;
        }


        public void checkoutN(byte[] buffer, int FirstLength)
        {
            int tamanho = buffer.Length;
            try
            {
                byte[] newPacketENC = new byte[tamanho - FirstLength - 4];
                Array.Copy(buffer, FirstLength + 4, newPacketENC, 0, newPacketENC.Length);
                if (newPacketENC.Length == 0)
                    return;

                int lengthPK = BitConverter.ToUInt16(newPacketENC, 0) & 0x7FFF;

                byte[] newPacketENC2 = new byte[lengthPK + 2];
                Array.Copy(newPacketENC, 2, newPacketENC2, 0, newPacketENC2.Length);


                byte[] newPacketGO = new byte[lengthPK + 2];

                Array.Copy(ComDiv.decrypt(newPacketENC2, Shift), 0, newPacketGO, 0, newPacketGO.Length);

                if (!CheckSeed(newPacketGO, false))
                    return;

                RunPacket(newPacketGO);
                checkoutN(newPacketENC, lengthPK);
            }
            catch{}
        }
        private bool FirstPacketCheck(ushort packetId)
        {
            bool r = true;
            if (packetId == 2561 || packetId == 2672) // Primeiro pacote valido
            {
                receiveFirstPacket = true;
            }
            else
            {
                r = false;
            }
            return r;
        }
        private void limitePackets()
        {
            CountReceivedpackets++;
            if (CountReceivedpackets > 200) 
            {
                string msg = GetIPAddress() + " Limite de pacotes por sessao atingido [" + CountReceivedpackets + "]";
                SaveLog.warning(msg);
                Firewall.sendBlock(GetIPAddress(), msg, 2);
                Close(true);
            }
        }
        private void RunPacket(byte[] buff)
        {
            
            // if (closed) return; // Mudado para o metodo "read"

            UInt16 opcode = BitConverter.ToUInt16(buff, 0);

            // Debug Receive
            
            /*
            {
                string debugData = "";
                foreach (string str2 in BitConverter.ToString(buff).Split('-', ',', '.', ':', '\t'))
                    debugData += " " + str2;
                Printf.warning("Receive [" + opcode + "]" + debugData);
            }
            */
            

            if (!receiveFirstPacket) // Nao recebeu ainda
            {
                if (!FirstPacketCheck(opcode) && opcode != 0)
                {
                    string msg = "Primeiro pacote nao recebido, conexao finalizada. [" + opcode + "]";
                    Printf.warning(msg);
                    SaveLog.warning(GetIPAddress() + " "+msg);
                    Firewall.sendBlock(GetIPAddress(), msg, 1);
                    Close(true);
                    return;
                }
            }
            
            
            ReceiveLoginPacket packet = null;
            switch (opcode)
            {
                case 528:
                    packet = new BASE_USER_GIFTLIST_REC(this, buff);
                    break;
                case 2561:
                case 2563:
                    LoginAttemps++;
                    if (LoginAttemps > 10)
                    {
                        string msg = GetIPAddress() + " Enviou o login mais de 10 vezes";
                        Printf.warning(msg);
                        SaveLog.warning(msg);
                        Firewall.sendBlock(GetIPAddress(), msg, 2);
                        Close(true);
                        return;
                    }
                    else
                    {
                        packet = new BASE_LOGIN_REC(this, buff);
                        break;
                    }
                    
                case 2565:
                    packet = new BASE_USER_INFO_REC(this, buff);
                    break;
                case 2666:
                    Printf.blue("A_2666_REC");
                    packet = new A_2666_REC(this, buff);
                    break;
                case 2672: // AutoLogin
                    Printf.info("BASE_LOGIN_THAI_REC");
                    //packet = new BASE_LOGIN_THAI_REC(this, buff);
                    return;
                    break;
                case 2567:
                    packet = new BASE_USER_CONFIGS_REC(this, buff);
                    break;
                case 2575: // [2575] 0F 0A 1E 4C
                    //packet = new UPDATE_GAMESERVERS_REC(this, buff);
                    return;
                case 2577:
                    packet = new BASE_SERVER_CHANGE_REC(this, buff); // Ultimo pacote?
                    break;
                case 2579:
                    packet = new BASE_USER_ENTER_REC(this, buff);
                    break;
                case 2581:
                    packet = new BASE_CONFIG_SAVE_REC(this, buff);
                    break;
                case 2642:
                    packet = new BASE_SERVER_LIST_REFRESH_REC(this, buff);
                    break;
                case 2654:
                    packet = new BASE_USER_EXIT_REC(this, buff);
                    break;
                case 2668: // INCOMPLETO
                    Printf.info("BASE_FREE_ITEMS_OF_LEVELUP_REC");
                    // packet = new BASE_FREE_ITEMS_OF_LEVELUP_REC(this, buff);
                    return;
                case 2678:
                    packet = new A_2678_REC(this, buff);
                    break;
                case 2698:
                    packet = new BASE_USER_INVENTORY_REC(this, buff);
                    break;

                case 0:
                    packet = new DEV_SERVER_DETAILS_REQ(this, buff);
                    break;

                default:
                    {
                        string msg = "[" + opcode + "] Opcode nao encontrado " + GetIPAddress(true);
                        Firewall.sendBlock(GetIPAddress(), msg, 1);
                        Printf.warning(msg);
                        SaveLog.warning(msg);
                        Close(true);
                        return;
                    }
            }
            new Thread(packet.run).Start();
        }
    }
}
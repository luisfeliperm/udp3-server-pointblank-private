using Auth.data;
using Auth.data.managers;
using Auth.data.model;
using Auth.data.sync;
using Auth.data.sync.server_side;
using Auth.global.serverpacket;
using Core.Firewall;
using Core.Logs;
using Core.managers;
using Core.managers.server;
using Core.models.enums.errors;
using Core.models.enums.global;
using Core.server;
using Core.Network;
using System;
using System.Net;
using System.Net.NetworkInformation;


namespace Auth.global.clientpacket
{
    public class BASE_LOGIN_REC : ReceiveLoginPacket
    {
        private string login, password, UserFileListMD5, d3d9MD5, GameVersion, PublicIP;
        private byte loginSize, passSize, connection;
        private ulong key;
        private ClientLocale GameLocale;
        private PhysicalAddress MacAddress;
        private IPAddress LocalIP;

        public BASE_LOGIN_REC(LoginClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            string readALl = readA();

            GameVersion = readC() + "." + readH() + "." + readH();
            GameLocale = (ClientLocale)readC();
            loginSize = readC();
            passSize = readC();
            login = readS(loginSize);

            password = EncryptPass.Get(readS(passSize));

            MacAddress = new PhysicalAddress(readB(6));

            readH();
            connection = readC();

            LocalIP = new IPAddress(readB(4));

            key = readUQ();
            UserFileListMD5 = readS(32);
            readS(16);
            d3d9MD5 = readS(32); // C:\Windows\SysWOW64\d3d9.dll ?
            PublicIP = _client.GetIPAddress();
        }

        public override void run()
        {
            try
            {
                /*
                switch (login)
                {
                    case "1":
                        // ID incorreta ou inexistente (Fecha Client)
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.LOGIN_EVENT_ERROR, login, 0));
                        break;
                    case "2":
                        // Este usuario esta conectado, tente conectar novamente mais tarde (Fecha)
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ALREADY_LOGIN_WEB, login, 0));
                        break;
                    case "3":
                        // Usuario ou senha incorreta
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_USER_PASS_FAIL, login, 0));
                        break;
                    case "4":
                        // A conexão foi finalizada
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_LOGOUTING, login, 0));
                        break;
                    case "5":
                        // Tempo para tentativa de conexão com o servidor excedido, tente mais tarde
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_TIME_OUT_1, login, 0));
                        break;
                    case "6":
                        // Falha ao logar, tente conectar mais tarde
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_TIME_OUT_2, login, 0));
                        break;
                    case "7":
                        // Sua conta está bloqueada
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_ACCOUNT, login, 0));
                        break;
                    case "8":
                        // Devido ao excesso de usuarios no servidor, não foi possivel conectar
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_SERVER_USER_FULL, login, 0));
                        break;
                    case "9":
                        // Falha na conexão com o servidor, tente mais tarde
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_DB_BUFFER_FAIL, login, 0));
                        break;
                    case "10":
                        // ID ou senha incorretos
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT, login, 0));
                        break;
                    case "11":
                        // ID ou senha incorretos
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT2, login, 0));
                        break;
                    case "12":
                        // Falha na autenticação do email
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_EMAIL_AUTH_ERROR, login, 0));
                        break;
                    case "13":
                        // IP Bloqueado
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_IP, login, 0));
                        break;
                    case "14":
                        // STBL_IDX_EP_LOGIN_OK_EMAIL_ALERT_2
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_EMAIL_ALERT2, login, 0));
                        break;
                    case "15":
                        // STR_POPUP_MESSAGE_MIGRATION
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_MIGRATION, login, 0));
                        break;
                    case "16":
                        // ID incorreta ou inexistente (Fecha)
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_NON_STRING, login, 0));
                        break;
                    case "17":
                        // Login bloqueado devido a região
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_COUNTRY, login, 0));
                        break;
                    case "18":
                        // Usuario ou senha incorreta
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVALID_ACCOUNT, login, 0));
                        break;
                    case "19":
                        // VAZIO (Fecha)
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_PC_BLOCK, login, 0));
                        break;
                    case "20":
                        // Falha ao listar inventario, o jogo será fechado (Fecha)
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVENTORY_FAIL, login, 0));
                        break;
                    case "21":
                        // Sua conta está bloqueada
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_INNER, login, 0));
                        break;
                    case "22":
                        // Sua conta está bloqueada
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_OUTER, login, 0));
                        break;
                    case "23":
                        // Sua conta está bloqueada
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_GAME, login, 0));
                        break;

                }

                return;
                */



                string Requisto_msg = "";
                EventErrorEnum erro = 0;
                bool block = false;

                
                if (ConfigGA.LauncherKey != 0 && key != ConfigGA.LauncherKey)
                {
                    Requisto_msg = "Chave: " + key + " não compatível [" + login + "]";
                    block = true;
                }
                else if (!ConfigGA.GameLocales.Contains(GameLocale))
                {
                    erro = EventErrorEnum.Login_BLOCK_COUNTRY;
                    Requisto_msg = "País: " + GameLocale + " do cliente bloqueado [" + login + "]";
                }
                else if (GameVersion != ServerConfig.ClientVersion)
                {
                    Requisto_msg = "Versão: " + GameVersion + " não compatível [" + login + "]";
                }
                
                else if (ConfigGA.UserFileList != "0" && UserFileListMD5.ToLower() != ConfigGA.UserFileList.ToLower())
                {
                    Requisto_msg = "UserFileList: " + UserFileListMD5 + " não compatível [" + login + "]";
                }
                else if (ConfigGA.ValidIP && 
                        (
                        !LocalIP.checkInSubNet(IPAddress.Parse("192.168.0.0"), 16) &&
                        !LocalIP.checkInSubNet(IPAddress.Parse("10.0.0.0"), 8) &&
                        !LocalIP.checkInSubNet(IPAddress.Parse("172.16.0.0"), 12)
                        )
                    )
                {
                    erro = EventErrorEnum.Login_BLOCK_IP;
                    Requisto_msg = "IP local inválido. {" + LocalIP + "} [" + login + "]";
                    block = true;
                }
                else if (!DirectxMD5.IsValid(d3d9MD5.ToLower()))
                {
                    Requisto_msg = "d3d9MD5 nao listada: " + d3d9MD5 + " [" + login + "]";
                    block = true;
                }




                if (!allowBot(login, password) && !ConfigGA.isTestMode)
                {
                    if (Requisto_msg != "")
                    {
                        Requisto_msg = "[" + PublicIP + "] " + Requisto_msg;

                        if(block)
                            Firewall.sendBlock(PublicIP, Requisto_msg, 2);

                        if (erro == 0 || ConfigGA.modeAtack)
                        {
                            _client.Close(true);
                        }
                        else
                        {
                            _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.LOGIN_EVENT_ERROR, login, 0));
                            _client.Close(false);
                        }

                        if (!ConfigGA.modeAtack)
                            SaveLog.LogLoginFaill(Requisto_msg);

                        Printf.info(Requisto_msg);

                        return;
                    }
                    if (passSize > ConfigGA.maxPassSize || passSize < ConfigGA.minPassSize)
                    {
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVALID_ACCOUNT, login, 0));
                        SaveLog.LogLoginFaill("LOGIN - Tamanho de senha invalido " + passSize + " [" + login + "]");
                        return;
                    }
                    if (loginSize > ConfigGA.maxLoginSize || loginSize < ConfigGA.minLoginSize)
                    {
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVALID_ACCOUNT, login, 0));
                        SaveLog.LogLoginFaill("LOGIN - Tamanho de login invalido " + loginSize + " [" + login + "]");
                        return;
                    }
                }

                // Verifica se o MAC ou IP tá bloqueado
                if (Banimento.GetBanStatus(MacAddress))
                {
                    Printf.info("Usuario: "+login+" tentou logar com MAC bloqueado");
                    SaveLog.LogLoginFaill("MAC " + MacAddress.ToString() + " banido [" + login + "]");
                    _client.SendPacket((SendPacket)new BASE_LOGIN_PAK(EventErrorEnum.Login_PC_BLOCK, login, 0));
                    _client.Close(false);
                    return;
                }

                _client._player = AccountManager.getInstance().getAccountDB(login, null, 0, 0);
                if (_client._player == null)
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVALID_ACCOUNT, login, 0L));
                    _client.Close(false);
                    return;
                }
                Account p = _client._player;
                if (p == null)
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_INVALID_ACCOUNT, login, 0));
                    SaveLog.LogLoginFaill("Conta retornada da DB é nula [" + login + "]");
                    _client.Close(false);
                    return;
                }
                if (!ConfigGA.isTestMode && !p.password.Equals(password))
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT2, login, 0));
                    SaveLog.LogLoginFaill("Senha inválida [" + login + "]");
                    return;
                }
                if (p.access < 0)
                {
                    Printf.info("Usuario: " + login + " banido tentou logar");
                    SaveLog.LogLoginFaill("LOGIN - Banido permanente [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_ACCOUNT, login, 0));
                    _client.Close(false);
                    return;
                }


                if (ConfigGA.onlyGM && !p.IsGM())
                {
                    SaveLog.LogLoginFaill("LOGIN - Login cancelado, apenas GM! [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_TIME_OUT_2, login, 0));
                    _client.Close(false);
                    return;
                }

                Account pCache = AccountManager.getInstance().getAccount(p.player_id, true);
                if (p._isOnline) // Usuario já esta online
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ALREADY_LOGIN_WEB, login, 0));
                    SaveLog.LogLoginFaill("LOGIN - Conta online [" + login + "]");
                    if (pCache != null && pCache._connection != null)
                    {
                        pCache.SendPacket(new AUTH_ACCOUNT_KICK_PAK(1));
                        pCache.SendPacket(new SERVER_MESSAGE_ERROR_PAK(2147487744));
                        pCache.Close(1000);
                    }
                    else
                        Auth_SyncNet.SendLoginKickInfo(p);
                    _client.Close(false);
                    return;
                }

                if (Banimento.checkBan(p.player_id))
                {
                    SaveLog.LogLoginFaill("LOGIN - Conta com ban ativo [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_ACCOUNT, login, 0));
                    _client.Close(false);
                    return;
                }


                //  Registrar ACESSO
                ComDiv.updateDB("contas", "player_id", p.player_id, new string[] { "lastip", "last_mac", "last_login" }, PublicIP, MacAddress, long.Parse(DateTime.Now.ToString("yyMMddHHmm")) );

                // Faz login
                p.SetPlayerId(p.player_id, 31);
                p._clanPlayers = ClanManager.getClanPlayers(p.clan_id, p.player_id);
                _client.SendPacket(new BASE_LOGIN_PAK(0, login, p.player_id));
                _client.SendPacket(new AUTH_WEB_CASH_PAK(0));
                if (p.clan_id > 0)
                    _client.SendPacket(new BASE_USER_CLAN_MEMBERS_PAK(p._clanPlayers));
                p._status.SetData(4294967295, p.player_id);
                p._status.updateServer(0);
                p.setOnlineStatus(true);
                if (pCache != null)
                    pCache._connection = _client;
                SEND_REFRESH_ACC.RefreshAccount(p, true);

                if(ConfigGA.displayLogin)
                    Printf.sucess("[" + login + "] Autenticou-se " + PublicIP);

                SaveLog.info("[LOGIN] user:"+login+ "; "+ PublicIP + "; " + MacAddress);

                // Libera no firewall
                Firewall.sendAllow(PublicIP);
            }
            catch (Exception ex)
            {
                SaveLog.fatal(ex.ToString());
                Printf.b_danger("[BASE_LOGIN_REC.run] Erro fatal!");
            }
        }

        private static bool allowBot(string login, string pass)
        {
            if (login.StartsWith("bot_") && pass.Equals("fa27bf569200e39f2e77e4567ba65d47") )
                return true;
            return false;
        }
    }
}
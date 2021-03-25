using Core;
using Core.models.enums;
using System.Text;

namespace Game
{
    public static class ConfigGS
    {
        public static string passw, gameIp;
        public static bool isTestMode, debugMode, winCashPerBattle, showCashReceiveWarn, EnableClassicRules;
        public static SERVER_UDP_STATE udpType;
        public static float maxClanPoints;
        public static ushort serverId, gamePort, syncPort, minRankVote, maxNickSize, minNickSize, maxBattleXP,
            maxBattleGP, maxChannelPlayers, minCreateGold, minCreateRank, maxActiveClans, maxBattleMY, pcCafe1XP, pcCafe1GP, pcCafe2XP, pcCafe2GP;
        public static int maxRepeatLatency, maxBattleLatency ;
        public static void Load()
        {
            ConfigFile configFile = new ConfigFile("config/game.ini");
            ConfigGB.dbHost = configFile.readString("dbhost", "localhost");
            ConfigGB.dbName = configFile.readString("dbname", "");
            ConfigGB.dbUser = configFile.readString("dbuser", "root");
            ConfigGB.dbPass = configFile.readString("dbpass", "");
            ConfigGB.dbPort = configFile.readUInt16("dbport", 0);

            serverId = configFile.readUInt16("serverId", 1);
            gameIp = configFile.readString("gameIp","127.0.0.1");
            gamePort = configFile.readUInt16("gamePort", 39190);
            syncPort = configFile.readUInt16("syncPort", 0);
            debugMode = configFile.readBoolean("debugMode", false);
            isTestMode = configFile.readBoolean("isTestMode", false);
            ConfigGB.EncodeText = Encoding.GetEncoding(configFile.readInt32("EncodingPage", 1252));
            EnableClassicRules = configFile.readBoolean("EnableClassicRules", false);
            winCashPerBattle = configFile.readBoolean("winCashPerBattle", true);
            showCashReceiveWarn = configFile.readBoolean("showCashReceiveWarn", true);


            pcCafe1XP = configFile.readUInt16("PcCafe1_PercentXP", 100);
            pcCafe1GP = configFile.readUInt16("PcCafe1_PercentGP", 60);
            pcCafe2XP = configFile.readUInt16("PcCafe2_PercentXP", 200);
            pcCafe2GP = configFile.readUInt16("PcCafe2_PercentGP", 120);


            minCreateRank = configFile.readUInt16("minCreateRank", 15);
            minCreateGold = configFile.readUInt16("minCreateGold", 7500);
            maxClanPoints = configFile.readFloat("maxClanPoints", 0);

            passw = configFile.readString("passw", "123321");
            maxChannelPlayers = configFile.readUInt16("maxChannelPlayers", 100);
            maxBattleXP = configFile.readUInt16("maxBattleXP", 1000);
            maxBattleGP = configFile.readUInt16("maxBattleGP", 1000);
            maxBattleMY = configFile.readUInt16("maxBattleMY", 1000);
            udpType = (SERVER_UDP_STATE)configFile.readByte("udpType", 1);
            minNickSize = configFile.readUInt16("minNickSize", 4);
            maxNickSize = configFile.readUInt16("maxNickSize", 16);
            minRankVote = configFile.readUInt16("minRankVote", 0);
            maxActiveClans = configFile.readUInt16("maxActiveClans", 500);
            maxBattleLatency = configFile.readInt32("maxBattleLatency", 0);
            maxRepeatLatency = configFile.readInt32("maxRepeatLatency", 0);
        }
    }
}
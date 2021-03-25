using Core;
using Core.models.enums.global;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth
{
    public static class ConfigGA
    {
        public static string authIp, UserFileList;
        public static bool isTestMode, Outpost, debugMode, ValidIP, displayLogin, onlyGM, modeAtack;
        public static ushort syncPort, minNickSize, minLoginSize, minPassSize, maxNickSize, maxLoginSize, maxPassSize;
        public static ulong LauncherKey;
        public static List<ClientLocale> GameLocales;

        public static void Load()
        {
            ConfigFile configFile = new ConfigFile("config/auth.ini");
            ConfigGB.dbHost = configFile.readString("dbhost", "localhost");
            ConfigGB.dbName = configFile.readString("dbname", "");
            ConfigGB.dbUser = configFile.readString("dbuser", "root");
            ConfigGB.dbPass = configFile.readString("dbpass", "");
            ConfigGB.dbPort = configFile.readUInt16("dbport", 0);

            authIp = configFile.readString("authIp", "127.0.0.1");
            syncPort = configFile.readUInt16("syncPort", 0);
            debugMode = configFile.readBoolean("debugMode", false);
            modeAtack = configFile.readBoolean("modeAtack", false);
            isTestMode = configFile.readBoolean("isTestMode", false);
            ValidIP = configFile.readBoolean("ValidIP", true);
            onlyGM = configFile.readBoolean("onlyGM", true);
            displayLogin = configFile.readBoolean("displayLogin", true);
            ConfigGB.EncodeText = Encoding.GetEncoding(configFile.readInt32("EncodingPage", 0));
            Outpost = configFile.readBoolean("Outpost", false);
            LauncherKey = configFile.readUInt64("LauncherKey", 0);
            UserFileList = configFile.readString("UserFileList", "0");
            minNickSize = configFile.readUInt16("minNickSize", 0);
            maxNickSize = configFile.readUInt16("maxNickSize", 0);
            minLoginSize = configFile.readUInt16("minLoginSize", 0);
            minPassSize = configFile.readUInt16("minPassSize", 0);

            maxLoginSize = configFile.readUInt16("maxLoginSize", 16);
            maxPassSize = configFile.readUInt16("maxPassSize", 16);
            if (maxLoginSize > 16){maxLoginSize = 16;}
            if (maxPassSize > 16){maxPassSize = 16;}

            GameLocales = new List<ClientLocale>();
            string strLocales = configFile.readString("GameLocales", "None");
            foreach (string splitedLocale in strLocales.Split(','))
            {
                ClientLocale clientLocale;
                Enum.TryParse<ClientLocale>(splitedLocale, out clientLocale);
                GameLocales.Add(clientLocale);
            }
        }
    }
}
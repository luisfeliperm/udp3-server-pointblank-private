namespace Core.managers.server
{
    public static class ServerConfig
    {
        public static string ClientVersion, ExitURL;
        public static bool missions, GiftSystem;

        public static void Load()
        {
            /*
            ClientVersion = "1.15.42";
            ExitURL = "https://bloodipb.com/";
            missions = true;
            GiftSystem = true;
            */

            ConfigFile configFile = new ConfigFile("config/geral.ini");
            ClientVersion = configFile.readString("clientVersion", "1.15.42");
            ExitURL = configFile.readString("ExitURL", "https://facebook.com/uchihaker");
            missions = configFile.readBoolean("missions", true);
            GiftSystem = configFile.readBoolean("GiftSystem", true);
        }
    }

}
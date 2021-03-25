using Core;

namespace pbserver_firewall.conf
{
    class Config
    {
        public static uint battlePort, gamePort, authPort;
        public static int[] timeBlock = new int[3];
        public Config()
        {
            ConfigFile configFile = new ConfigFile("config/api-protection/firewall.ini");
            authPort = configFile.readUInt16("authPort", 39190);
            gamePort = configFile.readUInt16("gamePort", 39192);
            battlePort = configFile.readUInt16("battlePort", 40008);


            timeBlock[0] = configFile.readInt32("grave", 0);
            timeBlock[1] = configFile.readInt32("perigo", 120);
            timeBlock[2] = configFile.readInt32("aviso", 5);
        }

    }
}

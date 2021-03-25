using Core.Logs;
using pbserver_firewall.conf;

namespace pbserver_firewall.Uteis
{
    class Tools
    {
        public static int getGravit(int x) // Pega gravidade da regra
        {
            switch (x)
            {
                case 0:
                    x = Config.timeBlock[0];
                    break;
                case 1:
                    x = Config.timeBlock[1];
                    break;
                case 2:
                    x = Config.timeBlock[2];
                    break;
                default:
                    x = Config.timeBlock[0];
                    Printf.info("[Warning] Nivel de gravidade invalido, foi setado para _GRAVE_");
                    break;
            }
            return x;
        }
    }
}

using Game.data.managers;
using System;
using System.Threading.Tasks;

namespace Game
{
    public static class cpuMonitor
    {
        public static int TestSlot = 1;
        public static async void updateRAM()
        {
            while (true)
            {
                Console.Title = "Point Blank - Game [Server: " + ConfigGS.serverId + "; Users: " + GameManager._socketList.Count + "; Loaded accs: " + AccountManager._contas.Count + "; RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                await Task.Delay(1000);
            }
        }
    }
}
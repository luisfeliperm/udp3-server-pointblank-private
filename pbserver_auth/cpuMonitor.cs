using Auth.data.managers;
using System;
using System.Threading.Tasks;

namespace Auth
{
    public class cpuMonitor
    {
        public static async void updateRAM2()
        {
            while (true)
            {
                Console.Title = "PointBlank - Auth [Users: " + LoginManager._socketList.Count + "; Loaded accs: " + AccountManager.getInstance()._contas.Count + "; RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                await Task.Delay(1000);
            }
        }
    }
}
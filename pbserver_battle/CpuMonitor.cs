using System;
using System.Threading.Tasks;

namespace Battle
{
    class CpuMonitor
    {
        public static async void Start()
        {
            while (true)
            {
                Console.Title = "Point Blank - Battle [RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                await Task.Delay(1000);
            }
        }
    }
}

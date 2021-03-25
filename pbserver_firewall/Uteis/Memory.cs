using pbserver_firewall.Rules;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace pbserver_firewall
{
    class Memory
    {
        public static ushort blockPerm = 0;
        public static async void updateRAM()
        {
            try
            {
                PerformanceCounter myAppCpu =
                new PerformanceCounter(
                    "Process", "% Processor Time", "pbserver_firewall", true);


                while (true)
                {
                    double pct = myAppCpu.NextValue();
                    Console.Title = "API Protection [Permit: "+Add_Allow_Rule.allowed.Count+"; Blockeds Temp:"+Monitoring.unlockQueue.Count + " Etern: "+ blockPerm + " || CPU: " + pct.ToString("n") + "% RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}

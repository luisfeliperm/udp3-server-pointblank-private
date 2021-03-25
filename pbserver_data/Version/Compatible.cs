using Core.Logs;
namespace Core.Version
{
    public class Compatible
    {
        private static string version = "4.0.0";
        public static bool Check(string appversion, string app)
        {
            if (appversion != version)
            {
                Printf.danger("Versões incompátiveis! Core v."+ version + " "+app+" v."+ appversion);
                return false;
            }
            return true;
        }
    }
}

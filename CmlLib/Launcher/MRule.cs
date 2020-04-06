using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public class MRule
    {
        static MRule()
        {
            OSName = getOSName();

            if (Environment.Is64BitOperatingSystem)
                Arch = "64";
            else
                Arch = "32";
        }

        public static string OSName { get; private set; }
        public static string Arch { get; private set; }

        private static string getOSName()
        {
            var osType = Environment.OSVersion.Platform;

            if (osType == PlatformID.MacOSX)
                return "osx";
            else if (osType == PlatformID.Unix)
                return "linux";
            else
                return "windows";
        }

        public bool CheckOSRequire(JArray arr)
        {
            var require = true;

            foreach (JObject job in arr)
            {
                var action = true; // true : "allow", false : "disallow"
                var containCurrentOS = true; // if 'os' JArray contains current os name

                foreach (var item in job)
                {
                    // action
                    if (item.Key == "action")
                        action = (item.Value.ToString() == "allow" ? true : false);

                    // os (containCurrentOS)
                    else if (item.Key == "os")
                        containCurrentOS = checkOSContains((JObject)item.Value);

                    else if (item.Key == "features") // etc
                        return false;
                }

                if (!action && containCurrentOS)
                    require = false;
                else if (action && containCurrentOS)
                    require = true;
                else if (action && !containCurrentOS)
                    require = false;
            }

            return require;
        }

        static bool checkOSContains(JObject job)
        {
            foreach (var os in job)
            {
                if (os.Key == "name" && os.Value.ToString() == OSName)
                    return true;
            }
            return false;
        }
    }
}

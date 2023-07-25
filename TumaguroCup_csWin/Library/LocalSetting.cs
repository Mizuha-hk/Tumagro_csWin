using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TumaguroCup_csWin.Library
{
    internal class LocalSetting
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static void RegistorApiKey(string apiKey)
        {
            localSettings.Values["apiKey"] = apiKey;
        }

        public static string LoadApiKey()
        {
            return localSettings.Values["apiKey"] as string;
        }
    }
}

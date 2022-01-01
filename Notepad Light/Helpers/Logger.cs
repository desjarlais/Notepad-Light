using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad_Light.Helpers
{
    class Logger
    {
        public static void Log(string logValue)
        {
            Properties.Settings.Default.ErrorLog.Add(DateTime.Now + Strings.semiColon + logValue);
            Properties.Settings.Default.Save();
        }

        public static void Clear()
        {
            Properties.Settings.Default.ErrorLog.Clear();
            Properties.Settings.Default.Save();
        }
    }
}

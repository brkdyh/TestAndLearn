using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class DebugLogger
    {
        public static void Log(string log)
        {
            Console.WriteLine(log);
        }

        public static void Log(string log,ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Log(log);
            Console.ResetColor();
        }
    }
}

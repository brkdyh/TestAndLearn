using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSConsole
{
    /// <summary>
    /// Mian Loop
    /// </summary>
    class MainLoop
    {
        static bool Exit = false;

        static void Main(string[] args)
        {
            while (!Exit)
            {
                Thread.Sleep(100);
                ProjectEnter.Instance().Update();
            }
        }
    }
}

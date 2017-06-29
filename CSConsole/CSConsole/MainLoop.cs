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
        static int FrameCount = 5;

        static bool Exit = false;

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            LogicEnter.Instance().Awake();

            LogicEnter.Instance().Start();

            while (!Exit)
            {
                LogicEnter.Instance().Update();

                LogicEnter.Instance().LateUpdate();

                Thread.Sleep((1000 / FrameCount));

                Time.CalDeltaTime();
            }
        }
    }
}

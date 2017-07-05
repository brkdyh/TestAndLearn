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
        static int TargetFrameCount = 10;

        static bool Exit = false;

        public static double WaitForUpdateTime
        {
            private set;
            get;
        }

        public static double UpdateTime
        {
            private set;
            get;
        }

        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            LogicEnter.Instance().Init();

            LogicEnter.Instance().Awake();

            LogicEnter.Instance().Start();

            while (!Exit)
            {
                ulong uSample = TimeSample.BeginSample();

                LogicEnter.Instance().Update();

                LogicEnter.Instance().LateUpdate();

                TimeSpan t = TimeSample.EndSample(uSample);

                WaitForUpdate(t);

                Time.CalDeltaTime();
            }

            Console.Read();
        }

        static void WaitForUpdate(TimeSpan t)
        {
            double dT = t.TotalMilliseconds;

            if (dT == 0) dT = 0.001d;

            UpdateTime = dT;

            WaitForUpdateTime = ((1000d / TargetFrameCount) - dT);

            if (WaitForUpdateTime > 0)
            {
                Thread.Sleep((int)WaitForUpdateTime + 1);
            }
        }
    }
}

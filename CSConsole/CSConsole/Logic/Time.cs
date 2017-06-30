using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class Time
    {
        

        static DateTime FrontFrameTime = DateTime.Now;

        public static double DeltaTime
        {
            private set;
            get;
        }

        public static void CalDeltaTime()
        {
            DateTime Now = DateTime.Now;
            DeltaTime = (Now - FrontFrameTime).TotalMilliseconds;
            FrontFrameTime = Now;
        }

        public static double WaitForUpdateTime
        {
            get { return MainLoop.WaitForUpdateTime; }
        }

        public static double UpdateTime
        {
            get { return MainLoop.UpdateTime; }
        }
    }
}

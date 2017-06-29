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

        public static float DeltaTime
        {
            private set;
            get;
        }

        public static void CalDeltaTime()
        {
            DateTime Now = DateTime.Now;
            DeltaTime = (float)Math.Abs(Now.Millisecond - FrontFrameTime.Millisecond);
            FrontFrameTime = Now;
        }
    }
}

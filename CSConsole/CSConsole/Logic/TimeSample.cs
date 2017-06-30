using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class TimeSample
    {
        public class TimeSampleData
        {
            public ulong GUID
            {
                private set;
                get;
            }

            DateTime BeginTime = DateTime.Now;

            public DateTime GetBeginTime()
            {
                return BeginTime;
            }

            public TimeSampleData(ulong GUID)
            {
                this.GUID = GUID;
                BeginTime = DateTime.Now;
            }
        }

        static ulong SampleDataGUID = 0;

        static Dictionary<ulong, TimeSampleData> SampleData = new Dictionary<ulong, TimeSampleData>();

        public static ulong BeginSample()
        {
            TimeSampleData t = new TimeSampleData(SampleDataGUID++);

            lock (SampleData)
            {
                SampleData.Add(t.GUID, t);
            }

            return t.GUID;
        }

        public static TimeSpan EndSample(ulong GUID)
        {
            DateTime begin = DateTime.MinValue;

            lock(SampleData)
            {
                if (SampleData.ContainsKey(GUID))
                {
                    begin = SampleData[GUID].GetBeginTime();
                    SampleData.Remove(GUID);
                }
            }


            if (begin != DateTime.MinValue)
            {
                TimeSpan t = DateTime.Now - begin;
                //Console.WriteLine("sample : " + t.TotalMilliseconds);
                return t;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class ProjectEnter
    {
        static ProjectEnter PE = null;

        private ProjectEnter() { }

        public static ProjectEnter Instance()
        {
            if (PE == null)
            {
                PE = new ProjectEnter();
            }

            return PE;
        }

        public void Update()
        {
            DebugLogger.Log("Prohect Enter Loop ==> Log Date : " + System.DateTime.Now);
        }
    }
}

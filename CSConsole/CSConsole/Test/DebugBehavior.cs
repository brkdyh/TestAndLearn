using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole.Test
{
    [ActiveOnRuntime]
    public class DebugBehavior : LogicBehavior
    {
        public override void Awake()
        {
            base.Awake();
            DebugLogger.Log("Awake!");
        }
    }
}

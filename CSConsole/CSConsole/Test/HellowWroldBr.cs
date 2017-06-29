using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole.Test
{
    [ActiveOnRuntime]
    public class HellowWroldBr : LogicBehavior
    {
        float fTimer = 0;

        public override void Awake()
        {
            base.Awake();
            DebugLogger.Log("Hellow Wrold！");
            //Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public override void Update()
        {
            DebugLogger.Log("Update at : " + DateTime.Now + " delta Time :" + Time.DeltaTime);
        }
    }
}

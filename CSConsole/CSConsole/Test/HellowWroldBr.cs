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
        double timer = 0f;

        public override void Awake()
        {
            base.Awake();
            DebugLogger.Log("Hellow Wrold！", ConsoleColor.Green);
        }

        public override void Update()
        {
            //for (int i = 0; i < 100000000; i++)
            //{

            //}
            DebugLogger.Log("Update at : " + DateTime.Now + " delta Time :" + Time.DeltaTime + "  ==> Wait for Update Time: " + Time.WaitForUpdateTime + "  ==>  UpateTime :" + Time.UpdateTime, ConsoleColor.Red);

            timer += Time.DeltaTime;

            DebugLogger.Log("Time sciene start : " + (timer / 1000).ToString("0.00"), ConsoleColor.Yellow);
        }
    }
}

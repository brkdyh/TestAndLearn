using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    [ActiveOnRuntime]
    public class LogicTimer : LogicBehavior
    {
        static LogicTimer _Instance = null;
        public LogicTimer Instance
        {

            get { return _Instance; }
        }

        public override void Awake()
        {
            _Instance = this;
        }

        public override void Start()
        {

        }

        public override void Update()
        {
#if CS_111
            DebugLogger.Log("111111");
#endif

#if CUS_CONDI
            DebugLogger.Log("222222");
#endif
        }

        public override void LateUpdate()
        {

        }
    }
}

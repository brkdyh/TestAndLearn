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

        }

        public override void LateUpdate()
        {

        }
    }
}

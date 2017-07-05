using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CSConsole
{
    public class LogicEnter : ILogicEnter
    {
        static LogicEnter PE = null;

        private LogicEnter() { }

        public static LogicEnter Instance()
        {
            if (PE == null)
            {
                PE = new LogicEnter();
            }

            return PE;
        }

        public void Init()
        {
            Assembly curAssembly = Assembly.GetAssembly(typeof(LogicEnter));

            Type[] types = curAssembly.GetTypes();

            foreach (Type t in types)
            {
                var cads = t.CustomAttributes;

                foreach (CustomAttributeData cad in cads)
                {
                    if (cad.AttributeType == typeof(ActiveOnRuntimeAttribute))
                    {
                        if (LogicUtil.Inherit(t, typeof(LogicBehavior)))
                        {
                            LogicManager.Instance().CreateLogicBehavior(t);
                        }
                    }
                }
            }
        }

        public void Update()
        {
            //DebugLogger.Log("Prohect Enter Loop ==> Log Date : " + System.DateTime.Now);
            LogicManager.Instance().Update();
        }

        public void Awake()
        {

        }

        public void Start()
        {

        }

        public void LateUpdate()
        {

        }
    }
}

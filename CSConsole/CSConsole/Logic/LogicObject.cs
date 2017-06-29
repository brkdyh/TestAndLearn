
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class LogicObject : CSConsole.Object
    {
        Dictionary<Type, List<LogicBehavior>> LogicBehaviorDic = new Dictionary<Type, List<LogicBehavior>>();

        public LogicObject()
        {

        }

        public sealed override void Destory(Object obj)
        {
            LogicManager.Instance().DestoryLogicObject(obj);
        }

        public void AddBehavior<T>()
            where T : LogicBehavior, new()
        {
            Type t = typeof(T);
            if (LogicBehaviorDic.ContainsKey(t))
            {
                List<LogicBehavior> be = LogicBehaviorDic[t];
                T tbe = LogicManager.Instance().CreateLogicBehavior<T>();
                be.Add(tbe);
            }
            else
            {
                List<LogicBehavior> be = new List<LogicBehavior>();
                T tbe = LogicManager.Instance().CreateLogicBehavior<T>();
                be.Add(tbe);
                LogicBehaviorDic.Add(t, be);
            }
        }
    }
}

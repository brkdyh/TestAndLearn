using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public class LogicManager : LogicBehavior
    {
        public sealed class DestoryLogicObjectResquest
        {
            public int ObjGUID
            {
                private set;
                get;
            }

            public DestoryLogicObjectResquest(int ObjGuid)
            {
                ObjGUID = ObjGuid;
            }
        }


        #region Instance

        static LogicManager logicManager = null;

        private LogicManager() { }

        public static LogicManager Instance()
        {
            if (logicManager == null)
            {
                logicManager = new LogicManager();
            }

            return logicManager;
        }

        #endregion

        Dictionary<int, Object> logicObjDic = new Dictionary<int, Object>();

        Queue<DestoryLogicObjectResquest> qDestoryRquest = new Queue<DestoryLogicObjectResquest>();

        Queue<LogicBehavior> qInitBehavior = new Queue<LogicBehavior>();

        //public LogicObject CreateLogicObject()
        //{
        //    LogicObject lObj = new LogicObject();
        //    logicObjDic.Add(lObj.GetHashCode(), lObj);
        //    return lObj;
        //}

        public T CreateLogicBehavior<T>()
            where T : LogicBehavior, new()
        {
            T lObj = new T();
            logicObjDic.Add(lObj.GetHashCode(), lObj);
            qInitBehavior.Enqueue(lObj);
            return lObj;
        }

        public LogicBehavior CreateLogicBehavior(Type t)
        {
            Assembly curAssembly = Assembly.GetAssembly(typeof(LogicManager));

            if (LogicUtil.Inherit(t, typeof(LogicBehavior)))
            {
                object obj = curAssembly.CreateInstance(t.FullName);
                if (obj != null)
                {
                    LogicBehavior be = (LogicBehavior)obj;
                    logicObjDic.Add(be.GetHashCode(), be);
                    qInitBehavior.Enqueue(be);
                    return be;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public void DestoryLogicObject(Object lObj)
        {
            DestoryLogicObjectResquest r = new DestoryLogicObjectResquest(lObj.GetHashCode());
            qDestoryRquest.Enqueue(r);
        }

        private void DoDestoryLogicObject(DestoryLogicObjectResquest r)
        {
            if (logicObjDic.ContainsKey(r.ObjGUID))
            {
                try
                {
                    Object Obj = logicObjDic[r.ObjGUID];
                    Obj.OnDestory();
                }
                catch (Exception ex)
                {
                    DebugLogger.Log("On Destory Exception :" + ex.Message);
                }
                logicObjDic.Remove(r.ObjGUID);
            }
        }

        public override void Update()
        {
            lock (qDestoryRquest)
            {
                while (qDestoryRquest.Count > 0)
                {
                    DestoryLogicObjectResquest r = qDestoryRquest.Dequeue();
                    DoDestoryLogicObject(r);
                }
            }

            lock (qInitBehavior)
            {
                while (qInitBehavior.Count > 0)
                {
                    LogicBehavior be = qInitBehavior.Dequeue();

                    try
                    {
                        be.Awake();
                    }
                    catch
                    {

                    }

                    try
                    {
                        be.Start();
                    }
                    catch
                    {

                    }
                }
            }

            var ilter = logicObjDic.GetEnumerator();

            while (ilter.MoveNext())
            {
                if (ilter.Current.Value is LogicBehavior)
                {
                    var be = ilter.Current.Value as LogicBehavior;

                    try
                    {
                        be.Update();
                    }
                    catch
                    {

                    }

                    try
                    {
                        be.LateUpdate();
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}

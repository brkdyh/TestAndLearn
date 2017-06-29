using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public static class LogicUtil
    {
        public static bool Inherit(Type t, Type it)
        {
            if (t == null)
                return false;

            if (t.BaseType == it)
            {
                return true;
            }
            else
            {
                if (t.BaseType == null)
                {
                    return false;
                }
                else
                {
                    return Inherit(t.BaseType, it);
                }
            }
        }
    }
}

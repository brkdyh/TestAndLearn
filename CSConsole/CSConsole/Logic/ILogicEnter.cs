using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSConsole
{
    public interface ILogicEnter
    {
        void Awake();
        void Start();
        void Update();
        void LateUpdate();
    }
}

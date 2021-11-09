using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Profiling;

namespace E
{
    [UnityEngine.ExecuteInEditMode]
    [AutoInstantiate(5)]
    public class HaHa : SuckDick<HaHa>
    {
        protected override void OnAwake()
        {
            UnityEngine.Debug.Log("enable啦");
        }
        
    }

    public class SuckDick<T> : GlobalBehaviour
    {
        protected override bool IsEnabled => true;
    }
}

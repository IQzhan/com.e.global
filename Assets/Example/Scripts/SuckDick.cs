using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E
{
    [UnityEngine.ExecuteInEditMode]
    [AutoInstantiate(5)]
    public class HaHa : SuckDick<HaHa>
    {

    }

    public class SuckDick<T> : GlobalBehaviour
    {
        protected override bool IsEnabled => true;
    }
}

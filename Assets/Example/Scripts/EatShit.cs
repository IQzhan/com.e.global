using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace E
{
    public partial class EatShit : MonoBehaviour
    {
        private void Update()
        {
            
        }
    }

    public static class ExtendEatShit
    {
        public static void Update(this EatShit eatShit)
        {
            Debug.Log("eatshit");
        }
    }
}

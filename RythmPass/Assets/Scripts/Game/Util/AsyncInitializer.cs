using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public interface IAsyncInitializer
    {
        public bool IsLoadComplete();
    }
}
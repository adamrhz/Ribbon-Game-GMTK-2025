using System.Collections;
using UnityEngine;

namespace Ribbon
{
    public class Controller<T>
    {
        public T Context;
        public virtual void Init(T context)
        {
            Context = context;
        }
    }
}
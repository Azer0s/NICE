using System;
using System.Collections.Generic;

namespace NICE.Hardware.Abstraction
{
    public abstract class Stateable
    {
        private readonly Dictionary<Type, object> _states = new Dictionary<Type, object>();

        public T? GetState<T>() where T : struct
        {
            T? val = null;
            lock (_states)
            {
                if (_states.ContainsKey(typeof(T)))
                {
                    val = (T) _states[typeof(T)];
                }
            }

            return val;
        }

        public void SetState<T>(T state) where T : struct
        {
            lock (_states)
            {
                _states[typeof(T)] = state;
            }
        }
    }
}
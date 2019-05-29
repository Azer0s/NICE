using System;
using System.Collections.Generic;

namespace NICE.API.Abstraction
{
    public abstract class Predicateable<T>
    {
        public List<Func<T, T>> Predicates = new List<Func<T, T>>();

        protected T Apply(T frame)
        {
            foreach (var predicate in Predicates)
            {
                frame = predicate(frame);
            }
            
            Predicates.Clear();

            return frame;
        }
    }
}